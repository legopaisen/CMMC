using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using CTBC;

namespace CMMC.Models.Batch
{
 public class Holiday : IDisposable
 {  
  public DateTime Date { get; set; }
  public string Type { get; set; }
  public string Description { get; set; } 

  public int Insert()
  {   
   int intReturn = 0;
   if(!this.IsExist(this.Date))
   {    
    using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
    {
     using (SqlCommand cmd = cn.CreateCommand())
     {
      cmd.CommandText = "INSERT INTO tblHolidays (hd_date, hd_Desc) VALUES (@pDate, @pDescription)";
      cmd.Parameters.Add(new SqlParameter("@pDate", this.Date));
      cmd.Parameters.Add(new SqlParameter("@pDescription", this.Description));
      cn.Open();
      intReturn = cmd.ExecuteNonQuery();
     }
    }
   }
   return intReturn;
  }

  public bool IsExist(DateTime pDate)
  {
   bool blnReturn = false;
   using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
   {
    using (SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = "SELECT COUNT(*) FROM CMMC_HolidaysTbl WHERE hd_Date = @pDate";
     cmd.Parameters.Add(new SqlParameter("@pDate", pDate));     
     cn.Open();
     blnReturn = cmd.ExecuteScalar().ToString().ToInt() > 0 ? true : false;
    }
   }
   return blnReturn;
  }

  public int Delete(DateTime pDate)
  {
  int intReturn = 0;
  using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
   {
    using(SqlCommand cmd= cn.CreateCommand())
    {
     cmd.CommandText = "DELETE FROM tblHolidays WHERE hd_Date = @pDate";
     cmd.Parameters.Add(new SqlParameter("@pDate", pDate));
     cn.Open();
     intReturn = cmd.ExecuteNonQuery();
    }    
   }
  return intReturn;
  }

  public int Update(DateTime pDate)
  {
   int intReturn = 0;
   using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
   {
    using (SqlCommand cmd = cn.CreateCommand())
    {
     if (this.IsExist(this.Date))
     {
      cmd.CommandText = "UPDATE tblHolidays SET hd_Date = @pDate,hd_Desc = @pDesc WHERE hd_Date = @pDate";
      cmd.Parameters.Add(new SqlParameter("@pDate", pDate));
      cn.Open();
      intReturn = cmd.ExecuteNonQuery();
     }
    }
   }
   return intReturn;
  }

  public List<Holiday> GetList()
  {
   List<Holiday> list = new List<Holiday>();
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
       list.Add(new Holiday()
       {
        Date = dr["hd_Date"].ToString().ToDateTime()
        ,Type = dr["hd_Type"].ToString()
        ,Description = dr["hd_Description"].ToString()
       });
      }
     }
    }
   }
   return list;
  }

  public bool IsHoliday(DateTime pDate)
  {
   bool blnIsHoliday = false;
   try
   {
    using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
    {
     using (SqlCommand cmd = cn.CreateCommand())
     {
      cmd.CommandText = "SELECT COUNT(*) FROM tblHolidays WHERE hd_Date = @pDate";
      cmd.Parameters.Add(new SqlParameter("@pDate", pDate));
      cn.Open();
      blnIsHoliday = cmd.ExecuteScalar().ToString().ToInt() > 0 ? true : false;      
     }
    }
   }
   catch(Exception e)
   {
    throw new ApplicationException(e.Message);
   }
 
   return blnIsHoliday;
  }

  public void Dispose()
  {
   GC.SuppressFinalize(this);
  }

 }
}
