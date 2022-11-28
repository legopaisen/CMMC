using System;
using System.Collections.Generic;
using System.Data.OracleClient;
using System.Linq;
using System.Web;

namespace CMMC.Models.Reports
{
    public class RptCMSAccountsModel
    {
        public string PCCC { get; set; }
        public string Branch { get; set; }
        public string CMSCode { get; set; }
        public string InvestorType { get; set; }
        public string CIFNumber { get; set; }
        public string AccountNumber { get; set; }
        public string AccountShortName { get; set; }
        public string AccountFirstName { get; set; }
        public string AccountMiddleName { get; set; }
        public string AccountLastName { get; set; }
        public string CurrentEmployer { get; set; }
        public string CMSAccountType { get; set; }
        public string AccountStatus { get; set; }
    }

    public class RptCMSAccounts
    {
        public List<RptCMSAccountsModel> GetCMSAccountsList(string pBranch, string pCmsCode, string pAcctType, string pAcctStatus, DateTime? pStartDate = null, DateTime? pEndDate = null)
        {
            List<RptCMSAccountsModel> list = new List<RptCMSAccountsModel>();
            string sQuery = @"SELECT 
                            CB.BRANCH_NAME,
                            CB.PCCC_CODE,
                            CB.CUSTOMER_ID,
                            CB.ACCOUNT_NUMBER,
                            CB.CUST_NAME
                            FROM CTBC_DEPOSIT_ACCOUNTS CB ";
                            
            sQuery += $"WHERE CB.BRANCH = '{pBranch}' ";
            sQuery += $"AND CB.PAYROLL_CODE = '{pCmsCode}'";
            sQuery += $"AND CB.ACCOUNT_STAT = '{pAcctStatus}' ";
            if(pAcctType != "151202")
            {
                sQuery += $"AND (CB.PRODUCT_NAME LIKE 'A/P%' OR CB.PRODUCT_NAME like 'AP CC%') "; //child
            }
            else
            {
                sQuery += $"AND CB.PRODUCT_ID = '{pAcctType}' ";
            }

            using (OracleConnection con = new OracleConnection(SharedFunctions.ODSConnectionString))
            {
                using (OracleCommand cmd = con.CreateCommand())
                {
                    con.Open();
                    cmd.CommandText = sQuery;
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            list.Add(new RptCMSAccountsModel()
                            {
                                Branch = reader["BRANCH_NAME"].ToString(),
                                PCCC = reader["PCCC_CODE"].ToString(),
                                CMSCode = pCmsCode,
                                CIFNumber = reader["CUSTOMER_ID"].ToString(),
                                AccountShortName = reader["CUST_NAME"].ToString(),
                                CMSAccountType = pAcctType,
                                AccountStatus = pAcctStatus
                            });
                        }
                    }

                }
            }
            return list;
        }
    }
}