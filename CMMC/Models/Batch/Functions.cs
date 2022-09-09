using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CTBC;
using System.Data.SqlClient;
using System.Reflection;
using System.IO;
using System.Data.OleDb;
using System.Xml.Serialization;
using Newtonsoft.Json;
using System.Data.OracleClient;

namespace CMMC.Models.Batch
{
    public class Functions : IDisposable
    {
        public void Dispose() { GC.SuppressFinalize(this); }

        public static SettingsStruct Settings { get; set; }

        public struct SettingsStruct
        {
            public string Email_Recipient { get; set; }
            public string Email_Sender { get; set; }
            public string SQLServer { get; set; }
            public string SQLUserID { get; set; }
            public string SQLPassword { get; set; }
            public string SQLDatabase { get; set; }
            public string WithdrawalFee { get; set; }
            public string LastRun { get; set; }
            public string ScheduleRun { get; set; }

            //DESTINATION
            public string DestinationChargingFile { get; set; }
            public string DestinationFileFormat { get; set; }
            public string DestinationFolder { get; set; }
            public string DestinationWithdrawalBackup { get; set; }

            //FILE REFERENCE: SHOULD BE FIX LOCATION
            public string SourceAccount { get; set; }
            public string SourceCustomer { get; set; }
            public string SourceADB { get; set; }
            public string SourceDumpINVV { get; set; }
            public string SourceWithdrawal { get; set; }

        }

        public string SqlConnectionstring
        {
            get
            {

                return CTBC.SQL.ConnectionStringBuilder.Build(CTBC.SQLDatabases.MS_SQL, Settings.SQLDatabase, Settings.SQLPassword, Settings.SQLUserID, Settings.SQLServer);
            }
        }

        public string AccessConnectionString
        {
            get
            {
                return CTBC.SQL.ConnectionStringBuilder.Build(CTBC.SQLDatabases.MS_Access, "FNS_ADB");
            }
        }

        public string GetPreviousBankingDate(DateTime pDate)
        {
            string strReturn = "";
            DateTime currdate = pDate;
            try
            {
                if (!IsWeekEnd(pDate) || !new Holiday().IsHoliday(pDate))
                {
                    currdate.AddDays(-1);
                    bool blnIsPreviousDateValid = false;

                    while (!blnIsPreviousDateValid)
                    {
                        if (!new Holiday().IsHoliday(currdate))
                        {
                            if (!new Models.Batch.Functions().IsWeekEnd(currdate))
                            {
                                blnIsPreviousDateValid = true;
                                break;
                            }
                        }
                        strReturn = currdate.AddDays(-1).ToString();
                    }
                }
            }
            catch (Exception e)
            {
                strReturn = "error";
                CTBC.Logs.Write("Get Previous Banking Date", e.Message, "Batch Function");
            }
            return strReturn;
        }

        public bool IsPreviousBankingDateProcessed(DateTime pDate)
        {
            bool blnReturn = false;
            DateTime bankingDate = pDate;
            DateTime dateTobeChecked = GetPreviousBankingDate(bankingDate).ToDateTime();
            using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
            {
                using (SqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "Select wd_transactionDate from withdrwaltxnDtl where wd_transactionDate = @pDate";
                    cmd.Parameters.Add(new SqlParameter("@pDate", dateTobeChecked));
                    cn.Open();
                    blnReturn = cmd.ExecuteScalar().ToString().ToInt() > 0 ? true : false;
                }
            }
            return blnReturn;
        }

        public void UpdateLastRun()
        {
            if (Settings.LastRun.ToDateTime() < DateTime.Today)
            {
                //if lastRun is less than the current run date: UPDATE
                using (CTBC.Utility.Registry registry = new CTBC.Utility.Registry(CTBC.Utility.Registry.RegistryType.LOCAL_MACHINE, @"SOFTWARE\CTBC\CMMC"))
                {
                    registry.Write("LAST_BATCH_RUN", DateTime.Today.ToString("MM/dd/yyyy"));
                }
            }
        }

        public void UpdateSchedule(DateTime pDate)
        {
            using (CTBC.Utility.Registry registry = new CTBC.Utility.Registry(CTBC.Utility.Registry.RegistryType.LOCAL_MACHINE, @"SOFTWARE\CTBC\CMMC"))
            {
                registry.Write("SCHEDULERUN", pDate.ToString());
            }
        }

        public DateTime GetLastRun()
        {
            DateTime dteReturn;
            dteReturn = Functions.Settings.LastRun.ToDateTime();
            return dteReturn;
        }

        public void ComputePenalty()
        {
            //do the process here of total penalty
            ComputeRequiredADB();
            ComputePenaltyCharges();
        }

        public void ATMProcessFile(DateTime pDate)
        {
            //int intCtr;
            DateTime dtePreviousBankingDate = GetPreviousBankingDate(pDate).ToDateTime();
            string strSourceFile = Functions.Settings.SourceWithdrawal;
            string strFormatFile = Functions.Settings.DestinationFileFormat;

            using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
            {
                using (SqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "usp_ProcessATMLogs";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.Add(new SqlParameter("@vchSourceFile", strSourceFile));
                    cmd.Parameters.Add(new SqlParameter("@vchFormatFile", strFormatFile));
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            DateRange drRange = (DateRange)GetDatesforWithdrawalCharges(pDate);
            while (drRange.From <= drRange.To)
            {
                using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
                {
                    using (SqlCommand cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = "usp_PopulateWithdrawalTxnTBL";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@TranDate", drRange.From));
                        cmd.CommandTimeout = 0;
                        cn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
                {
                    using (SqlCommand cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = "";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@TranDate", drRange.From));
                        cmd.CommandTimeout = 0;
                        cn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public void ComputePenaltyCharges()
        {
            //compute for penaltyaccording to transactions
            using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
            {
                using (SqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "usp_ComputPenaltyCharges";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void ComputeRequiredADB()
        {
            //compute requiredADB
            using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
            {
                using (SqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "usp_ComputeRequiredADB";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public bool IsFirstBankingDay()
        {
            bool blnReturn = false;
            DateTime dteStartDate = CTBC.DateTimeExtender.GetMonthStart(DateTime.Today);

            while (!blnReturn)
            {
                if (!new Holiday().IsHoliday(dteStartDate))
                {
                    if (!new Models.Batch.Functions().IsWeekEnd(dteStartDate))
                    {
                        blnReturn = true;
                        break;
                    }
                }
                dteStartDate = dteStartDate.AddDays(1);
            }

            blnReturn = dteStartDate.Equals(DateTime.Today);

            return blnReturn;
        }

        public bool IsWeekEnd(DateTime pDate)
        {
            bool blnIsReturn = false;

            if (pDate.DayOfWeek != DayOfWeek.Saturday || pDate.DayOfWeek != DayOfWeek.Sunday) //if not sat or sun
            {
                blnIsReturn = true;
            }

            return blnIsReturn;
        }

        public bool IsBankingday(DateTime pDate)
        {
            bool blnIsReturn = true;
            if (new Models.Batch.Functions().IsWeekEnd(pDate) || new Holiday().IsHoliday(pDate))
            {
                blnIsReturn = false;
            }
            return blnIsReturn;
        }

        public struct DateRange
        {
            public DateTime From { get; set; }
            public DateTime To { get; set; }
        }

        public object GetDatesforWithdrawalCharges(DateTime pDate)
        {
            object objReturn = new object();
            DateRange dteRange = new DateRange();
            DateTime dtePrevDate;

            try
            {
                if (IsBankingday(pDate))
                {
                    dtePrevDate = pDate.AddDays(-1);
                    if (!IsBankingday(dtePrevDate))
                    {
                        dtePrevDate.AddDays(-1);
                        dteRange.To = dtePrevDate;
                        while (!IsBankingday(dtePrevDate))
                        {
                            dtePrevDate.AddDays(-1);
                            if (IsBankingday(dtePrevDate))
                            {
                                dteRange.From = dtePrevDate.AddDays(-1);
                            }
                        }
                    }
                    else
                    {
                        dteRange.From = dtePrevDate;
                        dteRange.To = dtePrevDate;
                    }
                }
                objReturn = dteRange;
            }
            catch (Exception e)
            {
                objReturn = "error";
                CTBC.Logs.Write("GetDatesforWithdrawalCharges", e.Message, "Batch Function");

            }
            return objReturn;
        }

        public string UpdatePayCode()
        {
            string strReturn = "";
            try
            {
                using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
                {
                    using (SqlCommand cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = "usp_AddNewPaycode";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                strReturn = "success";
            }
            catch (Exception e)
            {
                strReturn = "error";
                CTBC.Logs.Write("UpdatePayCode", e.Message, "Batch Function");

            }
            return strReturn;
        }

        public void ComputeWithdrawalFee(DateTime pDate)
        {
            DateRange drRange = (DateRange)GetDatesforWithdrawalCharges(pDate);
            while (drRange.From <= drRange.To)
            {
                //Update Free Withdrawal Per Account table
                //Insert new records
                using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
                {
                    using (SqlCommand cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = "usp_PopulateFreeWithdrawalPerAcctTbl";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@TransDate", drRange.From));
                        cn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                //Populate Withdrawal transaction master table
                //Get all payroll accounts with withdrawal charge
                using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
                {
                    using (SqlCommand cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = "usp_PopulateWithdrawalTxnMstrTbl";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@TransDate", drRange.From));
                        cn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                drRange.From = drRange.From.AddDays(1);
            }
        }

        public void UpdateADB()
        {
            new SSIS().ProcessADB();
            using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
            {
                using (SqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "usp_UpdateADBCasa";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void GenerateChargingFile(DateTime pDate)
        {
            DateRange drRange = (DateRange)GetDatesforWithdrawalCharges(pDate);
            DateTime dteDate = pDate;

            string strAmt = "000000000000";
            string strCMMCFile = "CMMC_FEE_" + drRange.From.ToString("MM/dd/yyyy") + ".txt";
            string strCMMCDirectory = Functions.Settings.DestinationFolder + strCMMCFile;

            //HEADER
            using (FileStream fs = new FileStream(System.AppDomain.CurrentDomain.BaseDirectory, FileMode.OpenOrCreate))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine("BF" + dteDate.ToString("MM/dd/yyyy") + "");
                }
            }
            //---DETAIL---
            //ATM WITHDRAWAL CHARGES
            //Get records from CMMC_WithdrawalTxnMstr (Payroll Accounts)
            using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
            {
                using (SqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM CMMC_WITHDRAWALTxnMstr WHERE wt_TransactionNo between @TransactionFrom and @TransactionTo";
                    cmd.Parameters.Add(new SqlParameter("@TransactionFrom", drRange.From));
                    cmd.Parameters.Add(new SqlParameter("@TransactionTo", drRange.To));
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            for (int intWdrawalFee = 0; intWdrawalFee <= dr.GetSqlInt32(4) - dr.GetSqlInt32(5); intWdrawalFee++)
                            {

                                using (FileStream fs = new FileStream(System.AppDomain.CurrentDomain.BaseDirectory, FileMode.Append))
                                {
                                    using (StreamWriter sw = new StreamWriter(fs))
                                    {
                                        sw.WriteLine("01" + " " + dr.GetString(0) + "036" + strAmt + "" + "01");
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //Get records from CMMC_WithdrawalNonPayrollTxnDtl (Non-Payroll Accounts)
            using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
            {
                using (SqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM CMMC_WithdrawalNonPayrollTxnDtl WHERE wn_TransactionDate between @TransactionFrom and @TransactionTo";
                    cmd.Parameters.Add(new SqlParameter("@TransactionFrom", drRange.From));
                    cmd.Parameters.Add(new SqlParameter("@TransactionTo", drRange.To));
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            for (int intWdrawalFee = 0; intWdrawalFee <= dr.GetSqlInt32(3); intWdrawalFee++)
                            {
                                using (FileStream fs = new FileStream(System.AppDomain.CurrentDomain.BaseDirectory, FileMode.Append))
                                {
                                    using (StreamWriter sw = new StreamWriter(fs))
                                    {
                                        sw.WriteLine("01" + " " + dr.GetString(0) + "036" + strAmt + "" + "01");
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //PENALTY CHARGES
            if (IsFirstBankingDay())
            {
                DateTime dtePrevDate = GetPreviousBankingDate(pDate).ToDateTime();
                using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
                {
                    using (SqlCommand cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT pt_AccountNo, pt_Amount FROM CMMC_PenaltyTnxMstr INNER JOIN CMMC_Paycode ON pt_Paycode = pc_Paycode where pt_ProcedDate = @ProcessDate ORDER BY Account_No";
                        cmd.Parameters.Add(new SqlParameter("@ProcessDate", dtePrevDate));
                        cn.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                while (drRange.From <= drRange.To)
                                {
                                    using (FileStream fs = new FileStream(System.AppDomain.CurrentDomain.BaseDirectory, FileMode.Append))
                                    {
                                        using (StreamWriter sw = new StreamWriter(fs))
                                        {
                                            sw.WriteLine("01" + " " + dr.GetString(0) + "037" + dr.GetOrdinal("strAmt") + "" + "01");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //LAST PART OF GENERATION
            using (FileStream fs = new FileStream(System.AppDomain.CurrentDomain.BaseDirectory, FileMode.Append))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    //SUMMATION
                    sw.WriteLine("BR");
                    //TRAILER
                    sw.WriteLine("EF");
                }
            }
        }

        public void SendLogFile(string pFile)
        {
            using (CTBC.Mail.Mailer mail = new CTBC.Mail.Mailer(""))
            {
                mail.Attachment = pFile;
                mail.Body = "Batch done as of today!";
                mail.Recipient = Functions.Settings.Email_Recipient;
                mail.Sender = Functions.Settings.Email_Sender;
                mail.Send();
            }
        }

        public DateTime GetSchedule()
        {
            DateTime dteReturn;
            dteReturn = Functions.Settings.ScheduleRun.ToDateTime();
            return dteReturn;
        }

        public bool IsFNSADBReady(DateTime pYearDate)
        {
            int dteYearDate = pYearDate.Year;
            int dteMonthDate = pYearDate.Month;
            bool blnReturn;
            bool _blnReturn = false;
            using (OleDbConnection cn = new OleDbConnection(AccessConnectionString))
            {
                using (OleDbCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "Select January,Febraury,March,April,May,June,July,August,September,October,November,December FROM tbl_adb_processed WHERE Year = @Year";
                    cmd.Parameters.Add(new OleDbParameter("@Year", dteYearDate));
                    using (OleDbDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            for (int x = 1; x <= 12; x++)
                            {
                                if (dteMonthDate == x)
                                {
                                    if (dr.GetInt32(x) == -1)
                                    {
                                        _blnReturn = true;
                                        return _blnReturn;
                                    }
                                    else
                                    {
                                        _blnReturn = false;
                                        return _blnReturn;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            blnReturn = _blnReturn;
            return blnReturn;
        }

        public bool IsEveryday()
        {
            return false;
        }

        public void CheckFormatFiles()
        {

        }

        public void GenerateXMLFile(Booking.bookingMaster bookingmaster, Booking.bookingDetailsList bookingdetailslist) //for creation of booking
        {
            var dir = Path.GetFullPath("path");
            var file = Path.Combine(dir, "filename.xml");
            var stream = new MemoryStream();
            var streamWriter = new StreamWriter(file, false);
            var jsonWriter = new JsonTextWriter(streamWriter);
            var serializer = new JsonSerializer();
            List<Booking.bookingDetailsList> bookingDetailsList = new List<Booking.bookingDetailsList>();

            bookingDetailsList.Add(new Booking.bookingDetailsList()
            {
                entity = "GCTBCPH001"
            });

            //body
            var booking = new Booking
            {
                BM = new Booking.bookingMaster
                {
                    entity = "GCTBCPH001"
                },
                BD = bookingDetailsList
            };

            serializer.Formatting = Formatting.Indented;
            serializer.Serialize(jsonWriter, booking);
            jsonWriter.Flush();
            streamWriter.Flush();
            stream.Seek(0, SeekOrigin.Begin);
        }

    }
    public class Booking
    {
        [JsonProperty(PropertyName = "bookingMaster")]
        public bookingMaster BM { get; set; }

        [JsonProperty(PropertyName = "bookingDetailsList")]
        public List<bookingDetailsList> BD { get; set; }
        public class bookingMaster
        {
            public string entity { get; set; }
            public string transactionDate { get; set; }
            public int externalBusinessEventId { get; set; }
            public int forcedBookingIndicator { get; set; }
            public int orderingSystemId { get; set; }
            public string bookingRequestId { get; set; }
            public string transactionId { get; set; }
            public string transactionSource { get; set; }
            public int reversalCorrectionIndicator { get; set; }
            public string externalTransactionReference { get; set; }
        }
        public class bookingDetailsList
        {
            public string entity { get; set; }
            public string accountCurrency = "PHP";
            public double accountAmount { get; set; }
            public int accountNumberFormat { get; set; }
            public string accountNumber { get; set; }
            public int bookingType { get; set; }
            public string cashBlockId { get; set; }
            public string orginialValueDate { get; set; }
            public int customerExchangeRate { get; set; }
            public string valueDate { get; set; }
            public string narrativeId { get; set; }
            public int bookId { get; set; }
            public string accountingDate { get; set; }
            public double baseCurrencyAmount { get; set; }
            public double transactionAmount { get; set; }
            public string transactionCurrency { get; set; }
            public int cashIndicator { get; set; }
            public string narrativePlaceholders { get; set; }
            public string btcCode { get; set; }
            public string epcCode { get; set; }
            public string businessTransactionDetail1 { get; set; }
            public string businessTransactionDetail2 { get; set; }
            public string reconciliationReferenceId { get; set; }
            public int grossBookingRequired { get; set; }
            public int componentType { get; set; }
            public string virtualAccountNumber { get; set; }
            public string taxCode { get; set; }

        }
    }
}
