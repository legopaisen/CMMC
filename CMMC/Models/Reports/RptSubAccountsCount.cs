using System;
using System.Collections.Generic;
using System.Data.OracleClient;
using System.Linq;
using System.Web;

namespace CMMC.Models.Reports
{
    public class RptSubAccountsCountModel
    {
        public string Branch { get; set; }
        public string CMSCode { get; set; }
        public string CustomerName { get; set; }
        public string SubAccCASACount{ get; set; }
        public string SubAccCashCardCASACount { get; set; }
    }

    public class RptSubAccountsCount
    {
        public List<RptSubAccountsCountModel> GetSubAccountsList(string pBranch, string pCMSCode)
        {
            List<RptSubAccountsCountModel> list = new List<RptSubAccountsCountModel>();
            string strSQL = @"SELECT 
                            CB.BRANCH_NAME,
                            CB.PAYROLL_CODE,
                            CB.CUST_NAME,
                            COUNT(ACCOUNT_NUMBER) AS ACCOUNT_COUNT
                            FROM CTBC_DEPOSIT_ACCOUNTS CB
                            WHERE ";
            strSQL += $"CB.BRANCH = '{pBranch}' ";
            strSQL += $"AND CB.PAYROLL_CODE = '{pCMSCode}' ";
            strSQL += $"GROUP BY CB.BRANCH_NAME, CB.PAYROLL_CODE, CB.CUST_NAME";

            using (OracleConnection cn = new OracleConnection(SharedFunctions.ODSConnectionString))
            {
                using (OracleCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = strSQL;
                    cmd.CommandTimeout = 0;
                    cn.Open();
                    using (OracleDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new RptSubAccountsCountModel()
                            {
                                Branch = dr["BRANCH_NAME"].ToString(),
                                CMSCode = dr["PAYROLL_CODE"].ToString(),
                                CustomerName = dr["CUST_NAME"].ToString(),
                                SubAccCASACount = dr["ACCOUNT_COUNT"].ToString(),
                                SubAccCashCardCASACount = dr["ACCOUNT_COUNT"].ToString()
                            });
                        }
                    }
                }
            }
            return list;
        }
    }
}