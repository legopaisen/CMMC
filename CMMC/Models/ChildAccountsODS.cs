using System;
using System.Collections.Generic;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CMMC.Models
{
    public class ChildAccountsODS : IDisposable
    {
        public void Dispose() { GC.SuppressFinalize(this); }

        public struct Details
        {
            public string AccountNo { get; set; } //BBAN_NUM
            public string AccountName { get; set; } //BBAN_NUM
            public string BranchCode { get; set; }
            public string BranchName { get; set; }
            public string DateEnrolled { get; set; }
            public string ProductType { get; set; }
        }

        public List<Details> GetList(int pCMSCode)
        {
            List<Details> list = new List<Details>();
            DateTime dtEnrolled = new DateTime();
            string sQuery = "select ACCOUNT_NUMBER, BRANCH, BRANCH_NAME, PRODUCT_ID, PRODUCT_NAME, OPENING_DATE from CTBC_DEPOSIT_ACCOUNTS ";
            sQuery += $"where PAYROLL_CODE = '{pCMSCode}' and ACCOUNT_NUMBER is not null ";
            sQuery += $"(product_name like 'A/P%' OR product_name like 'AP CC%') and CUSTOMER_ID in (select bp_id from BANCS_CORPORATE_DETAILS)";
            try
            {
                using (OracleConnection cn = new OracleConnection(SharedFunctions.ODSConnectionString))
                {
                    using (OracleCommand cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = sQuery;
                        cn.Open();
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(new Details()
                                {
                                    AccountNo = reader["ACCOUNT_NUMBER"].ToString(),
                                    AccountName = reader["CUST_NAME"].ToString(),
                                    BranchName = reader["BRANCH_NAME"].ToString(),
                                    DateEnrolled = reader["BRANCH_NAME"].ToString(),
                                    BranchCode = reader["BRANCH"].ToString(),
                                    ProductType = reader["PRODUCT_NAME"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { }

            return list;
        }
    }
}