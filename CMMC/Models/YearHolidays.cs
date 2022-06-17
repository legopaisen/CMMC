using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using CTBC;
using System.Data;

namespace CMMC.Models
{
 public class YearHolidays
 {
  public void Dispose() { GC.SuppressFinalize(this); }

  public struct Details
  {  
   public DateTime Date { get; set; }
   public string Type { get; set; }
   public string Description { get; set; }
  }

  public List<Details> GetList()
  {
   List<Details> list = new List<Details>();
   using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
   {
    using (SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = "SELECT * FROM CMMC_HolidaysTbl ORDER BY hd_Date DESC";
     cn.Open();
     using (SqlDataReader dr = cmd.ExecuteReader())
     {
      while (dr.Read())
      {
       list.Add(new Details()
       {
        Date = dr["hd_Date"].ToString().ToDateTime()
        ,
        Type = dr["hd_Type"].ToString()
        ,
        Description = dr["hd_Description"].ToString()
       });
      }
     }
    }
   }
   return list;
  }
 }
}