using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMMC.Models.Reports
{
    public class RptTotADBRequirementModel
    {
        public int CMSCode { get; set; }
        public string AutoDebit { get; set; }
        public string AccountNo { get; set; }
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

            return list;
        }
    }
}