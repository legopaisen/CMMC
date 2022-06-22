using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMMC.Models.Reports
{
    public class RptClientAccountsTypeModel
    {
        public int CMSCode { get; set; }
        public string CustomerName { get; set; }
        public string Branch { get; set; }
        public string AccountType { get; set; }
        public string AccountNo { get; set; }
        public string AutoDebit { get; set; }
    }

    public class RptClientAccountsType
    {
        public List<RptClientAccountsTypeModel> GetClientAccountsList(string pBranch, DateTime? pStartDate = null, DateTime? pEndDate = null)
        {
            List<RptClientAccountsTypeModel> list = new List<RptClientAccountsTypeModel>();

            return list;
        }
    }
}