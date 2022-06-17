using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;
using Microsoft.VisualBasic;
using CMMC.Models;
using CTBC;

namespace CMMC.Models.Batch
{
 public class Customer : IDisposable
 {
  public void Dispose() { GC.SuppressFinalize(this); }
  public string CustomerNumber { get; set; }

  public string UpdateCustomer(bool pIsMonthlyUpdated, DateTime pDate)
  {
  string strReturn = "";
  bool IsMonthlyUpdated = pIsMonthlyUpdated;
  try {
   if (pIsMonthlyUpdated)
   {
    //COPY CUSTOMERONE.txt
    if (!Directory.Exists(Functions.Settings.DestinationFolder))
    {
     Directory.CreateDirectory(Functions.Settings.DestinationFolder);
    }
    else { File.Copy(Functions.Settings.SourceCustomer, Functions.Settings.DestinationFolder, true); }

    //LOAD CUSTOMERONE.txt
    string strCustomerFile = Functions.Settings.SourceCustomer + "CUSTOMERONE.txt";
    string strFormatFile = Functions.Settings.DestinationFileFormat + "CustLog.txt";
    using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
    {
     using (SqlCommand cmd = cn.CreateCommand())
     {
      cmd.CommandText = "usp_UpdateCustomer";
      cmd.CommandType = System.Data.CommandType.StoredProcedure;
      cmd.CommandTimeout = 0;
      cmd.Parameters.Add(new SqlParameter("@vchSourceFile", strCustomerFile));
      cmd.Parameters.Add(new SqlParameter("@vchFormatFile", strFormatFile));
      cn.Open();
      cmd.ExecuteNonQuery();
     }
    }
   }
   else
   {
    //COPY CUSTOMER.TXT
    if (Directory.Exists(Functions.Settings.DestinationFolder))
    {
     Directory.CreateDirectory(Functions.Settings.DestinationFolder);
    }
    else { File.Copy(Functions.Settings.SourceCustomer, Functions.Settings.DestinationFolder, true); }

    //LOAD CUSTOMER.TXT
    DateTime dtePreviousBankingDate = new Models.Batch.Functions().GetPreviousBankingDate(pDate).ToDateTime();
    string strPreviousDate = dtePreviousBankingDate.ToString("MM/dd/yyyy");

    string strCustomerFile = Functions.Settings.SourceCustomer + "_" + strPreviousDate.ToString().Substring(3, 4) + ".txt";
    string strFormatFile = Functions.Settings.DestinationFileFormat + "CustLog.txt";

    using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
    {
     using (SqlCommand cmd = new SqlCommand())
     {
      cmd.CommandText = "usp_InsertCustomer";
      cmd.CommandType = System.Data.CommandType.StoredProcedure;
      cmd.CommandTimeout = 0;
      cmd.Parameters.Add(new SqlParameter("@vchSourceFile", strCustomerFile));
      cmd.Parameters.Add(new SqlParameter("@vchFormatFile", strFormatFile));
      cn.Open();
      cmd.ExecuteNonQuery();
     }
    }
   }
   strReturn = "success";
  }
  catch (Exception e) {
   strReturn = "error";
   CTBC.Logs.Write("UpdateCustomer", e.Message, "Batch Customer");
  }
  return strReturn;
 }
 }
}
