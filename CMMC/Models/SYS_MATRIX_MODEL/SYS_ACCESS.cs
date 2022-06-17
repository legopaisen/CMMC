using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using CTBC;

namespace SYS_MATRIX.Models
{
 public class SYS_ACCESS_MODEL
 {
  public int AccessCode { get; set; }
  public string AccessName { get; set; }
  public string Description { get; set; }
  public bool IsActive { get; set; }
  public string CreatedBy { get; set; }
  public DateTime CreatedOn { get; set; }
  public string ModifiedBy { get; set; }
  public DateTime ModifiedOn { get; set; }
  public int GroupCode { get; set; }
  public int NotifiedGroupCode { get; set; }
  public UserTypeDetails UserType {get; set;}
  public List<SYS_ACCESS_MODS_MODEL> ModuleList { get; set; }
 }

 public struct UserTypeDetails
 {
  public string UserType { get; set; }
  public string UserTypeText { get; set; }
 }

 public class SYS_ACCESS : IDisposable
 {
  public void Dispose() { GC.SuppressFinalize(this); }

  public List<UserTypeDetails> GetUserType()
  {
   List<UserTypeDetails> usertype = new List<UserTypeDetails>();
   usertype.Add(new UserTypeDetails()
   {
    UserType = "0",
    UserTypeText = "Maker"
   });
   usertype.Add(new UserTypeDetails()
   {
    UserType = "1",
    UserTypeText = "Approver"
   });
   usertype.Add(new UserTypeDetails()
   {
    UserType = "2",
    UserTypeText = "User Administrator"
   });
   usertype.Add(new UserTypeDetails()
   {
    UserType = "3",
    UserTypeText = "Auditor"
   });
   usertype.Add(new UserTypeDetails()
   {
    UserType = "4",
    UserTypeText = "System Administrator"
   });
   return usertype;
  }

  public SYS_ACCESS_MODEL Fill(int pAccessCode)
  {
   SYS_ACCESS_MODEL access = new SYS_ACCESS_MODEL();
   try
   {
    string strSQL = "SELECT * FROM SYS_ACCESS WHERE Access_Code = @pAccessCode";
   
    using (SqlConnection cn = new SqlConnection(CMMC.Models.SharedFunctions.Connectionstring))
    {
     using (SqlCommand cmd = cn.CreateCommand())
     {
      cmd.CommandText = strSQL;
      cmd.Parameters.Add(new SqlParameter("pAccessCode", pAccessCode));
      cn.Open();
      using (SqlDataReader dr = cmd.ExecuteReader())
      {
       if (dr.Read())
       {
        access.AccessCode = dr["Access_Code"].ToString().ToInt();
        access.AccessName = dr["Access_Name"].ToString();
        access.Description = dr["Description"].ToString();
        access.IsActive = dr["IsActive"].ToString().Equals("1");
        access.CreatedBy = dr["CreatedBy"].ToString();
        access.CreatedOn = dr["CreatedOn"].ToString().ToDateTime();
        access.ModifiedBy = dr["ModifiedBy"].ToString();
        access.ModifiedOn = dr["ModifiedOn"].ToString().ToDateTime();
        access.NotifiedGroupCode = dr["NotifiedGroupCode"].ToString().ToInt();
        access.GroupCode = dr["Group_Code"].ToString().ToInt();
        access.ModuleList = new SYS_ACCESS_MODS().GetList(access.AccessCode);
        access.UserType = new UserTypeDetails() { UserType = dr["UserType"].ToString() };
       }
      }
     }     
    }
   }
   catch (Exception ex)
   {
    CTBC.Logs.Write("Fill", ex.Message, "SYS_ACCESS");
   }
   return access;
  }



  public int Delete(string pID)
  {
   int intReturn = 0;
   string strSQL = "Delete SYS_ACCESS WHERE Access_Code = @pCode";

   using (SqlConnection cn = new SqlConnection(CMMC.Models.SharedFunctions.Connectionstring))
   {
    using (SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = strSQL;
     cmd.Parameters.Add(new SqlParameter("@pCode", pID));
     cn.Open();
     intReturn = cmd.ExecuteNonQuery();
    }
    using (SYS_ACCESS_MODS modules = new SYS_ACCESS_MODS())
    {
     modules.DeleteModule(pID);
    }
   }
   
   return intReturn;
  }


  public int Insert(SYS_ACCESS_MODEL Model)
  {
   int intReturn = 0;
   try
   {
    string strSQL = "";
    strSQL += " INSERT INTO";
    strSQL += " SYS_ACCESS";
    strSQL += " VALUES(";
    strSQL += " @pAccessName,";
    strSQL += " @pDescription,";
    strSQL += " @pIsActive,";
    strSQL += " @pCreatedBy,";
    strSQL += " GETDATE(),";
    strSQL += " null,";
    strSQL += " null,";
    strSQL += " @pGroupCode,";
    strSQL += " @pNotifiedGroupCode,";
    strSQL += " @pUserType";
    strSQL += " ) ";
    strSQL += " SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY]";

    using (SqlConnection cn = new SqlConnection(CMMC.Models.SharedFunctions.Connectionstring))
    {
     using (SqlCommand cmd = cn.CreateCommand())
     {
      cmd.CommandText = strSQL;
      cmd.Parameters.Add(new SqlParameter("@pAccessName", Model.AccessName));
      cmd.Parameters.Add(new SqlParameter("@pDescription", Model.Description));
      cmd.Parameters.Add(new SqlParameter("@pCreatedBy", Model.CreatedBy));
      cmd.Parameters.Add(new SqlParameter("@pGroupCode", Model.GroupCode));
      cmd.Parameters.Add(new SqlParameter("@pNotifiedGroupCode", Model.NotifiedGroupCode));
      cmd.Parameters.Add(new SqlParameter("@pUserType", Model.UserType.UserType));
      cmd.Parameters.Add(new SqlParameter("@pIsActive", Model.IsActive ? "1" : "0"));
      cn.Open();
      intReturn = cmd.ExecuteScalar().ToString().ToInt();
      
      using (SYS_ACCESS_MODS modules = new SYS_ACCESS_MODS())
      {
       modules.Insert(Model.ModuleList.Select(x => { 
        x.AccessCode = intReturn; 
        return x; }).ToList());
      }
     }
    }
   }
   catch (Exception ex)
   {
    CTBC.Logs.Write("Insert", ex.Message, "SYS_ACCESS");
   }
   return intReturn;
  }

  public int ProcessAction(List<int> pIDs,bool pIsActivate, string pDeactivatedBy)
  {
   int intReturn = 0;
   string strSQL = "";

   strSQL += "UPDATE SYS_ACCESS ";
   strSQL += "SET ";
   strSQL += "IsActive = @pIsActive ";
   strSQL += ",ModifiedBy = @pModifiedBy ";
   strSQL += ",ModifiedOn = GETDATE() ";
   strSQL += "WHERE ";
   strSQL += "Access_Code = @pAccessCode";

   using(SqlConnection cn = new SqlConnection(CMMC.Models.SharedFunctions.Connectionstring))
   {
    using(SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = strSQL;
     cn.Open();
     foreach(int intID in pIDs)
     {
      cmd.Parameters.Clear();
      cmd.Parameters.Add(new SqlParameter("@pAccessCode", intID));
      cmd.Parameters.Add(new SqlParameter("@pIsActive", pIsActivate ? "1" : "0"));
      cmd.Parameters.Add(new SqlParameter("@pModifiedBy", pDeactivatedBy));
      intReturn += cmd.ExecuteNonQuery();
     }
    }
   }
   return intReturn;
  }

  

  public int Update(SYS_ACCESS_MODEL Model)
  {
   int intReturn = 0;
   try
   {
    string strSQL = "";
    strSQL += " UPDATE";
    strSQL += " SYS_ACCESS";
    strSQL += " SET";
    strSQL += " ACCESS_NAME = @pAccessName,";
    strSQL += " DESCRIPTION = @pDescription,";
    strSQL += " ISACTIVE = @pIsActive,";
    strSQL += " MODIFIEDBY = @pModifiedBy,";
    strSQL += " MODIFIEDON = GETDATE(),";
    strSQL += " GROUP_CODE = @pGroupCode,";
    strSQL += " NOTIFIEDGROUPCODE = @pNotifiedGroupCode,";
    strSQL += " USERTYPE = @pUserType";
    strSQL += " WHERE ACCESS_CODE = @pAccessCode";

    using (SqlConnection cn = new SqlConnection(CMMC.Models.SharedFunctions.Connectionstring))
    {
     using (SqlCommand cmd = cn.CreateCommand())
     {
      cmd.CommandText = strSQL;
      cmd.Parameters.Add(new SqlParameter("@pAccessName", Model.AccessName));
      cmd.Parameters.Add(new SqlParameter("@pDescription", Model.Description));
      cmd.Parameters.Add(new SqlParameter("@pIsActive", Model.IsActive ? "1" : "0"));
      cmd.Parameters.Add(new SqlParameter("@pModifiedBy", Model.ModifiedBy));
      cmd.Parameters.Add(new SqlParameter("@pGroupCode", Model.GroupCode));
      cmd.Parameters.Add(new SqlParameter("@pAccessCode", Model.AccessCode));
      cmd.Parameters.Add(new SqlParameter("@pNotifiedGroupCode", Model.NotifiedGroupCode));
      cmd.Parameters.Add(new SqlParameter("@pUserType", Model.UserType.UserType));

      cn.Open();
      intReturn = cmd.ExecuteNonQuery();
      using(SYS_ACCESS_MODS module = new SYS_ACCESS_MODS())
      {
       module.Delete(Model.AccessCode);
       module.Insert(Model.ModuleList);
      }
     }
    }
   }
   catch (Exception ex)
   {
    CTBC.Logs.Write("Update", ex.Message, "SYS_ACCESS");
   }
   return intReturn;
  }

  public List<SYS_ACCESS_MODEL> GetList()
  {
   List<SYS_ACCESS_MODEL> list = new List<SYS_ACCESS_MODEL>();

   string strSQL = "";
   strSQL += "SELECT ";
   strSQL += " ACCESS_CODE ";
   strSQL += " ,ACCESS_NAME ";
   strSQL += " ,DESCRIPTION ";
   strSQL += " ,ISACTIVE ";
   strSQL += " ,CREATEDBY ";
   strSQL += " ,CREATEDON ";
   strSQL += " ,MODIFIEDBY ";
   strSQL += " ,MODIFIEDON ";
   strSQL += " ,GROUP_CODE ";
   strSQL += " ,NOTIFIEDGROUPCODE ";
   strSQL += " ,UserType ";
   strSQL += " FROM SYS_ACCESS ";

   using(SqlConnection cn = new SqlConnection(CMMC.Models.SharedFunctions.Connectionstring))
   {
    using(SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = strSQL;
     cn.Open();
     using(SqlDataReader dr = cmd.ExecuteReader())
     {
      while(dr.Read())
      {
       list.Add(new SYS_ACCESS_MODEL()
       { 
        AccessCode = dr["Access_Code"].ToString().ToInt()
        ,AccessName = dr["Access_Name"].ToString()
        ,Description = dr["Description"].ToString()
        ,IsActive = dr["IsActive"].ToString().Equals("1")
        ,CreatedBy = dr["CreatedBy"].ToString()
        ,CreatedOn = dr["CreatedOn"].ToString().ToDateTime()
        ,ModifiedBy = dr["ModifiedBy"].ToString()
        ,ModifiedOn = dr["ModifiedOn"].ToString().ToDateTime()
        ,UserType = new UserTypeDetails() { UserType = dr["UserType"].ToString() }
        ,NotifiedGroupCode = dr["NotifiedGroupCode"].ToInt()
       });
      }
     }
    }
   }
   return list;
  }
 }
}