using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMMC.Models.Reports
{
    public class RptSubAccountsCountModel
    {
        public string Branch { get; set; }
        public int CMSCode { get; set; }
        public string CustomerName { get; set; }
        public int SubAccCASACount{ get; set; }
        public int SubAccCashCardCASACount { get; set; }
    }

    public class RptSubAccountsCount
    {
        public List<RptSubAccountsCountModel> GetSubAccountsList(string pBranch, DateTime? pStartDate = null, DateTime? pEndDate = null)
        {
            List<RptSubAccountsCountModel> list = new List<RptSubAccountsCountModel>();

            return list;
        }
    }
}