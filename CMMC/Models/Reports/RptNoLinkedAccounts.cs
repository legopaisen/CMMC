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
            string sQuery = "";
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
                            list.Add(new RptNoLinkedAccountsModel()
                            {
                                CMSCode = reader.GetInt32(0),
                                AutoDebit = reader.GetString(1),
                                AccountNo = "NO LINKED ACCOUNTS",
                                CustomerName = reader.GetString(3),
                                TotRequiredADB = reader.GetDecimal(4)
                            });
                        }
                    }
                }
            }
                return list;
        }
    }
}