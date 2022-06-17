using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMMC.Models.Reports
{
    public class RptUncollectedPenFeeModel
    {
        public int CMSCode { get; set; }
        public string AccountNo { get; set; }
        public string Branch { get; set; }
        public decimal TotRequiredADB { get; set; }
        public decimal ActualADB { get; set; }
        public int CMSPenaltyFee { get; set; }
        public string CustomerName { get; set; }
    }

    public class RptUncollectedPenFee
    {
        public List<RptUncollectedPenFeeModel> GetUncollectedPenFeeList(string pBranch, DateTime? pStartDate = null, DateTime? pEndDate = null)
        {
            List<RptUncollectedPenFeeModel> list = new List<RptUncollectedPenFeeModel>();

            return list;
        }
    }
}