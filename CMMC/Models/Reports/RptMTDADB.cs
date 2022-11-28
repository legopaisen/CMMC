using System;
using System.Collections.Generic;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CMMC.Models.Reports
{
    public class RptMTDADBModel
    {
        public string Branch { get; set; }
        public int CMSCode { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public decimal MTDADB { get; set; }
    }
    
    public class RptMTDADB
    {
        public List<RptMTDADBModel> GetMTDADBList(string pBranch, DateTime? pStartDate = null, DateTime? pEndDate = null)
        {
            List<CMSCode.Details> CmsCodeList = new List<CMSCode.Details>();
            CMSCode cmscode = new CMSCode();
            CmsCodeList = cmscode.GetListApproved();
            List<RptMTDADBModel> list = new List<RptMTDADBModel>();
            string sQuery = @"SELECT CB.BRANCH_NAME,
                            CB.PAYROLL_CODE,
                            CB.BRANCH,
                            CB.ACCOUNT_NUMBER,
                            CB.CUST_NAME,
                            BA.ACNT_POS_ID,
                            ROUND((SUM(BA.MVMNT_AMT) / COUNT(BA.MVMNT_AMT)), 2) AS MVMNT_AMT,
                            BA.MVMNT_CRNCY
                            FROM BANCS_ACCOUNT_BALANCE BA,
                            CTBC_DEPOSIT_ACCOUNTS CB
                            WHERE CB.POS_ID = BA.ACNT_POS_ID ";
            sQuery += @"AND BA.MVMNT_CRNCY = 'PHP'
                        AND CB.PAYROLL_CODE IS NOT NULL
                        AND EXTRACT(MONTH FROM BA.MVMNT_GEN_DT) = '11'
                        AND EXTRACT(YEAR FROM BA.MVMNT_GEN_DT) = '2022'
                        GROUP BY
                            CB.Branch_name,
                            CB.PAYROLL_CODE,
                            CB.BRANCH,
                            CB.ACCOUNT_NUMBER,
                            CB.CUST_NAME,
                            BA.ACNT_POS_ID,
                            BA.MVMNT_CRNCY";

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
                            string sODSCMScode = reader["PAYROLL_CODE"].ToString();
                            if(CmsCodeList.Any(x => x.CMSCode.ToString() == sODSCMScode))
                            {
                                list.Add(new RptMTDADBModel()
                                {
                                    Branch = reader["BRANCH_NAME"].ToString(),
                                    CMSCode = Convert.ToInt32(reader["PAYROLL_CODE"].ToString()),
                                    AccountNumber = reader["ACCOUNT_NUMBER"].ToString(),
                                    AccountName = reader["CUST_NAME"].ToString(),
                                    MTDADB = Convert.ToDecimal(reader["MVMNT_AMT"])
                                });
                            }
                           
                        }
                    }

                }
            }
            return list;
        }
    }
}