using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMMC.Models.Reports
{
    public class RptNoLinkedAccountsModel
    {
        public int CMSCode { get; set; }
        public string AutoDebit { get; set; }
        public string AccountNo { get; set; }
        public string CustomerName { get; set; }
        public decimal TotRequiredADB { get; set; }
    }

    public class RptNoLinkedAccounts
    {
        public List<RptNoLinkedAccountsModel> GetNoLinkedAccountsList(string pBranch, DateTime? pStartDate = null, DateTime? pEndDate = null)
        {
            List<RptNoLinkedAccountsModel> list = new List<RptNoLinkedAccountsModel>();

            return list;
        }
    }
}