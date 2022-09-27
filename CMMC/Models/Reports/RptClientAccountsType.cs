using System;
using System.Collections.Generic;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CMMC.Models.Reports
{
    public class RptClientAccountsTypeModel
    {
        public int CMSCode { get; set; }
        public string CustomerName { get; set; }
        public string Branch { get; set; }
        public string AccountType { get; set; }
        public long AccountNo { get; set; }
        public string AutoDebit { get; set; }
    }

    public class RptClientAccountsType
    {
        public List<RptClientAccountsTypeModel> GetClientAccountsList(string pClientsAccountType, DateTime? pStartDate = null, DateTime? pEndDate = null)
        {
            List<RptClientAccountsTypeModel> list = new List<RptClientAccountsTypeModel>();
            string sQuery = @"select CMS.CMSCode, CMS.Description, BR.BranchName, AI.Tagging, AI.AccountNo, CMS.IsAutoDebit from cmscodes CMS
                                INNER JOIN Branches BR ON BR.BranchCode = CMS.BranchCode
                                INNER JOIN AccountInformations AI ON AI.CMSCode = CMS.CMSCode
                                where AI.Tagging = '" + pClientsAccountType + "'" +
                                "order by AI.Tagging, CMS.CMSCode";
            using (SqlConnection con = new SqlConnection(SharedFunctions.Connectionstring))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();
                    cmd.CommandText = sQuery;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string sAutoDebit = string.Empty;
                            int iCMSCode = reader.GetInt32(0);
                            int iAutoDebit = Convert.ToInt32(reader.GetValue(5));
                            if (iAutoDebit == 0)
                                sAutoDebit = "N";
                            else
                                sAutoDebit = "Y";

                            using(OracleConnection oracon = new OracleConnection(SharedFunctions.OracleConnection))
                            {
                                using(OracleCommand oracmd = oracon.CreateCommand())
                                {
                                    oracon.Open();
                                    sQuery = $"select ACCOUNT_NUMBER from CTBC_DEPOSIT_ACCOUNTS where PAYROLL_CODE = '{iCMSCode}'";

                                }
                            }

                            list.Add(new RptClientAccountsTypeModel()
                            {
                                CMSCode = iCMSCode,
                                CustomerName = reader.GetString(1),
                                Branch = reader.GetString(2),
                                AccountType = reader.GetString(3),
                                AccountNo = reader.GetInt64(4),
                                AutoDebit = sAutoDebit,
                            });
                        }
                    }
                }
            }
            return list;
        }
    }
}