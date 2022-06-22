using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CMMC.Models.Reports
{
    public class RptTotADBRequirementModel
    {
        public int CMSCode { get; set; }
        public string AutoDebit { get; set; }
        public long AccountNo { get; set; }
        public string Branch { get; set; }
        public string CustomerName { get; set; }
        public string EnrolledServices { get; set; }
        public DateTime DateEnrolled { get; set; }
        public decimal MotherRequiredADB { get; set; }
        public decimal SubRequiredADB { get; set; }
        public int EmployeeCount { get; set; }
        public decimal TotRequiredADB { get; set; }
    }

    public class RptTotADBRequirement
    {
        public List<RptTotADBRequirementModel> GetTotADBRequirementList(string pBranch, DateTime? pStartDate = null, DateTime? pEndDate = null)
        {
            List<RptTotADBRequirementModel> list = new List<RptTotADBRequirementModel>();
            List<RptTotADBRequirementModel> listReturn = new List<RptTotADBRequirementModel>();
            string sQuery = @"select CMS.CMSCode, CMS.IsAutoDebit, AI.AccountNo, BR.BranchName, CMS.Description, CMS.CreatedOn, 
                            sum(SV.MotherRequiredADB) as MotherRequiredADB, sum(SV.SubRequiredADB) as SubRequiredADB, sum(SV.MinNumberEmployee) as MinNumberEmployee, sum(SV.MotherRequiredADB + SV.SubRequiredADB) as TotRequiredADB from cmscodes CMS
                            INNER JOIN BRANCHES BR ON BR.BranchCode = CMS.BranchCode
                            INNER JOIN AccountInformations AI ON AI.CMSCode = CMS.CMSCode
                            INNER JOIN ServicesAvailment SV ON SV.CMSCode = CMS.CMSCode
                            group by CMS.CMSCode, CMS.IsAutoDebit, AI.AccountNo, BR.BranchName, CMS.Description, CMS.CreatedOn
                            order by CMS.CMSCode";
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
                            int iAutoDebit = Convert.ToInt32(reader.GetValue(1));
                            if (iAutoDebit == 0)
                                sAutoDebit = "N";
                            else
                                sAutoDebit = "Y";

                            list.Add(new RptTotADBRequirementModel()
                            {
                                CMSCode = reader.GetInt32(0),
                                AutoDebit = sAutoDebit,
                                AccountNo = reader.GetInt64(2),
                                Branch = reader.GetString(3),
                                CustomerName = reader.GetString(4),
                                EnrolledServices = "",
                                DateEnrolled = reader.GetDateTime(5),
                                MotherRequiredADB = reader.GetDecimal(6),
                                SubRequiredADB = reader.GetDecimal(7),
                                EmployeeCount = reader.GetInt32(8),
                                TotRequiredADB = reader.GetDecimal(9)
                            });
                        }
                    }

                    foreach (var item in list)
                    {
                        string sServices = string.Empty;
                        cmd.CommandText = $"select sv.ServiceName from servicesavailment SA INNER JOIN services SV ON SV.ServiceID = SA.ServiceID where SA.CMSCode = '{item.CMSCode}'";
                        using (SqlDataReader reader2 = cmd.ExecuteReader())
                        {
                            while (reader2.Read())
                            {
                                sServices += "• " + reader2.GetString(0) + "\n";
                            }
                        }

                        listReturn.Add(new RptTotADBRequirementModel()
                        {
                            CMSCode = item.CMSCode,
                            AutoDebit = item.AutoDebit,
                            AccountNo = item.AccountNo,
                            Branch = item.Branch,
                            CustomerName = item.CustomerName,
                            EnrolledServices = sServices,
                            DateEnrolled = item.DateEnrolled,
                            MotherRequiredADB = item.MotherRequiredADB,
                            SubRequiredADB = item.SubRequiredADB,
                            EmployeeCount = item.EmployeeCount,
                            TotRequiredADB = item.TotRequiredADB
                        });
                    }
                }
            }
            return listReturn;
        }
    }
}