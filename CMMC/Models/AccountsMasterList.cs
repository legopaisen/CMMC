using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CTBC;
using System.Data.SqlClient;

namespace CMMC.Models
{
 public class AccountsMasterList : IDisposable
 {
  public void Dispose() { GC.SuppressFinalize(this); }

  public struct Details
  {
   public int ID { get; set; }
   public int CMSCode { get; set; }
   public string AccountNumber { get; set; }
   public string AccountName { get; set; }
   public string BranchCode { get; set; }
   public DateTime DateEnrolled { get; set; }
   public string InvestmentType { get; set; }
   public DateTime EffectivityDate { get; set; }
   public string Tag { get; set; }
   public string IsActive { get; set; }
  }

  public List<Details> GetAcccountNotAddedList(Details details)
  {
   List<Details> list = new List<Details>();
   string strSQL = "SELECT * FROM ";
   strSQL += "AccountsMasterList ";
   strSQL += "WHERE AccountNo NOT IN ";
   strSQL += "(Select Distinct AccountNo FROM AccountInformations WHERE CMSCode = @pCMSCode) ";
   try
   {
    using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
    {
     using (SqlCommand cmd = cn.CreateCommand())
     {
      cmd.CommandText = strSQL;
      cmd.Parameters.Add(new SqlParameter("@pCMSCode", details.CMSCode));
      cn.Open();
      using (SqlDataReader dr = cmd.ExecuteReader())
      {
       while (dr.Read())
       {
        list.Add(new Details
        {
         AccountName = dr["AccountName"].ToString(),
         AccountNumber = dr["AccountNo"].ToString(),
         BranchCode = dr["BranchCode"].ToString(),
         DateEnrolled = dr["DateEnrolled"].ToString().ToDateTime(),
         EffectivityDate = dr["EffectivityDate"].ToString().ToDateTime(),
         InvestmentType = dr["InvestmentType"].ToString(),
         ID = dr["ID"].ToString().ToInt(),
         Tag = dr["Tagging"].ToString(),
         IsActive = dr["IsActive"].ToString()
        });
       }
      }
     }
    }
   }
   catch(Exception ex){
    CTBC.Logs.Write("GetAccountNotAddedList", ex.Message, "Accounts Masterlist");
   }


   return list;
  }

  public List<Details> GetAcccountNotAddedRelatedList(Details details)
  {
   List<Details> list = new List<Details>();
   string strSQL = "SELECT * FROM ";
   strSQL += "AccountsMasterList ";
   strSQL += "WHERE AccountNo NOT IN ";
   strSQL += "(Select Distinct AccountID FROM RelatedAccounts WHERE LinkedCMSCode = @pCMSCode) ";
   try
   {
    using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
    {
     using (SqlCommand cmd = cn.CreateCommand())
     {
      cmd.CommandText = strSQL;
      cmd.Parameters.Add(new SqlParameter("@pCMSCode", details.CMSCode));
      cn.Open();
      using (SqlDataReader dr = cmd.ExecuteReader())
      {
       while (dr.Read())
       {
        list.Add(new Details
        {
         AccountName = dr["AccountName"].ToString(),
         AccountNumber = dr["AccountNo"].ToString(),
         BranchCode = dr["BranchCode"].ToString(),
         DateEnrolled = dr["DateEnrolled"].ToString().ToDateTime(),
         EffectivityDate = dr["EffectivityDate"].ToString().ToDateTime(),
         InvestmentType = dr["InvestmentType"].ToString(),
         ID = dr["ID"].ToString().ToInt(),
         Tag = dr["Tagging"].ToString(),
         IsActive = dr["IsActive"].ToString()
        });
       }
      }
     }
    }
   }
   catch (Exception e)
   {
    CTBC.Logs.Write("GetAccountNotAddedRelatedList", e.Message, "Accounts Masterlist");

   }


   return list;
  }

  public List<Details> GetAccountNoAddedList(Details details)
  {
   List<Details> list = new List<Details>();
   string strSQL = "SELECT * FROM ";
   strSQL += "AccountsMasterList ";
   strSQL += "WHERE AccountNo IN ";
   strSQL += "(Select Distinct AccountNo FROM AccountInformations WHERE CMSCode = @pCMSCode) ";
   try
   {
    using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
    {
     using (SqlCommand cmd = cn.CreateCommand())
     {
      cmd.CommandText = strSQL;
      cmd.Parameters.Add(new SqlParameter("@pCMSCode", details.CMSCode));
      cn.Open();
      using (SqlDataReader dr = cmd.ExecuteReader())
      {
       while (dr.Read())
       {
        list.Add(new Details
        {
         AccountNumber = dr["AccountNo"].ToString(),
         AccountName = dr["AccountName"].ToString()
        });
       }
      }
     }
    }
   }
   catch (Exception e)
   {
    CTBC.Logs.Write("GetAccountNoAddedList", e.Message, "Account Masterlist");
   }


   return list;
  }

 }
}