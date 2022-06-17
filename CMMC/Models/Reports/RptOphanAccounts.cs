using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMMC.Models.Reports
{
    public class RptOphanAccountsModel
    {
        public string Branch { get; set; }
        public int CMSCode { get; set; }
        public string InvestorType { get; set; }
        public int CIFNumber{ get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
    }

    public class RptOphanAccounts
    {
        public List<RptOphanAccountsModel> GetOphanAccountsList(string pBranch, DateTime? pStartDate = null, DateTime? pEndDate = null)
        {
            List<RptOphanAccountsModel> list = new List<RptOphanAccountsModel>();

            return list;
        }
    }
}