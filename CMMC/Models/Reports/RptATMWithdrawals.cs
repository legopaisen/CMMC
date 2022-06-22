using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMMC.Models.Reports
{
    public class RptATMWithdrawalsModel
    {
        public string ChildAccount { get; set; }
        public string ChildAccountName { get; set; }
        public string CardNumber { get; set; }
        public int PCCC { get; set; }
        public string Branch { get; set; }
        public int CMSCode { get; set; }
        public string CustomerName { get; set; }
        public string TransactionCodeDesc { get; set; }
        public int TransactionAmount { get; set; }
        public DateTime TransactionDateTime { get; set; }
        public string ATMTerminalIDDesc{ get; set; }
        public int NoOfFreeWithdrawals { get; set; }
    }
    
    public class RptATMWithdrawals
    {
        public List<RptATMWithdrawalsModel> GetATMWithdrawalsList(string pBranch, DateTime? pStartDate = null, DateTime? pEndDate = null)
        {
            List<RptATMWithdrawalsModel> list = new List<RptATMWithdrawalsModel>();

            return list;
        }
    }
}