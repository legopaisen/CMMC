using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMMC.Models.Reports
{
    public class RptAlienSubAccountsModel
    {
        public int CMSCode { get; set; }
        public string Branch { get; set; }
        public int CountPerBranch { get; set; }
    }

    public class RptAlienSubAccounts
    {
        public List<RptAlienSubAccountsModel> GetAlienSubAccountsList(string pBranch, DateTime? pStartDate = null, DateTime? pEndDate = null)
        {
            List<RptAlienSubAccountsModel> list = new List<RptAlienSubAccountsModel>();

            return list;
        }
    }
}