using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CMMC.Models.Reports
{
    public class RptNoLinkedAccountsModel
    {
        public int CMSCode { get; set; }
        public string AutoDebit { get; set; }
        public string AccountNo { get; set; }
        public string CustomerName { get; set; }
        public decimal TotRequiredADB { get; set; }
    }

    public class RptNoLinkedAccounts
    {
        public List<RptNoLinkedAccountsModel> GetNoLinkedAccountsList(string pBranch, DateTime? pStartDate = null, DateTime? pEndDate = null)
        {
            List<RptNoLinkedAccountsModel> list = new List<RptNoLinkedAccountsModel>();
            
            string sQuery = @"select CMS.CMSCode, CMS.IsAutoDebit, cms.Description, sum(SA.MotherRequiredADB) as MotherRequiredADB from cmscodes CMS
                            INNER JOIN ServicesAvailment SA ON SA.CMSCode = CMS.CMSCode
                            where NOT EXISTS(select * from AccountInformations AI where AI.CMSCode = CMS.CMSCode and investmentCode = '0701')
                            group by CMS.CMSCode, CMS.IsAutoDebit, cms.Description
                            order by CMS.CMSCode";
            using (SqlConnection con = new SqlConnection(SharedFunctions.Connectionstring))
            {
                using(SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();
                    cmd.CommandText = sQuery;
                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string sAutoDebit = string.Empty;
                            int iAutoDebit = Convert.ToInt32(reader.GetValue(1));
                            if (iAutoDebit == 0)
                                sAutoDebit = "N";
                            else
                                sAutoDebit = "Y";

                            list.Add(new RptNoLinkedAccountsModel()
                            {
                                CMSCode = reader.GetInt32(0),
                                AutoDebit = sAutoDebit,
                                AccountNo = "NO LINKED ACCOUNTS",
                                CustomerName = reader.GetString(2),
                                TotRequiredADB = reader.GetDecimal(3)
                            });
                        }
                    }
                }
            }
                return list;
        }
    }
}