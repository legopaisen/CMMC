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
 public class PenaltyCharges : IDisposable
 {
  public void Dispose() { GC.SuppressFinalize(this); }

  public struct Details
  {
   public int CMSCode { get; set; }
   public string DebitAccountNo { get; set; }
   public decimal BasePenalty { get; set; }
   public decimal PenaltyFee { get; set; }
   public bool IsAutoDebit { get; set; }
   public string Status { get; set; }
  }

  public Details Fill(int pID)
  {
   Details details = new Details();
   using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
   {
    using (SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = "SELECT * FROM PenaltyCharges WHERE CMSCode = @pID";
     cmd.Parameters.Add(new SqlParameter("@pID",pID));
     cn.Open();
     using(SqlDataReader dr = cmd.ExecuteReader())
     {
      if(dr.Read())
      {
       details.CMSCode = dr["CMSCode"].ToString().ToInt();
       details.DebitAccountNo = dr["DebitAccountNo"].ToString();
       details.BasePenalty = dr["BasePenalty"].ToString().ToDecimal();
       details.PenaltyFee = dr["PenaltyFee"].ToString().ToDecimal();
       details.IsAutoDebit = dr["IsAutoDebit"].ToString().ToBoolean();
       details.Status = dr["Status"].ToString();
      };
     }
    }
   }
   return details;
  }

  public int Insert(Details pDetials)
  {
   int intReturn = 0;
   string strSQL = "";
   strSQL += "INSERT INTO PenaltyCharges ";
   strSQL += "VALUES ";
   strSQL += "( "; 
   strSQL += "@pCMSCode ";
   strSQL += ",@pDebitAccountNo ";
   strSQL += ",@pBasePenalty ";
   strSQL += ",@pPenaltyFee ";
   strSQL += ",@pIsAutoDebit ";
   strSQL += ",@pStatus ";
   strSQL += ") ";
   try
   {
    using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
    {
     using (SqlCommand cmd = cn.CreateCommand())
     {
      cmd.CommandText = strSQL;
      cmd.Parameters.Add(new SqlParameter("@pCMSCode", pDetials.CMSCode));
      cmd.Parameters.Add(new SqlParameter("@pDebitAccountNo", pDetials.DebitAccountNo ?? DBNull.Value.ToString()));
      cmd.Parameters.Add(new SqlParameter("@pBasePenalty", pDetials.BasePenalty));
      cmd.Parameters.Add(new SqlParameter("@pPenaltyFee", pDetials.PenaltyFee));
      cmd.Parameters.Add(new SqlParameter("@pIsAutoDebit", pDetials.IsAutoDebit));
      cmd.Parameters.Add(new SqlParameter("@pStatus", pDetials.Status));
      cn.Open();
      intReturn = cmd.ExecuteNonQuery();
     }
    }
   }
   catch(Exception e){
    CTBC.Logs.Write("Insert", e.Message, "Penalty Charges");
   }
  
   return intReturn;
  }

  public int Update(Details pDetails)
  {
   int intReturn = 0;
   string strSQL = "";
   strSQL += "UPDATE PenaltyCharges ";
   strSQL += "SET ";
   strSQL += "DebitAccountNo = @pDebitAccountNo ";
   strSQL += ",BasePenalty = @pBasePenalty ";
   strSQL += ",PenaltyFee = @pPenaltyFee ";
   strSQL += ",IsAutoDebit = @pIsAutoDebit ";
   strSQL += "WHERE CMSCode = @pCMSCode ";

   try
   {
    using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
    {
     using (SqlCommand cmd = cn.CreateCommand())
     {
      cmd.CommandText = strSQL;
      cmd.Parameters.Add(new SqlParameter("@pCMSCode", pDetails.CMSCode));
      cmd.Parameters.Add(new SqlParameter("@pDebitAccountNo", pDetails.DebitAccountNo ?? DBNull.Value.ToString()));
      cmd.Parameters.Add(new SqlParameter("@pPenaltyFee", pDetails.PenaltyFee));
      cmd.Parameters.Add(new SqlParameter("@pBasePenalty", pDetails.BasePenalty));
      cmd.Parameters.Add(new SqlParameter("@pIsAutoDebit", pDetails.IsAutoDebit));
      cn.Open();
      intReturn = cmd.ExecuteNonQuery();
     }
    }
   }
   catch(Exception e)
   {
    CTBC.Logs.Write("Update", e.Message, "Penalty Charges");
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
     cmd.CommandText = "DELETE FROM PenaltyCharges WHERE CMSCode = @pCMSCode";
     cmd.Parameters.Add(new SqlParameter("@pCMSCode", pCMSCode));
     cn.Open();
     intReturn = cmd.ExecuteNonQuery();
    }
   }
   return intReturn;
  }

  public List<Details> GetList()
  {
   List<Details> list = new List<Details>();
   using(SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
   {
    using(SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = "";
     cn.Open();
     using(SqlDataReader dr = cmd.ExecuteReader())
     {
      while(dr.Read())
      {
       list.Add(new Details(){
        CMSCode = dr["CMSCode"].ToString().ToInt()
        ,DebitAccountNo = dr["DebitAccountNo"].ToString()
        ,BasePenalty = dr["BasePenalty"].ToString().ToDecimal()
        ,PenaltyFee = dr["PenaltyFee"].ToString().ToDecimal()
        ,IsAutoDebit = dr["IsAutoDebit"].ToString().ToBoolean()
       });
      }
     }
    }
   }
   return list;
  }

 }
}