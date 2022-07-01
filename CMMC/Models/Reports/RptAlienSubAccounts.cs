using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CMMC.Models.Reports
{
    public class RptAlienSubAccountsModel
    {
        public int CMSCode { get; set; }
        public string Branch { get; set; }
        public int CountPerBranch { get; set; }
    }

    public class RptAlienSubAccounts
    {
        public List<RptAlienSubAccountsModel> GetAlienSubAccountsList(string pBranch, DateTime? pStartDate = null, DateTime? pEndDate = null)
        {
            List<RptAlienSubAccountsModel> list = new List<RptAlienSubAccountsModel>();
            string sQuery = @"select A.ac_PayCode, A.BranchName, count(A.ac_paycode) as count from (
                            select CA.ac_PayCode, BR.BranchName, count(ac_paycode) as count  from cmmc_accounts CA
                            INNER JOIN BRANCHES BR ON CA.ac_BranchCode = BR.BranchCode
                            where CA.ac_paycode != '0'
                            and CA.ac_InvestmentType = '0702'
                            group by CA.ac_PayCode, BR.BranchName) A
                            group by A.ac_PayCode, A.BranchName
                            HAVING COUNT(A.ac_paycode) > 1
                            order by A.ac_paycode";
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
                            list.Add(new RptAlienSubAccountsModel()
                            {
                                CMSCode = reader.GetInt32(0),
                                Branch = reader.GetString(1),
                                CountPerBranch = reader.GetInt32(2),
                            });
                        }
                    }
                }
            }
            return list;
        }
    }
}