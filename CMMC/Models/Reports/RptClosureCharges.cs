using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMMC.Models.Reports
{
    public class RptClosureChargesModel
    {
        public DateTime Date { get; set; }
        public decimal RequiredADB { get; set; }
        public int ADBPenaltyFee { get; set; }
        public int AmountDebited { get; set; }
        public int AmountNotDebited { get; set; }
    }

    public class RptClosureCharges
    {
        public List<RptClosureChargesModel> GetClosureChargesList(string pBranch, DateTime? pStartDate = null, DateTime? pEndDate = null)
        {
            List<RptClosureChargesModel> list = new List<RptClosureChargesModel>();

            return list;
        }
    }
}