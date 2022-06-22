using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMMC.Models.Reports
{
    public class RptMonthlyADBPenFeeModel
    {
        public int CMSCode { get; set; }
        public int PCCC { get; set; }
        public string CustomerName { get; set; }
        public string AccountNumber { get; set; }
        public decimal RequiredADB { get; set; }
        public decimal MTDADBAccount { get; set; }
        public decimal AggregateADBAccounts { get; set; }
        public string SubAccounts { get; set; }
        public decimal RequiredSubAccountADB { get; set; }
        public decimal TotRequiredFixedADB { get; set; }
        public decimal TotRequiredSubAccADB { get; set; }
        public decimal PenFeePerAccount { get; set; }
        public decimal TotPenaltyFee { get; set; }
        public decimal ActualAmountDebited { get; set; }
        public decimal AmountNotDebited { get; set; }
        public string AutoDebit { get; set; }
    }

        public class RptMonthlyADBPenFee
    {
        public List<RptMonthlyADBPenFeeModel> GetMonthlyADBPenFeeList(string pBranch, DateTime? pStartDate = null, DateTime? pEndDate = null)
        {
            List<RptMonthlyADBPenFeeModel> list = new List<RptMonthlyADBPenFeeModel>();

            return list;
        }
    }
}