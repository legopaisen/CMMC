using System;
using System.Collections.Generic;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CMMC.Models
{
    public class BANCS_ACCOUNT : IDisposable
    {
        public void Dispose() { GC.SuppressFinalize(this); }

        public struct Details
        {
            public string AccountNumber { get; set; } //BBAN_NUM
            public string PayrollCode { get; set; } //BBAN_NUM
            public string AccountName { get; set; }
            public string BranchCode { get; set; }
            public string BranchName { get; set; }
            public string DateEnrolled { get; set; }
            public string InvestmentDesc { get; set; } //INSTRMNT_ID //ODS - product type
            public string ProductName { get; set; }
            public string StatusCode { get; set; }
            public string Status { get; set; }
        }

        public Details Fill(int pCMSCode)
        {
            Details details = new Details();
            DateTime dtEnrolled = new DateTime();
            string sQuery = "select ACCOUNT_NUMBER, CUST_NAME, BRANCH, BRANCH_NAME, PRODUCT_ID, PRODUCT_NAME, ACCOUNT_STAT, ACCOUNT_STAT_DESC from CTBC_DEPOSIT_ACCOUNTS ";
            sQuery += $"where PAYROLL_CODE = '{pCMSCode}' and ACCOUNT_NUMBER is not null ";
            sQuery += $"and PRODUCT_ID = '151202'"; //151202 01 - SAVINGS DEP (PHP) CORP PB
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
                            if (reader.Read())
                            {
                                sQuery = $"select CREATEDON from cmscodes where CMSCode = '{pCMSCode}'";
                                using (SqlConnection con = new SqlConnection(SharedFunctions.Connectionstring))
                                {
                                    using (SqlCommand sqlcmd = con.CreateCommand())
                                    {
                                        sqlcmd.CommandText = sQuery;
                                        con.Open();
                                        using (SqlDataReader sqlreader = sqlcmd.ExecuteReader())
                                        {
                                            if (sqlreader.Read())
                                            {
                                                dtEnrolled = sqlreader.GetDateTime(0);
                                            }
                                        }
                                    }
                                }
                                details.AccountNumber = reader["ACCOUNT_NUMBER"].ToString();
                                details.AccountName = reader["CUST_NAME"].ToString();
                                details.BranchName = reader["BRANCH_NAME"].ToString();
                                details.BranchCode = reader["BRANCH"].ToString();
                                details.InvestmentDesc = reader["PRODUCT_NAME"].ToString();
                                details.Status = reader["ACCOUNT_STAT_DESC"].ToString();
                            }
                        }
                    }
                }
            }
            catch(Exception ex) { }

            return details;
        }

        public List<Details> GetPayrollCodeList() //get used paycodes ods
        {
            List<Details> list = new List<Details>();
            string sQuery = "select PAYROLL_CODE from CTBC_DEPOSIT_ACCOUNTS where PRODUCT_ID = '151202' and PAYROLL_CODE IS NOT NULL";
            using(OracleConnection con = new OracleConnection(SharedFunctions.ODSConnectionString))
            {
                using (OracleCommand cmd = con.CreateCommand())
                {
                    con.Open();
                    cmd.CommandText = sQuery;
                    using(OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            list.Add(new Details()
                            {
                                PayrollCode = reader.GetString(0)
                            });
                        }
                    }
                }
            }
            return list;
        }

        public List<Details> GetList(int pCMSCode)
        {
            List<Details> list = new List<Details>();
            DateTime dtEnrolled = new DateTime();
            string sQuery = "select ACCOUNT_NUMBER, CUST_NAME, BRANCH, BRANCH_NAME, PRODUCT_ID, PRODUCT_NAME, ACCOUNT_STAT, ACCOUNT_STAT_DESC from CTBC_DEPOSIT_ACCOUNTS ";
            sQuery += $"where PAYROLL_CODE = '{pCMSCode}' and ACCOUNT_NUMBER is not null ";
            sQuery += $"and PRODUCT_ID = '151202'"; //151202 01 - SAVINGS DEP (PHP) CORP PB
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
                                sQuery = $"select CREATEDON from cmscodes where CMSCode = '{pCMSCode}'";
                                using(SqlConnection con = new SqlConnection(SharedFunctions.Connectionstring))
                                {
                                    using(SqlCommand sqlcmd = con.CreateCommand())
                                    {
                                        sqlcmd.CommandText = sQuery;
                                        con.Open();
                                        using(SqlDataReader sqlreader = sqlcmd.ExecuteReader())
                                        {
                                            if (sqlreader.Read())
                                            {
                                                dtEnrolled = sqlreader.GetDateTime(0);
                                            }
                                        }
                                    }
                                }
                                list.Add(new Details()
                                {
                                    AccountNumber = reader["ACCOUNT_NUMBER"].ToString(),
                                    AccountName = reader["CUST_NAME"].ToString(),
                                    BranchName = reader["BRANCH_NAME"].ToString(),
                                    DateEnrolled = dtEnrolled.ToShortDateString(),
                                    BranchCode = reader["BRANCH"].ToString(),
                                    InvestmentDesc = reader["PRODUCT_NAME"].ToString(),
                                    Status = reader["ACCOUNT_STAT_DESC"].ToString()
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