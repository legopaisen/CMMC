using System;
using System.Collections.Generic;
using System.Data.OracleClient;
using System.Linq;
using System.Web;

namespace CMMC.Models
{
    public class ODSADBBalance
    {
        public struct ODSADBBalanceModel
        {
            public string ACNT_POS_OD { get; set; }
            public string MVMNT_GEN_DT { get; set; }
            public string OU_ID { get; set; }
            public string DM_LSTUPDDT { get; set; }
            public string MVMMNT_AMT { get; set; }
            public string MVMNT_CRNCY { get; set; }
        }

        public ODSADBBalanceModel Fill(string sID, string sMonth)
        {
            OracleConnection oracon = new OracleConnection(SharedFunctions.OracleConnection);
            ODSADBBalanceModel model = new ODSADBBalanceModel();
            string sQuery = "select ACNT_POS_ID, MVMNT_GEN_DT, OU_ID, DM_LSTUPDDT, MVMMNT_AMT, MVMNT_CRNCY FROM BANCS_ACCOUNT BALANCE";
            sQuery += $" where ACNT_POS_ID = '{sID}' and EXTRACT(month from MVMNT_GEN_DT) = '{sMonth}' and EXTRACT(year from MVMNT_GEN_DT) = '{DateTime.Now.Year.ToString()}'";


            return model;
        }
    }
    
}