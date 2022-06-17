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


 public class ServiceOptions : IDisposable
 {
  public void Dispose() { GC.SuppressFinalize(this); }

  public string MaxFreeTransaction { get; set; }

  public string PendingRequest { get; set; }


  public struct AvailedDetails
  {
   public int CMSCode { get; set; }
   public string ServiceName { get; set; }
   public int ServiceID { get; set; }
   public RelationshipManagerModel RMID { get; set; }
   public DateTime EnrolledOn { get; set; }
   public string EnrolledBy { get; set; }
   public DateTime ModifiedOn { get; set; }
   public string ModifiedBy { get; set; }
   public string Status { get; set; }
   public decimal MotherRequiredADB { get; set; }
   public decimal SubRequiredADB { get; set; }
   public int MinNumberEmployee { get; set; }
   public bool IsActive { get; set; }
  }
 
  public List<ServiceOptions> GetFreeTransaction()
  {  
    var FreeTransaction = new List<ServiceOptions>  
   {
    new ServiceOptions{MaxFreeTransaction = "Unlimited"},
    new ServiceOptions{MaxFreeTransaction = "0"},
    new ServiceOptions{MaxFreeTransaction = "1"},
    new ServiceOptions{MaxFreeTransaction = "2"},
    new ServiceOptions{MaxFreeTransaction = "3"},
    new ServiceOptions{MaxFreeTransaction = "4"},
    new ServiceOptions{MaxFreeTransaction = "5"},
    new ServiceOptions{MaxFreeTransaction = "6"},
    new ServiceOptions{MaxFreeTransaction = "7"},
    new ServiceOptions{MaxFreeTransaction = "8"},
    new ServiceOptions{MaxFreeTransaction = "9"},
    new ServiceOptions{MaxFreeTransaction = "10"}    
   };
    return FreeTransaction;      
  }

  
  public AvailedDetails Fill(int pCMSCode)
  {
   AvailedDetails details = new AvailedDetails();
   string strSQL = "";
   strSQL += "SELECT *, RM.RMFullName as FullName FROM ServicesAvailment SA ";
   strSQL += "INNER JOIN ServiceOption SO ";
   strSQL += "ON ";
   strSQL += "SA.ServiceOptionID = SO.ID ";
    strSQL += "WHERE CMSCode = @pCMSCode";

   try
   {
    using(SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring)){
     using(SqlCommand cmd = cn.CreateCommand()){
      cmd.CommandText = strSQL;
      cmd.Parameters.Add(new SqlParameter("@pCMSCode",pCMSCode));
      cn.Open();
      using(SqlDataReader dr = cmd.ExecuteReader()){
       if(dr.Read()){
        details.CMSCode = dr["CMSCode"].ToString().ToInt();
        details.EnrolledBy = dr["EnrolledBy"].ToString();
        details.EnrolledOn = dr["EnrolledOn"].ToString().ToDateTime();
        details.MinNumberEmployee = Convert.ToInt32(dr["MinNumberEmployee"].ToString());
        details.ModifiedBy = dr["ModifiedBy"].ToString();
        details.ModifiedOn = dr["ModifiedOn"].ToString().ToDateTime();
        details.MotherRequiredADB = dr["MotherRequiredADB"].ToString().ToDecimal();
        details.ServiceID = dr["ServiceID"].ToString().ToInt();
        details.ServiceName = dr["ServiceName"].ToString();
        details.Status = dr["Status"].ToString();
        details.RMID = new RelationshipManagerModel() { ID = Convert.ToInt32(dr["RMID"].ToString()) };
        details.SubRequiredADB = dr["SubRequiredADB"].ToString().ToDecimal();
       }      
      }
     }
    }
   }
   catch (Exception e) {
    CTBC.Logs.Write("Fill", e.Message, "Service Options");
   }
   return details;
  }

   
  public int Insert(AvailedDetails details)
  {
   int intReturn = 0;

    string strSQL = "";
    strSQL += "INSERT INTO ServicesAvailment ";
    strSQL += "VALUES ";
    strSQL += "( ";
    strSQL += "@pCMSCode ";
    strSQL += ",@pServiceID ";
    strSQL += ",@pRMID ";
    strSQL += ",@pServiceOptionID ";
    strSQL += ",@pEnrolledOn ";
    strSQL += ",@pEnrolledBy ";
    strSQL += ",@pModifiedOn ";
    strSQL += ",@pModifiedBy ";
    strSQL += ",@pStatus ";
    strSQL += ",@pMotherRequiredADB ";
    strSQL += ",@pSubRequiredADB ";
    strSQL += ",@pMinNumberEmployee ";
    strSQL += ") ";   

    try
    {
     using(SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring)){
      using(SqlCommand cmd = cn.CreateCommand()){
       cmd.CommandText = strSQL;
       cmd.Parameters.Add(new SqlParameter("@pCMSCode", details.CMSCode));
       cmd.Parameters.Add(new SqlParameter("@pServiceID", details.ServiceID));
       cmd.Parameters.Add(new SqlParameter("@pRMID", details.RMID.ID));
       cmd.Parameters.Add(new SqlParameter("@pServiceOptionID", intReturn));
       cmd.Parameters.Add(new SqlParameter("@pEnrolledOn", details.EnrolledOn));
       cmd.Parameters.Add(new SqlParameter("@pEnrolledBy", details.EnrolledBy ?? DBNull.Value.ToString()));
       cmd.Parameters.Add(new SqlParameter("@pModifiedOn", details.ModifiedOn));
       cmd.Parameters.Add(new SqlParameter("@pModifiedBy", details.ModifiedBy ?? DBNull.Value.ToString()));
       cmd.Parameters.Add(new SqlParameter("@pStatus", details.Status ?? DBNull.Value.ToString()));
       cmd.Parameters.Add(new SqlParameter("@pMotherRequiredADB", details.MotherRequiredADB));
       cmd.Parameters.Add(new SqlParameter("@pSubRequiredADB", details.SubRequiredADB));
       cmd.Parameters.Add(new SqlParameter("@pMinNumberEmployee", details.MinNumberEmployee));
       cn.Open();
       cmd.ExecuteNonQuery();
      }
     }
    }
    catch (Exception e) {
     CTBC.Logs.Write("Insert", e.Message, "Service Options");
    }
   return intReturn;
  }

  public int Update(AvailedDetails details)
  {
   int intReturn = 0;
   string strSQL = "";
   strSQL += "UPDATE ServicesAvailment ";
   strSQL += "SET ";
   strSQL += "CMSCode = @pCMSCode ";
   strSQL += ",ServiceID = @pServiceID ";
   strSQL += ",RMID = @pRMID ";
   strSQL += ",ServiceOptionID = @pServiceOptionID ";
   strSQL += ",ModifiedOn = @pModifiedOn ";
   strSQL += ",ModifiedBy = @pModifiedBy ";
   strSQL += ",Status = @pStatus ";
   strSQL += ",MotherRequiredADB = @pMotherRequiredADB ";
   strSQL += ",SubRequiredADB = @pSubRequiredADB ";
   strSQL += ",MinNumberEmployee = @pMinNumberEmployee ";
   strSQL += "WHERE ";
   strSQL += "CMSCode = @pCMSCode ";
   strSQL += "AND ServiceID = @pServiceID ";
   strSQL += "AND ServiceOptionID = @pServiceOptionID ";

   try
   {
    using(SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring)){
     using(SqlCommand cmd = cn.CreateCommand()){
      cmd.CommandText = strSQL;
      cmd.Parameters.Add(new SqlParameter("@pCMSCode", details.CMSCode));
      cmd.Parameters.Add(new SqlParameter("@pServiceID", details.ServiceID));
      cmd.Parameters.Add(new SqlParameter("@pRMID", details.RMID.ID));
      cmd.Parameters.Add(new SqlParameter("@pModifiedOn", details.ModifiedOn));
      cmd.Parameters.Add(new SqlParameter("@pModifiedBy", details.ModifiedBy ?? DBNull.Value.ToString()));
      cmd.Parameters.Add(new SqlParameter("@pStatus", details.Status ?? DBNull.Value.ToString()));
      cmd.Parameters.Add(new SqlParameter("@pMotherRequiredADB", details.MotherRequiredADB));
      cmd.Parameters.Add(new SqlParameter("@pSubRequiredADB", details.SubRequiredADB));
      cmd.Parameters.Add(new SqlParameter("@pMinNumberEmployee", details.MinNumberEmployee));
      intReturn = cmd.ExecuteNonQuery();
     }
    }
   }
   catch (Exception e) {
    CTBC.Logs.Write("Update", e.Message, "Service Options");

   }
   return intReturn;
  }

  public int Delete(int pServiceOptionID, int pServiceID)
  {
   int intReturn = 0;
   string strSQL = "";
   strSQL += "DELETE FROM ServicesAvailment WHERE ServiceOptionID = @pServiceOptionID AND ServiceID = @pServiceID; ";
   try
   {
    using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
    {
     using (SqlCommand cmd = cn.CreateCommand())
     {
      cmd.CommandText = strSQL;
      cmd.Parameters.Add(new SqlParameter("@pServiceID", pServiceID));
      cn.Open();
      intReturn = cmd.ExecuteNonQuery();
     }
    }
   }
   catch(Exception e)
   {
    CTBC.Logs.Write("Delete", e.Message, "Service Options");

   }
   
   return intReturn;
  }

  public int CancelandRemove(int pCMSCode)
  {
   int intReturn = 0;
   string strSQL = "";
   strSQL += "DELETE FROM ServicesAvailment WHERE CMSCode = @pCMSCode ";
   strSQL += "DELETE FROM ServiceOption WHERE CMSCode = @pCMSCode ";
   try
   {
    using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
    {
     using (SqlCommand cmd = cn.CreateCommand())
     {
      cmd.CommandText = strSQL;
      cmd.Parameters.Add(new SqlParameter("@pCMSCode", pCMSCode));
      cn.Open();
      intReturn = cmd.ExecuteNonQuery();
     }
    }
   }
   catch (Exception e)
   {
    CTBC.Logs.Write("Cancel and Remove", e.Message, "Service Options");

   }
   return intReturn;
  }

  public List<AvailedDetails> GetList(int pCMSCode)
  {
   List<AvailedDetails> list = new List<AvailedDetails>();

   string strSQL = "";
   strSQL += "SELECT ";
   strSQL += "SA.ServiceID ";
   strSQL += ",SA.RMID ";
   strSQL += ",SA.ServiceOptionID ";
   strSQL += ",S.ServiceName ";
   strSQL += ",SA.AvailedOn ";
   strSQL += ",SA.AvailedBy ";
   strSQL += ",SA.ModifiedOn ";
   strSQL += ",SA.ModifiedBy ";
   strSQL += ",SA.Status ";
   strSQL += ",SA.MotherRequiredADB ";
   strSQL += ",SA.SubRequiredADB ";
   strSQL += ",SA.MinNumberEmployee ";
   strSQL += "FROM ServicesAvailment SA ";
   strSQL += "INNER JOIN Services S ON S.ServiceID = SA.ServiceID ";

   strSQL += "WHERE SA.CMSCode = @pCMSCode ";

   SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring);
   cn.Open();
   SqlCommand cmd = cn.CreateCommand();
   cmd.Transaction = cn.BeginTransaction();
   SqlTransaction trans = cmd.Transaction;

   try
   {
    cmd.CommandText = strSQL;
    cmd.Parameters.Add(new SqlParameter("@pCMSCode", pCMSCode));
    using (SqlDataReader dr = cmd.ExecuteReader())
    {
     while (dr.Read())
     {
      list.Add(new AvailedDetails()
      {
       ServiceID = dr["ServiceID"].ToString().ToInt(),
       RMID = new RelationshipManagerModel() { ID = dr["RMID"].ToString().ToInt() },
       ServiceName = dr["ServiceName"].ToString(),
       EnrolledOn = dr["AvailedOn"].ToString().ToDateTime(),
       EnrolledBy = dr["AvailedBy"].ToString(),
       ModifiedOn = dr["ModifiedOn"].ToString().ToDateTime(),
       ModifiedBy = dr["ModifiedBy"].ToString(),
       Status = dr["Status"].ToString(),
       MotherRequiredADB = dr["MotherRequiredADB "].ToString().ToDecimal(),
       SubRequiredADB = dr["SubRequiredADB"].ToString().ToDecimal(),
       MinNumberEmployee = Convert.ToInt32(dr["MinNumberEmployee"].ToString())
      });
     }
    }
   }
   catch (Exception e)
   {
    CTBC.Logs.Write("Get List", e.Message, "Service Options");

   }
   return list;
  }

  public List<AvailedDetails> GetAvailmentList(int pCMSCode)
  {   
   List<AvailedDetails> list = new List<AvailedDetails>();
   string strSQL = "";
   strSQL += "SELECT ";
   strSQL += "SA.ServiceID ";
   strSQL += ",SA.RMID ";
   strSQL += ",S.ServiceName ";
   strSQL += ",SA.EnrolledOn ";
   strSQL += ",SA.EnrolledBy ";
   strSQL += ",SA.ModifiedOn ";
   strSQL += ",SA.ModifiedBy ";
   strSQL += ",SA.Status ";
   strSQL += ",SA.MotherRequiredADB ";
   strSQL += ",SA.SubRequiredADB ";   
   strSQL += ",SA.MinNumberEmployee ";
   strSQL += "FROM ServicesAvailment SA ";
   strSQL += "INNER JOIN Services S ON S.ServiceID = SA.ServiceID ";

   strSQL += "WHERE SA.CMSCode = @pCMSCode ";   
   
    SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring);
    cn.Open();
    SqlCommand cmd = cn.CreateCommand();
    cmd.Transaction = cn.BeginTransaction();
    SqlTransaction trans = cmd.Transaction;

   try
   {
    cmd.CommandText = strSQL;
    cmd.Parameters.Add(new SqlParameter("@pCMSCode", pCMSCode));
    using (SqlDataReader dr = cmd.ExecuteReader())    
    {
     while (dr.Read())
     {
      list.Add(new AvailedDetails()
      {
       ServiceID = dr["ServiceID"].ToString().ToInt(),
       RMID = new RelationshipManagerModel() { ID = Convert.ToInt32(dr["RMID"].ToString()) },
       ServiceName = dr["ServiceName"].ToString(),
       EnrolledOn = dr["EnrolledOn"].ToString().ToDateTime(),
       EnrolledBy = dr["EnrolledBy"].ToString(),
       ModifiedOn = dr["ModifiedOn"].ToString().ToDateTime(),
       ModifiedBy = dr["ModifiedBy"].ToString(),
       Status = dr["Status"].ToString(),
       MotherRequiredADB = dr["MotherRequiredADB"].ToString().ToDecimal(),
       SubRequiredADB = dr["SubRequiredADB"].ToString().ToDecimal(),
       MinNumberEmployee = Convert.ToInt32(dr["MinNumberEmployee"].ToString())
      });
     }     
    }
   }
   catch(Exception e){
    CTBC.Logs.Write("Get Availment List", e.Message, "Service Options");
   }
   return list;
  }

  public List<AvailedDetails> GetServiceName(AvailedDetails pDetails)
  {
   List<AvailedDetails> list = new List<AvailedDetails>();
   string strSQL = "";
   strSQL += " SELECT ServiceName, ServiceID, IsActive FROM Services WHERE ServiceID Not in ( ";
   strSQL += "Select ServiceID FROM ServicesAvailment where CMSCode = @pCMSCode AND Status in (1,2)) ";
   strSQL += "ORDER BY ServiceName";

   using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
   {
    using (SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = strSQL;
     cmd.Parameters.Add(new SqlParameter("@pCMSCode", pDetails.CMSCode));
     cn.Open();
     using (SqlDataReader dr = cmd.ExecuteReader())
     {
      while (dr.Read())
      {
       list.Add(new AvailedDetails() { 
       ServiceName = dr["ServiceName"].ToString(),
       ServiceID = dr["ServiceID"].ToString().ToInt(),
       IsActive = dr["IsActive"].ToString().Equals("1")
       }
       );
      }
     }
    }
   }

   return list;
  }

  public List<AvailedDetails> GetServiceNameForRequest(int pCMSCode)
  {
   List<AvailedDetails> list = new List<AvailedDetails>();
   string strSQL = "";
   strSQL += " SELECT ServiceName, ServiceID, IsActive FROM Services WHERE ServiceID Not in ( ";
   strSQL += "Select ServiceID FROM ServicesAvailment where CMSCode = @pCMSCode AND Status in (1,2)) ";
   strSQL += "ORDER BY ServiceName";

   using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
   {
    using (SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = strSQL;
     cmd.Parameters.Add(new SqlParameter("@pCMSCode", pCMSCode));
     cn.Open();
     using (SqlDataReader dr = cmd.ExecuteReader())
     {
      while (dr.Read())
      {
       list.Add(new AvailedDetails()
       {
        ServiceName = dr["ServiceName"].ToString(),
        ServiceID = dr["ServiceID"].ToString().ToInt(),
        IsActive = dr["IsActive"].ToString().Equals("1")
       }
       );
      }
     }
    }
   }
   return list;
  }

  public List<AvailedDetails> ServiceName() {
   List<AvailedDetails> list = new List<AvailedDetails>();
   string strSQL = "";
   strSQL += " SELECT ServiceName, ServiceID, IsActive FROM Services ";

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
       list.Add(new AvailedDetails()
       {
        ServiceName = dr["ServiceName"].ToString(),
        ServiceID = dr["ServiceID"].ToString().ToInt(),
        IsActive = dr["IsActive"].ToString().Equals("1")
       }
       );
      }
     }
    }
   }

   return list;
  }

  public List<AvailedDetails> GetAvailedServiceID(int pCMSCode)
  {
   List<AvailedDetails> list = new List<AvailedDetails>();
   string strSQL = "";
   strSQL += "SELECT ";
   strSQL += "SA.ServiceID ";
   strSQL += "FROM ServicesAvailment SA ";
   strSQL += "INNER JOIN Services S ON S.ServiceID = SA.ServiceID ";
   strSQL += "WHERE SA.CMSCode = @pCMSCode ";

   using(SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
   {
    using(SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = strSQL;
     cmd.Parameters.Add(new SqlParameter("@pCMSCode", pCMSCode));
     cn.Open();
     using(SqlDataReader dr = cmd.ExecuteReader())
     {
      while(dr.Read())
      {
       list.Add(new AvailedDetails{
        ServiceID = dr["ServiceID"].ToString().ToInt()
       });
      }
     }
    }
   }
   return list;
  }
 
  public List<AvailedDetails> GetEverything(int pCMSCode)
  {
   List<AvailedDetails> list = new List<AvailedDetails>();
   
   string strSQL = "";
   strSQL += "SELECT S.ServiceName, SA.Status as SASTATUS, RM.RMFullName as FullName, * FROM ServicesAvailment SA ";
   strSQL += "INNER JOIN Services S ON S.ServiceID = SA.ServiceID ";
   strSQL += "INNER JOIN Status ST ON ST.StatusID = SA.Status ";
   strSQL += "INNER JOIN RelationshipManager RM ON RM.ID = SA.RMID ";
   strSQL += "WHERE SA.CMSCode = @pCMSCode ";
   strSQL += "AND SA.Status in (1,2)";

   SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring);
   cn.Open();
   SqlCommand cmd = cn.CreateCommand();
   cmd.Transaction = cn.BeginTransaction();
   SqlTransaction trans = cmd.Transaction;
   try 
   {
    cmd.CommandText = strSQL;
    cmd.Parameters.Add(new SqlParameter("@pCMSCode", pCMSCode));
    using(SqlDataReader dr = cmd.ExecuteReader())    
    {
     while(dr.Read()) 
     {
      list.Add(new AvailedDetails 
      {
       EnrolledBy = dr["EnrolledBy"].ToString(),
       EnrolledOn = dr["EnrolledOn"].ToString().ToDateTime(),
       CMSCode = dr["CMSCode"].ToString().ToInt(),
       ModifiedBy = dr["ModifiedBy"].ToString(),
       ModifiedOn = dr["ModifiedOn"].ToString().ToDateTime(),
       ServiceID = dr["ServiceID"].ToString().ToInt(),
       RMID = new RelationshipManagerModel() { ID = Convert.ToInt32(dr["RMID"].ToString().Length > 0 ? dr["RMID"] : 0), RMFullName = dr["FullName"].ToString() },
       ServiceName = dr["ServiceName"].ToString(),
       Status = dr["SASTATUS"].ToString(), 
       SubRequiredADB = dr["SubRequiredADB"].ToString().ToDecimal(),
       MotherRequiredADB = dr["MotherRequiredADB"].ToString().ToDecimal(),
       MinNumberEmployee = Convert.ToInt32(dr["MinNumberEmployee"].ToString())
      });
     }
    }
   }
   catch(Exception e)
   {
    CTBC.Logs.Write("Get Everything", e.Message, "Service Options");

   }
    return list;
  }

  public ServiceOptions GetPendingRequest()
  {
   ServiceOptions total = new ServiceOptions();
   using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
   {
    using (SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = "SELECT COUNT(*) as total FROM ServicesAvailment WHERE Status = 1";
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