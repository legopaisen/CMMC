using CMMC.Models;
using CMMC.Models.Batch;
using CMMC.Models.Batch_Computation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMMC.Controllers
{
    //[UserAuthorization]
    [OutputCache(Duration = 0, NoStore = true, VaryByParam = "*")]
    public class RegularBatchRunController : Controller
    {
        public ActionResult Index()
        {
            //BatchRunHistoryModel model = BatchRunHistory.GetCurrentlyRunningProcess();
            //ViewBag.HasCurrentRun = model != null;
            //if (model != null)
            //{
            //  ViewBag.ProcessName = model.Task.ProcessName;
            //  ViewBag.DateProcessed = model.DateProcessed.ToString("MMM dd, yyyy");
            //  ViewBag.RunDate = model.RunDate.ToString("MMM dd, yyyy hh:mm tt");
            //}
            return View();
        }

        public JsonResult GetCurrentRun()
        {
            BatchRunHistoryModel model = BatchRunHistory.GetCurrentlyRunningProcess();
            if (model == null)
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(model, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Display()
        {
            SystemCore.SearchResponse response = new SystemCore.SearchResponse();
            int numberOfItems = Request["limit"] == null ? 10 : Convert.ToInt32(Request["limit"]);
            int pageNumber = Request["offset"] == null ? 0 : Convert.ToInt32(Request["offset"]);

            if (pageNumber > 0)
            {
                pageNumber = pageNumber / numberOfItems;
            }

            string searchKey = Request["search"] == null ? "" : Request["search"].ToString();
            string runType = Request["runType"] == null ? "" : Request["runType"].ToString();
            long totalSize = 0;
            List<BatchRunHistoryModel> models = new BatchRunHistory().GetList(out totalSize, pageNumber, numberOfItems, searchKey, runType).OrderByDescending(x => x.ID).ToList();
            response.rows = models.Cast<object>().ToList();
            response.total = totalSize;

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Run(DateTime id)
        {
            SystemCore.SystemResponse response = new SystemCore.SystemResponse();
            try
            {
                string message = "";
                List<BatchProcessModel> processes = new BatchProcess().GetList();
                //processes.Remove(processes.First(x => x.ProcessCode == 3));

                if (BatchRunHistory.HasFailedRun())
                {
                    response.ResponseStatus = SystemCore.ResponseStatus.FAILED;
                    response.Description = "Cannot run new batch if there are previous failed runs";
                }
                else if (_isBatchRunning())
                {
                    response.ResponseStatus = SystemCore.ResponseStatus.FAILED;
                    //response.Description = message;
                    response.Description = "Another process of batch processing is currently running.";
                }
                if (!_checkRunRequirements(id, processes, out message))
                {
                    response.ResponseStatus = SystemCore.ResponseStatus.FAILED;
                    //response.Description = message;
                    response.Description = message;//"Some files/paths required to run this date are missing.";
                }
                else if (new Holiday().IsExist(id))
                {
                    response.ResponseStatus = SystemCore.ResponseStatus.FAILED;
                    response.Description = "Process date cannot be a holiday.";
                }
                else if (id.DayOfWeek == DayOfWeek.Saturday || id.DayOfWeek == DayOfWeek.Sunday)
                {
                    response.ResponseStatus = SystemCore.ResponseStatus.FAILED;
                    response.Description = "Process date cannot be a weekend.";
                }
                else
                {
                    using (CTBC.Utility.Registry registry = new CTBC.Utility.Registry(CTBC.Utility.Registry.RegistryType.LOCAL_MACHINE, @"SOFTWARE\CTBC\CMMC\"))
                    {
                        string cmmcBatchLoc = registry.Read("CMMCBATCHLOCATION");

                        ////Batch exe
                        System.Diagnostics.ProcessStartInfo p = new System.Diagnostics.ProcessStartInfo(cmmcBatchLoc);
                        p.Arguments = string.Format("{0} {1} {2}",
                          "2",
                          "\"" + id.ToString("MM/dd/yyyy") + "\"",
                          "\"" + Session["UserID"].ToString()) + "\"";

                        System.Diagnostics.Process.Start(p);

                        response.ResponseStatus = SystemCore.ResponseStatus.SUCCESS;
                        response.Description = "Manual run of CMMC batch processing has started.";

                        //ODSFunctions odsfunction = new ODSFunctions();
                        //odsfunction.StartGeneration();

                    }
                }
            }
            catch (Exception ex)
            {
                response.ResponseStatus = SystemCore.ResponseStatus.FAILED;
                response.Description = "Failed to run.\n" + ex.Message;
                CTBC.Logs.Write("Run", ex.Message, "RegularBatchRunController");
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ContinueRun(BatchRunHistoryModel Model)
        {
            SystemCore.SystemResponse response = new SystemCore.SystemResponse();

            try
            {
                if (_isBatchRunning())
                {
                    response.ResponseStatus = SystemCore.ResponseStatus.FAILED;
                    response.Description = "Another process of batch processing is currently running.";
                }
                else
                {
                    List<BatchRunHistoryModel> models = BatchRunHistory.GetFailedRun(Model.RunGroupID);
                    using (CTBC.Utility.Registry registry = new CTBC.Utility.Registry(CTBC.Utility.Registry.RegistryType.LOCAL_MACHINE, @"SOFTWARE\CTBC\CMMC\"))
                    {
                        string cmmcBatchLoc = registry.Read("CMMCBATCHLOCATION");

                        System.Diagnostics.ProcessStartInfo p = new System.Diagnostics.ProcessStartInfo(cmmcBatchLoc);
                        p.Arguments = string.Format("{0} {1} {2}",
                          "3",
                          Model.RunGroupID,
                          "\"" + Session["UserID"].ToString() + "\"");

                        System.Diagnostics.Process.Start(p);

                        response.ResponseStatus = SystemCore.ResponseStatus.SUCCESS;
                        response.Description = "Rerun of CMMC batch processing has started.";
                    }
                }
            }
            catch (Exception ex)
            {
                response.ResponseStatus = SystemCore.ResponseStatus.FAILED;
                response.Description = "Failed to rerun.\n" + ex.Message;
                CTBC.Logs.Write("ContinueRun", ex.Message, "RegularBatchRunController");
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public static bool _isBatchRunning()
        {
            bool blnReturn = false; //default

            using (CTBC.Utility.Registry registry = new CTBC.Utility.Registry(CTBC.Utility.Registry.RegistryType.LOCAL_MACHINE, @"SOFTWARE\CTBC\CMMC"))
            {
                if (System.Diagnostics.Process.GetProcessesByName(registry.Read("SOURCE_PROGRAMNAME")).Length > 0) //if the length of the processname is greater than 0
                {
                    blnReturn = true;
                }
            }
            return blnReturn;
        }

        private bool _checkRunRequirements(DateTime pCurrentDate, List<BatchProcessModel> pProcesses, out string pMessage)
        {
            bool blnReturn = false; //default
            string errors = "";
            pMessage = "";

            DateTime PreviousDate;
            DateTime[] PreviousDates;

            using (CTBC.Utility.Registry registry = new CTBC.Utility.Registry(CTBC.Utility.Registry.RegistryType.LOCAL_MACHINE, @"SOFTWARE\CTBC\CMMC"))
            {
                string AccountFileLabel = "ACCOUNT";
                string CustomerFileLabel = "CUSTOMER";
                string DumpINVVFileLabel = "DUMPINVV";

                //get the values in registry and pass it to  the declared variables
                string FormatFilesPath = registry.Read("SOURCE_FMTFilesPath");
                string FileLoadPath = registry.Read("SOURCE_FILELOADPATH");
                string INVVFileLabel = registry.Read("SOURCE_INVVFILENAME");
                string INVVPath = registry.Read("SOURCE_INVVPath");
                string ACCTPath = registry.Read("SOURCE_ACCTPath");
                string CustomerFilePath = registry.Read("SOURCE_CUSTPath");
                string ADBCASAPath = registry.Read("SOURCE_ADBCASAPath");
                string FNSADBConnectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Persist Security Info=False;",
                     ADBCASAPath + registry.Read("SOURCE_FNSADBFILE"));
                bool Backup = Convert.ToBoolean(registry.Read("SOURCE_BACKUP"));
                string BackupPath = registry.Read("SOURCE_BACKUPPATH");
                string BulkInsertFolder = registry.Read("SOURCE_BULKINSERTFOLDER");
                string DUMPINVVPath = registry.Read("SOURCE_DUMPINVVPATH");

                string ChargingFilePath = registry.Read("TARGET_CHARGINGFILEPATH");
                double WithdrawalFee = Convert.ToDouble(registry.Read("DEFAULTVALUES_WithdrawalFee"));
                string Emails = registry.Read("DEFAULTVALUES_Email");

                bool UpdateAccount = Convert.ToBoolean(registry.Read("MONTHLYUPDATE_UPDATEACCT"));
                bool UpdateCustomer = Convert.ToBoolean(registry.Read("MONTHLYUPDATE_UPDATECUST"));
                string AccountOnePath = registry.Read("MONTHLYUPDATE_ACCTONEPATH");
                string CustomerOnePath = registry.Read("MONTHLYUPDATE_CUSTONEPATH");

                string FTPIPAddress = registry.Read("FTP_IPADDRESS");
                string FTPTimeOut = registry.Read("FTP_TIMEOUT");
                string FTPDestination = registry.Read("FTP_DESTINATION");

                string SMTPServer = registry.Read("NOTIFICATION_SMTPSERVER");
                string Sender = registry.Read("NOTIFICATION_SENDER");
                string SubjectError = registry.Read("NOTIFICATION_SUBJECTERROR");
                string SubjectSuccess = registry.Read("NOTIFICATION_SUBJECTSUCCESS");
                string BodyError = registry.Read("NOTIFICATION_BODYERROR");
                string BodySuccess = registry.Read("NOTIFICATION_BODYSUCCESS");
                string LogFilePath = registry.Read("NOTIFICATION_LOGFILEPATH");

                string UserID = registry.Read("USER_USERID");

                string ADBTransferJobName = registry.Read("ADBTRANSFER_JOBNAME");

                bool IsScheduleEveryday = registry.Read("SCHEDULE_EVERYDAY").Equals("TRUE");

                //PreviousDate = BatchSchedule.GetPreviousBankingDay(pCurrentDate, IsScheduleEveryday);


                if (IsScheduleEveryday) //IsScheduleEveryday is from the declared variable - if true
                {
                    //Get Previous banking date
                    PreviousDate = pCurrentDate.AddDays(-1);

                    //Get dates of withdrawal charges to be generated
                    PreviousDates = new DateTime[] { PreviousDate };
                }
                else //if false
                {
                    //Get Previous banking date
                    PreviousDate = BatchSchedule.GetPreviousBankingDay(pCurrentDate, IsScheduleEveryday); //GetPreviousBankingDay-Model\Batch Computation\BatchSchedule.cs

                    //Get dates of withdrawal charges to be generated
                    PreviousDates = BatchSchedule.GetDatesofWithdrawalChargesToBeGenerated(pCurrentDate, PreviousDate);
                }


                int _missingFilesCount = 0;
                bool _firstBankingDay = false;
                bool _notBankingDay = false;

                //CHECK FOR FILE LOAD PATH
                if (!Directory.Exists(FileLoadPath)) //FileLoadPath is from the declared variable which the value SOURCE_FILELOADPATH in the registry
                {
                    //errors += string.Format("\n{0}", FileLoadPath);
                    //_missingFilesCount++;
                }

                //CHECK FOR ACCOUNT/CUSTOMER/DUMPINVV FILE
                if (pProcesses.Count(x => x.ProcessCode == (int)ProcessType.RELOAD_ACCOUNTS_DAILY) > 0) //RELOAD_ACCOUNTS_DAILY (\\Controller\BatchComputationController.cs)
                {
                    _firstBankingDay = false;
                    _notBankingDay = false;
                }
                else if (pProcesses.Count(x => x.ProcessCode == (int)ProcessType.RELOAD_ACCOUNTS_DAILY) > 0) //processes.count is greteater than 0
                {
                    _firstBankingDay = true;
                    _notBankingDay = false;
                }

                string acctPathLocation = "";
                string acctFile = "";

                string custFileLocation = "";
                string custFile = "";

                string dumpFileLocation = "";
                string dumpFile = "";

                if (!_firstBankingDay) //if not firstbankingday
                {
                    if (!_notBankingDay) //if it is not a bankingday
                    {
                        acctPathLocation = ACCTPath + PreviousDate.ToString("yyyy_MM") + "\\";
                        acctFile = (AccountFileLabel + PreviousDate.ToString("_MMdd") + ".TXT").ToUpper();

                        custFileLocation = CustomerFilePath + PreviousDate.ToString("yyyy_MM") + "\\";
                        custFile = (CustomerFileLabel + PreviousDate.ToString("_MMdd") + ".TXT").ToUpper();

                        dumpFileLocation = DUMPINVVPath + PreviousDate.ToString("yyyy_MM") + "\\";
                        dumpFile = (DumpINVVFileLabel + PreviousDate.ToString("_MMdd") + ".TXT").ToUpper();

                        if (!Directory.Exists(dumpFileLocation)) //not exist in directory
                        {
                            //errors += string.Format("\n{0}", dumpFileLocation);
                            //_missingFilesCount++;
                        }

                        if (!System.IO.File.Exists(dumpFileLocation + dumpFile)) //not exist
                        {
                            //errors += string.Format("\n{0}", dumpFileLocation + dumpFile);
                            //_missingFilesCount++;
                        }

                    }
                }
                else
                {
                    if (!_notBankingDay)
                    {
                        if (PreviousDate.Month == DateTime.Now.Month)
                        {
                            acctPathLocation = AccountOnePath + PreviousDate.AddMonths(-1).ToString("yyyy_MM") + "\\";
                            custFileLocation = CustomerOnePath + PreviousDate.AddMonths(-1).ToString("yyyy_MM") + "\\";
                        }
                        else
                        {
                            acctPathLocation = AccountOnePath + PreviousDate.ToString("yyyy_MM") + "\\";
                            custFileLocation = CustomerOnePath + PreviousDate.ToString("yyyy_MM") + "\\";
                        }
                        acctFile = "ACCOUNTONE.TXT";
                        custFile = "CUSTOMERONE.TXT";
                    }
                }


                if (!Directory.Exists(acctPathLocation))
                {
                    //errors += string.Format("\n{0}", acctPathLocation);
                    //_missingFilesCount++;
                }

                if (!System.IO.File.Exists(acctPathLocation + acctFile))
                {
                    //errors += string.Format("\n{0}", acctPathLocation + acctFile);
                    //_missingFilesCount++;
                }

                if (!Directory.Exists(custFileLocation))
                {
                    //errors += string.Format("\n{0}", custFileLocation);
                    //_missingFilesCount++;
                }

                if (!System.IO.File.Exists(custFileLocation + custFile))
                {
                    //errors += string.Format("\n{0}", custFileLocation + custFile);
                    //_missingFilesCount++;
                }
                //////////////////////////////////////////////////////////


                //ATM WITHDRAWAL FILES
                foreach (DateTime date in PreviousDates)
                {
                    string invvLocation = INVVPath + date.ToString("yyyy_MM") + "\\";
                    string invvFile = (INVVFileLabel + date.ToString("MMdd") + ".TXT");

                    if (Backup)
                    {
                        if (!Directory.Exists(BackupPath))
                        {
                            //errors += string.Format("\n{0}", BackupPath);
                            //_missingFilesCount++;
                        }
                    }

                    if (!Directory.Exists(invvLocation)) //if the invvlocation is not exist in directory
                    {
                        //errors += string.Format("\n{0}", invvLocation);
                        //_missingFilesCount++;
                    }

                    if (!System.IO.File.Exists(invvLocation + invvFile))
                    {
                        //errors += string.Format("\n{0}", invvLocation + invvFile);
                        //_missingFilesCount++;
                    }
                }
                //////////////////////////////////////////////////////////

                //FNS ADB
                string CASAADB = ADBCASAPath + "FNS_ADB.mdb";
                string mdbFile = ChargingFilePath + @"Fileload\FNS_ADB.mdb";

                if (!Directory.Exists(ADBCASAPath)) //if the adbcasapath is not exist in the directory = true
                {
                    //errors += string.Format("\n{0}", ADBCASAPath);
                    //_missingFilesCount++;
                }

                if (!System.IO.File.Exists(CASAADB))
                {
                    //errors += string.Format("\n{0}", CASAADB);
                    //_missingFilesCount++;
                }

                if (!Directory.Exists(ChargingFilePath + @"Fileload\"))
                {
                    //errors += string.Format("\n{0}", ChargingFilePath + @"Fileload\");
                    //_missingFilesCount++;
                }
                //////////////////////////////////////////////////////////

                //CHARGING FILE
                if (!Directory.Exists(ChargingFilePath))
                {
                    //errors += string.Format("\n{0}", ChargingFilePath);
                    //_missingFilesCount++;
                }
                //////////////////////////////////////////////////////////

                if (_missingFilesCount > 0)
                {
                    pMessage = "Cannot schedule run due missing files/paths:\n" + errors;
                }

                blnReturn = _missingFilesCount == 0;
            }

            return blnReturn;
        }
    }
}
