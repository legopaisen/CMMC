using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using CTBC;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Runtime.Serialization;


namespace CMMC.Models
{
 public class RelatedAccounts : IDisposable
 {
  public void Dispose() { GC.SuppressFinalize(this); }

  public string PendingRequest { get; set; }

  public struct Details
  {
   public int ID { get; set; } 
   public string AccountID { get; set; }
   public string AccountName { get; set; }
   public DateTime DateAdded { get; set; }
   public string AddedBy { get; set; }
   public string Status { get; set; }
   public int CMSCode { get; set; }
   public int LinkedCMSCode { get; set; }
   public string StatusName { get; set; }
  }
  
  public Details Fill(int pLinkedCMSCode)
  {
   Details details = new Details();
   using(SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring)){
    using(SqlCommand cmd = cn.CreateCommand()){
     cmd.CommandText = "SELECT * FROM RelatedAccounts WHERE LinkedCMSCode = @pLinkedCMSCode";
     cmd.Parameters.Add(new SqlParameter("@pLinkedCMSCode", pLinkedCMSCode));
     cn.Open();
     using(SqlDataReader dr = cmd.ExecuteReader()){
      if(dr.Read()){
       details.ID = dr["ID"].ToString().ToInt();
       details.AccountID = dr["AccountID"].ToString();
       details.AccountName = dr["AccountName"].ToString();
       details.AddedBy = dr["AddedBy"].ToString();
       details.LinkedCMSCode = dr["LinkedCMSCode"].ToString().ToInt();
       details.DateAdded = dr["DateAdded"].ToString().ToDateTime();
       details.Status = dr["Status"].ToString();    
      }
     }
    }
   }
   return details;
  }

  public int Insert(Details pdetails)
  {
   int intReturn = 0;
   string strSQL = "INSERT INTO ";
   strSQL += "RelatedAccounts ";
   strSQL += "VALUES ";
   strSQL += "( ";
   strSQL += "@pAccountID ";
   strSQL += ",@pAccountName ";
   strSQL += ",@pDateAdded ";
   strSQL += ",@pAddedBy ";
   strSQL += ",@pStatus ";
   strSQL += ",@pCMSCode ";
   strSQL += ",@pLinkedCMSCode ";
   strSQL += ") ";
   try
   {
    using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
    {
     using (SqlCommand cmd = cn.CreateCommand())
     {
      cmd.CommandText = strSQL;
      cmd.Parameters.Add(new SqlParameter("@pAccountID", pdetails.AccountID));
      cmd.Parameters.Add(new SqlParameter("@pAccountName", pdetails.AccountName));
      cmd.Parameters.Add(new SqlParameter("@pDateAdded", pdetails.DateAdded));
      cmd.Parameters.Add(new SqlParameter("@pAddedBy", pdetails.AddedBy));
      cmd.Parameters.Add(new SqlParameter("@pLinkedCMSCode", pdetails.LinkedCMSCode));
      cmd.Parameters.Add(new SqlParameter("@pStatus", pdetails.Status));
      cmd.Parameters.Add(new SqlParameter("@pCMSCode", pdetails.CMSCode));
      cn.Open();
      intReturn = cmd.ExecuteNonQuery();
     }
    }
   }
  catch(Exception e){
   CTBC.Logs.Write("Insert", e.Message, "Related Accounts");
  }
   return intReturn;
  }

  public int Update(Details pdetails)
  {
   int intReturn = 0;
   string strSQL = "UPDATE ";
   strSQL += "RelatedAccounts ";
   strSQL += "SET ";
   strSQL += "AccountName = @pAccountName ";
   strSQL += ",DateAdded = @pDateAdded ";
   strSQL += ",AddedBy = @pAddedBy ";
   strSQL += ",Status = @pStatus ";
   strSQL += "WHERE ";
   strSQL += "LinkedCMSCode = @pLinkedCMSCode ";
   strSQL += "AND AccountID = @pAccountID ";
   using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
   {
    using (SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = strSQL;
     cmd.Parameters.Add(new SqlParameter("@pAccountID",pdetails.AccountID));
     cmd.Parameters.Add(new SqlParameter("@pAccountName",pdetails.AccountName));
     cmd.Parameters.Add(new SqlParameter("@pDateAdded", pdetails.DateAdded));
     cmd.Parameters.Add(new SqlParameter("@pAddedBy",pdetails.AddedBy));
     cmd.Parameters.Add(new SqlParameter("@pStatus",pdetails.Status));
     cmd.Parameters.Add(new SqlParameter("@pLinkedCMSCode", pdetails.LinkedCMSCode));
     cn.Open();
     intReturn = cmd.ExecuteNonQuery();
    }
   }
   return intReturn;
  }

  public int Delete(int pCMSCode, string pAccountID)
  {
   int intReturn = 0;
   using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
   {
    using (SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = "DELETE FROM RelatedAccounts WHERE LinkedCMSCode = @pLinkedCMSCode AND AccountID = @pAccountID";
     cmd.Parameters.Add(new SqlParameter("@pLinkedCMSCode", pCMSCode));
     cmd.Parameters.Add(new SqlParameter("@pAccountID", pAccountID));
     cn.Open();
     intReturn = cmd.ExecuteNonQuery();
    }
   }
   return intReturn;
  }

  public int CancelandRemove(int pCMSCode)
  {
   int intReturn = 0;
   using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
   {
    using (SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = "DELETE FROM RelatedAccounts WHERE LinkedCMSCode = @pLinkedCMSCode";
     cmd.Parameters.Add(new SqlParameter("@pLinkedCMSCode", pCMSCode));
     cn.Open();
     intReturn = cmd.ExecuteNonQuery();
    }
   }
   return intReturn;
  }
  
  public List<Details> GetList(int pCMSCode)
  {
   List<Details> list = new List<Details>();
   string strSQL = "SELECT * FROM RelatedAccounts R ";
   strSQL += "INNER JOIN Status ST ON  St.StatusID = R.Status ";
   strSQL += "WHERE LinkedCMSCode = @pLinkedCMSCode ";


   using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
   {
    using (SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = strSQL;
     cmd.Parameters.Add(new SqlParameter("@pLinkedCMSCode", pCMSCode));
     cn.Open();
     using (SqlDataReader dr = cmd.ExecuteReader())
     {
      while (dr.Read())
      {
       list.Add(new Details
       {
        ID = dr["ID"].ToString().ToInt(),
        AccountID = dr["AccountID"].ToString(),
        AccountName = dr["AccountName"].ToString(),
        DateAdded = dr["DateAdded"].ToString().ToDateTime(),
       AddedBy = dr["AddedBy"].ToString(),
       Status = dr["Status"].ToString(),
        LinkedCMSCode = dr["LinkedCMSCode"].ToString().ToInt(),
        StatusName = dr["StatusName"].ToString()
       });
      }
     }
    }
   }
   return list;
  }

  public List<Details> GetNotAddedRelatedAccount(int pCMSCode)
  {
   List<Details> list = new List<Details>();
   string strSQL = "SELECT DISTINCT AccountNo, rtrim(ltrim(AccountName)) as AccountName ";
   strSQL += "FROM AccountInformations ";
   strSQL += "WHERE ";
   strSQL += "AccountNo NOT IN ";
   strSQL += "(SELECT AccountID FROM RelatedAccounts WHERE LinkedCMSCode = @pLinkedCMSCode)";

   using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
   {
    using (SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = strSQL;
     cmd.Parameters.Add(new SqlParameter("@pLinkedCMSCode", pCMSCode));
     cn.Open();
     using (SqlDataReader dr = cmd.ExecuteReader())
     {
      while (dr.Read())
      {
       list.Add(new Details
       {
        AccountID = dr["AccountNo"].ToString(),
        AccountName = dr["AccountName"].ToString()
       });
      }
     }
    }
   }
   return list;
  }

  public RelatedAccounts GetPendingRequest()
  {
   RelatedAccounts total = new RelatedAccounts();
   using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
   {
    using (SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = "SELECT COUNT(*) as total FROM RelatedAccounts WHERE Status = 1";
     cn.Open();
     using (SqlDataReader dr = cmd.ExecuteReader())
     {
      if (dr.Read())
      {
       total.PendingRequest = dr["total"].ToString();
      }
     }
    }
   }
   return total;
  }

 }
}