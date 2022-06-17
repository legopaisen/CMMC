using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
//using Microsoft.SqlServer.Dts.Runtime;

namespace CMMC.Models.Batch
{
 public class SSIS
 {
  public void ProcessADB()
  {
   //string strpackageLocation =Main.Settings.DestinationFolder;
   //Application application = new Application();
   //Package package = application.LoadPackage(strpackageLocation, null);
   //DTSExecResult packageResults = package.Execute();
   








   //SqlConnection jobConnection;
   //SqlCommand jobCommand;
   //SqlParameter jobParameter;
   //SqlParameter jobReturnValue;
   //int jobResult;

   //jobConnection = new SqlConnection("Data Source=(local);Initial Catalog=msdb;Integrated Security=SSPI");
   //jobCommand = new SqlCommand("sp_start_job", jobConnection);
   //jobCommand.CommandType = CommandType.StoredProcedure;

   //jobReturnValue = new SqlParameter("@RETURN_VALUE", SqlDbType.Int);
   //jobReturnValue.Direction = ParameterDirection.ReturnValue;
   //jobCommand.Parameters.Add(jobReturnValue);

   //jobCommand.Parameters.Add(new SqlParameter("@RETURN_VALUE", jobReturnValue));

   //jobParameter = new SqlParameter("@job_name", SqlDbType.VarChar);
   //jobParameter.Direction = ParameterDirection.Input;
   //jobCommand.Parameters.Add(jobParameter);
   //jobParameter.Value = "RunSSISPackage";

   //jobConnection.Open();
   //jobCommand.ExecuteNonQuery();
   //jobResult = (Int32)jobCommand.Parameters["@RETURN_VALUE"].Value;
   //jobConnection.Close();

   //switch (jobResult)
   //{
   // case 0:
   //  // SUCCESS
   //  break;
   // default:
   //  //FAILED
   //  break;
   //}
   //Console.Read();
  }
 }
}
