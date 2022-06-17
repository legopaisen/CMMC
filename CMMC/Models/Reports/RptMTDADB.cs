using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMMC.Models.Reports
{
    public class RptMTDADBModel
    {
        public string Branch { get; set; }
        public int CMSCode { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public decimal MTDADB { get; set; }
    }
    
    public class RptMTDADB
    {
        public List<RptMTDADBModel> GetMTDADBList(string pBranch, DateTime? pStartDate = null, DateTime? pEndDate = null)
        {
            List<RptMTDADBModel> list = new List<RptMTDADBModel>();

            return list;
        }
    }
}