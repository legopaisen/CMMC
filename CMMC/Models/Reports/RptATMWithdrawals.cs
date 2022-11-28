using System;
using System.Collections.Generic;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CMMC.Models.Reports
{
    public class RptATMWithdrawalsModel
    {
        public string ChildAccount { get; set; }
        public string ChildAccountName { get; set; }
        public string CardNumber { get; set; }
        public string PCCC { get; set; }
        public string Branch { get; set; }
        public string CMSCode { get; set; }
        public string CustomerName { get; set; }
        public string TransactionCodeDesc { get; set; }
        public int TransactionAmount { get; set; }
        public string TransactionDateTime { get; set; }
        public string ATMTerminalIDDesc{ get; set; }
        public string NoOfFreeWithdrawals { get; set; }
    }
    
    public class RptATMWithdrawals
    {
        public List<RptATMWithdrawalsModel> GetATMWithdrawalsList(string pBranch, DateTime? pStartDate = null, DateTime? pEndDate = null)
        {
            List<RptATMWithdrawalsModel> list = new List<RptATMWithdrawalsModel>();
            string sMaxWithdrawals = string.Empty;
            string sQuery = @"SELECT 
                                AV.SOURCE_ACCOUNT_NUMBER,
                                BD.NM1,
                                AV.CARD_NUMBER,
                                DA.PCCC_CODE,
                                DA.BRANCH_NAME,
                                DA.PAYROLL_CODE,
                                BD.NM1,
                                AV.TRANSACTION_AMOUNT,
                                AV.TRANSACTION_LOCAL_DATE,
                                AV.CARD_ACCEPTOR_ID
                                FROM C##PCARD.V_AUTHO_ACTIVITY_VIEW AV, c##GBPDEV1.BANCS_CORPORATE_DETAILS BD, c##GBPDEV1.CTBC_DEPOSIT_ACCOUNTS DA
                                WHERE AV.SOURCE_ACCOUNT_NUMBER = DA.ACCOUNT_NUMBER
                                AND DA.POS_ID = BD.BP_ID
                                AND AV.ACTION_CODE = '000'
                                AND AV.SOURCE_ACCOUNT_TYPE = '10'
                                AND AV.BILLING_CURRENCY    = '608'
                                AND AV.PROCESSING_CODE = '01' ";
            sQuery += $"AND TO_CHAR(AV.TRANSACTION_LOCAL_DATE, 'DD-MON-RR') BETWEEN '{pStartDate.Value.ToShortDateString()}' AND '{pEndDate.Value.ToShortDateString()}' ";
            sQuery += $"AND DA.BRANCH_NAME =  '{pBranch}' ";

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
                            string s = reader.GetString(0);
                            using(SqlConnection sqlcon = new SqlConnection(SharedFunctions.Connectionstring))
                            {
                                string w = reader.GetString(0);
                                using(SqlCommand sqlcmd = sqlcon.CreateCommand())
                                {
                                    sqlcon.Open();
                                    sqlcmd.CommandText = "select MaxWithdrawalPaidByEmployer from cmscodes";
                                    using(SqlDataReader sqlreader = sqlcmd.ExecuteReader())
                                    {
                                        if (sqlreader.Read())
                                        {
                                            sMaxWithdrawals = sqlreader.GetString(0);
                                        }
                                    }    
                                }
                            }

                            list.Add(new RptATMWithdrawalsModel()
                            {
                                ChildAccount = reader["SOURCE_ACCOUNT_NUMBER"].ToString(),
                                ChildAccountName = reader["NM1"].ToString(),
                                CardNumber = reader["CARD_NUMBER"].ToString(),
                                PCCC = reader["PCCC_CODE"].ToString(),
                                Branch = reader["BRANCH_NAME"].ToString(),
                                CMSCode = reader["PAYROLL_CODE"].ToString(),
                                CustomerName = reader["NM1"].ToString(),
                                TransactionCodeDesc = reader["PROCESSING_CODE"].ToString(),
                                TransactionAmount = Convert.ToInt32(reader["TRANSACTION_AMOUNT"]),
                                TransactionDateTime = reader["TRANSACTION_LOCAL_DATE"].ToString(),
                                ATMTerminalIDDesc = reader["CARD_ACCEPTOR_ID"].ToString(),
                                NoOfFreeWithdrawals = sMaxWithdrawals
                            });
                        }
                    }

                    return list;
                }
            }
        }
    }
}