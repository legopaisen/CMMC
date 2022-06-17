using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using CTBC;

namespace CMMC.Models
{
 public class Services : IDisposable
 {
  public void Dispose() { GC.SuppressFinalize(this); }

  public struct Details
  {
   public int ServiceID { get; set; }
   public string ServiceName { get; set; }
   public bool IsActive { get; set; }
   public string Status { get; set; }
   public string CreatedBy { get; set; }
   public DateTime CreatedOn { get; set; }
   public string ModifiedBy { get; set; }
   public DateTime ModifiedOn { get; set; }
   public decimal MotherRequiredADB { get; set; }
   public decimal SubRequiredADB { get; set; }
   public string MinNumberEmployee { get; set; }  
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
       details.ServiceID = dr["ServiceID"].ToString().ToInt();
       details.ServiceName = dr["ServiceName"].ToString();
       details.IsActive = dr["IsActive"].ToString().ToBoolean();
       details.Status = dr["Status"].ToString();
       details.CreatedBy = dr["CreatedBy"].ToString();
       details.CreatedOn = dr["CreatedOn"].ToString().ToDateTime();
       details.ModifiedBy = dr["ModifiedBy"].ToString();
       details.ModifiedOn = dr["ModifiedOn"].ToString().ToDateTime();
       details.MotherRequiredADB = dr["MotherRequiredADB"].ToString().ToDecimal();
       details.SubRequiredADB = dr["SubRequiredADB"].ToString().ToDecimal();
       details.MinNumberEmployee = dr["MinNumberEmployee"].ToString();
      }
     }
    }
   }
   return details;
  }

  public int Insert(Details pDetails)
  {
   int intReturn = 0;
   string strSQL = "";
   strSQL += "INSERT INTO Services ";
   strSQL += "VALUES";
   strSQL += "(";
   strSQL += "@pServiceName";
   strSQL += ",@pIsActive";
   strSQL += ",@pStatus";
   strSQL += ",@pCreatedBy";
   strSQL += ",@pCreatedOn";
   strSQL += ",@pModifiedBy";
   strSQL += ",@pModifiedOn";
   strSQL += ",@pMotherRequiredADB";
   strSQL += ",@pSubRequiredADB";
   strSQL += ",@pMinNumberEmployee";
   strSQL += ")";

   using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
   {
    using (SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = strSQL;
     cmd.Parameters.Add(new SqlParameter("@pServiceName", pDetails.ServiceName));
     cmd.Parameters.Add(new SqlParameter("@pIsActive", pDetails.IsActive));
     cmd.Parameters.Add(new SqlParameter("@pStatus", pDetails.Status));
     cmd.Parameters.Add(new SqlParameter("@pCreatedBy", pDetails.CreatedBy));
     cmd.Parameters.Add(new SqlParameter("@pCreatedOn", pDetails.CreatedOn));
     cmd.Parameters.Add(new SqlParameter("@pModifiedOn", pDetails.ModifiedOn));
     cmd.Parameters.Add(new SqlParameter("@pModifiedBy", pDetails.ModifiedBy));
     cmd.Parameters.Add(new SqlParameter("@pMotherRequiredADB", pDetails.MotherRequiredADB));
     cmd.Parameters.Add(new SqlParameter("@pSubRequiredADB", pDetails.SubRequiredADB));
     cmd.Parameters.Add(new SqlParameter("@pMinNumberEmployee", pDetails.MinNumberEmployee));
     cn.Open();
     intReturn = cmd.ExecuteNonQuery();
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
   strSQL += ",Status = @pStatus ";
   strSQL += ",ModifiedBy = @pModifiedBy ";
   strSQL += ",ModifiedOn = GETDATE() ";
   strSQL += ",MotherRequiredADB = @pMotherRequiredADB ";
   strSQL += ",SubRequiredADB = @pSubRequiredADB ";
   strSQL += ",MinNumberEmployee = @pMinNumberEmployee ";
   strSQL += "WHERE ";
   strSQL += "ServiceID = @pID ";

   using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
   {
    using (SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = strSQL;
     cmd.Parameters.Add(new SqlParameter("@pID", pDetails.ServiceID));
     cmd.Parameters.Add(new SqlParameter("@pServiceName", pDetails.ServiceName));
     cmd.Parameters.Add(new SqlParameter("@pIsActive", pDetails.IsActive));
     cmd.Parameters.Add(new SqlParameter("@pStatus", pDetails.Status));
     cmd.Parameters.Add(new SqlParameter("@pModifiedBy", pDetails.ModifiedBy));
     cmd.Parameters.Add(new SqlParameter("@pSubRequiredADB", pDetails.MotherRequiredADB));
     cmd.Parameters.Add(new SqlParameter("@pSubRequiredADB", pDetails.SubRequiredADB));
     cmd.Parameters.Add(new SqlParameter("@pMinNumberEmployee", pDetails.MinNumberEmployee));     
     cn.Open();
     intReturn = cmd.ExecuteNonQuery();
    }
   }
   return intReturn;
  }

  public int Delete(Details pDetails)
  {
   int intReturn = 0;
   using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
   {
    using (SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = "DELETE * FROM Services WHERE ServiceID = @pID";
     cmd.Parameters.Add(new SqlParameter("@pID",pDetails.ServiceID));
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
   strSQL += " ORDER BY ServiceID, ServiceName ";


   using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
   {
    using (SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = strSQL;
     cn.Open();
     using(SqlDataReader dr = cmd.ExecuteReader())
     {
      while(dr.Read())
      {
       list.Add(new Details(){
       ServiceID = dr["ServiceID"].ToString().ToInt(),
       ServiceName = dr["ServiceName"].ToString(),
       IsActive = dr["IsActive"].ToString().ToBoolean(),
       Status = dr["Status"].ToString(),
       CreatedBy = dr["CreatedBy"].ToString(),
       CreatedOn = dr["CreatedOn"].ToString().ToDateTime(),
       ModifiedBy = dr["ModifiedBy"].ToString(),
       ModifiedOn = dr["ModifiedOn"].ToString().ToDateTime(),
       MotherRequiredADB = dr["MotherRequiredADB"].ToString().ToDecimal(),
       SubRequiredADB = dr["SubRequiredADB"].ToString().ToDecimal(),
       MinNumberEmployee = dr["MinNumberEmployee"].ToString()
       });
      }
     }
    }
   }
   return list;
  }
 }
}
