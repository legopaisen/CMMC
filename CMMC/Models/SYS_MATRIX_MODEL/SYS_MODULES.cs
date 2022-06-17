using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using CTBC;

namespace SYS_MATRIX.Models
{
 public class SYS_MODULES_MODEL
 {
  public string Module_Code { get; set; }
  public string ModuleName { get; set; }
  public int ViewIndex { get; set; }
  public string Attribute1 { get; set; }
  public string Attribute2 { get; set; }
  public string Attribute3 { get; set; }
 }

 public class SYS_MODULES : IDisposable
 {
  public void Dispose() { GC.SuppressFinalize(this); }

  public SYS_MODULES_MODEL Fill(string pModuleCode)
  {
   SYS_MODULES_MODEL module = new SYS_MODULES_MODEL();
   try
   {
    using (SqlConnection cn = new SqlConnection(CMMC.Models.SharedFunctions.Connectionstring))
    {
     using (SqlCommand cmd = cn.CreateCommand())
     {
      cmd.CommandText = "SELECT * FROM SYS_MODULES WHERE Module_Code = @pModuleCode";
      cmd.Parameters.Add(new SqlParameter("@pModuleCode", pModuleCode));

      cn.Open();
      using (SqlDataReader reader = cmd.ExecuteReader())
      {
       if (reader.Read())
       {
        module.Module_Code = reader["Module_Code"].ToString();
        module.ModuleName = reader["ModuleName"].ToString();
        module.ViewIndex = reader["View_Index"].ToString().ToInt();
        module.Attribute1 = reader["Attribute1"].ToString();
        module.Attribute2 = reader["Attribute2"].ToString();
        module.Attribute3 = reader["Attribute3"].ToString();
       }
      }
     }
    }
   }
   catch (Exception ex)
   {
    CTBC.Logs.Write("Fill", ex.Message, "SYS_MODULES");
   }
   return module;
  }

  public List<SYS_MODULES_MODEL> GetModulesByAccessCode(int pAccessCode)
  {
   List<SYS_MODULES_MODEL> list = new List<SYS_MODULES_MODEL>();
   string strSQL = "";
   strSQL += "SELECT ";
   strSQL += "SA.Module_Code ";
   strSQL += ", SA.AccessCode ";
   strSQL += ", SM.ModuleName ";
   strSQL += ", SM.View_Index ";
   strSQL += ", SM.Attribute1 ";
   strSQL += ", SM.Attribute2 ";
   strSQL += ", SM.Attribute3 ";
   strSQL += ", SA.Permission ";
   strSQL += "FROM SYS_ACCESS_MODS SA ";
   strSQL += "INNER JOIN SYS_MODULES SM ON SM.Module_Code = SA.Module_Code ";
   strSQL += "WHERE SA.AccessCode = @pAccessCode";

   using (SqlConnection cn = new SqlConnection(CMMC.Models.SharedFunctions.Connectionstring))
   {
    using (SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = strSQL;
     cmd.Parameters.Add(new SqlParameter("@pAccessCode", pAccessCode));
     cn.Open();
     using (SqlDataReader dr = cmd.ExecuteReader())
     {
      while (dr.Read())
      {
       SYS_MODULES_MODEL moduledetails = new SYS_MODULES_MODEL();
       moduledetails.Attribute1 = dr["Attribute1"].ToString();
       moduledetails.Attribute2 = dr["Attribute2"].ToString();
       moduledetails.Attribute3 = dr["Attribute3"].ToString();
       moduledetails.ModuleName = dr["ModuleName"].ToString();
       moduledetails.ViewIndex = dr["View_Index"].ToString().ToInt();
       moduledetails.Module_Code = dr["Module_Code"].ToString();        
      }
     }
    }
   }
   return list;
  }

  public List<SYS_MODULES_MODEL> GetList()
  {
   List<SYS_MODULES_MODEL> list = new List<SYS_MODULES_MODEL>();
   using(SqlConnection  cn = new SqlConnection(CMMC.Models.SharedFunctions.Connectionstring))
   {
    using(SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = "SELECT * FROM SYS_MODULES WHERE View_Index <> '0' ORDER BY View_Index";
     cn.Open();
     using(SqlDataReader dr = cmd.ExecuteReader())
     {
      while(dr.Read())
      {
       list.Add(new SYS_MODULES_MODEL()
       { 
        Module_Code = dr["Module_Code"].ToString()
        ,ModuleName = dr["ModuleName"].ToString()
        ,ViewIndex = dr["View_Index"].ToString().ToInt()
        ,Attribute1 = dr["Attribute1"].ToString()
        ,Attribute2 = dr["Attribute2"].ToString()
        ,Attribute3 = dr["Attribute3"].ToString()
       });
      }
     }
    }
   }
   return list;
  }

 }
}