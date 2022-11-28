using System;
using System.Collections.Generic;
using System.Data.OracleClient;
using System.Linq;
using System.Web;

namespace CMMC.Models.Reports
{
    public class RptADBPerformanceModel
    {
        public string AccountNumber { get; set; }
        public decimal RequiredADB { get; set; }
        public decimal MTDADB { get; set; }
        public decimal AggregateADB { get; set; }
        public string SubAccounts { get; set; }
        public decimal RequiredSubAccountADB { get; set; }
        public decimal TotRequiredFixedADB { get; set; }
        public decimal TotRequiredSubAccADB { get; set; }
    }
    
    public class RptADBPerformance
    {
        public List<RptADBPerformanceModel> GetADBPerformanceList(string pCmsCode, DateTime? pStartDate = null, DateTime? pEndDate = null)
        {
            List<RptADBPerformanceModel> list = new List<RptADBPerformanceModel>();
            string sQuery = "";

            //sQuery = "select ACNT_POS_ID, OU_ID, SUM(MVMNT_AMT)/COUNT(MVMNT_AMT) as MVMNT_AMT, MVMNT_CRNCY FROM BANCS_ACCOUNT_BALANCE";
            //sQuery += $" where ACNT_POS_ID = '{sPosID}' and EXTRACT(month from MVMNT_GEN_DT) = '{sMonth}' and EXTRACT(year from MVMNT_GEN_DT) = '{sYear}'";
            //sQuery += $" group by ACNT_POS_ID, OU_ID, MVMNT_CRNCY";

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

                            list.Add(new RptADBPerformanceModel()
                            {

                            });
                        }
                    }

                }
            }
            return list;
        }
    }
}