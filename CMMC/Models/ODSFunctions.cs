using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace CMMC.Models
{
    public class ODSFunctions : IDisposable
    {
        public void Dispose() { GC.SuppressFinalize(this); }

        [XmlRoot("CreateBookingRequest")]
        public class CreateBookingRequest
        {
            [XmlElement("CreateBookingInputDetails")]
            public CreateBookingInputDetails createbookinginputDetails { get; set; }
            public struct CreateBookingInputDetails
            {
                [XmlElement("BookingMaster")]
                public List<bookingMaster> BookingMaster { get; set; }
            }
            public struct bookingMaster
            {
                public string BookingRequestID { get; set; }
                public string ExternalBusinessEventID { get; set; }
                public int ForcedBookingIndicator { get; set; }
                public int ReversalCorrectionIndicator { get; set; }
                public string TransactionDate { get; set; }
                public string TransactionID { get; set; }
                public string TransactionSource { get; set; }
                public int OrderingSystemID { get; set; }
                public string Entity { get; set; }

                [XmlElement("BookingDetails")]
                public List<bookingDetails> BookingDetails { get; set; }
            }
            public struct bookingDetails
            {
                public AccountAmount AccountAmount { get; set; }
                public AccountNumber AccountNumber { get; set; }
                public BaseCurrencyAmount BaseCurrencyAmount { get; set; }
                public int BookingType { get; set; }
                public string BusinessTransactionDetail1 { get; set; }
                public string BusinessTransactionDetail2 { get; set; }
                public string CashBlockID { get; set; }
                public int CustomerExchangeRate { get; set; }
                public string CashIndicator { get; set; }
                public string ReconciliationReferenceID { get; set; }
                public string NarrativeID { get; set; }
                NarrativePlaceholders? _narrativePlaceholders;
                public NarrativePlaceholders NarrativePlaceholders
                {
                    get { return (NarrativePlaceholders)_narrativePlaceholders; }
                    set { _narrativePlaceholders = value; }
                }
                public bool NarrativePlaceholdersSpecified
                {
                    get { return _narrativePlaceholders != null; }
                }
                public string BtcCode { get; set; }
                public string EpcCode { get; set; }
                public TransactionAmount TransactionAmount { get; set; }
                public string ValueDate { get; set; }
                public string AccountingDate { get; set; }
                public string Entity { get; set; }
            }
            public struct AccountAmount
            {
                public decimal Amount { get; set; }
                public string Crncy { get; set; }
            }
            public struct AccountNumber
            {
                public int AcntFrmt { get; set; }
                public string AcntNumber { get; set; }
            }
            public struct BaseCurrencyAmount
            {
                public decimal Amount { get; set; }
                public string Crncy { get; set; }
            }
            public struct NarrativePlaceholders
            {
                public string PLHName { get; set; }
                public string PLHValue { get; set; }
            }
            public struct TransactionAmount
            {
                public decimal Amount { get; set; }
                public string Crncy { get; set; }
            }

        }

        private void InsertBookingID(int iBookingIDSeries)
        {
            string sQuery = "DELETE FROM CMMC_BookingRequestIDList ";
            sQuery += $"where TransDate = '{DateTime.Now:yyyy-MM-dd}'";

            using (SqlConnection con = new SqlConnection(SharedFunctions.Connectionstring))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();
                    cmd.CommandText = sQuery;
                    if (cmd.ExecuteNonQuery() == 0)
                    { }
                }
            }

            sQuery = "INSERT INTO CMMC_BookingRequestIDList (transdate, BookingRequestID) ";
            sQuery += $"values('{DateTime.Now:yyyy-MM-dd}', '{iBookingIDSeries}')";

            using (SqlConnection con = new SqlConnection(SharedFunctions.Connectionstring))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();
                    cmd.CommandText = sQuery;
                    if(cmd.ExecuteNonQuery() == 0) 
                    { }
                }
            }
        }

        private int GetNewBookingSeries()
        {
            int iSeq = 0;
            string sYear = DateTime.Now.Year.ToString();
            string sMonth = DateTime.Now.Month.ToString("d2");

            using(SqlConnection con = new SqlConnection(SharedFunctions.Connectionstring))
            {
                using(SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();
                    cmd.CommandText = $"select isnull(MAX(BookingRequestID),0)+1 from CMMC_BookingRequestIDList where MONTH(TransDate) = '{sMonth}' and YEAR(TransDate) = '{sYear}'";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            iSeq = reader.GetInt32(0);
                        }
                    }
                }
            }

            GetBookingID(iSeq);

            return iSeq;
        }

        private string GetBookingID(int iSeqCnt)
        {
            string sBookingID = string.Empty;
            string sYear = DateTime.Now.Year.ToString();
            string sMonth = DateTime.Now.Month.ToString("d2");
            string sDay = DateTime.Now.Day.ToString("d2");
            string sDate = sYear + sMonth + sDay;
            string sPrefixSystem = "CMMC";
            string sSequence = string.Empty;
            switch (iSeqCnt.ToString().Length)
            {
                case 1: sSequence = "00000000000000" + iSeqCnt.ToString(); break;
                case 2: sSequence = "0000000000000" + iSeqCnt.ToString(); break;
                case 3: sSequence = "000000000000" + iSeqCnt.ToString(); break;
                case 4: sSequence = "00000000000" + iSeqCnt.ToString(); break;
                case 5: sSequence = "0000000000" + iSeqCnt.ToString(); break;
                case 6: sSequence = "000000000" + iSeqCnt.ToString(); break;
                case 7: sSequence = "00000000" + iSeqCnt.ToString(); break;
                case 8: sSequence = "0000000" + iSeqCnt.ToString(); break;
                case 9: sSequence = "000000" + iSeqCnt.ToString(); break;
                case 10: sSequence = "00000" + iSeqCnt.ToString(); break;
                case 11: sSequence = "0000" + iSeqCnt.ToString(); break;
                case 12: sSequence = "000" + iSeqCnt.ToString(); break;
                case 13: sSequence = "00" + iSeqCnt.ToString(); break;
                case 14: sSequence = "0" + iSeqCnt.ToString(); break;
                case 15: sSequence = iSeqCnt.ToString(); break;
            }

            sBookingID = sPrefixSystem + sDate + sSequence;

            return sBookingID;
        }

        public List<CreateBookingRequest.bookingMaster> FillMaster()
        {
            List<CreateBookingRequest.bookingMaster> bookingMasterlist = new List<CreateBookingRequest.bookingMaster>();
            List<Models.CMSCode.Details> cmscodelist = new List<CMSCode.Details>();
            List<ODS_Paycodes.Details> odspaycodeslist = new List<ODS_Paycodes.Details>();
            ODS_Paycodes odspaycodes = new ODS_Paycodes();
            Models.CMSCode cmscode = new CMSCode();
            cmscodelist = cmscode.GetListApproved();
            odspaycodeslist = odspaycodes.GetListAccount();
            string sBookingID = string.Empty;
            string sYear = DateTime.Now.Year.ToString();
            string sMonth = DateTime.Now.Month.ToString("d2");
            string sDay = DateTime.Now.Day.ToString("d2");
            string sDateToday = sYear + sMonth + sDay;
            int iCntBookingID = 0;
            var matchlist = odspaycodeslist.Where(item => !cmscodelist.Any(item2 => item2.CMSCode.ToString() == item.Payroll_Code));

            iCntBookingID = GetNewBookingSeries();

            foreach (var accountno in matchlist)
            {
                sBookingID = GetBookingID(iCntBookingID);
                bookingMasterlist.Add(new CreateBookingRequest.bookingMaster()
                {
                    BookingRequestID = sBookingID,
                    ExternalBusinessEventID = "19001",
                    ForcedBookingIndicator = 2,
                    ReversalCorrectionIndicator = 3,
                    TransactionDate = sDateToday,
                    TransactionID = "61-15355",
                    TransactionSource = "3777",
                    OrderingSystemID = 19,
                    Entity = "GCTBCPH001",
                    BookingDetails = FillBookingDetails(accountno.Account_Number, accountno.Payroll_Code),
                });
                iCntBookingID++;
            }
            InsertBookingID(iCntBookingID);


            return bookingMasterlist;
        }

        public List<CreateBookingRequest.bookingDetails> FillBookingDetails(string sAccountNo, string sCMSCode)
        {
            ODSADBBalance odsadbbalance = new ODSADBBalance();
            ODSADBBalance.ODSADBBalanceModel adbmodel = new ODSADBBalance.ODSADBBalanceModel();
            List<CreateBookingRequest.bookingDetails> BookingDetails = new List<CreateBookingRequest.bookingDetails>();
            int iCMSCode = 0;
            decimal dAmount = 0;
            string sYear = DateTime.Now.Year.ToString();
            string sMonth = DateTime.Now.Month.ToString("d2");
            string sDay = DateTime.Now.Day.ToString("d2");
            string sDateToday = sYear + sMonth + sDay;
            decimal dWithdrawalFee = GetWithdrawalFees(sAccountNo, sMonth);
            int.TryParse(sCMSCode, out iCMSCode);
            adbmodel = odsadbbalance.GetADB(sAccountNo, DateTime.Now.Month.ToString("d2"), iCMSCode, DateTime.Now.Year.ToString());
            dAmount = odsadbbalance.ComputePenalty(sAccountNo, adbmodel.MVMMNT_AMT, iCMSCode);

            //debit
            BookingDetails.Add(new CreateBookingRequest.bookingDetails()
            {
                AccountAmount = new CreateBookingRequest.AccountAmount
                {
                    Amount = dAmount + dWithdrawalFee,
                    Crncy = "PHP"
                },
                AccountNumber = new CreateBookingRequest.AccountNumber
                {
                    AcntFrmt = 1,
                    AcntNumber = sAccountNo
                },
                BaseCurrencyAmount = new CreateBookingRequest.BaseCurrencyAmount()
                {
                    Amount = dAmount + dWithdrawalFee,
                    Crncy = "PHP"
                },
                BookingType = 1,
                CustomerExchangeRate = 1,
                CashIndicator = "NO",
                NarrativeID = "20577",
                NarrativePlaceholders = new CreateBookingRequest.NarrativePlaceholders()
                {
                    PLHName = "REMITTANCEINFORMATION_LINE1",
                    PLHValue = "ABC"
                },
                BtcCode = "125",
                EpcCode = "8011",
                TransactionAmount = new CreateBookingRequest.TransactionAmount()
                {
                    Amount = dAmount + dWithdrawalFee,
                    Crncy = "PHP"
                },
                ValueDate = sDateToday, //yyyymmdd
                Entity = "GCTBCPH001"
            });

            //credit
            BookingDetails.Add(new CreateBookingRequest.bookingDetails()
            {
                AccountAmount = new CreateBookingRequest.AccountAmount
                {
                    Amount = dAmount + dWithdrawalFee,
                    Crncy = "PHP"
                },
                AccountNumber = new CreateBookingRequest.AccountNumber
                {
                    AcntFrmt = 2,
                    AcntNumber = "001010000161" // sample gl codes - 660187, 001010000158
                },
                BaseCurrencyAmount = new CreateBookingRequest.BaseCurrencyAmount()
                {
                    Amount = dAmount + dWithdrawalFee,
                    Crncy = "PHP"
                },
                BookingType = 2,
                CustomerExchangeRate = 1,
                CashIndicator = "NO",
                TransactionAmount = new CreateBookingRequest.TransactionAmount()
                {
                    Amount = dAmount + GetWithdrawalFees(sAccountNo, sMonth),
                    Crncy = "PHP"
                },
                ValueDate = sDateToday, //yyyymmdd
                Entity = "GCTBCPH001"
            });

            return BookingDetails;
        }

        public void StartGeneration()
        {
            List<CreateBookingRequest.bookingMaster> bookingmasterlist = new List<CreateBookingRequest.bookingMaster>();
            List<CreateBookingRequest.bookingDetails> bookingdetails = new List<CreateBookingRequest.bookingDetails>();
            bookingmasterlist = FillMaster();

            var filePath = GenerateXMLFile(bookingmasterlist);
            UploadXML(filePath);
        }

        private void InsertFileSeries(int iSeries)
        {
            string sQuery = $"DELETE FROM CMMC_XMLFileSeries where FileDate = '{DateTime.Now.ToString("yyyy-MM-dd")}'";
            using (SqlConnection con = new SqlConnection(SharedFunctions.Connectionstring))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();
                    cmd.CommandText = sQuery;
                    if (cmd.ExecuteNonQuery() == 0)
                    { }
                }
            }

            sQuery = "INSERT INTO CMMC_XMLFileSeries (CurrFileID, FileDate) ";
            sQuery += $"values('{iSeries}', '{DateTime.Now.ToString("yyyy-MM-dd")}')";

            using (SqlConnection con = new SqlConnection(SharedFunctions.Connectionstring))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();
                    cmd.CommandText = sQuery;
                    if (cmd.ExecuteNonQuery() == 0)
                    { }
                }
            }
        }

        private string GetFileNameSeries()
        {
            string sSeries = string.Empty;
            int lSeries = 0;
            using (SqlConnection con = new SqlConnection(SharedFunctions.Connectionstring))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();
                    cmd.CommandText = $"select isnull(MAX(CurrFileID),0)+1 from CMMC_XMLFileSeries where MONTH(FileDate) = '{DateTime.Now.Month.ToString("d2")}' and YEAR(FileDate) = '{DateTime.Now.Year.ToString("d2")}'";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            lSeries = reader.GetInt32(0);
                        }
                    }
                }
            }

            int iSeriesCnt = lSeries.ToString().Length;
            switch (iSeriesCnt)
            {
                case 1: sSeries = "00000000" + lSeries.ToString(); break;
                case 2: sSeries = "0000000" + lSeries.ToString(); break;
                case 3: sSeries = "000000" + lSeries.ToString(); break;
                case 4: sSeries = "00000" + lSeries.ToString(); break;
                case 5: sSeries = "0000" + lSeries.ToString(); break;
                case 6: sSeries = "000" + lSeries.ToString(); break;
                case 7: sSeries = "00" + lSeries.ToString(); break;
                case 8: sSeries = "0" + lSeries.ToString(); break;
                case 9: sSeries = lSeries.ToString(); break;
            }

            InsertFileSeries(lSeries);

            return sSeries;
        }

        public string GenerateXMLFileName()
        {
            string XMLFileName = string.Empty;
            string sDay = DateTime.Now.Day.ToString("d2");
            string sMonth = DateTime.Now.Month.ToString("d2");
            string sYear = DateTime.Now.Year.ToString();

            XMLFileName = "BKNGREQ_CMMC_BANCS_001001_" + sYear + sMonth + sDay + "-" + GetFileNameSeries() + "_P_N_T_RQ";

            return XMLFileName;
        }

        public string GenerateXMLFile(List<CreateBookingRequest.bookingMaster> bookingmasterlist) //for creation of booking
        {
            string XMLFilePath = string.Empty;
            var dir = Path.GetFullPath(@"C:\CMMC\FileUpload");
            var file = Path.Combine(dir, GenerateXMLFileName() + ".xml");
            var streamWriter = new StreamWriter(file, false);
            var xmlserializer = new XmlSerializer(typeof(CreateBookingRequest));
            var xmlnamespace = new XmlSerializerNamespaces();
            List<CreateBookingRequest.bookingDetails> bookingDetails = new List<CreateBookingRequest.bookingDetails>();
            List<CreateBookingRequest.bookingMaster> bookingMaster = new List<CreateBookingRequest.bookingMaster>();

            var CreateBookingRequest = new CreateBookingRequest
            {
                createbookinginputDetails = new CreateBookingRequest.CreateBookingInputDetails
                {
                    BookingMaster = bookingmasterlist
                },

            };

            xmlnamespace.Add("", "");
            xmlserializer.Serialize(streamWriter, CreateBookingRequest, xmlnamespace);
            streamWriter.Close();
            XMLFilePath = file;
            return XMLFilePath;
        }

        public decimal GetWithdrawalFees(string sRefNo, string sMonth)
        {
            decimal dWithdrawalFees = 0;
            string sYear = DateTime.Now.Year.ToString();
            string sQuery = @"SELECT *
                            FROM V_AUTHO_ACTIVITY_VIEW
                            WHERE ACTION_CODE = '000'
                            AND SOURCE_ACCOUNT_TYPE = '10'
                            AND BILLING_CURRENCY = '608'
                            AND PROCESSING_CODE = '01' ";
            sQuery += $"AND SOURCE_ACCOUNT_NUMBER = {sRefNo} ";
            sQuery += $"AND EXTRACT(MONTH FROM TRANSACTION_LOCAL_DATE) = '{sMonth}' ";
            sQuery += $"AND EXTRACT(YEAR FROM TRANSACTION_LOCAL_DATE) = '{sYear}' ";
            using (OracleConnection oracon = new OracleConnection(SharedFunctions.OracleConnection))
            {
                using (OracleCommand oracmd = oracon.CreateCommand())
                {
                    oracon.Open();
                    oracmd.CommandText = sQuery;
                    using (OracleDataReader orareader = oracmd.ExecuteReader())
                    {
                        if (orareader.Read())
                        {

                        }
                    }
                }
            }

            return dWithdrawalFees;
        }

        public void UploadXML(string sFilePath)
        {
            string host = "10.64.44.22";
            int port = 22;
            string username = "t000512";
            string password = "gbp2022";
            string destination = "/home/appgbp/apps/nfspath/SIGBP_BKNGMNG_IN/in";

            string filePath = sFilePath;

            using (var client = new SftpClient(host, port, username, password))
            {
                client.Connect();
                client.ChangeDirectory(destination);
                if (client.IsConnected)
                {
                    using(var stream = new FileStream(filePath, FileMode.Open))
                    {
                        client.BufferSize = 4 * 1024;
                        client.UploadFile(stream, Path.GetFileName(filePath));
                        stream.Close();
                    }
                }
                else
                {
                    
                }
            }
        }

        public int LoadBranches()
        {
            int iReturn = 0;
            string sQuery = "select MU_ID, MU_NAME from MANAGEMENT_UNIT";
            using(OracleConnection con = new OracleConnection())
            {
                using (OracleCommand cmd = new OracleCommand())
                {

                }
            }

            return iReturn;
        }

    }
}