using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using CTBC;


namespace SYS_MATRIX.Models
{
 public class SYS_USERS_MODEL
 {
  public string UserID { get; set; }
  public string FullName { get; set; }
  public string Email { get; set; }
  public bool IsActive { get; set; }
  public int Access_Code { get; set; }
  public string Access_Name { get; set; }
  public string CreatedBy { get; set; }
  public DateTime CreatedOn { get; set; }
  public string ModifiedBy { get; set; }
  public DateTime ModifiedOn { get; set; }
  public string Attribute1 { get; set; }
  public string Attribute2 { get; set; }
  public string Attribute3 { get; set; }
  public bool IsOnline { get; set; }
  public string LastIPUsed { get; set; }
  public DateTime LastOnlineOn { get; set; }
  public int GroupCode { get; set; }
  public string GroupName { get; set; }
  public int DepartmentCode { get; set; }
  public string DepartmentName { get; set; }
  public int UnitCode { get; set; }
  public string UnitName { get; set; }
  public bool IsGroupHead { get; set; }
  public bool IsDepartmentHead { get; set; }
  public bool IsUnitHead { get; set; }
  public string UserType { get; set; }
 }

 public class SYS_USERS : IDisposable
 {
  public void Dispose() { GC.SuppressFinalize(this); }

  public SYS_USERS_MODEL Fill(string UserID)
  {
   SYS_USERS_MODEL user = new SYS_USERS_MODEL();
   using (SqlConnection cn = new SqlConnection(CMMC.Models.SharedFunctions.Connectionstring))
   {
    using (SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = "SELECT SU.*, SA.UserType FROM SYS_USERS SU LEFT JOIN SYS_ACCESS SA ON SA.ACCESS_CODE = SU.Access_Code WHERE SU.UserID = @pUserID";
     cmd.Parameters.Add(new SqlParameter("pUserID", UserID));
     cn.Open();
     using (SqlDataReader dr = cmd.ExecuteReader())
     {
      if (dr.Read())
      {
       user.UserID = dr["UserID"].ToString();
       user.FullName = dr["FullName"].ToString();
       user.Email = dr["Email"].ToString();
       user.IsActive = dr["IsActive"].ToString().Equals("1");
       user.Access_Code = dr["Access_Code"].ToString().ToInt();
       user.CreatedBy = dr["CreatedBy"].ToString();
       user.CreatedOn = dr["CreatedOn"].ToString().ToDateTime();
       user.ModifiedBy = dr["ModifiedBy"].ToString();
       user.ModifiedOn = dr["ModifiedOn"].ToString().ToDateTime();
       user.Attribute1 = dr["Attribute1"].ToString();
       user.Attribute2 = dr["Attribute2"].ToString();
       user.Attribute3 = dr["Attribute3"].ToString();
       user.LastOnlineOn = dr["LastOnlineOn"].ToString().ToDateTime();
       user.UserType = dr["UserType"].ToString();
      }
     }
    }
   }
   return user;
  }

  public int Insert(SYS_USERS_MODEL Model)
  {
   int intReturn = 0;
   string strSQL = "";
   strSQL += "INSERT INTO SYS_USERS ";
   strSQL += "VALUES ";
   strSQL += "(@pUserID ";
   strSQL += ",@pFullName ";
   strSQL += ",@pEmail ";
   strSQL += ",@pIsActive ";
   strSQL += ",@pAccess_Code ";
   strSQL += ",@pCreatedBy ";
   strSQL += ",GETDATE() ";
   strSQL += ",NULL ";
   strSQL += ",NULL ";
   strSQL += ",@pAttribute1 ";
   strSQL += ",@pAttribute2 ";
   strSQL += ",@pAttribute3 ";
   strSQL += ",NULL) ";
 
   if (!this.IsExist(Model.UserID, Model.Access_Code))
   {
    try
    {
     using (SqlConnection cn = new SqlConnection(CMMC.Models.SharedFunctions.Connectionstring))
     {
      using (SqlCommand cmd = cn.CreateCommand())
      {
       cmd.CommandText = strSQL;
       cmd.Parameters.Add(new SqlParameter("@pUserID", Model.UserID));
       cmd.Parameters.Add(new SqlParameter("@pFullName", Model.FullName));
       cmd.Parameters.Add(new SqlParameter("@pEmail", Model.Email == null ? "" : Model.Email));
       cmd.Parameters.Add(new SqlParameter("@pIsActive", Model.IsActive ? '1' : '0'));
       cmd.Parameters.Add(new SqlParameter("@pAccess_Code", Model.Access_Code));
       cmd.Parameters.Add(new SqlParameter("@pCreatedBy", Model.CreatedBy));
       cmd.Parameters.Add(new SqlParameter("@pAttribute1", Model.Attribute1 == null ? "" : Model.Attribute1));
       cmd.Parameters.Add(new SqlParameter("@pAttribute2", Model.Attribute2 == null ? "" : Model.Attribute2));
       cmd.Parameters.Add(new SqlParameter("@pAttribute3", Model.Attribute3 == null ? "" : Model.Attribute3));
       cn.Open();
       intReturn = cmd.ExecuteNonQuery();
      }
     }
    }
    catch (Exception e)
    {
     CTBC.Logs.Write("Insert", e.Message, "SYS_USERS");
    }
   }
   return intReturn;
  }

  public bool IsExist(string pUserID, int pAccessCode)
  {
   bool blnReturn = false;
   using (SqlConnection cn = new SqlConnection(CMMC.Models.SharedFunctions.Connectionstring))
   {
    using(SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = "SELECT COUNT(*) AS COUNTER FROM SYS_USERS WHERE UserID = @pUserID AND Access_Code = @pAccessCode";
     cmd.Parameters.Add(new SqlParameter("@pUserID", pUserID));
     cmd.Parameters.Add(new SqlParameter("@pAccessCode", pAccessCode));
     cn.Open();
     blnReturn = cmd.ExecuteScalar().ToString().ToInt() > 0;
    }
   }
   return blnReturn;
  }

  public int UserProcessAction(List<string> pIDs, bool IsActivate, string pDeactivatedBy)
  {
   int intReturn = 0;
   string strSQL = "";
   strSQL += "Update SYS_USERS ";
   strSQL += "SET ";
   strSQL += "IsActive = @pIsActive ";
   strSQL += ",ModifiedBy = @pModifiedBy ";
   strSQL += ",ModifiedOn = GETDATE() ";
   strSQL += "WHERE ";
   strSQL += "UserID = @pUserID";
   using (SqlConnection cn = new SqlConnection(CMMC.Models.SharedFunctions.Connectionstring))
   {
    using (SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = strSQL;
     cn.Open();
     foreach (string intIDs in pIDs)
     {
      cmd.Parameters.Clear();
      cmd.Parameters.Add(new SqlParameter("@pUserID", intIDs));
      cmd.Parameters.Add(new SqlParameter("@pIsActive", IsActivate ? "1" : "0"));
      cmd.Parameters.Add(new SqlParameter("@pModifiedBy", pDeactivatedBy));
      intReturn += cmd.ExecuteNonQuery();
     }
    }
   }
   return intReturn;
  }

  public int Update(SYS_USERS_MODEL Model)
  {
   int intReturn = 0;
   try
   {
    string strSQL = "";
    strSQL += "UPDATE SYS_USERS ";
    strSQL += "SET ";
    strSQL += "IsActive = @pIsActive ";
    strSQL += ",Access_Code = @pAccess_Code ";
    strSQL += ",ModifiedBy = @pModifiedBy ";
    strSQL += ",ModifiedOn = GETDATE() ";
    strSQL += ",Attribute1 = @pAttribute1 ";
    strSQL += ",Attribute2 = @pAttribute2 ";
    strSQL += ",Attribute3 = @pAttribute3 ";
    strSQL += "WHERE UserID = @pUserID ";

     using (SqlConnection cn = new SqlConnection(CMMC.Models.SharedFunctions.Connectionstring))
     {
      using (SqlCommand cmd = cn.CreateCommand())
      {
       cmd.CommandText = strSQL;
       cmd.Parameters.Add(new SqlParameter("@pUserID", Model.UserID));
       cmd.Parameters.Add(new SqlParameter("@pIsActive", Model.IsActive ? "1" : "0"));
       cmd.Parameters.Add(new SqlParameter("@pAccess_Code", Model.Access_Code));
       cmd.Parameters.Add(new SqlParameter("@pModifiedBy", Model.ModifiedBy));
       cmd.Parameters.Add(new SqlParameter("@pAttribute1", Model.Attribute1 == null ? " " : Model.Attribute1));
       cmd.Parameters.Add(new SqlParameter("@pAttribute2", Model.Attribute2 == null ? " " : Model.Attribute2));
       cmd.Parameters.Add(new SqlParameter("@pAttribute3", Model.Attribute3 == null ? " " : Model.Attribute3));
       cn.Open();
       intReturn = cmd.ExecuteNonQuery();
      }
     }
    }
   catch (Exception ex)
   {
    CTBC.Logs.Write("Update", ex.Message, "SYS_USERS");
   }
   return intReturn;
  }

  public List<SYS_USERS_MODEL> GetList()
  {
   List<SYS_USERS_MODEL> list = new List<SYS_USERS_MODEL>();
   string strSQL = "";
   strSQL += "SELECT ";
   strSQL += "SU.UserID  ";
   strSQL += " ,SU.FullName ";
   strSQL += " ,SU.IsActive ";
   strSQL += " ,SU.Email ";
   strSQL += " ,SU.Access_Code ";
   strSQL += " ,SU.CreatedBy ";
   strSQL += " ,SU.CreatedOn ";
   strSQL += " ,SU.ModifiedBy ";
   strSQL += " ,SU.ModifiedOn ";
   strSQL += " ,SU.Attribute1 ";
   strSQL += " ,SU.Attribute2 ";
   strSQL += " ,SU.Attribute3 ";
   strSQL += " ,SU.LastOnlineOn ";
   strSQL += " ,SA.ACCESS_NAME ";
   strSQL += "FROM SYS_USERS SU ";
   strSQL += "INNER JOIN SYS_ACCESS SA ON SA.ACCESS_CODE = SU.Access_Code ";
   strSQL += "ORDER BY SU.FullName ";

   using (SqlConnection cn = new SqlConnection(CMMC.Models.SharedFunctions.Connectionstring))
   {
    using (SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = strSQL;
     cn.Open();
     using (SqlDataReader dr = cmd.ExecuteReader())
     {
      while (dr.Read())
      {
       list.Add(new SYS_USERS_MODEL()
       {         
        UserID = dr["UserID"].ToString()
        ,FullName = dr["FullName"].ToString()
        ,Email = dr["Email"].ToString()
        ,IsActive = dr["IsActive"].ToString().Equals("1")
        ,Access_Code = dr["Access_Code"].ToString().ToInt()
        ,Access_Name = dr["ACCESS_NAME"].ToString()
        ,CreatedBy = dr["CreatedBy"].ToString()
        ,CreatedOn = dr["CreatedOn"].ToString().ToDateTime()
        ,ModifiedBy = dr["ModifiedBy"].ToString()
        ,ModifiedOn = dr["ModifiedOn"].ToString().ToDateTime()
        ,Attribute1 = dr["Attribute1"].ToString()
        ,Attribute2 = dr["Attribute2"].ToString()
        ,Attribute3 = dr["Attribute3"].ToString()
        ,LastOnlineOn = dr["LastOnlineOn"].ToString().ToDateTime()
       });
      }
     }
    }
   }
   return list;
  }

  public List<SYS_USERS_MODEL> GetCount(string pUserID = "")
  {
   List<SYS_USERS_MODEL> list = new List<SYS_USERS_MODEL>();
   string strSQL = "";
   strSQL += "SELECT ";
   strSQL += "SU.* ";
   strSQL += ", SA.ACCESS_NAME ";
   strSQL += ", SA.GROUP_CODE";
   strSQL += ", SA.USERTYPE ";
   strSQL += "FROM  ";
   strSQL += "SYS_USERS SU ";
   strSQL += "INNER JOIN SYS_ACCESS SA ON SA.ACCESS_CODE = SU.Access_Code ";
   if (pUserID.Length > 0)
   {
    strSQL += "WHERE SU.UserID = @pUserID ";
   }
   strSQL += "ORDER BY SU.FullName ";
   try
   {
    using (SqlConnection cn = new SqlConnection(CMMC.Models.SharedFunctions.Connectionstring))
    {
     using (SqlCommand cmd = cn.CreateCommand())
     {
      cmd.CommandText = strSQL;
      if (pUserID.Length > 0)
      {
       cmd.Parameters.Add(new SqlParameter("@pUserID", pUserID));
      }
      cn.Open();
      using (SqlDataReader dr = cmd.ExecuteReader())
      {
       while (dr.Read())
       {
        list.Add(new SYS_USERS_MODEL()
        {
         UserID = dr["UserID"].ToString()
         ,FullName = dr["FullName"].ToString()
         ,Email = dr["Email"].ToString()
         ,IsActive = dr["IsActive"].ToString().Equals("1")
         ,Access_Name = dr["ACCESS_NAME"].ToString()
         ,CreatedBy = dr["CreatedBy"].ToString()
         ,CreatedOn = dr["CreatedOn"].ToString().ToDateTime()
         ,ModifiedBy = dr["ModifiedBy"].ToString()
         ,ModifiedOn = dr["ModifiedOn"].ToString().ToDateTime()
         ,Attribute1 = dr["Attribute1"].ToString().ToEscapeString()
         ,Attribute2 = dr["Attribute2"].ToString().ToEscapeString()
         ,Attribute3 = dr["Attribute3"].ToString().ToEscapeString()
         ,LastOnlineOn = dr["LastOnlineOn"].ToString().ToDateTime()
         ,GroupCode = dr["GROUP_CODE"].ToString().ToInt()
         ,UserType = dr["USERTYPE"].ToString()
        });
       }
      }
     }
    }
   }
   catch (Exception ex)
   {
    CTBC.Logs.Write("GetCount", ex.Message, "SYS_USERS");
   }
   return list;
  }

  public int Delete(string pID)
  {
   int intReturn = 0;
   string strSQL = "DELETE SYS_USERS WHERE UserID = @pID";

   using (SqlConnection cn = new SqlConnection(CMMC.Models.SharedFunctions.Connectionstring))
   {
    using (SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = strSQL;
     cmd.Parameters.Add(new SqlParameter("@pID", pID));
     cn.Open();
     intReturn = cmd.ExecuteNonQuery();
    }
   }
   return intReturn;
  }

  public List<SYS_USERS_MODEL> GetApproverDetails()
  {
   List<SYS_USERS_MODEL> list = new List<SYS_USERS_MODEL>();
   string strSQL = "";
   strSQL += "SELECT UserID, FullName ";
   strSQL += "FROM Sys_Users ";
   strSQL += "WHERE ACCESS_CODE = '3' and IsActive = '1' ";

   using (SqlConnection cn= new SqlConnection(CMMC.Models.SharedFunctions.Connectionstring))
   {
    using (SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = strSQL;
     cn.Open();
     using (SqlDataReader dr = cmd.ExecuteReader())
     {
      while (dr.Read())
      {
       list.Add(new SYS_USERS_MODEL
       {
        UserID = dr["UserID"].ToString(),
        FullName = dr["FullName"].ToString()
       });
      }
     }
    }
   }
   return list;
  }


  public List<SYS_USERS_MODEL> GetMakerDetails()
  {
   List<SYS_USERS_MODEL> list = new List<SYS_USERS_MODEL>();
   string strSQL = "";
   strSQL += "SELECT UserID, FullName ";
   strSQL += "FROM Sys_Users ";
   strSQL += "WHERE ACCESS_CODE = '2' and IsActive = '1' ";

   using (SqlConnection cn = new SqlConnection(CMMC.Models.SharedFunctions.Connectionstring))
   {
    using (SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = strSQL;
     cn.Open();
     using (SqlDataReader dr = cmd.ExecuteReader())
     {
      while (dr.Read())
      {
       list.Add(new SYS_USERS_MODEL
       {
        UserID = dr["UserID"].ToString(),
        FullName = dr["FullName"].ToString()
       });
      }
     }
    }
   }
   return list;
  }


  public List<SYS_USERS_MODEL> GetUserDetailsList(string pUserID = "")
  {
   string strSQL = "";
   strSQL += "SELECT ";
   strSQL += "SU.*  ";
   strSQL += ", SA.ACCESS_NAME ";
   strSQL += "FROM SYS_USERS SU ";
   strSQL += "INNER JOIN SYS_ACCESS SA ON SA.ACCESS_CODE = SU.Access_Code ";   
   strSQL += pUserID.Length > 0 ? "WHERE S.UserID = @pUserID " : "";
   strSQL += "ORDER BY SU.FullName ";
   List<SYS_USERS_MODEL> list = new List<SYS_USERS_MODEL>();

   using (SqlConnection cn = new SqlConnection(CMMC.Models.SharedFunctions.Connectionstring))
   {
    using (SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = strSQL;
     if (pUserID.Length > 0)
     {
      cmd.Parameters.Add(new SqlParameter("@pUserID", pUserID));
     }

     cn.Open();
     using (SqlDataReader dr = cmd.ExecuteReader())
     {
      while (dr.Read())
      {
       list.Add(new SYS_USERS_MODEL()
       {
        UserID = dr["UserID"].ToString()
        ,FullName = dr["FullName"].ToString()
        ,Email = dr["Email"].ToString()
        ,IsActive = dr["IsActive"].ToString().Equals("1")
        ,Access_Code = dr["Access_Code"].ToString().ToInt()
        ,CreatedBy = dr["CreatedBy"].ToString()
        ,CreatedOn = dr["CreatedOn"].ToString().ToDateTime()
        ,ModifiedBy = dr["ModifiedBy"].ToString()
        ,ModifiedOn = dr["ModifiedOn"].ToString().ToDateTime()
        ,Attribute1 = dr["Attribute1"].ToString()
        ,Attribute2 = dr["Attribute2"].ToString()
        ,Attribute3 = dr["Attribute3"].ToString()
        ,LastOnlineOn = dr["LastOnlineOn"].ToString().ToDateTime()        
       });
      }
     }
    }
   }
   return list;
  }

  public string GetTypeUser(string pNetworkID)
  {
   string strReturn = "";
   string strSQL = "SELECT UserType FROM ";
   strSQL += "SYS_USERS ";
   strSQL += "WHERE UserID = @pUserID";
   using (SqlConnection cn = new SqlConnection(CMMC.Models.SharedFunctions.Connectionstring))
   {
    using (SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = strSQL;
     cmd.Parameters.Add(new SqlParameter("@pUserID", pNetworkID));
     cn.Open();
     strReturn = cmd.ExecuteScalar().ToString();
    }
   }
   return strReturn;
  }

  public void UpdateLastLogOn(string pUserID, int pAccessCode)
  {
   try
   {
    using (SqlConnection cn = new SqlConnection(CMMC.Models.SharedFunctions.Connectionstring))
    {
     using (SqlCommand cmd = cn.CreateCommand())
     {
      cmd.CommandText = "UPDATE SYS_USERS SET LastOnlineOn=GETDATE() WHERE UserID = @pUserID AND Access_Code = @pAccessCode";
      cmd.Parameters.Add(new SqlParameter("@pUserID", pUserID));
      cmd.Parameters.Add(new SqlParameter("@pAccessCode", pAccessCode));
      cn.Open();
      cmd.ExecuteNonQuery();
     }
    }
   }
   catch (Exception ex)
   {
    CTBC.Logs.Write("UpdateLastLogOn", ex.Message, "SYS_USERS");
   }
  }

  public List<SYS_USERS_MODEL> GetApproverTypeList(int pAccessCode)
  {
   List<SYS_USERS_MODEL> list = new List<SYS_USERS_MODEL>();
   string strSQL = "SELECT * FROM SYS_USERS ";
   strSQL += "WHERE Access_Code = @pAccessCode and IsActive = 1";
   try
   {
    using (SqlConnection cn = new SqlConnection(CMMC.Models.SharedFunctions.Connectionstring))
    {
     using (SqlCommand cmd = cn.CreateCommand())
     {
      cmd.CommandText = strSQL;
      cmd.Parameters.Add(new SqlParameter("@pAccessCode", pAccessCode = 3));
      cn.Open();
      using (SqlDataReader dr = cmd.ExecuteReader())
      {
       while (dr.Read())
       {
        list.Add(new SYS_USERS_MODEL
        {
         UserID = dr["UserID"].ToString(),
         FullName = dr["FullName"].ToString()
        });
       }
      }
     }
    }
   }
   catch (Exception e)
   {
    CTBC.Logs.Write("GetApproverTypeList", e.Message, "SYS_USERS");
   }
   return list;
  }
 }
}