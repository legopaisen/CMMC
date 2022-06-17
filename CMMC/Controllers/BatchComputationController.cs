using CMMC.Models;
using CMMC.Models.Batch;
using CMMC.Models.Batch_Computation;
using CTBC.Network;
using Newtonsoft.Json;
using SYS_MATRIX.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMMC.Controllers
{
  [UserAuthorization]
  [OutputCache(Duration = 0, NoStore = true, VaryByParam = "*")]
  public class BatchComputationController : Controller
  {
    //
    // GET: /BatchComputation/

    public ActionResult Index()
    {
      return View();
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
      List<BatchScheduleModel> models = new BatchSchedule().GetList(out totalSize, pageNumber, numberOfItems, searchKey, runType);
      response.rows = models.Cast<object>().ToList();
      response.total = totalSize;
      return Json(response, JsonRequestBehavior.AllowGet);
    }

    public JsonResult IsHolidayOrWeekend(DateTime pDate)
    {
      bool blnReturn = false;
      blnReturn = new Holiday().IsExist(pDate);
      return Json(blnReturn, JsonRequestBehavior.AllowGet);
    }

    public ActionResult Schedule()
    {
      //GET PREVIOUS RUN
      BatchScheduleModel previousRun = new BatchSchedule().GetLastRun();
      if (previousRun.ID > 0)
      {
        ViewBag.HasPreviousRun = 1;
        ViewBag.LastRun = previousRun.StartTime.ToString("MMM dd, yyyy hh:mm tt");
        ViewBag.RunBy = previousRun.CreatedBy;
        ViewBag.Status = (int)previousRun.Status;
        ViewBag.RunType = previousRun.RunType == RunType.REGULAR ? "Regular" : "Scheduled";
        ViewBag.Remarks = previousRun.Remarks;
      }
      else
      {
        ViewBag.HasPreviousRun = 0;
      }
      ViewBag.CryptoHashCode = new SystemCore().CreateSessionHash();

      //GET NEXT RUN
      BatchScheduleModel nextRun = new BatchSchedule().GetNextRun();
      if (nextRun.ID > 0)
      {
        ViewBag.HasNextRun = 1;
        ViewBag.NextRun = nextRun.StartTime.ToString("MMM dd, yyyy hh:mm tt");
      }
      else
      {
        ViewBag.HasNextRun = 0;
      }
      return View();
    }

    public ActionResult Manual()
    {
      return View();
    }

    public ActionResult Generate()
    {
      return View();
    }

    public JsonResult Insert(BatchScheduleModel Model)
    {
      string[] arr = Model.CreatedBy.Split(' ');
      string strNetworkID = new SystemCore().DecryptStringAES(arr[0], arr[2]);
      string strPassword = new SystemCore().DecryptStringAES(arr[1], arr[2]);
      //string strNetworkID = new SystemCore().DecryptStringAES(Request["hdnUserIDEncrypted"].ToString(), Request["HashCode"].ToString());
      //string strPassword = new SystemCore().DecryptStringAES(Request["hdnPasswordEncrypted"].ToString(), Request["HashCode"].ToString());
      SystemCore.SystemResponse response = new SystemCore.SystemResponse();
      CTBC.Cryptography.AES crypto = new CTBC.Cryptography.AES(SystemCore.SecurityKey);
      string strLDAPPath = "LDAP://chinatrust.com.ph";
      string strLDAPUsername = "57kfZnu8PbRQDLMcXz+EJg==";
      string strLDAPPassword = "PrMYssXe9/K59cERN3IMeA==";
      strNetworkID = strNetworkID.ToUpper();
      if (new SYS_USERS().GetList().Where(x => x.UserID.ToUpper().Equals(strNetworkID) && x.IsActive).ToList().Count > 0)
      {
        ActiveDirectory ad = new ActiveDirectory(strLDAPPath, crypto.Decrypt(strLDAPUsername), crypto.Decrypt(strLDAPPassword));
        if (ad.ErrorException == null)
        {
          ActiveDirectory.UserDetails details = ad.GetUserDetailsSingle(strNetworkID);
          if (!details.IsLockout && !details.IsAccountExpired && !details.IsAccountDisabled)
          {
            if (!CTBC.Network.Credential.Logon(strNetworkID, "CTCBPH_GL2", strPassword))
            {
              response.ResponseStatus = SystemCore.ResponseStatus.FAILED;
              response.Description = "Authentication Failed.";
            }
            else
            {
              SYS_USERS_MODEL user = new SYS_USERS().Fill(strNetworkID);
              string userModules = JsonConvert.SerializeObject(Session["ModuleList"]);
              var modules = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SYS_MATRIX.Models.SYS_ACCESS_MODS_MODEL>>(userModules);
              if (modules.Count(x => x.ModuleCode == "BatchComputation") > 0)
              {
                response.ResponseStatus = SystemCore.ResponseStatus.SUCCESS;
              }
              else
              {
                response.ResponseStatus = SystemCore.ResponseStatus.FAILED;
                response.Description = "You have no access to run this process.";
              }
            }
          }
          else
          {
            // if user account is locked expired or disabled
            response.ResponseStatus = SystemCore.ResponseStatus.FAILED;
            response.Description = "Network ID is locked out. Please contact System Administrator.";
          }
        }
        else
        {//if active directory error
          response.ResponseStatus = SystemCore.ResponseStatus.FAILED;
          response.Description = "LDAP Error: " + ad.ErrorException.Message.Replace("\r\n", "");
        }
      }//if user is not on cmmc user list
      else
      {
        response.ResponseStatus = SystemCore.ResponseStatus.FAILED;
        response.Description = "User ID is not enrolled in the system.";
      }
      if (response.ResponseStatus != SystemCore.ResponseStatus.FAILED)
      {
        Model.CreatedBy = strNetworkID;//Session["UserID"].ToString();
        Model.ModifiedBy = strNetworkID;//Session["UserID"].ToString();
        Model.Remarks = Model.Remarks == null ? "" : Model.Remarks;
        Model.RunType = RunType.SCHEDULED;
        Model.Status = RunStatus.PENDING;
        string message = "";
        if (BatchRunHistory.HasFailedRun())
        {
          response.ResponseStatus = SystemCore.ResponseStatus.FAILED;
          response.Description = "Cannot run new batch if there are previous failed runs";
        }
        else if (!_checkRunRequirements(Model.ProcessDate, Model.Processes, out message))
        {
          response.ResponseStatus = SystemCore.ResponseStatus.FAILED;
          //response.Description = message;
          response.Description = "Some files/paths required to run this date are missing.";
        }
        else if (new Holiday().IsExist(Model.ProcessDate))
        {
          response.ResponseStatus = SystemCore.ResponseStatus.FAILED;
          response.Description = "Process date cannot be a holiday.";
        }
        else if (Model.ProcessDate.DayOfWeek == DayOfWeek.Saturday || Model.ProcessDate.DayOfWeek == DayOfWeek.Sunday)
        {
          response.ResponseStatus = SystemCore.ResponseStatus.FAILED;
          response.Description = "Process date cannot be a weekend.";
        }
        else if (new BatchSchedule().HasPending(Model.ProcessDate, Model.Processes.Select(x => x.ProcessCode).ToArray()))
        {
          response.ResponseStatus = SystemCore.ResponseStatus.FAILED;
          response.Description = "1 or more processes are already scheduled for selected date to process.";
        }
        else
        {
          int intReturn = new BatchSchedule().Insert(Model);
          if (intReturn > 0)
          {
            response.ResponseStatus = SystemCore.ResponseStatus.SUCCESS;
            response.Description = "New Batch Run Schedule successfully saved.";
          }
          else
          {
            response.ResponseStatus = SystemCore.ResponseStatus.FAILED;
            response.Description = "Error creating new batch schedule.";
          }
        }
      }
      return Json(response, JsonRequestBehavior.AllowGet);
      //return View();
    }

    public JsonResult GetProcessList()
    {
      List<BatchProcessModel> processes = new BatchProcess().GetList();
      List<BatchProcessModel> _newOrder = new List<BatchProcessModel>();
      _newOrder.Add(processes[0]);
      _newOrder.Add(processes[1]);
      _newOrder.Add(processes[2]);
      _newOrder.Add(processes[7]);
      _newOrder.Add(processes[3]);
      _newOrder.Add(processes[4]);
      _newOrder.Add(processes[5]);
      _newOrder.Add(processes[6]);
      return Json(_newOrder, JsonRequestBehavior.AllowGet);
    }

    public JsonResult GetSchedules()
    {
      List<BatchScheduleModel> schedules = new BatchSchedule().GetList();
      return Json(schedules, JsonRequestBehavior.AllowGet);
    }

    public ActionResult Details(int id)
    {
      BatchScheduleModel model = new BatchSchedule().Fill(id);
      return PartialView(model);
    }

    public JsonResult Delete(BatchScheduleModel Model)
    {
      Model.ModifiedBy = Session["UserID"].ToString();
      int affRows = new BatchSchedule().Delete(Model);
      return Json(affRows, JsonRequestBehavior.AllowGet);
    }

    [HttpPost]
    public JsonResult SaveScheduleRun(DateTime pDate, string pAutoRun)
    {
      new Models.Batch_Computation.ScheduledRun().SaveSchedule(pDate, pAutoRun);
      return Json("success", JsonRequestBehavior.DenyGet);
    }

    [HttpPost]
    public JsonResult GetLastRun(DateTime pDate)
    {
      new Models.Batch_Computation.ScheduledRun().GetLastRun(pDate);
      return Json("", JsonRequestBehavior.AllowGet);
    }

    private bool _checkRunRequirements(DateTime pCurrentDate, List<BatchProcessModel> pProcesses, out string pMessage)
    {
      bool blnReturn = false;
      string errors = "";
      pMessage = "";
      DateTime PreviousDate;
      DateTime[] PreviousDates;

      using (CTBC.Utility.Registry registry = new CTBC.Utility.Registry(CTBC.Utility.Registry.RegistryType.LOCAL_MACHINE, @"SOFTWARE\CTBC\CMMC"))
      {
        string AccountFileLabel = "ACCOUNT";
        string CustomerFileLabel = "CUSTOMER";
        string DumpINVVFileLabel = "DUMPINVV";
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

        if (IsScheduleEveryday)
        {
          //Get Previous banking date
          PreviousDate = pCurrentDate.AddDays(-1);

          //Get dates of withdrawal charges to be generated
          PreviousDates = new DateTime[] { PreviousDate };
        }
        else
        {
          //Get Previous banking date
          PreviousDate = BatchSchedule.GetPreviousBankingDay(pCurrentDate, IsScheduleEveryday);

          //Get dates of withdrawal charges to be generated
          PreviousDates = BatchSchedule.GetDatesofWithdrawalChargesToBeGenerated(pCurrentDate, PreviousDate);
        }

        int _missingFilesCount = 0;
        bool _firstBankingDay = false;
        bool _notBankingDay = false;

        //CHECK FOR FILE LOAD PATH
        if (!Directory.Exists(FileLoadPath))
        {
          errors += string.Format("Path not found: {0}\n", FileLoadPath);
          _missingFilesCount++;
        }

        //CHECK FOR ACCOUNT/CUSTOMER/DUMPINVV FILE
        if (pProcesses.Count(x => x.ProcessCode == (int)ProcessType.RELOAD_ACCOUNTS_DAILY) > 0)
        {
          _firstBankingDay = false;
          _notBankingDay = false;
        }
        else if (pProcesses.Count(x => x.ProcessCode == (int)ProcessType.RELOAD_ACCOUNTS_DAILY) > 0)
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

        if (!_firstBankingDay)
        {
          if (!_notBankingDay)
          {
            acctPathLocation = ACCTPath + PreviousDate.ToString("yyyy_MM") + "\\";
            acctFile = (AccountFileLabel + PreviousDate.ToString("_MMdd") + ".TXT").ToUpper();

            custFileLocation = CustomerFilePath + PreviousDate.ToString("yyyy_MM") + "\\";
            custFile = (CustomerFileLabel + PreviousDate.ToString("_MMdd") + ".TXT").ToUpper();

            dumpFileLocation = DUMPINVVPath + PreviousDate.ToString("yyyy_MM") + "\\";
            dumpFile = (DumpINVVFileLabel + PreviousDate.ToString("_MMdd") + ".TXT").ToUpper();

            if (!Directory.Exists(dumpFileLocation))
            {
              errors += string.Format("Path not found: {0}\n", dumpFileLocation);
              _missingFilesCount++;
            }

            if (!System.IO.File.Exists(dumpFileLocation + dumpFile))
            {
              errors += string.Format("File not found: {0}\n", dumpFileLocation + dumpFile);
              _missingFilesCount++;
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
          errors += string.Format("Path not found: {0}\n", acctPathLocation);
          _missingFilesCount++;
        }

        if (!System.IO.File.Exists(acctPathLocation + acctFile))
        {
          errors += string.Format("File not found: {0}\n", acctPathLocation + acctFile);
          _missingFilesCount++;
        }

        if (!Directory.Exists(custFileLocation))
        {
          errors += string.Format("Path not found: {0}\n", custFileLocation);
          _missingFilesCount++;
        }

        if (!System.IO.File.Exists(custFileLocation + custFile))
        {
          errors += string.Format("File not found: {0}\n", custFileLocation + custFile);
          _missingFilesCount++;
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
              errors += string.Format("Path not found: {0}\n", BackupPath);
              _missingFilesCount++;
            }
          }

          if (!Directory.Exists(invvLocation))
          {
            errors += string.Format("Path not found: {0}\n", invvLocation);
            _missingFilesCount++;
          }

          if (!System.IO.File.Exists(invvLocation + invvFile))
          {
            errors += string.Format("File not found: {0}\n", invvLocation + invvFile);
            _missingFilesCount++;
          }
        }
        //////////////////////////////////////////////////////////

        //FNS ADB
        string CASAADB = ADBCASAPath + "FNS_ADB.mdb";
        string mdbFile = ChargingFilePath + @"Fileload\FNS_ADB.mdb";

        if (!Directory.Exists(ADBCASAPath))
        {
          errors += string.Format("Path not found: {0}\n", ADBCASAPath);
          _missingFilesCount++;
        }

        if (!System.IO.File.Exists(CASAADB))
        {
          errors += string.Format("File not found: {0}\n", CASAADB);
          _missingFilesCount++;
        }

        if (!Directory.Exists(ChargingFilePath + @"Fileload\"))
        {
          errors += string.Format("Path not found: {0}\n", ChargingFilePath + @"Fileload\");
          _missingFilesCount++;
        }
        //////////////////////////////////////////////////////////


        //CHARGING FILE
        if (!Directory.Exists(ChargingFilePath))
        {
          errors += string.Format("Path not found: {0}\n", ChargingFilePath);
          _missingFilesCount++;
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

  public enum ProcessType
  {
    RELOAD_ACCOUNTS_DAILY = 1,
    RELOAD_ACCOUNTS_FULL_UPDATE = 2,
    REPROCESS_WITHDRAWALS = 3,
    REGENERATE_CHARGING_FILE = 4,
    REPROCESS_ADB = 5,
    INCLUDE_MONTHEND_ADB = 6,
    SEND_ATM_DEBITFILE = 7
  }
}
