using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using CTBC;

namespace CMMC.Models
{
 public class AuditTrailModel
 {
  public int AuditTrailCode { get; set; }
  public string UserID { get; set; }
  public string Module { get; set; }
  public string IPAddress { get; set; }
  public DateTime CreatedOn { get; set; }
  public string OldValues { get; set; }
  public string NewValues { get; set; }
 }

 public class UserListModel
 {
  public string UserID { get; set; }
  public string FullName { get; set; }
  public string Email { get; set; }
  public string Access_Name { get; set; }
  public string IsActive { get; set; }
  public string CreatedBy { get; set; }
  public DateTime CreatedOn { get; set; }
  public string ModifiedBy { get; set; }
  public DateTime ModifiedOn { get; set; }
  public string Permission { get; set; }
  public string Module_Code { get; set; }
  public DateTime? LastOnlineOn { get; set; }
  public string UserType { get; set; }
 }

 public class AuditTrail : IDisposable
 {
  public void Dispose() { GC.SuppressFinalize(this); }

  public List<AuditTrailModel> GetList()
  {
   List<AuditTrailModel> list = new List<AuditTrailModel>();
   using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
   {
    using (SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = "SELECT * FROM AuditTrail";
     cn.Open();
     using (SqlDataReader dr = cmd.ExecuteReader())
     {
      while (dr.Read())
      {
       list.Add(new AuditTrailModel()
       {
        AuditTrailCode = dr["AuditTrailCode"].ToInt()
        ,UserID = dr["UserID"].ToString()
        ,Module = dr["Module"].ToString()
        ,IPAddress = dr["IPAddress"].ToString()
        ,CreatedOn = dr["CreatedOn"].ToDateTime()
        ,OldValues = dr["OldValues"].ToString()
        ,NewValues = dr["NewValues"].ToString()
       });
      }
     }
    }
   }
   return list;
  }

  public List<AuditTrailModel> GetAuditTrail(string pModule, DateTime? pStartDate = null, DateTime? pEndDate = null)
  {
   List<AuditTrailModel> list = new List<AuditTrailModel>();
   string strSQL = "";
   strSQL += "SELECT AT.*";
   strSQL += ", SM.Module_Code";
   strSQL += ", SM.ModuleName ";
   strSQL += "FROM AuditTrail AT ";
   strSQL += "LEFT JOIN SYS_MODULES SM ON SM.Module_Code = AT.Module ";
   if (pModule != null)
   {
    strSQL += "WHERE AT.Module LIKE '%' + @pModule + '%' ";
   }
   if (pStartDate != null && pEndDate == null)
   {
    strSQL += "AND CONVERT(DATE, CreatedOn) >= @pStartDate ";
   }
   else if (pStartDate == null && pEndDate != null)
   {
    strSQL += "AND CONVERT(DATE, CreatedOn) <= @pEndDate ";
   }
   else if (pStartDate != null && pEndDate != null)
   {
    strSQL += "AND CONVERT(DATE, CreatedOn) BETWEEN @pStartDate AND @pEndDate ";
   }
   strSQL += "ORDER BY AT.CreatedOn desc ";

   try
   {
    using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
    {
     using (SqlCommand cmd = cn.CreateCommand())
     {
      cmd.CommandText = strSQL;
      if (pModule != null)
      {
       cmd.Parameters.Add(new SqlParameter("@pModule", pModule));
      }

      if (pStartDate != null)
      {
       cmd.Parameters.Add(new SqlParameter("@pStartDate", pStartDate));
      }

      if (pEndDate != null)
      {
       cmd.Parameters.Add(new SqlParameter("@pEndDate", pEndDate));
      }
      cn.Open();
      using (SqlDataReader dr = cmd.ExecuteReader())
      {
       while (dr.Read())
       {
        list.Add(new AuditTrailModel()
        {
         AuditTrailCode = dr["AuditTrailCode"].ToInt()
         ,UserID = dr["UserID"].ToString()
         ,Module = dr["ModuleName"].ToString()
         ,IPAddress = dr["IPAddress"].ToString()
         ,CreatedOn = dr["CreatedOn"].ToDateTime()
         ,OldValues = dr["OldValues"].ToString()
         ,NewValues = dr["NewValues"].ToString()
        });
       }
      }
     }
    }
   }
   catch (Exception ex)
   {
    Logs.Write("GetAuditTrail", ex.Message, "AuditTrail");
   }
   return list;
  }

  public List<UserListModel> GetUserList()
  {
   List<UserListModel> list = new List<UserListModel>();
   string strSQL = "";

   strSQL += " SELECT ";
   strSQL += " SU.UserID ";
   strSQL += " ,SU.FullName ";
   //strSQL += " ,SU.Email ";
   strSQL += " ,SA.Access_Name ";
   strSQL += " ,UserType = CASE WHEN SA.UserType = '0' THEN 'Maker' ";
   strSQL += "                  WHEN SA.UserType = '1' THEN 'Approver' ";
   strSQL += "                  WHEN SA.UserType = '2' THEN 'System Administrator' ";
   strSQL += "                  WHEN SA.UserType = '3' THEN 'Auditor' ";
   strSQL += "             ELSE '' ";
   strSQL += "          END ";
   strSQL += " ,Permission = CASE WHEN SAM. Permission = 'F' THEN 'Full Access' ";
   strSQL += "                    WHEN SAM.Permission = 'R' THEN 'Read Only' ";
   strSQL += "               ELSE '' ";
   strSQL += "         END ";
   strSQL += "  ,IsActive = CASE When SU.IsActive = '1' THEN 'Active' ";
   strSQL += "		                 WHEN SU.IsActive = '0' THEN 'Inactive' ";
   strSQL += "			           ELSE '' ";
   strSQL += "	        END ";
   strSQL += " ,Module_Code = CASE WHEN SAM.Module_Code = 'Access' THEN 'User Maintenance' ";
			strSQL += "                   		WHEN SAM.Module_Code = 'BatchComputation' THEN 'Batch Settings' "; 
			strSQL += "                   		WHEN SAM.Module_Code = 'CreateCmsCode' THEN 'Create CMS Code' ";
			strSQL += "                   		WHEN SAM.Module_Code = 'Dashboard' THEN 'Dashboard' ";
			strSQL += "                   		WHEN SAM.Module_Code = 'EditCmsCode' THEN 'Edit CMS Code' ";
			strSQL += "                   		WHEN SAM.Module_Code = 'EditRequest'THEN 'Edit Request' ";
			strSQL += "                   		WHEN SAM.Module_Code = 'Enrollment' THEN 'Enrollment' ";
			strSQL += "                   		WHEN SAM.Module_Code = 'Home' THEN 'Home' ";
   strSQL += "                   		WHEN SAM.Module_Code = 'RelationshipManager' THEN 'Relationship Manager Maintenance' ";
			strSQL += "                   		WHEN SAM.Module_Code = 'Report' THEN 'Reports' ";
			strSQL += "                   		WHEN SAM.Module_COde = 'SystemSettings' THEN 'System Settings' ";
   strSQL += "                     WHEN SAM.Module_Code = 'SystemParameter' THEN 'Parameter Maintenance' "; 
			strSQL += "              	ELSE '' ";
   strSQL += "             END ";
   strSQL += " ,SU.LastOnlineOn ";
   strSQL += "  FROM SYS_USERS SU ";
   strSQL += " INNER JOIN SYS_ACCESS SA ON SA.ACCESS_CODE = SU.Access_Code ";
   strSQL += " INNER JOIN SYS_ACCESS_MODS SAM ON SAM.AccessCode = SA.ACCESS_CODE ";
   strSQL += " WHERE SU.UserID IN (SELECT UserID FROM SYS_USERS) ";

   using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
   {
    cn.Open();
    using (SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = strSQL;
     using (SqlDataReader dr = cmd.ExecuteReader())
     {
      while (dr.Read())
      {
       list.Add(new UserListModel()
       {
        UserID = dr["UserID"].ToString()
        ,FullName = dr["FullName"].ToString()
        //,Email = dr["Email"].ToString()
        ,Access_Name = dr["Access_Name"].ToString()
        ,UserType = dr["UserType"].ToString()
        ,IsActive = dr["IsActive"].ToString()
        ,Module_Code = dr["Module_Code"].ToString()
        ,Permission = dr["Permission"].ToString()
        ,LastOnlineOn = dr["LastOnlineOn"] == null ? null : dr["LastOnlineOn"].ToString().ToDateTimeParse()
       });
      }
     }
    }
   }
   return list;
  }

  public int Insert(AuditTrailModel Model)
  {
   int intReturn = 0;
   string strSQL = "";
   strSQL += "INSERT INTO ";
   strSQL += "AuditTrail ";
   strSQL += "VALUES( ";
   strSQL += "@pUserID";
   strSQL += ", @pModule";
   strSQL += ", @pIPAddress";
   strSQL += ", @pCreatedOn";
   strSQL += ", @pOldValues";
   strSQL += ", @pNewValues";
   strSQL += " )";

   try
   {
    using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
    {
     using (SqlCommand cmd = cn.CreateCommand())
     {
      cmd.CommandText = strSQL;
      cmd.Parameters.Add(new SqlParameter("@pUserID", Model.UserID));
      cmd.Parameters.Add(new SqlParameter("@pModule", Model.Module));
      cmd.Parameters.Add(new SqlParameter("@pIPAddress", Model.IPAddress));
      cmd.Parameters.Add(new SqlParameter("@pCreatedOn", DateTime.Now));
      cmd.Parameters.Add(new SqlParameter("@pOldValues", Model.OldValues == null ? "" : Model.OldValues));
      cmd.Parameters.Add(new SqlParameter("@pNewValues", Model.NewValues == null ? "" : Model.NewValues));
      cn.Open();
      intReturn = cmd.ExecuteNonQuery();
     }
    }
   }
   catch (Exception ex)
   {
    CTBC.Logs.Write("Insert", ex.Message, "AuditTrail");
   }
   return intReturn;
  }
 }
}