using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using CTBC;

namespace SYS_MATRIX.Models
{
 public class SYS_ACCESS_MODS_MODEL
 {
  public string ModuleCode { get; set; }
  public int AccessCode { get; set; }
  public string Permission { get; set; }
  public string PermissionText { get; set; }
 }

 public class SYS_ACCESS_USER_MODULES
 {
  public string ModuleCode { get; set; }
  public int AccessCode { get; set; }
  public string Permission { get; set; }
 }

 public class SYS_ACCESS_MODS : IDisposable
 {
  public void Dispose() { GC.SuppressFinalize(this); }

  public int Insert(List<SYS_ACCESS_MODS_MODEL> Model)
  {
   int intReturn = 0;
   try
   {
    string strSQL = "";
    strSQL += " INSERT INTO SYS_ACCESS_MODS";
    strSQL += " VALUES( ";
    strSQL += " @pModuleCode,";
    strSQL += " @pAccessCode,";
    strSQL += " @pPermission";
    strSQL += " )";

    using (SqlConnection cn = new SqlConnection(CMMC.Models.SharedFunctions.Connectionstring))
    {
     using (SqlCommand cmd = cn.CreateCommand())
     {
      cmd.CommandText = strSQL;
      cn.Open();
      foreach(SYS_ACCESS_MODS_MODEL module in Model)
      {
       cmd.Parameters.Clear();
       cmd.Parameters.Add(new SqlParameter("@pModuleCode", module.ModuleCode));
       cmd.Parameters.Add(new SqlParameter("@pAccessCode", module.AccessCode));
       cmd.Parameters.Add(new SqlParameter("@pPermission", module.Permission));
       intReturn += cmd.ExecuteNonQuery();
      }
     }
    }
   }
   catch (Exception ex)
   {
    CTBC.Logs.Write("Insert", ex.Message, "SYS_ACCESS_MODS");
   }
   return intReturn;
  }

  public int DeleteModule(string pID)
  {
   int intReturn = 0;
   string strSQL = "DELETE SYS_ACCESS_MODS  WHERE AccessCode = @pCode";

   using (SqlConnection cn = new SqlConnection(CMMC.Models.SharedFunctions.Connectionstring))
   {
    using (SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = strSQL;
     cmd.Parameters.Add(new SqlParameter("@pCode", pID));
     cn.Open();
     intReturn = cmd.ExecuteNonQuery();
    }
   }
   return intReturn;
  }

  public int Delete(int pAccessCode)
  {
   int intReturn = 0;
   try
   {
    string strSQL = "";
    strSQL += " DELETE FROM SYS_ACCESS_MODS WHERE AccessCode = @pAccessCode";
    using (SqlConnection cn = new SqlConnection(CMMC.Models.SharedFunctions.Connectionstring))
    {
     using (SqlCommand cmd = cn.CreateCommand())
     {
      cn.Open();
      cmd.CommandText = strSQL;

      cmd.Parameters.Clear();
      cmd.Parameters.Add(new SqlParameter("@pAccessCode", pAccessCode));
      intReturn += cmd.ExecuteNonQuery();
     }
    }
   }
   catch (Exception ex)
   {
    CTBC.Logs.Write("Delete", ex.Message, "SYS_ACCESS_MODS");
   }
   return intReturn;
  }



  public struct ModuleDetails
  {
   public int AccessCode { get; set; }
   public string ModuleCode { get; set; }
   public string ModuleName { get; set; }
   public int ViewIndex { get; set; }
   public string Attribute1 { get; set; }
   public string Attribute2 { get; set; }
   public string Attribute3 { get; set; }
   public string Permission { get; set; }
  }

  public List<SYS_ACCESS_MODS_MODEL> GetList(int pAccessCode)
  {
   List<SYS_ACCESS_MODS_MODEL> list = new List<SYS_ACCESS_MODS_MODEL>();
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

   using(SqlConnection cn = new SqlConnection(CMMC.Models.SharedFunctions.Connectionstring))
   {
    using(SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = strSQL;
     cmd.Parameters.Add(new SqlParameter("@pAccessCode", pAccessCode));
     cn.Open();
     using(SqlDataReader dr = cmd.ExecuteReader())
     {
      while(dr.Read())
      {
       SYS_MODULES_MODEL moduledetails = new SYS_MODULES_MODEL();
       moduledetails.Attribute1 = dr["Attribute1"].ToString();
       moduledetails.Attribute2 = dr["Attribute2"].ToString();
       moduledetails.Attribute3 = dr["Attribute3"].ToString();
       moduledetails.ModuleName = dr["ModuleName"].ToString();
       moduledetails.ViewIndex = dr["View_Index"].ToString().ToInt();
       
       list.Add(new SYS_ACCESS_MODS_MODEL()
       {          
         AccessCode = Convert.ToInt32(dr["AccessCode"].ToString())
         , Permission = dr["Permission"].ToString()
         , ModuleCode = dr["Module_Code"].ToString()         
       });
      }
     }
    }
   }
   return list;
  }

  public List<SYS_ACCESS_MODS_MODEL> GetPermissionList()
  {
   List<SYS_ACCESS_MODS_MODEL> perm = new List<SYS_ACCESS_MODS_MODEL>();
   perm.Add(new SYS_ACCESS_MODS_MODEL()
   {
    Permission = "F",
    PermissionText = "Full Access"
   });
   perm.Add(new SYS_ACCESS_MODS_MODEL()
   {
    Permission = "R",
    PermissionText = "Read Access"
   });
   return perm;
  }

 }
}