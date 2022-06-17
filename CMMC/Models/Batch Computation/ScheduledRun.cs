using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using CTBC;

namespace CMMC.Models.Batch_Computation
{
 public class ScheduledRun: IDisposable
 {
  public void Dispose() { GC.SuppressFinalize(this); }

  public struct Details
  {
   public int ScheduleID { get; set; }
   public DateTime ScheduledDate { get; set; }
   public string AddedBy { get; set; }
   public DateTime LastRun { get; set; }
   public string Status { get; set; }
  }

  public void SaveSchedule(DateTime pDate, string pAutoRun)
  {
   try
   {
    using (CTBC.Utility.Registry registry = new CTBC.Utility.Registry(CTBC.Utility.Registry.RegistryType.LOCAL_MACHINE, @"SOFTWARE\CTBC\CMMC"))
    {
     registry.Write("SCHEDULERUN", pDate.ToString());
     registry.Write("SCHEDULE_AUTORUN", pAutoRun);
    }
   }
   catch(Exception e){
    CTBC.Logs.Write("SaveSchedule", e.Message, "Batch Schedule Run");
   }
  }

  public void UpdateLastRun(DateTime pLastRun)
  {
   try
   {
    using (CTBC.Utility.Registry registry = new CTBC.Utility.Registry(CTBC.Utility.Registry.RegistryType.LOCAL_MACHINE, @"SOFTWARE\CTBC\CMMC"))
    {
     registry.Write("LAST_BATCH_RUN", pLastRun.ToString());
    }
   }
   catch(Exception e){
    CTBC.Logs.Write("UpdateLastRun", e.Message, "Batch Schedule Run");

   }
    
  }

  public DateTime GetLastRun(DateTime pDate)
  {
   DateTime dteReturn = DateTime.Today;
   try
   {
    using (CTBC.Utility.Registry registry = new CTBC.Utility.Registry(CTBC.Utility.Registry.RegistryType.LOCAL_MACHINE, @"SOFTWARE\CTBC\CMMC"))
    {
     dteReturn = registry.Read("LAST_BATCH_RUN").ToDateTime();
    }
   }
   catch(Exception e){
    CTBC.Logs.Write("GetLastRun", e.Message, "Batch Schedule Run");

   }
   return dteReturn;
  }
 }
}