using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CMMC.Models.Reports
{
    public class RptForeignCIFNumberModel
    {
        public string Branch { get; set; }
        public int CMSCode { get; set; }
        public string CIFNumber { get; set; }
        public string AccountNo { get; set; }
        public string CustomerName { get; set; }
    }

    public class RptForeignCIFNumber
    {
        public List<RptForeignCIFNumberModel> GetForeignAccountsList(string pBranch, DateTime? pStartDate = null, DateTime? pEndDate = null)
        {
            List<RptForeignCIFNumberModel> list = new List<RptForeignCIFNumberModel>();
            string sQuery = @"select CMA.ac_BranchCode, CMA.ac_PayCode, CMA.ac_CustNo, CMA.ac_AccountNo from cmmc_accounts CMA
                                where CMA.ac_InvestmentType = '0701' ";
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
                            list.Add(new RptForeignCIFNumberModel()
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