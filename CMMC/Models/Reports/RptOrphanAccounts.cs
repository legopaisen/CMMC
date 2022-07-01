using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CMMC.Models.Reports
{
    public class RptOrphanAccountsModel
    {
        public string Branch { get; set; }
        public string CMSCode { get; set; }
        public string InvestorType { get; set; }
        public string CIFNumber{ get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
    }

    public class RptOrphanAccounts
    {
        public List<RptOrphanAccountsModel> GetOrphanAccountsList(string pBranch, DateTime? pStartDate = null, DateTime? pEndDate = null)
        {
            List<RptOrphanAccountsModel> list = new List<RptOrphanAccountsModel>();
            string sQuery = @"select BR.BranchName, CMA.ac_PayCode, CMA.ac_InvestmentType, 
                                CMA.ac_CustNo, CMA.ac_AccountNo, AI.AccountName
                                from cmmc_accounts CMA
                                INNER JOIN Branches BR ON BR.branchcode = CMA.ac_BranchCode 
                                INNER JOIN AccountInformations AI ON CMA.ac_AccountNo = AI.AccountNo
                                where (CMA.ac_InvestmentType <> '0701' and
                                CMA.ac_InvestmentType <> '0702' and
                                CMA.ac_InvestmentType <> '0705') and
                                CMA.ac_PayCode != '0'
                                OR
                                ((CMA.ac_InvestmentType = '0701' OR
                                CMA.ac_InvestmentType = '0702' OR
                                CMA.ac_InvestmentType = '0703') AND
                                CMA.ac_PayCode = '0')";
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
                            list.Add(new RptOrphanAccountsModel()
                            {
                                Branch = reader.GetString(0),
                                CMSCode = reader.GetString(1),
                                InvestorType = reader.GetString(2),
                                CIFNumber = reader.GetString(3),
                                AccountNumber = reader.GetString(4),
                                AccountName = reader.GetString(5),
                            });
                        }
                    }
                }
            }
            return list;
        }
    }
}