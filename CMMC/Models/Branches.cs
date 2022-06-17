using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using CTBC;
using System.Data;


namespace CMMC.Models
{
 public class Branches : IDisposable
 {
  public void Dispose() { GC.SuppressFinalize(this); }

  public string Cluster { get; set; }

  public struct Details
  {
   public string BranchCode { get; set; }
   public string BranchName { get; set; }
   public bool IsActive { get; set; }
   public string Status { get; set; }
   public string CreatedBy { get; set; }
   public DateTime CreatedOn { get; set; }
   public string ModifiedBy { get; set; }
   public DateTime? ModifiedOn { get; set; }
   public string Email { get; set; }
   public string BhossEmail { get; set; }
   public string Cluster { get; set; }
  }

  public List<Branches> GetCluster()
  {
   var tag = new List<Branches>  
   {
    new Branches{Cluster = "Metro"},
    new Branches{Cluster = "North"},
    new Branches{Cluster = "South"},
    new Branches{Cluster = "VisMin"},
   };
   return tag;
  }

  public Details Fill(string pBranchCode)
  {
   Details details = new Details();
   try
   {
    string strSQL = "SELECT * FROM Branches WHERE BranchCode = @pBranchCode";

    using (SqlConnection cn = new SqlConnection(CMMC.Models.SharedFunctions.Connectionstring))
    {
     using (SqlCommand cmd = cn.CreateCommand())
     {
      cmd.CommandText = strSQL;
      cmd.Parameters.Add(new SqlParameter("@pBranchCode", pBranchCode));
      cn.Open();
      using (SqlDataReader dr = cmd.ExecuteReader())
      {
       if (dr.Read())
       {
        details.BranchCode = dr["BranchCode"].ToString();
        details.BranchName = dr["BranchName"].ToString();
        details.IsActive = dr["IsActive"].ToString().Equals("1");
        details.CreatedBy = dr["CreatedBy"].ToString();
        details.CreatedOn = dr["CreatedOn"].ToString().ToDateTime();
        details.ModifiedBy = dr["ModifiedBy"].ToString() == null ? "" : dr["ModifiedBy"].ToString();
        details.ModifiedOn = dr["ModifiedOn"].ToString().ToDateTimeParse();
        details.Status = dr["Status"].ToString();
        details.Email = dr["Email"].ToString();
        details.BhossEmail = dr["BHOSS_Email"].ToString();
        details.Cluster = dr["Cluster"].ToString() == null ? "" : dr["Cluster"].ToString();
       }
      }
     }
    }
   }
   catch (Exception ex)
   {
    CTBC.Logs.Write("Fill", ex.Message, "Branches");
   }

   return details;
  }
  
  public DataTable GetADBReport(string pBranch, DateTime? pStartDate = null, DateTime? pEndDate = null)
  {
   List<Details> list = new List<Details>();
   DataTable dt = new DataTable();
   string strSQL = "";
   strSQL += " SELECT	A.pt_AccountNo, A.pt_PayCode, UPPER(B.[Description]) AS Description, YEAR(A.pt_ProcessDate) AS Year, MONTH(A.pt_ProcessDate) AS Month, A.pt_RequiredADB, A.pt_ActualADB, A.pt_Amount ";
   strSQL += " FROM	dbo.CMMC_PenaltyTxnMstr A ";
   strSQL += "	LEFT JOIN [dbo].[CMSCodes] B ON A.pt_PayCode = CAST(B.[CMSCode] AS VARCHAR(6)) ";

   if (pStartDate != null && pEndDate == null)
   {
    strSQL += "	WHERE	A.pt_ProcessDate >= @pStartDate ";
   }
   else if (pEndDate != null && pStartDate == null)
   {
    strSQL += "	WHERE	A.pt_ProcessDate <= @pEndDate ";
   }
   else if (pStartDate != null && pEndDate != null)
   {
    strSQL += "	WHERE	A.pt_ProcessDate BETWEEN @pStartDate AND @pEndDate ";
   }
   else
   {
    strSQL += "	WHERE	A.pt_ProcessDate = A.pt_ProcessDate ";
   }
   strSQL += "	AND SUBSTRING(A.pt_AccountNo,1,3) = RIGHT('000' + CAST(@pBranchCode AS VARCHAR(3)), 3) ";
   strSQL += "	ORDER	BY A.pt_ProcessDate, CAST(A.pt_PayCode AS INTEGER), A.pt_AccountNo ";

   try
   {
    using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
    {
     using (SqlCommand cmd = cn.CreateCommand())
     {
      cmd.CommandText = strSQL;
      
      if (pStartDate != null)
      {
      cmd.Parameters.Add(new SqlParameter("@pStartDate", pStartDate));
      }
      if (pEndDate != null)
      {
      cmd.Parameters.Add(new SqlParameter("@pEndDate", pEndDate));
      }
      cmd.Parameters.Add(new SqlParameter("@pBranchCode", pBranch));

      cn.Open();
      using (SqlDataAdapter da = new SqlDataAdapter(cmd))
      {
       da.Fill(dt);
      }
      cn.Close();
     }
    }
   }
   catch (Exception ex)
   {
    CTBC.Logs.Write("GetADBReport", ex.Message, "Branches");
   }
   return dt;
  }

  public int BranchProcessAction(List<int> IDs, bool IsActivate, string DeactivatedBy)
  {
   int intReturn = 0;
   string strSQL = "";

   strSQL += "UPDATE Branches ";
   strSQL += "SET ";
   strSQL += "IsActive = @pIsActive ";
   strSQL += ",ModifiedBy = @pModifiedBy ";
   strSQL += ",ModifiedOn = GETDATE() ";
   strSQL += "WHERE ";
   strSQL += "BranchCode = @pBranchCode ";

   using (SqlConnection cn = new SqlConnection(CMMC.Models.SharedFunctions.Connectionstring))
   {
    using (SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = strSQL;
     cn.Open();
     foreach (int intIDs in IDs)
     {
      cmd.Parameters.Clear();
      cmd.Parameters.Add(new SqlParameter("@pBranchCode", intIDs));
      cmd.Parameters.Add(new SqlParameter("@pIsActive", IsActivate ? "1" : "0"));
      cmd.Parameters.Add(new SqlParameter("@pModifiedBy", DeactivatedBy));
      intReturn += cmd.ExecuteNonQuery();
     }
    }
   }
   return intReturn;
  }

  public int Insert(Details pDetails)
  {
   int intReturn = 0;
   string strSQL = "";
   strSQL += "INSERT INTO Branches ";
   strSQL += "VALUES ";
   strSQL += "(@pBranchCode ";
   strSQL += ",@pBranchName ";
   strSQL += ",@pIsActive ";
   strSQL += ",NULL ";
   strSQL += ",@pCreatedBy ";
   strSQL += ",GETDATE() ";
   strSQL += ",NULL ";
   strSQL += ",NULL ";
   strSQL += ",@pEmail ";
   strSQL += ",@pBhossEmail ";
   strSQL += ",@pCluster) ";

   if (!this.IsExist(pDetails.BranchCode))
   {
    try
    {
     using (SqlConnection cn = new SqlConnection(CMMC.Models.SharedFunctions.Connectionstring))
     {
      using (SqlCommand cmd = cn.CreateCommand())
      {
       cmd.CommandText = strSQL;
       cmd.Parameters.Add(new SqlParameter("@pBranchCode", pDetails.BranchCode));
       cmd.Parameters.Add(new SqlParameter("@pBranchName", pDetails.BranchName));
       cmd.Parameters.Add(new SqlParameter("@pIsActive", pDetails.IsActive ? "1" : "0"));
       cmd.Parameters.Add(new SqlParameter("@pCreatedBy", pDetails.CreatedBy));
       cmd.Parameters.Add(new SqlParameter("@pEmail", pDetails.Email == null ? "" : pDetails.Email));
       cmd.Parameters.Add(new SqlParameter("@pBhossEmail", pDetails.BhossEmail == null ? "" : pDetails.BhossEmail));
       cmd.Parameters.Add(new SqlParameter("@pCluster", pDetails.Cluster == null ? "" : pDetails.Cluster));
       cn.Open();
       intReturn = cmd.ExecuteNonQuery();
      }
     }
    }
    catch (Exception e)
    {
     CTBC.Logs.Write("Insert", e.Message, "Branches");
    }
   }
   return intReturn;
  }

  public bool IsExist(string pBranchCode)
  {
   bool blnReturn = false;
   using (SqlConnection cn = new SqlConnection(CMMC.Models.SharedFunctions.Connectionstring))
   {
    using (SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = "SELECT COUNT(*) AS COUNTER FROM Branches WHERE BranchCode = @pBranchCode";
     cmd.Parameters.Add(new SqlParameter("@pBranchCode", pBranchCode));
     cn.Open();
     blnReturn = cmd.ExecuteScalar().ToString().ToInt() > 0;
    }
   }
   return blnReturn;
  }

  public int Update(Details pDetails)
  {
   int intReturn = 0;
   string strSQL = "";
   strSQL += " UPDATE branches ";
   strSQL += " SET "; 
   strSQL += " BranchName = @pBranchName ";
   strSQL += " ,IsActive = @pIsActive ";
   strSQL += " ,Status = @pStatus ";
   strSQL += " ,ModifiedBy = @pModifiedBy ";
   strSQL += " ,ModifiedOn = GETDATE() ";
   strSQL += " ,Email = @pEmail ";
   strSQL += " ,BHOSS_Email = @pBhossEmail ";
   strSQL += " ,Cluster = @pCluster ";
   strSQL += " WHERE ";
   strSQL += " BranchCode = @pBranchCode ";
   
   using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
   {
    using (SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = strSQL;
     cmd.Parameters.Add(new SqlParameter("@pBranchCode", pDetails.BranchCode));
     cmd.Parameters.Add(new SqlParameter("@pBranchName", pDetails.BranchName));
     cmd.Parameters.Add(new SqlParameter("@pIsActive", pDetails.IsActive ? "1" : "0"));
     cmd.Parameters.Add(new SqlParameter("@pStatus", pDetails.Status == null ? "" : pDetails.Status));
     cmd.Parameters.Add(new SqlParameter("@pEmail", pDetails.Email));
     cmd.Parameters.Add(new SqlParameter("@pModifiedBy", pDetails.ModifiedBy));
     cmd.Parameters.Add(new SqlParameter("@pBhossEmail", pDetails.BhossEmail));
     cmd.Parameters.Add(new SqlParameter("@pCluster", pDetails.Cluster));
     cn.Open();
     intReturn = cmd.ExecuteNonQuery();
    }
   }
   return intReturn;
  }

  //public int Delete(Details pDetails)
  //{
  // int intReturn = 0;
  // string strSQL = "";
  // strSQL += "DELETE * FROM Branches";
  // strSQL += "WHERE ";
  // strSQL += "BranchCode = @pCMSCode ";

  // using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
  // {
  //  using (SqlCommand cmd = cn.CreateCommand())
  //  {
  //   cmd.CommandText = strSQL;
  //   cmd.Parameters.Add(new SqlParameter("@pBranchCode", pDetails.BranchCode));
  //   cn.Open();
  //   intReturn = cmd.ExecuteNonQuery();
  //  }
  // }
  // return intReturn;
  //}

  public int Delete(string pID)
  {
   int intReturn = 0;
   string strSQL = "Delete Branches WHERE BranchCode = @pCode";

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

  public List<Details> GetList()
  {
   List<Details> list = new List<Details>();
   string strSQL = "";
   strSQL += "SELECT ";
   strSQL += " BranchCode ";
   strSQL += " ,BranchName ";
   strSQL += " ,IsActive ";
   strSQL += " ,Status ";
   strSQL += " ,CreatedBy ";
   strSQL += " ,CreatedOn ";
   strSQL += " ,ModifiedBy ";
   strSQL += " ,ModifiedOn ";
   strSQL += " ,Email ";
   strSQL += " ,BHOSS_Email ";
   strSQL += " ,Cluster ";
   strSQL += " FROM Branches ";
   strSQL += " ORDER BY BranchCode, BranchName";

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
       list.Add(new Details()
       {
        BranchCode = dr["BranchCode"].ToString()
        ,BranchName = dr["BranchName"].ToString()
        ,IsActive = dr["IsActive"].ToString().Equals("1")
        ,CreatedBy = dr["CreatedBy"].ToString()
        ,CreatedOn = dr["CreatedOn"].ToString().ToDateTime()
        ,ModifiedBy = dr["ModifiedBy"].ToString()
        ,ModifiedOn = dr["ModifiedOn"].ToString().ToDateTime()
        ,Status = dr["Status"].ToString()
        ,Email = dr["Email"].ToString()
        ,BhossEmail = dr["BHOSS_Email"].ToString()
        ,Cluster = dr["Cluster"].ToString()
       });
      }
     }
    }
   }
   return list;
  }

  public List<Details> GetBranchesName()
  {
   List<Details> list = new List<Details>();
   string strSQL = "";
   strSQL += "SELECT * ";
   strSQL += "FROM ";
   strSQL += "Branches ";

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
       list.Add(new Details
       {
        BranchName = dr["BranchName"].ToString(),
        BranchCode = dr["BranchCode"].ToString(),
        IsActive = dr["IsActive"].ToString().Equals("1")
       });
      }
     }
    }
   }
   return list;
  }
 }
}