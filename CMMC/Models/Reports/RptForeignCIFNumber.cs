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
        public string CMSCode { get; set; }
        public string CIFNumber { get; set; }
        public string AccountNo { get; set; }
        public string CustomerName { get; set; }
    }

    public class RptForeignCIFNumber
    {
        public List<RptForeignCIFNumberModel> GetForeignCIFNumberList(string pBranch, DateTime? pStartDate = null, DateTime? pEndDate = null)
        {
            List<RptForeignCIFNumberModel> list = new List<RptForeignCIFNumberModel>();
            List<string> lstPaycode = new List<string>();
            string sQuery = @"select ac_paycode from cmmc_accounts where ac_investmentType = '0701' and ac_PayCode != '0'";
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
                            lstPaycode.Add(reader.GetString(0));
                        }
                    }

                    foreach(var item in lstPaycode)
                    {
                        sQuery = @"SELECT BR.BranchName, CMA.ac_paycode, CMA.ac_custNo, CMA.ac_accountNo, CONCAT(CUS.cu_Name1, CUS.cu_Name2)
                            FROM cmmc_accounts CMA
                            INNER JOIN Branches BR ON BR.BranchCode = CMA.ac_BranchCode
                            INNER JOIN cmmc_customer CUS ON CUS.cu_custNo = CMA.ac_custNo
                            WHERE ac_custNo NOT IN (
                                SELECT ac_custNo
                                FROM cmmc_accounts
                                GROUP BY ac_custNo
                                HAVING COUNT(*) > 1
                            )";
                        sQuery += $"and ac_paycode = '{item}' and ac_investmentType = '0701'";

                        cmd.CommandText = sQuery;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(new RptForeignCIFNumberModel()
                                {
                                    Branch = reader.GetString(0),
                                    CMSCode = reader.GetString(1),
                                    CIFNumber = reader.GetString(2),
                                    AccountNo = reader.GetString(3),
                                    CustomerName = reader.GetString(4)
                                });
                            }
                        }
                    }
                }
            }
            return list;
        }
    }
}