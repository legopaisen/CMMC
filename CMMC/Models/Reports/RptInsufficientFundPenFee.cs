using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMMC.Models.Reports
{
    public class RptInsufficientFundPenFeeModel
    {
        public int CMSCode { get; set; }
        public string AccountNo { get; set; }
        public string Branch { get; set; }
        public string CustomerName { get; set; }
        public decimal TotRequiredADB { get; set; }
        public decimal ActualADB { get; set; }
        public int CMSADBPenaltyFee { get; set; }
        public decimal ActualADBPrevious { get; set; }
        public decimal DeductedBalance { get; set; }
        public decimal UncollectedPenFee { get; set; }
    }

    public class RptInsufficientFundPenFee
    {
        public List<RptInsufficientFundPenFeeModel> GetInsufficientFundPenFeeList(string pBranch, DateTime? pStartDate = null, DateTime? pEndDate = null)
        {
            List<RptInsufficientFundPenFeeModel> list = new List<RptInsufficientFundPenFeeModel>();

            return list;
        }
    }
}