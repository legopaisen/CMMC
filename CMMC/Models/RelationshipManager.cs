using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using CTBC;

namespace CMMC.Models
{
 public class RelationshipManagerModel 
 {
  public int ID { get; set; }
  public string RMFullName { get; set; }
  public DateTime AddedDate { get; set; }
  public string AddedBy { get; set; }
  public DateTime ModifiedDate { get; set; }
  public string ModifiedBy { get; set; }
  public int BranchAssigned { get; set; }
  
 }

 public class RelationshipManager : IDisposable
 {
  public void Dispose() { GC.SuppressFinalize(this); }

  public RelationshipManagerModel Fill(string ID)
  {
   RelationshipManagerModel relmanReturn = new RelationshipManagerModel();
   string strSQL = "SELECT * FROM RelationshipManager WHERE ID = @pID";

   using(SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring)){
    using(SqlCommand cmd = cn.CreateCommand()){
     cmd.CommandText = strSQL;
     cmd.Parameters.Add(new SqlParameter("@pID", ID));
     cn.Open();
     using(SqlDataReader dr = cmd.ExecuteReader()){
      if(dr.Read()){
       relmanReturn.AddedBy = dr["AddedBy"].ToString();
       relmanReturn.AddedDate = dr["AddedDate"].ToString().ToDateTime();
       relmanReturn.ModifiedBy = dr["ModifiedBy"].ToString();
       relmanReturn.ModifiedDate = dr["ModifiedDate"].ToString().ToDateTime();
       relmanReturn.BranchAssigned = dr["BranchAssigned"].ToString().ToInt();
       relmanReturn.ID = dr["ID"].ToString().ToInt();
       relmanReturn.RMFullName = dr["RMFullName"].ToString();
      }
     }
    }   
   }
   return relmanReturn;
  }

  public int Insert(RelationshipManagerModel pDetails)
  {
   int intReturn = 0;
   string strSQL = "INSERT INTO ";
   strSQL += "RelationshipManager ";
   strSQL += "VALUES ( ";
   strSQL += "@pRMFullName ";
   strSQL += ",@pAddedDate ";
   strSQL += ",@pAddedBy ";
   strSQL += ",null ";
   strSQL += ",null ";
   strSQL += ",@pBranchAssigned ";
   strSQL += ")";

  try
   {
    if (!IsExist(pDetails))
    {
     using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
     {
      using (SqlCommand cmd = cn.CreateCommand())
      {
       cmd.CommandText = strSQL;
       cmd.Parameters.Add(new SqlParameter("@pRMFullName", pDetails.RMFullName));
       cmd.Parameters.Add(new SqlParameter("@pAddedDate", SharedFunctions.ServerDate));
       cmd.Parameters.Add(new SqlParameter("@pAddedBy", pDetails.AddedBy));
       cmd.Parameters.Add(new SqlParameter("@pBranchAssigned", pDetails.BranchAssigned));
       cn.Open();
       intReturn = cmd.ExecuteNonQuery();
      }
     }
    }
    else { intReturn = 2; }
   }
   catch (Exception e) {
    CTBC.Logs.Write("Insert", e.Message, "Relationship Manager");
   }
   return intReturn;  
  }

  public bool IsExist(RelationshipManagerModel pDetails) 
  {
   bool blnReturn = false;
   string strSQL = "SELECT Count(*) FROM ";
   strSQL += "RelationshipManager ";
   strSQL += "WHERE RMFullName = @pRMFullName and ";
   strSQL += "BranchAssigned = @pBranchAssigned ";

   using(SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring)){
    using(SqlCommand cmd = cn.CreateCommand()){
     cmd.CommandText = strSQL;
     cmd.Parameters.Add(new SqlParameter("@pRMFullName", pDetails.RMFullName));
     cmd.Parameters.Add(new SqlParameter("@pBranchAssigned", pDetails.BranchAssigned));
     cn.Open();
     blnReturn = cmd.ExecuteScalar().ToString().ToInt() > 0;
    }
   }
   return blnReturn;
  }

  public int Update(RelationshipManagerModel pDetails) 
  {
   int intReturn = 0;
   string strSQL = "UPDATE RelationshipManager ";
   strSQL += "SET ";
   strSQL += "RMFullName = @pRMFULLNAME ";
   strSQL += ",BranchAssigned = @pBranchAssigned ";
   strSQL += ",ModifiedBy = @pModifiedBy ";
   strSQL += ",ModifiedDate = @pModifiedDate ";
   strSQL += "WHERE Id = @pID ";

   try
   {
    if (!IsExist(pDetails))
    {
     using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
     {
      using (SqlCommand cmd = cn.CreateCommand())
      {
       cmd.CommandText = strSQL;
       cmd.Parameters.Add(new SqlParameter("@pID", pDetails.ID));
       cmd.Parameters.Add(new SqlParameter("@pRMFullName", pDetails.RMFullName));
       cmd.Parameters.Add(new SqlParameter("@pBranchAssigned", pDetails.BranchAssigned));
       cmd.Parameters.Add(new SqlParameter("@pModifiedBy", pDetails.ModifiedBy));
       cmd.Parameters.Add(new SqlParameter("@pModifiedDate", DateTime.Now));
       cn.Open();
       intReturn = cmd.ExecuteNonQuery();
      }
     }
    }
    else { intReturn = 2; }
   }catch(Exception e){
    CTBC.Logs.Write("Update", e.Message, "Relationship Manager");
   }

   return intReturn;
  }

  public int Delete(string pID) 
  {
   int intReturn = 0;
   string strSQL = "Delete RelationshipManager WHERE ID = @pID";

   using(SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring)){
    using(SqlCommand cmd = cn.CreateCommand()){
     cmd.CommandText = strSQL;
     cmd.Parameters.Add(new SqlParameter("@pID", pID));
     cn.Open();
     intReturn = cmd.ExecuteNonQuery();
    }
   }
   return intReturn;
  }

  public List<RelationshipManagerModel> GetList() 
  {
   List<RelationshipManagerModel> list = new List<RelationshipManagerModel>();
   string strSQL = "Select * FROM RelationshipManager WHERE ID > 0";

   using(SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring)){
    using(SqlCommand cmd = cn.CreateCommand()){
     cmd.CommandText = strSQL;
     cn.Open();
     using(SqlDataReader dr = cmd.ExecuteReader()){
      while (dr.Read())
      {
       list.Add(new RelationshipManagerModel
       {
        ID = dr["ID"].ToString().ToInt(),
        RMFullName = dr["RMFullName"].ToString(),
        AddedBy = dr["AddedBy"].ToString(),
        AddedDate = dr["AddedDate"].ToString().ToDateTime(),
        BranchAssigned = dr["BranchAssigned"].ToString().ToInt(),
        ModifiedBy = dr["ModifiedBy"].ToString(),
        ModifiedDate = dr["ModifiedDate"].ToString().ToDateTime()
       });
      }
     }
    }
   }

   return list;
  }

 }
}
