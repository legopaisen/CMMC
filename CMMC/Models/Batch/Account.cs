using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;
using Microsoft.VisualBasic;
using CTBC;

namespace CMMC.Models.Batch
{
 public class Account : IDisposable
 {
  public void Dispose() { GC.SuppressFinalize(this); }
  public string AccountNumber { get; set; }

  public string UpdateAccount(bool pIsMonthlyUpdate, DateTime pDate)
  {
   string strReturn = "";
   bool IsmonthlyUpdate = pIsMonthlyUpdate;
   try {
    if (pIsMonthlyUpdate)
    {
     //COPY ACCOUNTONE.TXT
     if (!Directory.Exists(Functions.Settings.DestinationFolder))
     {
      Directory.CreateDirectory(Functions.Settings.DestinationFolder);
     }
     else { File.Copy(Functions.Settings.SourceAccount, Functions.Settings.DestinationFolder, true); }

     //LOAD ACCOUNTSONE.TXT
     string strAccountOneFile = Functions.Settings.SourceAccount + "ACCOUNTONE.txt";
     string strFormatFile = Functions.Settings.DestinationFileFormat + "CustLog.txt";
     using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
     {
      using (SqlCommand cmd = cn.CreateCommand())
      {
       cmd.CommandText = "usp_UpdateAccounts";
       cmd.CommandType = System.Data.CommandType.StoredProcedure;
       cmd.CommandTimeout = 0;
       cmd.Parameters.Add(new SqlParameter("@vchSourceFile", strAccountOneFile));
       cmd.Parameters.Add(new SqlParameter("@vchFormatFile", strFormatFile));
       cn.Open();
       cmd.ExecuteNonQuery();
      }
     }
     IsmonthlyUpdate = false;
    }
    else
    {
     //COPY ACCOUNT.TXT
     if (!Directory.Exists(Functions.Settings.DestinationFolder))
     {
      Directory.CreateDirectory(Functions.Settings.DestinationFolder);
     }
     //COPY ACCOUNT.TXT
     File.Copy(Functions.Settings.SourceAccount, Functions.Settings.DestinationFolder, true);
     //COPY DUMPINVV FILE
     File.Copy(Functions.Settings.SourceDumpINVV, Functions.Settings.DestinationFolder, true);

     //LOAD ACCOUNTS.TXT
     DateTime dtePreviousBankingDate = new Models.Batch.Functions().GetPreviousBankingDate(pDate).ToDateTime();
     string strPreviousDate = dtePreviousBankingDate.ToString("MM/dd/yyyy");

     string strAccountFile = Functions.Settings.SourceAccount + "_" + strPreviousDate.ToString().Substring(3, 4) + ".txt";
     string strFormatFile = Functions.Settings.DestinationFileFormat + "CustLog.txt";
     string strDumpinvvFile = Functions.Settings.SourceDumpINVV + "DUMPINVV_" + strPreviousDate.ToString().Substring(3, 4) + ".txt";

     using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
     {
      using (SqlCommand cmd = cn.CreateCommand())
      {
       cmd.CommandText = "usp_InsertAccounts";
       cmd.CommandType = System.Data.CommandType.StoredProcedure;
       cmd.CommandTimeout = 0;
       cmd.Parameters.Add(new SqlParameter("@vchSourceFile", strAccountFile)); //ACCOUNT.TXT
       cmd.Parameters.Add(new SqlParameter("@vchFormatFile", strFormatFile)); //FORMAT
       cmd.Parameters.Add(new SqlParameter("@vchSourceFile2", strDumpinvvFile)); //DUMPINVV.TXT
       cn.Open();
       cmd.ExecuteNonQuery();
      }
     }
     IsmonthlyUpdate = true;
    }
    strReturn = "success";
   }
   catch(Exception e){
    strReturn = "error";
    CTBC.Logs.Write("UpdateAccount", e.Message, "Batch Account");

   }
   return strReturn;
  }
 }
}
