using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMMC.Models.Reports
{
    public class RptCMSAccountsModel
    {
        public int PCCC { get; set; }
        public string Branch { get; set; }
        public int CMSCode { get; set; }
        public string InvestorType { get; set; }
        public string CIFNumber { get; set; }
        public string AccountNumber { get; set; }
        public string AccountShortName { get; set; }
        public string AccountFirstName { get; set; }
        public string AccountMiddleName { get; set; }
        public string AccountLastName { get; set; }
        public string CurrentEmployer { get; set; }
        public string CMSAccountType { get; set; }
        public string AccountStatus { get; set; }
    }

    public class RptCMSAccounts
    {
        public List<RptCMSAccountsModel> GetCMSAccountsList(string pBranch, DateTime? pStartDate = null, DateTime? pEndDate = null)
        {
            List<RptCMSAccountsModel> list = new List<RptCMSAccountsModel>();

            return list;
        }
    }
}