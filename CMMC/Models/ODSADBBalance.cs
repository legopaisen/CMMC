using System;
using System.Collections.Generic;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CMMC.Models
{
    public class ODSADBBalance
    {
        public struct ODSADBBalanceModel
        {
            public string ACNT_POS_ID { get; set; }
            public string MVMNT_GEN_DT { get; set; }
            public string OU_ID { get; set; }
            public string DM_LSTUPDDT { get; set; }
            public decimal MVMMNT_AMT { get; set; }
            public string MVMNT_CRNCY { get; set; }
        }

        public decimal GetUncollectedFees(string sID)
        {
            decimal dAmt = 0;
            SqlConnection con = new SqlConnection(SharedFunctions.Connectionstring);
            string sQuery = "select UncollectedFees from CMMC_Uncollected_Fees";
            sQuery += $" where AccountPOS_ID = '{sID}'";

            using(SqlCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = sQuery;
                con.Open();
                using(SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        decimal.TryParse(reader["UncollectedFees"].ToString(), out dAmt);
                    }
                }
            }

            return dAmt;
        }
        public void DeleteUncollectedAmount(string sID)
        {
            SqlConnection con = new SqlConnection(SharedFunctions.Connectionstring);
            string sQuery = $"DELETE FROM CMMC_Uncollected_Fees where AccountPOS_ID = '{sID}'";
            
            using(SqlCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = sQuery;
                con.Open();
                if(cmd.ExecuteNonQuery() == 0)
                { }
            }
        }
        public void InsertUncollectedAmount(string sID, decimal dAmt)
        {
            string dtToday = DateTime.Now.ToString("yyyy-MM-dd");
            SqlConnection con = new SqlConnection(SharedFunctions.Connectionstring);
            string sQuery = $"INSERT INTO CMMC_Uncollected_Fees (AccountPOS_ID, ChargingDate, UncollectedFees)";
            sQuery += $" values('{sID}', '{dtToday}', '{dAmt}')";
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = sQuery;
                con.Open();
                if (cmd.ExecuteNonQuery() == 0)
                { }
            }
        }

        public decimal ComputePenalty(string sPosID, decimal dBalance, int iCMSCode)
        {
            CMSCode.Details cmsdetails = new CMSCode.Details();
            CMSCode cmsmodel = new CMSCode();
            cmsdetails = cmsmodel.Fill(iCMSCode);

            decimal dNewUncollectedFee = 0;
            decimal dPrevUncollectedFee = GetUncollectedFees(sPosID);

            if (dBalance < (cmsdetails.PenaltyFee + dPrevUncollectedFee))
            {
                dBalance -= cmsdetails.PenaltyFee + dPrevUncollectedFee;
                dNewUncollectedFee = Math.Abs(dBalance);

                DeleteUncollectedAmount(sPosID);
                InsertUncollectedAmount(sPosID, dNewUncollectedFee);
                dBalance = 0;
            }
            else
                dBalance = dBalance + dPrevUncollectedFee;

            return dBalance;
        }

        public ODSADBBalanceModel GetADB(string sPosID, string sMonth, int iCMSCode)
        {
            CMSCode.Details cmsdetails = new CMSCode.Details();
            CMSCode cmsmodel = new CMSCode();
            cmsdetails = cmsmodel.Fill(iCMSCode);

            ODSADBBalanceModel adbModel = new ODSADBBalanceModel();
            string sQuery = "select ACNT_POS_ID, OU_ID, SUM(MVMNT_AMT)/COUNT(MVMNT_AMT) as MVMNT_AMT, MVMNT_CRNCY FROM BANCS_ACCOUNT_BALANCE";
            sQuery += $" where ACNT_POS_ID = '{sPosID}' and EXTRACT(month from MVMNT_GEN_DT) = '{sMonth}' and EXTRACT(year from MVMNT_GEN_DT) = '{DateTime.Now.Year.ToString()}'";
            sQuery += $" group by ACNT_POS_ID, OU_ID, MVMNT_CRNCY";

            using (OracleConnection oracon = new OracleConnection(SharedFunctions.ODSConnectionString))
            {
                using(OracleCommand cmd = oracon.CreateCommand())
                {
                    cmd.CommandText = sQuery;
                    oracon.Open();
                    using(OracleDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            decimal dBalance = 0;
                            decimal dPrevUncollectedFee = GetUncollectedFees(sPosID);
                            decimal.TryParse(reader["MVMNT_AMT"].ToString(), out dBalance);
                            adbModel.MVMMNT_AMT = dBalance;
                            adbModel.ACNT_POS_ID = reader["ACNT_POS_ID"].ToString();
                            adbModel.OU_ID = reader["OU_ID"].ToString();
                            adbModel.MVMNT_CRNCY = reader["MVMNT_CRNCY"].ToString();

                            if (dBalance < (cmsdetails.PenaltyFee + dPrevUncollectedFee))
                            {
                                adbModel.MVMMNT_AMT = 0;
                                dBalance -= cmsdetails.PenaltyFee + dPrevUncollectedFee;
                                decimal dNewUncollectedFee = Math.Abs(dBalance);

                                DeleteUncollectedAmount(sPosID);
                                InsertUncollectedAmount(sPosID, dNewUncollectedFee);
                            }
                            else
                                adbModel.MVMMNT_AMT = dBalance + dPrevUncollectedFee;
                        }
                    }
                }
            }
            return adbModel;
        }
    }
    
}