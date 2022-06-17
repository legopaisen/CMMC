using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using CTBC;

namespace CMMC.Models
{
 public class InvestmentTypes : IDisposable
 {
  public void Dispose() { GC.SuppressFinalize(this); }

  public struct InvestmentTypeDetails
  {
   public string InvestmentCode { get; set; }
   public string InvestmentDescription { get; set; }
   public bool IsActive { get; set; }
   public string Status { get; set; }
   public string CreatedBy { get; set; }
   public DateTime CreatedOn { get; set; }
   public string ModifiedBy { get; set; }
   public DateTime? ModifiedOn { get; set; }
  }

  public InvestmentTypeDetails Fill (string pInvestmentCode)
  {
   InvestmentTypeDetails investmentdetails = new InvestmentTypeDetails();
   using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
   {
    using (SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = "SELECT * FROM InvestmentType WHERE it_InvstTypeCode = @pInvestmentCode";
     cmd.Parameters.Add(new SqlParameter("@pInvestmentCode", pInvestmentCode));
     cn.Open();
     using (SqlDataReader dr = cmd.ExecuteReader())
     {
      if (dr.Read())
      {
       investmentdetails.InvestmentCode = dr["it_InvstTypeCode"].ToString();
       investmentdetails.InvestmentDescription = dr["it_InvstTypeDesc"].ToString();
       investmentdetails.IsActive = dr["IsActive"].ToString().Equals("1");
       investmentdetails.Status = dr["Status"].ToString();
       investmentdetails.CreatedBy = dr["CreatedBy"].ToString();
       investmentdetails.CreatedOn = dr["CreatedOn"].ToString().ToDateTime();
       investmentdetails.ModifiedBy = dr["ModifiedBy"].ToString();
       investmentdetails.ModifiedOn = dr["ModifiedOn"].ToString().ToDateTimeParse();
      }
     }
    }
   }
   return investmentdetails;
  }

  public List<InvestmentTypeDetails> GetList()
  {
   List<InvestmentTypeDetails> list = new List<InvestmentTypeDetails>();
   string strSQL = "";
   strSQL += "SELECT ";
   strSQL += " it_InvstTypeCode ";
   strSQL += " ,it_InvstTypeDesc ";
   strSQL += " ,IsActive ";
   strSQL += " ,Status ";
   strSQL += " ,CreatedBy ";
   strSQL += " ,CreatedOn ";
   strSQL += " ,ModifiedBy ";
   strSQL += " ,ModifiedOn ";
   strSQL += " FROM InvestmentType ";
   strSQL += " ORDER BY it_InvstTypeCode, it_InvstTypeDesc";

   using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
   {
    using (SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = strSQL;
     cn.Open();
     using (SqlDataReader dr = cmd.ExecuteReader())
     {
      while (dr.Read())
      {
       list.Add(new InvestmentTypeDetails()
       {
        InvestmentCode = dr["it_InvstTypeCode"].ToString(),
        InvestmentDescription = dr["it_InvstTypeDesc"].ToString(),
        IsActive = dr["IsActive"].ToString().Equals("1"),
        Status = dr["Status"].ToString(),
        CreatedBy = dr["CreatedBy"].ToString(),
        CreatedOn = dr["CreatedOn"].ToString().ToDateTime(),
        ModifiedBy = dr["ModifiedBy"].ToString(),
        ModifiedOn = dr["ModifiedOn"].ToString().ToDateTimeParse()
       });
      }
     }
    }
   }
   return list;
  }

  public bool IsExist(string pInvestmentTypeCode)
  {
   bool blnReturn = false;
   using (SqlConnection cn = new SqlConnection(CMMC.Models.SharedFunctions.Connectionstring))
   {
    using (SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = "SELECT COUNT(*) AS COUNTER FROM InvestmentType WHERE it_InvstTypeCode = @pInvestmentTypeCode";
     cmd.Parameters.Add(new SqlParameter("@pInvestmentTypeCode", pInvestmentTypeCode));
     cn.Open();
     blnReturn = cmd.ExecuteScalar().ToString().ToInt() > 0;
    }
   }
   return blnReturn;
  }

  public int Insert(InvestmentTypeDetails pDetails)
  {
   int intReturn = 0;
   string strSQL = "";
   strSQL += "INSERT INTO InvestmentType ";
   strSQL += " VALUES ";
   strSQL += "(@pInvstTypeCode ";
   strSQL += " ,@pInvstTypeDesc ";
   strSQL += " ,@pIsActive ";
   strSQL += " ,NULL ";
   strSQL += " ,@pCreatedBy ";
   strSQL += " ,@pCreatedOn ";
   strSQL += " ,NULL ";
   strSQL += " ,NULL)";
 
   if (!this.IsExist(pDetails.InvestmentCode))
   {
     using (SqlConnection cn = new SqlConnection(CMMC.Models.SharedFunctions.Connectionstring))
     {
      using (SqlCommand cmd = cn.CreateCommand())
      {
       cmd.CommandText = strSQL;
       cmd.Parameters.Add(new SqlParameter("@pInvstTypeCode", pDetails.InvestmentCode));
       cmd.Parameters.Add(new SqlParameter("@pInvstTypeDesc", pDetails.InvestmentDescription));
       cmd.Parameters.Add(new SqlParameter("@pIsActive", pDetails.IsActive ? "1" : "0"));
       cmd.Parameters.Add(new SqlParameter("@pCreatedBy", pDetails.CreatedBy));
       cmd.Parameters.Add(new SqlParameter("@pCreatedOn", pDetails.CreatedOn));
       cn.Open();
       intReturn = cmd.ExecuteNonQuery();
      }
     }
   }
   return intReturn;
  }

  public int Update(InvestmentTypeDetails pDetails)
  {
   int intReturn = 0;
   string strSQL = "";
   strSQL += "UPDATE InvestmentType ";
   strSQL += "SET ";
   strSQL += "it_InvstTypeDesc = @pInvestmentDesc ";
   strSQL += ",IsActive = @pIsActive ";
   strSQL += ",Status = NULL ";
   strSQL += ",ModifiedBy = @pModifiedBy ";
   strSQL += ",ModifiedOn = GETDATE() ";
   strSQL += "WHERE ";
   strSQL += "it_InvstTypeCode = @pInvestmentCode ";

   using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
   {
    using (SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = strSQL;
     cmd.Parameters.Add(new SqlParameter("@pInvestmentCode", pDetails.InvestmentCode));
     cmd.Parameters.Add(new SqlParameter("@pInvestmentDesc", pDetails.InvestmentDescription));
     cmd.Parameters.Add(new SqlParameter("@pIsActive", pDetails.IsActive ? "1" : "0"));
     cmd.Parameters.Add(new SqlParameter("@pModifiedBy", pDetails.ModifiedBy));
     cn.Open();
     intReturn = cmd.ExecuteNonQuery();
    }
   }
   return intReturn;
  }

  public int InvestmentProcessAction(List<string> IDs, bool IsActivate, string DeactivatedBy)
  {
   int intReturn = 0;
   string strSQL = "";

   strSQL += "UPDATE InvestmentType ";
   strSQL += "SET ";
   strSQL += "IsActive = @pIsActive ";
   strSQL += ",ModifiedBy = @pModifiedBy ";
   strSQL += ",ModifiedOn = GETDATE() ";
   strSQL += "WHERE ";
   strSQL += "it_InvstTypeCode = @pCode ";

   using (SqlConnection cn = new SqlConnection(CMMC.Models.SharedFunctions.Connectionstring))
   {
    using (SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = strSQL;
     cn.Open();
     foreach (string intIDs in IDs)
     {
      cmd.Parameters.Clear();
      cmd.Parameters.Add(new SqlParameter("@pCode", intIDs));
      cmd.Parameters.Add(new SqlParameter("@pIsActive", IsActivate ? "1" : "0"));
      cmd.Parameters.Add(new SqlParameter("@pModifiedBy", DeactivatedBy));
      intReturn += cmd.ExecuteNonQuery();
     }
    }
   }

   return intReturn;
  }

  public int Delete(string pID)
  {
   int intReturn = 0;
   string strSQL = "Delete InvestmentType WHERE it_InvstTypeCode = @pCode";
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
 }
}