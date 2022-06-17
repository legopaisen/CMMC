using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMMC.Models.Reports
{
    public class RptADBPerformanceModel
    {
        public string AccountNumber { get; set; }
        public decimal RequiredADB { get; set; }
        public decimal MTDADB { get; set; }
        public decimal AggregateADB { get; set; }
        public string SubAccounts { get; set; }
        public decimal RequiredSubAccountADB { get; set; }
        public decimal TotRequiredFixedADB { get; set; }
        public decimal TotRequiredSubAccADB { get; set; }
    }
    
    public class RptADBPerformance
    {
        public List<RptADBPerformanceModel> GetADBPerformanceList(string pBranch, DateTime? pStartDate = null, DateTime? pEndDate = null)
        {
            List<RptADBPerformanceModel> list = new List<RptADBPerformanceModel>();

            return list;
        }
    }
}