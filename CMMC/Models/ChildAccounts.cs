using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using CTBC;
using CTBC.Excel;
using System.Data;

namespace CMMC.Models
{
 public class ChildAccounts : IDisposable
 {
  public void Dispose() { GC.SuppressFinalize(this); }

  public struct Details
  {
   public int ID { get; set; }
   public int CMSCode { get; set; }
   public string AccountNo { get; set; }
   public string AccountName { get; set; }
   public string BranchCode { get; set; }
   public string BranchName { get; set; }
   public DateTime DateEnrolled { get; set; }
   public string InvestmentDesc { get; set; }
   public string InvestmentCode { get; set; }
   public DateTime EffectivityDate { get; set; }
   public string Tagging { get; set; }
   public string Status { get; set; }
  }

  public struct DataForExcel
  {
   public string AccountNo { get; set; }
   public string AccountName { get; set; }
   public string BranchCode { get; set; }
   public DateTime DateEnrolled { get; set; }
   public string InvestmentDesc { get; set; }
  }

  public List<Details> GetList(int pCMSCode)
  {
   List<Details> list = new List<Details>();
   string strSQL = "SELECT * ";
   strSQL += "FROM ";
   strSQL += "ChildAccounts ";
   strSQL += "WHERE ";
   strSQL += "CMSCode = @pCMSCode ";
   strSQL += "AND InvestmentCode = 702";

   using(SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring)){
    using(SqlCommand cmd = cn.CreateCommand()){
     cmd.CommandText = strSQL;
     cmd.Parameters.Add(new SqlParameter("@pCMSCode", pCMSCode));
     cn.Open();
     using(SqlDataReader dr =  cmd.ExecuteReader()){
      while(dr.Read()){
       list.Add(new Details{
        AccountNo = dr["AccountNo"].ToString(),
        AccountName = dr["AccountName"].ToString(),
        BranchCode = dr["BranchCode"].ToString(),
        CMSCode = dr["CMSCode"].ToString().ToInt(),
        DateEnrolled = dr["DateEnrolled"].ToString().ToDateTime(),
        EffectivityDate = dr["EffectivityDate"].ToString().ToDateTime(),
        ID = dr["ID"].ToString().ToInt(),
        InvestmentCode = dr["InvestmentCode"].ToString(),
        InvestmentDesc = dr["InvestmentDesc"].ToString(),
        Status = dr["Status"].ToString(),
        Tagging = dr["Tagging"].ToString()       
       });
      }
     }
    }
   }
   return list;
  }
 }
}