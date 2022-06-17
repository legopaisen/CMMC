using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using CTBC;

namespace CMMC.Models
{
 public class ServiceTypes : IDisposable
 {
   public void Dispose() { GC.SuppressFinalize(this); }

   public string Category { get; set; }

  public struct Details
  {
   public string ServiceID { get; set; }
   public string ServiceName { get; set; }
   public bool IsActive { get; set; }
   public string Status { get; set; }
   public string CreatedBy { get; set; }
   public DateTime CreatedOn { get; set; }
   public string ModifiedBy { get; set; }
   public DateTime? ModifiedOn { get; set; }
   public string ServiceCategory { get; set; }
  }

  public List<ServiceTypes> GetCategory()
  {
   var tag = new List<ServiceTypes>  
   {
    new ServiceTypes{Category = "NetBanking"},
    new ServiceTypes{Category = "BancNet"},
    new ServiceTypes{Category = "Others"},
   };
   return tag;
  }

  public Details Fill(string pID)
  {
   Details details = new Details();
   using(SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
   {
    using(SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = "SELECT * FROM Services WHERE ServiceID = @pID";
     cmd.Parameters.Add(new SqlParameter("@pID", pID));
     cn.Open();
     using(SqlDataReader dr = cmd.ExecuteReader())
     {
      if(dr.Read())
      {
       details.ServiceID = dr["ServiceID"].ToString();
       details.ServiceName = dr["ServiceName"].ToString();
       details.IsActive = dr["IsActive"].ToString().Equals("1");
       details.Status = dr["Status"].ToString();
       details.CreatedBy = dr["CreatedBy"].ToString();
       details.CreatedOn = dr["CreatedOn"].ToString().ToDateTime();
       details.ModifiedBy = dr["ModifiedBy"].ToString() == null ? "" : dr["ModifiedBy"].ToString();
       details.ModifiedOn = dr["ModifiedOn"].ToString().ToDateTimeParse();
       details.ServiceCategory = dr["ServiceCategory"].ToString() == null ? "" : dr["ServiceCategory"].ToString();
      }
     }
    }
   }
   return details;
  }

  public int ServicesProcessAction(List<int> IDs, bool IsActivate, string DeactivatedBy)
  {
   int intReturn = 0;
   string strSQL = "";

   strSQL += "UPDATE Services ";
   strSQL += "SET ";
   strSQL += "IsActive = @pIsActive ";
   strSQL += ",ModifiedBy = @pModifiedBy ";
   strSQL += ",ModifiedOn = GETDATE() ";
   strSQL += "WHERE ";
   strSQL += "ServiceID = @pServiceID ";

   using (SqlConnection cn = new SqlConnection(CMMC.Models.SharedFunctions.Connectionstring))
   {
    using (SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = strSQL;
     cn.Open();
     foreach (int intIDs in IDs)
     {
      cmd.Parameters.Clear();
      cmd.Parameters.Add(new SqlParameter("@pServiceID", intIDs));
      cmd.Parameters.Add(new SqlParameter("@pIsActive", IsActivate ? "1" : "0"));
      cmd.Parameters.Add(new SqlParameter("@pModifiedBy", DeactivatedBy));
      intReturn += cmd.ExecuteNonQuery();
     }
    }
   }
   return intReturn;
  }

  public bool IsExist(string pServiceName)
  {
   bool blnReturn = false;
   using (SqlConnection cn = new SqlConnection(CMMC.Models.SharedFunctions.Connectionstring))
   {
    using (SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = "SELECT COUNT(*) AS COUNTER FROM Services WHERE ServiceName = @pServiceName";
     cmd.Parameters.Add(new SqlParameter("@pServiceName", pServiceName));
     cn.Open();
     blnReturn = cmd.ExecuteScalar().ToString().ToInt() > 0;
    }
   }
   return blnReturn;
  }

  public int Insert(Details pDetails)
  {
   int intReturn = 0;
   string strSQL = "";
   strSQL += "INSERT INTO Services ";
   strSQL += "VALUES";
   strSQL += "(";
   strSQL += "@pServiceName ";
   strSQL += ",@pIsActive ";
   strSQL += ",NULL ";
   strSQL += ",@pCreatedBy ";
   strSQL += ",@pCreatedOn ";
   strSQL += ",NULL ";
   strSQL += ",NULL ";
   strSQL += ",@pServiceCategory)";

   if (!this.IsExist(pDetails.ServiceName))
   {
    try
    {
     using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
     {
      using (SqlCommand cmd = cn.CreateCommand())
      {
       cmd.CommandText = strSQL;
       cmd.Parameters.Add(new SqlParameter("@pServiceName", pDetails.ServiceName));
       cmd.Parameters.Add(new SqlParameter("@pIsActive", pDetails.IsActive ? "1" : "0"));
       cmd.Parameters.Add(new SqlParameter("@pCreatedBy", pDetails.CreatedBy));
       cmd.Parameters.Add(new SqlParameter("@pCreatedOn", pDetails.CreatedOn));
       cmd.Parameters.Add(new SqlParameter("@pServiceCategory", pDetails.ServiceCategory));
       cn.Open();
       intReturn = cmd.ExecuteNonQuery();
      }
     }

    }
    catch (Exception e)
    {
     CTBC.Logs.Write("Insert", e.Message, "Service Types");

    }
   }
   return intReturn;
  }

  public int Update(Details pDetails)
  {
   int intReturn = 0;
   string strSQL = "";
   strSQL += "UPDATE Services ";
   strSQL += "SET ";   
   strSQL += "ServiceName = @pServiceName ";
   strSQL += ",IsActive = @pIsActive ";
   strSQL += ",Status = NULL ";
   strSQL += ",ModifiedBy = @pModifiedBy ";
   strSQL += ",ModifiedOn = GETDATE() ";
   strSQL += ",ServiceCategory = @pServiceCategory ";
   strSQL += "WHERE ";
   strSQL += "ServiceID = @pID ";

   using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
   {
    using (SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = strSQL;
     cmd.Parameters.Add(new SqlParameter("@pID", pDetails.ServiceID));
     cmd.Parameters.Add(new SqlParameter("@pServiceName", pDetails.ServiceName));
     cmd.Parameters.Add(new SqlParameter("@pIsActive", pDetails.IsActive ? "1" : "0"));
     cmd.Parameters.Add(new SqlParameter("@pModifiedBy", pDetails.ModifiedBy));
     cmd.Parameters.Add(new SqlParameter("@pServiceCategory", pDetails.ServiceCategory));
     cn.Open();
     intReturn = cmd.ExecuteNonQuery();
    }
   }
   return intReturn;
  }

  public int Delete(string pID)
  {
   int intReturn = 0;
   string strSQL = "Delete Services WHERE ServiceID = @pCode";

   using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
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

  public List<Details> GetList()
  {
   List<Details> list = new List<Details>();
   string strSQL = "";
   strSQL += "SELECT ";
   strSQL += " ServiceID ";
   strSQL += " ,ServiceName ";
   strSQL += " ,IsActive ";
   strSQL += " ,Status ";
   strSQL += " ,CreatedBy ";
   strSQL += " ,CreatedOn ";
   strSQL += " ,ModifiedOn ";
   strSQL += " ,ModifiedBy ";
   strSQL += " ,ServiceCategory ";
   strSQL += " FROM Services ";
   strSQL += " ORDER BY ServiceID, ServiceName " ;
 
   try
   {
   using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
   {
    using (SqlCommand cmd = cn.CreateCommand())
    {
     cn.Open();
     cmd.CommandText = strSQL ;
     
     using(SqlDataReader dr = cmd.ExecuteReader())
     {
      while(dr.Read())
      {
       list.Add(new Details(){
       ServiceID = dr["ServiceID"].ToString(),
       ServiceName = dr["ServiceName"].ToString(),
       IsActive = dr["IsActive"].ToString().Equals("1"),
       Status = dr["Status"].ToString(),
       CreatedBy = dr["CreatedBy"].ToString(),
       CreatedOn = dr["CreatedOn"].ToString().ToDateTime(),
       ModifiedBy = dr["ModifiedBy"].ToString(),
       ModifiedOn = dr["ModifiedOn"].ToString().ToDateTimeParse(),
       ServiceCategory = dr["ServiceCategory"].ToString()
       });
      }
     }
    }
   }
   }
   catch(Exception ex)
   {
   }
   return list;
  }
 }
 }
