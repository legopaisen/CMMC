using System;
using System.Collections.Generic;
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

            return list;
        }
    }
}