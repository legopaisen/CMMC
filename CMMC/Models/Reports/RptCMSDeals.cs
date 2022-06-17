using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMMC.Models.Reports
{
    public class RptCMSDealsModel
    {
        public int CMSCode { get; set; }
        public string CustomerName { get; set; }
        public string AvailedService { get; set; }
        public DateTime DateEnrolled{ get; set; }
        public int TotalOBLinkedAccounts{ get; set; }
        public int TotalOBAggAccounts { get; set; }
        public decimal TotalMTDADBLinkedAccounts { get; set; }
        public decimal TotalMTDADBAggAccounts { get; set; }
        public string IndustryType { get; set; }
        public string RMName { get; set; }
        public int NoOfLinkedAccountsCASA{ get; set; }
        public int NoOfLinkedChildCashCard { get; set; }
        public decimal ChildCASAMTDADB { get; set; }
        public decimal ChildCashCardMTDADB { get; set; }
        public decimal IncomeCashCardReload{ get; set; }
        public decimal IncomeIssuerFee { get; set; }
        public decimal IncomeIBFTFee { get; set; }
        public decimal BusinessType { get; set; }
        public decimal BusinessUnit { get; set; }
        public int PCCC { get; set; }
    }

    public class RptCMSDeals
    {
        public List<RptCMSDealsModel> GetCMSDealsList(string pBranch, DateTime? pStartDate = null, DateTime? pEndDate = null)
        {
            List<RptCMSDealsModel> list = new List<RptCMSDealsModel>();

            return list;
        }
    }
}