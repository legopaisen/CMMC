using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using CTBC;

namespace CMMC.Models
{
 public class Requests : IDisposable
 {
  public void Dispose() { GC.SuppressFinalize(this); }

  public int RequestCode { get; set; }
  public string CreatedBy { get; set; }
  public DateTime CreatedOn { get; set; }
  public string ModifiedBy { get; set; }
  public DateTime ModifiedOn { get; set; }
  public string IPAddress { get; set; }
  public string AssignedApprover { get; set; }
  public List<RequestList> RequestsList { get; set; }

  public struct RequestDashBoard
  {
   //public int Index { get; set; }
   public string Description { get; set; }
   public int Count { get; set; }
  }

  public void FillInfo()
  {
   try
   {
    string strSQL = "";
    strSQL += "SELECT ";
    strSQL += "* ";
    strSQL += "FROM Requests ";
    strSQL += "WHERE RequestCode = @pRequestCode";

    using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
    {
     using (SqlCommand cmd = cn.CreateCommand())
     {
      cmd.CommandText = strSQL;
      cmd.Parameters.Add(new SqlParameter("@pRequestCode", this.RequestCode));
      cn.Open();
      using (SqlDataReader dr = cmd.ExecuteReader())
      {
       if (dr.Read())
       {
        this.RequestCode = dr["RequestCode"].ToString().ToInt();
        this.CreatedBy = dr["CreatedBy"].ToString();
        this.CreatedOn = dr["CreatedOn"].ToString().ToDateTime();
        this.ModifiedBy = dr["ModifiedBy"].ToString();
        this.ModifiedOn = dr["ModifiedOn"].ToString().ToDateTime();
        this.IPAddress = dr["IPAddress"].ToString();
        this.AssignedApprover = dr["AssignedApprover"].ToString();
       }
      }
     }
    }
   }
   catch (Exception ex)
   {
    CTBC.Logs.Write("Fill Info", ex.Message, "Request");
   }
  }

  public void Fill()
  {
   try
   {
    string strSQL = "";
    strSQL += "SELECT ";
    strSQL += "* ";
    strSQL += "FROM Requests ";
    strSQL += "WHERE RequestCode = @pRequestCode";

    using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
    {
     using (SqlCommand cmd = cn.CreateCommand())
     {
      cmd.CommandText = strSQL;
      cmd.Parameters.Add(new SqlParameter("@pRequestCode", this.RequestCode));
      cn.Open();
      using (SqlDataReader dr = cmd.ExecuteReader())
      {
       if (dr.Read())
       {
        this.RequestCode = dr["RequestCode"].ToString().ToInt();
        this.CreatedBy = dr["CreatedBy"].ToString();
        this.CreatedOn = dr["CreatedOn"].ToString().ToDateTime();
        this.ModifiedBy = dr["ModifiedBy"].ToString();
        this.ModifiedOn = dr["ModifiedOn"].ToString().ToDateTime();
        this.IPAddress = dr["IPAddress"].ToString();
        this.AssignedApprover = dr["AssignedApprover"].ToString();
        this.RequestsList = new RequestList().GetRequestList(dr["RequestCode"].ToString().ToInt());
       }
      }
     }
    }
   }
   catch (Exception ex)
   {
    CTBC.Logs.Write("Fill", ex.Message, "Request");
   }
  }

  public int Insert(bool pIsEmployee = false)
  {
   int intReturn = 0;
   try
   {
    string strSQL = "";
    strSQL += "INSERT INTO Requests VALUES (";
    strSQL += "@pCreatedBy ";
    strSQL += ", GETDATE() ";
    strSQL += ", @pModifiedBy ";
    strSQL += ", GETDATE() ";
    strSQL += ", @pIPAddress ";
    strSQL += ", @pAssignedApprover ";
    strSQL += ")";
    strSQL += " SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY]";

    using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
    {
     using (SqlCommand cmd = cn.CreateCommand())
     {
      cmd.CommandText = strSQL;
      cmd.Parameters.Add(new SqlParameter("@pCreatedBy", this.CreatedBy));
      cmd.Parameters.Add(new SqlParameter("@pModifiedBy", this.ModifiedBy));
      cmd.Parameters.Add(new SqlParameter("@pIPAddress", this.IPAddress));
      cmd.Parameters.Add(new SqlParameter("@pAssignedApprover", this.AssignedApprover));
      cn.Open();

      intReturn = Convert.ToInt32(cmd.ExecuteScalar());

      if (intReturn > 0 && this.RequestsList != null)
      {
       this.RequestCode = intReturn;
       foreach (RequestList request in this.RequestsList)
       {
        using (RequestList reqList = request)
        {
         reqList.Request = new Requests();
         reqList.Request.RequestCode = this.RequestCode;
         //if (pIsEmployee) { reqList.Insert(); }
         //else { intReturn = reqList.Insert(); }
         reqList.Insert();
        }
       }
      }
     }
    }
   }
   catch (Exception ex)
   {
    CTBC.Logs.Write("Insert", ex.Message, "Request");
   }
   return intReturn;
  }

  public int Update()
  {
   int intReturn = 0;
   try
   {
    string strSQL = "";
    strSQL += "UPDATE Requests SET ";
    strSQL += "ModifiedBy = @pModifiedBy ";
    strSQL += ", ModifiedOn = GETDATE() ";
    strSQL += ", AssignedApprover = @pAssignedApprover ";
    strSQL += "WHERE RequestCode = @pRequestCode";

    using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
    {
     using (SqlCommand cmd = cn.CreateCommand())
     {
      cmd.CommandText = strSQL;
      cmd.Parameters.Add(new SqlParameter("@pModifiedBy", this.ModifiedBy));
      cmd.Parameters.Add(new SqlParameter("@pAssignedApprover", this.AssignedApprover));
      cn.Open();
      intReturn = cmd.ExecuteNonQuery();

      if (intReturn > 0 && this.RequestsList != null)
      {
       foreach (RequestList request in this.RequestsList)
       {
        using (RequestList reqList = request)
        {
         reqList.Update(reqList);
        }
       }
      }
     }
    }
   }
   catch (Exception ex)
   {
    CTBC.Logs.Write("Update", ex.Message, "Request");
   }

   return intReturn;
  }

  public void FillRequest(SharedFunctions.Status? pStatus = null)
  {
   List<Requests> requests = new List<Requests>();
   try
   {
    string strSQL = "";
    strSQL += " SELECT ";
    strSQL += " req.RequestCode AS [RequestCode],";
    strSQL += " req.CreatedBy AS [CreatedBy],";
    strSQL += " req.CreatedOn AS [CreatedOn],";
    strSQL += " req.ModifiedBy AS [ModifiedBy],";
    strSQL += " req.ModifiedOn AS [ModifiedOn],";
    strSQL += " req.IPAddress AS [IPAddress],";
    strSQL += " req.AssignedApprover AS [AssignedApprover],";
    strSQL += " reqList.RequestListCode AS [RequestListCode],";
    strSQL += " reqList.[Module] AS [Module],";
    strSQL += " reqList.AffectedTable AS [AffectedTable],";
    strSQL += " reqList.[Action] AS [Action],";
    strSQL += " reqList.NewValues AS [NewValues],";
    strSQL += " reqList.OldValues AS [OldValues],";
    strSQL += " reqList.WhereValues AS [WhereValues],";
    strSQL += " reqList.Remarks AS [Remarks],";
    strSQL += " reqList.Status AS [Status],";
    strSQL += " reqList.ApprovedBy AS [ApprovedBy],";
    strSQL += " reqList.ApprovedOn AS [ApprovedOn]";
    strSQL += " FROM Requests req ";
    strSQL += " LEFT JOIN RequestList reqList ON req.RequestCode = reqList.RequestCode";
    strSQL += " WHERE";
    strSQL += " req.RequestCode = @pRequestCode";
    strSQL += pStatus != null ? "" : " AND reqList.Status = @pStatus";

    using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
    {
     using (SqlCommand cmd = cn.CreateCommand())
     {
      cmd.CommandText = strSQL;

      cmd.Parameters.Add(new SqlParameter("@pRequestCode", this.RequestCode));
      if (pStatus != null) { cmd.Parameters.Add(new SqlParameter("@pStatus", (int)pStatus)); }

      cn.Open();
      using (SqlDataReader dr = cmd.ExecuteReader())
      {
       bool isRequestSpecified = false;
       while (dr.Read())
       {
        if (!isRequestSpecified)
        {
         isRequestSpecified = true;
         //if (requests.Count(x => x.RequestCode == Convert.ToInt32(dr["RequestCode"])) == 0)
         //{
         //Requests req = new Requests();
         this.RequestCode = dr["RequestCode"].ToString().ToInt();
         this.CreatedBy = dr["CreatedBy"].ToString();
         this.CreatedOn = dr["CreatedOn"].ToString().ToDateTime();
         this.ModifiedBy = dr["ModifiedBy"].ToString();
         this.ModifiedOn = dr["ModifiedOn"].ToString().ToDateTime();
         this.IPAddress = dr["IPAddress"].ToString();
         this.AssignedApprover = dr["AssignedApprover"].ToString();
        
         this.RequestsList = new List<RequestList>();
         RequestList reqList = new RequestList();
         reqList.Action = dr["Action"].ToString();
         reqList.AffectedTable = dr["AffectedTable"].ToString();
         reqList.ApprovedBy = dr["ApprovedBy"].ToString();
         if (string.IsNullOrEmpty(dr["ApprovedOn"].ToString())) { reqList.ApprovedOn = null; }
         else { reqList.ApprovedOn = Convert.ToDateTime(dr["ApprovedOn"]); }
         reqList.Module = dr["Module"].ToString();
         reqList.NewValues = new RequestList().ToListValues(dr["NewValues"].ToString());
         reqList.OldValues = new RequestList().ToListValues(dr["OldValues"].ToString());
         reqList.WhereValues = new RequestList().ToListValues(dr["WhereValues"].ToString());
         reqList.Remarks = dr["Remarks"].ToString();
         reqList.Request = new Requests()
         {
          RequestCode = dr["RequestCode"].ToString().ToInt(),
          CreatedBy = dr["CreatedBy"].ToString(),
          CreatedOn = dr["CreatedOn"].ToString().ToDateTime(),
          ModifiedBy = dr["ModifiedBy"].ToString(),
          ModifiedOn = dr["ModifiedOn"].ToString().ToDateTime(),
          IPAddress = dr["IPAddress"].ToString(),
          AssignedApprover = dr["AssignedApprover"].ToString(),
         };
         reqList.RequestListCode = Convert.ToInt32(dr["RequestListCode"]);
         reqList.Status = dr["Status"].ToString();

         this.RequestsList.Add(reqList);
         //requests.Add(req);
        }
        else
        {
         RequestList reqList = new RequestList();
         reqList.Action = dr["Action"].ToString();
         reqList.AffectedTable = dr["AffectedTable"].ToString();
         reqList.ApprovedBy = dr["ApprovedBy"].ToString();
         if (string.IsNullOrEmpty(dr["ApprovedOn"].ToString())) { reqList.ApprovedOn = null; }
         else { reqList.ApprovedOn = Convert.ToDateTime(dr["ApprovedOn"]); }
         reqList.Module = dr["Module"].ToString();
         reqList.NewValues = new RequestList().ToListValues(dr["NewValues"].ToString());
         reqList.OldValues = new RequestList().ToListValues(dr["OldValues"].ToString());
         reqList.WhereValues = new RequestList().ToListValues(dr["WhereValues"].ToString());
         reqList.Remarks = dr["Remarks"].ToString();
         reqList.Request = new Requests()
         {
          RequestCode = dr["RequestCode"].ToString().ToInt(),
          CreatedBy = dr["CreatedBy"].ToString(),
          CreatedOn = dr["CreatedOn"].ToString().ToDateTime(),
          ModifiedBy = dr["ModifiedBy"].ToString(),
          ModifiedOn = dr["ModifiedOn"].ToString().ToDateTime(),
          IPAddress = dr["IPAddress"].ToString(),
          AssignedApprover = dr["AssignedApprover"].ToString(),
         };
         reqList.RequestListCode = Convert.ToInt32(dr["RequestListCode"]);
         reqList.Status = dr["Status"].ToString();

         this.RequestsList.Add(reqList);
         //requests.First(x => x.RequestCode == Convert.ToInt32(dr["RequestCode"])).RequestsList.Add(reqList);
        }
       }
      }
     }
    }
   }
   catch (Exception ex)
   {
    CTBC.Logs.Write("Fill Request", ex.Message, "Request");
   }
   //return requests;
  }

  public List<Requests> GetRequestList(string pModule = "", SharedFunctions.Status? pStatus = null)
  {
   List<Requests> requests = new List<Requests>();
   try
   {
    string strSQL = "";
    strSQL += " SELECT ";
    strSQL += " req.RequestCode AS [RequestCode],";
    strSQL += " req.CreatedBy AS [CreatedBy],";
    strSQL += " req.CreatedOn AS [CreatedOn],";
    strSQL += " req.ModifiedBy AS [ModifiedBy],";
    strSQL += " req.ModifiedOn AS [ModifiedOn],";
    strSQL += " req.IPAddress AS [IPAddress],";
    strSQL += " req.AssignedApprover AS [AssignedApprover],";
    strSQL += " reqList.RequestListCode AS [RequestListCode],";
    strSQL += " reqList.[Module] AS [Module],";
    strSQL += " reqList.AffectedTable AS [AffectedTable],";
    strSQL += " reqList.[Action] AS [Action],";
    strSQL += " reqList.NewValues AS [NewValues],";
    strSQL += " reqList.OldValues AS [OldValues],";
    strSQL += " reqList.WhereValues AS [WhereValues],";
    strSQL += " reqList.Remarks AS [Remarks],";
    strSQL += " reqList.Status AS [Status],";
    strSQL += " reqList.ApprovedBy AS [ApprovedBy],";
    strSQL += " reqList.ApprovedOn AS [ApprovedOn]";
    strSQL += " FROM Requests req ";
    strSQL += " LEFT JOIN RequestList reqList ON req.RequestCode = reqList.RequestCode";
    if (pModule != "" || pStatus != null)
    {
     strSQL += " WHERE";
     strSQL += (pModule != "" ? " reqList.Module IN(@pModule" : "") + (pModule == "Employees" ? ",'Organization')" : ")");
     //strSQL += pModule == "Employees" ? " OR reqList.Module = 'Organization')" : "";
     strSQL += pStatus == null ? "" : (pModule != "" ? " AND" : "") + " reqList.Status = @pStatus";
    }

    using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
    {
     using (SqlCommand cmd = cn.CreateCommand())
     {
      cmd.CommandText = strSQL;

      if (pModule != "") { cmd.Parameters.Add(new SqlParameter("@pModule", pModule)); }
      if (pStatus != null) { cmd.Parameters.Add(new SqlParameter("@pStatus", (int)pStatus)); }

      cn.Open();
      using (SqlDataReader dr = cmd.ExecuteReader())
      {
       while (dr.Read())
       {
        if (requests.Count(x => x.RequestCode == Convert.ToInt32(dr["RequestCode"])) == 0)
        {
         Requests req = new Requests();
         req.RequestCode = dr["RequestCode"].ToString().ToInt();
         req.CreatedBy = dr["CreatedBy"].ToString();
         req.CreatedOn = dr["CreatedOn"].ToString().ToDateTime();
         req.ModifiedBy = dr["ModifiedBy"].ToString();
         req.ModifiedOn = dr["ModifiedOn"].ToString().ToDateTime();
         req.IPAddress = dr["IPAddress"].ToString();
         req.AssignedApprover = dr["AssignedApprover"].ToString();
         
         req.RequestsList = new List<RequestList>();
         RequestList reqList = new RequestList();
         reqList.Action = dr["Action"].ToString();
         reqList.AffectedTable = dr["AffectedTable"].ToString();
         reqList.ApprovedBy = dr["ApprovedBy"].ToString();
         if (string.IsNullOrEmpty(dr["ApprovedOn"].ToString())) { reqList.ApprovedOn = null; }
         else { reqList.ApprovedOn = Convert.ToDateTime(dr["ApprovedOn"]); }
         reqList.Module = dr["Module"].ToString();
         reqList.NewValues = new RequestList().ToListValues(dr["NewValues"].ToString());
         reqList.OldValues = new RequestList().ToListValues(dr["OldValues"].ToString());
         reqList.WhereValues = new RequestList().ToListValues(dr["WhereValues"].ToString());
         reqList.Remarks = dr["Remarks"].ToString();
         reqList.Request = new Requests()
          {
           RequestCode = dr["RequestCode"].ToString().ToInt(),
           CreatedBy = dr["CreatedBy"].ToString(),
           CreatedOn = dr["CreatedOn"].ToString().ToDateTime(),
           ModifiedBy = dr["ModifiedBy"].ToString(),
           ModifiedOn = dr["ModifiedOn"].ToString().ToDateTime(),
           IPAddress = dr["IPAddress"].ToString(),
           AssignedApprover = dr["AssignedApprover"].ToString()
          };
         reqList.RequestListCode = Convert.ToInt32(dr["RequestListCode"]);
         reqList.Status = dr["Status"].ToString();

         req.RequestsList.Add(reqList);
         requests.Add(req);
        }
        else
        {
         RequestList reqList = new RequestList();
         reqList.Action = dr["Action"].ToString();
         reqList.AffectedTable = dr["AffectedTable"].ToString();
         reqList.ApprovedBy = dr["ApprovedBy"].ToString();
         if (string.IsNullOrEmpty(dr["ApprovedOn"].ToString())) { reqList.ApprovedOn = null; }
         else { reqList.ApprovedOn = Convert.ToDateTime(dr["ApprovedOn"]); }
         reqList.Module = dr["Module"].ToString();
         reqList.NewValues = new RequestList().ToListValues(dr["NewValues"].ToString());
         reqList.OldValues = new RequestList().ToListValues(dr["OldValues"].ToString());
         reqList.WhereValues = new RequestList().ToListValues(dr["WhereValues"].ToString());
         reqList.Remarks = dr["Remarks"].ToString();
         reqList.Request = new Requests()
         {
          RequestCode = dr["RequestCode"].ToString().ToInt(),
          CreatedBy = dr["CreatedBy"].ToString(),
          CreatedOn = dr["CreatedOn"].ToString().ToDateTime(),
          ModifiedBy = dr["ModifiedBy"].ToString(),
          ModifiedOn = dr["ModifiedOn"].ToString().ToDateTime(),
          IPAddress = dr["IPAddress"].ToString(),
          AssignedApprover = dr["AssignedApprover"].ToString(),
         };
         reqList.RequestListCode = Convert.ToInt32(dr["RequestListCode"]);
         reqList.Status = dr["Status"].ToString();

         requests.First(x => x.RequestCode == Convert.ToInt32(dr["RequestCode"])).RequestsList.Add(reqList);
        }
       }
      }
     }
    }
   }
   catch (Exception ex)
   {
    CTBC.Logs.Write("Get Request List", ex.Message, "Request");
   }
   return requests;
  }

  public bool HasRequest(string pID)
  {
   bool blnReturn = false;
   try
   {
    string strSQL = "";
    strSQL += " SELECT";
    strSQL += " COUNT(*)";
    strSQL += " FROM";
    strSQL += " RequestList";
    strSQL += " WHERE";
    strSQL += " RequestCode IN(SELECT RequestCode FROM Requests WHERE CreatedBy = @pCreatedBy)";
    strSQL += " AND";
    strSQL += " Status = '0'";

    using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
    {
     using (SqlCommand cmd = cn.CreateCommand())
     {
      cmd.CommandText = strSQL;
      cmd.Parameters.Add(new SqlParameter("@pCreatedBy", pID));
      cn.Open();
      blnReturn = Convert.ToInt32(cmd.ExecuteScalar()) > 0;
     }
    }
   }
   catch (Exception ex)
   {
    CTBC.Logs.Write("Has request", ex.Message, "Request");
   }
   return blnReturn;
  }

  public Requests GetRequestsByRequestListCode(int pRequesListCode)
  {
   Requests requests = new Requests();
   string strSQL = "SELECT * FROM Requests ";
   strSQL += "WHERE RequestCode = ";
   strSQL += "(Select RequestCode FROM RequestList ";
   strSQL += "WHERE RequestListCode = @pRequestListCode) ";
   try
   {
    using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
    {
     using (SqlCommand cmd = cn.CreateCommand())
     {
      cmd.CommandText = strSQL;
      cmd.Parameters.Add(new SqlParameter("@pRequestListCode", pRequesListCode));
      cn.Open();
      using (SqlDataReader dr = cmd.ExecuteReader())
      {
       if (dr.Read())
       {
        requests.CreatedBy = dr["CreatedBy"].ToString();
        requests.CreatedOn = dr["CreatedOn"].ToString().ToDateTime();
        requests.IPAddress = dr["IPAddress"].ToString();
        requests.ModifiedBy = dr["ModifiedBy"].ToString();
        requests.ModifiedOn = dr["ModifiedOn"].ToString().ToDateTime();
        requests.RequestCode = dr["RequestCode"].ToString().ToInt();
        requests.AssignedApprover = dr["AssignedApprover"].ToString();
       }
      }
     }
    }
   }
   catch(Exception ex)
   {
    CTBC.Logs.Write("Get Request By Request List Code", ex.Message, "Request");
   }   
   return requests;
  }
  
 }
}