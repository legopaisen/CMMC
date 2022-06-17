using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMMC.Models.Reports
{
    public class RptCustomerProfileModel
    {
        public string StatusFlag { get; set; }
        public decimal RequiredADB { get; set; }
        public string AggregateFlag{ get; set; }
        public decimal TotalRequiredFixedADB { get; set; }
        public decimal RequiredSubAccountADB { get; set; }
        public int CMSCode { get; set; }
        public string DebitAccountNo { get; set; }
        public string HigherFlag { get; set; }
        public string AutoDebit { get; set; }
        public string ServicesAvailed { get; set; }
        public DateTime DateEnabled{ get; set; }
    }

    public class RptCustomerProfile
    {
        public List<RptCustomerProfileModel> GetCustomerProfileList(string pBranch, DateTime? pStartDate = null, DateTime? pEndDate = null)
        {
            List<RptCustomerProfileModel> list = new List<RptCustomerProfileModel>();

            return list;
        }
    }
}