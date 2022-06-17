using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMMC.Models
{
 public class Report : IDisposable
 {
  public void Dispose() { GC.SuppressFinalize(this); }

  public string reportName { get; set; }
 
  public List<Report> GetReport()
  {
   var report = new List<Report>
   {   
   new Report{reportName = "---------------------------------------------------"},
   new Report{reportName = "Actual Payroll ATM Withdrawals Report"},
   new Report{reportName = "Actual Payroll ATM Withdrawals Report (Summary)"},
   new Report{reportName = "ATM Terminal Withdrawal Report"},
   new Report{reportName = "---------------------------------------------------"},
   new Report{reportName = "Actual Company MTD ADB Report"},
   new Report{reportName = "Actual Employee MTD ADB Report"},
   new Report{reportName = "ADB Requirement Report"},
   new Report{reportName = "ADB Penalty Report"},
   //new Report{reportName = ""},
   //new Report{reportName = ""},
   //new Report{reportName = ""},
   //new Report{reportName = ""},
   //new Report{reportName = ""},
   
   //ako na
   
   };                
   return report;
  } 
 }
}