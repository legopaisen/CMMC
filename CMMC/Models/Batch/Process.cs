using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CTBC;
using System.Data.SqlClient;
using CMMC.Models;

namespace CMMC.Models.Batch
{
    public class Process : IDisposable
    {
        public void Dispose() { GC.SuppressFinalize(this); }

        //BATCH COMPUTATION DELEGATE-EVENT HANDLER
        public delegate void BatchStartHandler();
        public delegate void BatchProcessHandler();
        public delegate void BatchStopHandler();
        public delegate void BatchErrorHandler(string strErrorMessage);
        public delegate void BatchCompleteHandler();
        public delegate void BatchRestartHandler();
        public delegate void BatchCancelHandler();

        public event BatchStartHandler OnStart;
        public event BatchProcessHandler OnProcess;
        public event BatchStopHandler OnStop;
        public event BatchErrorHandler OnError;
        public event BatchCompleteHandler OnComplete;
        public event BatchRestartHandler OnRestart;

        private bool blnIsRunning;

        //BATCH COMPUTATION EVENT
        public void Start(DateTime pDate)
        {
            this.DoMainProcess(pDate);
            this.blnIsRunning = true;
        }

        private void DoMainProcess(DateTime pDate)
        {
            while (this.blnIsRunning)
            {
                try
                {
                    DateTime dtePrevDate = new Batch.Functions().GetPreviousBankingDate(pDate).ToDateTime();
                    Functions.DateRange dtePrevDates = (Functions.DateRange)new Batch.Functions().GetDatesforWithdrawalCharges(pDate);
                    using (Functions main = new Functions())
                    {
                        while (dtePrevDates.From <= dtePrevDates.To)
                        {
                            if (main.IsFirstBankingDay())
                            {
                                new SSIS().ProcessADB();
                                if (new Batch.Functions().IsBankingday(dtePrevDates.From))
                                {
                                    //UPDATE ACCOUNT WITH ACCOUNTONE.txt for FirstBaking day
                                    new Account().UpdateAccount(true, dtePrevDates.From);
                                    new Customer().UpdateCustomer(true, dtePrevDates.From);
                                    if (main.IsFNSADBReady(dtePrevDates.From))
                                    {
                                        main.UpdateADB();
                                        main.ComputePenalty();
                                    }
                                }
                            }
                            else
                            {
                                if (new Batch.Functions().IsBankingday(dtePrevDates.From))
                                {
                                    //UPDATE ACCOUNT WITH ACCOUNT.txt not for Banking Day
                                    new Account().UpdateAccount(false, dtePrevDates.From);
                                    new Customer().UpdateCustomer(false, dtePrevDates.From);
                                }
                            }
                            if (new Batch.Functions().IsBankingday(dtePrevDates.From))
                            {
                                main.UpdatePayCode();
                                if (main.IsPreviousBankingDateProcessed(dtePrevDates.From))
                                {
                                    Error();
                                    blnIsRunning = false;
                                }
                                else
                                {
                                    main.ATMProcessFile(dtePrevDates.From);
                                    main.ComputeWithdrawalFee(dtePrevDates.From);
                                    main.GenerateChargingFile(dtePrevDates.From);
                                    main.UpdateLastRun();
                                    main.SendLogFile("CMMC.txt"); // LogFilesendhere
                                    Complete();
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    if (OnError != null)
                    {
                        OnError(e.Message);
                        Stop();
                    }
                }
            }
        }

        public void Stop()
        {
            if (OnStop != null)
            {
                this.blnIsRunning = false;
            }
        }

        public void Error()
        {
            if (OnError != null)
            {
                this.blnIsRunning = false;
                this.Stop();
            }
        }

        public void Restart(DateTime pDate)
        {
            if (OnRestart != null)
            {
                Stop();
                Start(pDate);
            }
        }

        public void Complete()
        {
            if (OnComplete != null)
            {
                this.blnIsRunning = false;
                this.Stop();
            }
        }

        public string GetDateandTime()
        {
            string strDate = "";
            strDate = SharedFunctions.ServerDate.ToString("mm/dd/yy hh:mm:ss tt");
            strDate += " \t";
            return strDate;
        }

        private void ProcessBatch(DateTime pDate)
        {
            StringBuilder sb = new StringBuilder();
            bool isError = false;
            bool isFirstBankingDay = new Functions().IsFirstBankingDay() ? true : false; ;
            bool isFNSADBReady = new Functions().IsFNSADBReady(pDate) ? true : false; ;
            bool isEveryDay = new Functions().IsEveryday() ? true : false;
            bool isBankingDay = new Functions().IsBankingday(pDate) ? true : false;
            bool isHoliday = new Models.Batch.Holiday().IsHoliday(pDate) ? true : false;
            bool isWeekend = new Functions().IsWeekEnd(pDate) ? true : false;
            bool isAccntUpdated = false;
            bool isCustUpdated = false;
            string strPreviousBangkingday = "";
            string strPreviousDates = "";

            using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
            {
                using (SqlCommand cmd = cn.CreateCommand())
                {
                    cn.Open();
                    if (cn.State == System.Data.ConnectionState.Open)
                    {
                        sb.Append(GetDateandTime() + "- Batch Processing started.");
                        using (Batch.Functions func = new Functions())
                        {
                            //Checking for FNS ADB
                            if (isFirstBankingDay)
                            {
                                if (!isFNSADBReady)
                                {
                                    sb.Append(GetDateandTime() + "- First Banking Day - FNS_ADB.mdb is not yet ready!.");
                                    sb.Append(GetDateandTime() + "- Batch Processing terminated.");
                                    return;
                                }
                            }

                            if (isHoliday)
                            {
                                if (!isEveryDay)
                                {
                                    isError = true;
                                    sb.Append(GetDateandTime() + "- Batch Processing terminated." + "     Reason: Holiday" + "Holiday name here");
                                    sb.Append(GetDateandTime() + "- No records were processed.");
                                }
                                isBankingDay = false;
                            }

                            if (isWeekend)
                            {
                                if (!isEveryDay)
                                {
                                    isError = true;
                                    sb.Append(GetDateandTime() + "- Batch Processing terminated." + "     Reason: Saturday/Sunday");
                                    sb.Append(GetDateandTime() + "- No records were processed.");
                                }
                                isBankingDay = false;
                            }

                            if (!isError)
                            {
                                string strreturnval = func.GetPreviousBankingDate(pDate);
                                if (strreturnval != "error")
                                {
                                    strPreviousBangkingday = strreturnval;
                                }
                                else
                                {
                                    isError = true;
                                    sb.Append(GetDateandTime() + "- Internal error: Parsing previous banking date was unsuccessful");
                                }
                            }
                            if (!isError)
                            {
                                object objreturnval = func.GetDatesforWithdrawalCharges(pDate);
                                if ((string)objreturnval != "error")
                                {
                                    strPreviousDates = (string)objreturnval;
                                }
                                else
                                {
                                    isError = true;
                                    sb.Append(GetDateandTime() + "- Internal error: Parsing previous dates for process was unsuccessful");
                                }
                            }

                            //edit here strPreviousDates
                            if (!isError) { func.CheckFormatFiles(); }

                            //update account and customer
                            if (!isError)
                            {
                                //-- accounts
                                sb.Append(GetDateandTime() + "Started updating accounts");
                                string strval = new Models.Batch.Account().UpdateAccount(isFirstBankingDay ? true : false, pDate);
                                if (strval == "error")
                                {
                                    isError = true;
                                    isAccntUpdated = false;
                                    sb.Append(GetDateandTime() + "Internal error: Updating account was unsuccessful");
                                }
                                else
                                {
                                    isAccntUpdated = true;
                                    sb.Append(GetDateandTime() + "Done updating accounts");
                                }
                                // -- customer
                                sb.Append(GetDateandTime() + "Started updating customer");
                                string strval2 = new Models.Batch.Customer().UpdateCustomer(isFirstBankingDay ? true : false, pDate);
                                if (strval2 == "error")
                                {
                                    isError = true;
                                    isCustUpdated = false;
                                    sb.Append(GetDateandTime() + "Internal error: Updating customer was unsuccessful");
                                }
                                else
                                {
                                    isCustUpdated = true;
                                    sb.Append(GetDateandTime() + "Done updating customer");
                                }
                            }

                            //updatepaycode\
                            sb.Append(GetDateandTime() + "Started updating paycode");
                            string strupdatepaycode = func.UpdatePayCode();
                            if (strupdatepaycode == "error")
                            {
                                sb.Append(GetDateandTime() + "Internal error: Updating paycode unsuccessful");
                            }
                            else
                            {
                                sb.Append(GetDateandTime() + "Done Updating Paycode");
                            }

                            //final checking for account and customer update
                            if (!isError)
                            {
                                // -- accounts
                                if (!isAccntUpdated)
                                {
                                    sb.Append(GetDateandTime() + "Started updating accounts");
                                    string strval = new Models.Batch.Account().UpdateAccount(false, pDate);
                                    if (strval == "error")
                                    {
                                        isError = true;
                                        isAccntUpdated = false;
                                        sb.Append(GetDateandTime() + "Internal error: Updating account was unsuccessful");
                                    }
                                    else
                                    {
                                        isAccntUpdated = true;
                                        sb.Append(GetDateandTime() + "Done updating accounts");
                                    }
                                }
                                //-- customer
                                if (!isCustUpdated)
                                {
                                    sb.Append(GetDateandTime() + "Started updating customer");
                                    string strval = new Models.Batch.Customer().UpdateCustomer(false, pDate);
                                    if (strval == "error")
                                    {
                                        isError = true;
                                        isCustUpdated = false;
                                        sb.Append(GetDateandTime() + "Internal error: Updating customer was unsuccessful");
                                    }
                                    else
                                    {
                                        isCustUpdated = true;
                                        sb.Append(GetDateandTime() + "Done updating customer");
                                    }
                                }


                            }

                        }
                    }
                    else
                    {
                        sb.Append(GetDateandTime() + "- Database not connected.");
                        sb.Append(GetDateandTime() + "- Batch Processing terminated.");
                        return;
                    }
                }
            }
        }
    }
}
