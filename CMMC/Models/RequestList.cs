using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using CTBC;
using System.Data;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Runtime.Serialization;
using System.Net.Mail;
using System.Text;
using System.Data.OracleClient;

namespace CMMC.Models
{
    public class ParamData
    {
        public int RequestCode { get; set; }
        public string CMSCode { get; set; }
    }


    public class MaintainedAccount
    {
        public string CMSCode { get; set; }
        public int MotherCount { get; set; }
        public int ChildCount { get; set; }
    }

    public class RequestList : IDisposable
    {
        public void Dispose() { GC.SuppressFinalize(this); }

        public int RequestListCode { get; set; }
        public Requests Request { get; set; }
        public string Module { get; set; }
        public string Action { get; set; }
        public List<Values> NewValues { get; set; }
        public List<Values> OldValues { get; set; }
        public List<Values> WhereValues { get; set; }
        public string AffectedTable { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedOn { get; set; }
        public string ConcernedCMSCode { get; set; }

        public struct Values
        {
            public string FieldName { get; set; }
            public object Value { get; set; }
        }

        public enum Actions
        {
            DELETE = 0,
            UPDATE = 1,
            INSERT = 2
        }

        DateTime rngMin = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;
        DateTime rngMax = (DateTime)System.Data.SqlTypes.SqlDateTime.MaxValue;

        //Field::Value 
        private string ToStringValues(Values pValue)
        {
            string strReturn = "";
            strReturn += pValue.FieldName + "::" + pValue.Value;
            return strReturn;
        }

        //FieldValue|FieldValue
        private string ToStringValues(List<Values> pValue)
        {
            string strReturn = "";
            int intIndex = 0;
            foreach (Values value in pValue)
            {
                intIndex++;
                strReturn += this.ToStringValues(value) + (intIndex.Equals(pValue.Count) ? "" : "|");
            }
            return strReturn;
        }

        //Convert Values to their respective datatype
        private string ConvertValues(object pValue, string pFieldName, bool pIsDefined)
        {
            string strReturn = "";
            if (pValue != null)
            {
                strReturn = pValue.ToString();
            }
            else
            {
                pValue = "";
            }

            DateTime dteReturn = DateTime.Now;
            bool blnValidate = false;
            if (pIsDefined)
            {
                blnValidate = DateTime.TryParse(pValue.ToString(), out dteReturn);

                switch (pFieldName)
                {
                    case "BranchCode":
                        strReturn = pValue.ToString().Replace(";", ", ");
                        break;
                    case "CreatedBy":
                    case "ModifiedBy":
                        if (string.IsNullOrEmpty(pValue.ToString()))
                        {
                            strReturn = "N/A";
                        }
                        else if (pValue == "SYSTEM")
                        {
                            strReturn = pValue.ToString();
                        }
                        else
                        {
                            strReturn = pValue.ToString();
                        }

                        break;
                    case "CreatedOn":
                    case "ModifiedOn":
                        if (dteReturn.Year > 1900)
                        {
                            strReturn = dteReturn.ToString("MMM dd, yyyy hh:mm:ss");
                        }
                        else
                        {
                            strReturn = "";
                        }
                        break;
                    case "MaxFreeTransaction":
                        if (pValue == "Unlimited")
                        {
                            strReturn = "-1";
                        }
                        else
                        {
                            strReturn = pValue.ToString().Trim();
                        }
                        break;
                    case "Status":
                        strReturn = ((SharedFunctions.Status)Enum.Parse(typeof(SharedFunctions.Status), pValue.ToString())).ToString();
                        break;
                    case "IsActive":
                        strReturn = (pValue.ToString().Equals("1") || pValue.ToString().Equals("True")) ? true.ToString() : false.ToString();
                        break;
                    case "SubRequiredADB":
                    case "MotherRequiredADB":
                    case "WithdrawalFeePerTransaction":
                        strReturn = pValue.ToDecimal().ToString("0.00");
                        break;
                    case "RMID":
                        int id = 0;
                        if (pValue.ToString().Length > 0)
                        {
                            if (int.TryParse(pValue.ToString(), out id))
                            {
                                strReturn = id.ToString();
                            }
                            else
                            {
                                strReturn = ((RelationshipManagerModel)pValue).ID.ToString();
                            }
                        }
                        break;
                    default:
                        strReturn = pValue.ToString();
                        break;
                }

                if (blnValidate)
                {
                    //strReturn = dteReturn.to
                }
            }
            return strReturn;
        }

        //private string ConvertValues(string pValue, string pFieldName, bool pIsDefined)
        //{
        //  string strReturn = pValue;
        //  DateTime dteReturn = DateTime.Now;
        //  bool blnValidate = false;
        //  if (pIsDefined)
        //  {
        //    blnValidate = DateTime.TryParse(pValue, out dteReturn);

        //    switch (pFieldName)
        //    {
        //      case "BranchCode":
        //        strReturn = pValue.ToString().Replace(";", ", ");
        //        break;
        //      case "CreatedBy":
        //      case "ModifiedBy":
        //        if (string.IsNullOrEmpty(pValue))
        //        {
        //          strReturn = "N/A";
        //        }
        //        else if (pValue == "SYSTEM")
        //        {
        //          strReturn = pValue;
        //        }
        //        else
        //        {
        //          strReturn = pValue;
        //        }

        //        break;
        //      case "CreatedOn":
        //      case "ModifiedOn":
        //        if (dteReturn.Year > 1900)
        //        {
        //          strReturn = dteReturn.ToString("MMM dd, yyyy hh:mm:ss");
        //        }
        //        else
        //        {
        //          strReturn = "";
        //        }
        //        break;
        //      case "MaxFreeTransaction":
        //        if (pValue == "Unlimited")
        //        {
        //          strReturn = "-1";
        //        }
        //        else
        //        {
        //          strReturn = pValue.Trim();
        //        }
        //        break;
        //      case "Status":
        //        strReturn = ((SharedFunctions.Status)Enum.Parse(typeof(SharedFunctions.Status), pValue)).ToString();
        //        break;
        //      case "IsActive":
        //        strReturn = (pValue.Equals("1") || pValue.Equals("True")) ? true.ToString() : false.ToString();
        //        break;
        //      case "SubRequiredADB":
        //      case "MotherRequiredADB":
        //      case "WithdrawalFeePerTransaction":
        //        strReturn = pValue.ToDecimal().ToString("0.00");
        //        break;
        //      default:
        //        strReturn = pValue;
        //        break;
        //    }

        //    if (blnValidate)
        //    {
        //      //strReturn = dteReturn.to
        //    }
        //  }
        //  return strReturn;
        //}

        public string RemoveWhitespace(string str)
        {
            return string.Join("", str.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
        }


        //parse string return LIST
        public List<Values> ToListValues(string pValue, bool pIsDefined = true)
        {
            List<Values> values = new List<Values>();
            string[] strValues = pValue.Split(new char[] { '|' });
            try
            {
                foreach (string strValue in strValues)
                {
                    string[] str = strValue.Split(new string[] { "::" }, StringSplitOptions.None);
                    values.Add(new Values()
                    {
                        FieldName = str[0]
                     ,
                        Value = pIsDefined ? this.ConvertValues(str[1], str[0], pIsDefined) : str[1]
                    });
                }
            }
            catch (Exception e)
            {
                CTBC.Logs.Write("To List Values", e.Message, "RequestList");
            }
            return values;
        }

        public void Fill(bool pIsDefined = true)
        {
            try
            {
                string strSQL = "";
                strSQL += "SELECT ";
                strSQL += "* ";
                strSQL += "FROM RequestList ";
                strSQL += "WHERE RequestCode = @pRequestCode ";
                //strSQL += "AND RequestListCode = @pRequestListCode";

                using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
                {
                    using (SqlCommand cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = strSQL;
                        cmd.Parameters.Add(new SqlParameter("@pRequestCode", this.RequestListCode));
                        //cmd.Parameters.Add(new SqlParameter("@pRequestListCode", this.RequestListCode));
                        cn.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                this.Request = new Requests();
                                this.Request.RequestCode = dr["RequestCode"].ToString().ToInt();
                                this.Request.Fill(); //FILL DATA OF REQUESTOR
                                this.RequestListCode = dr["RequestListCode"].ToString().ToInt();
                                this.Module = dr["Module"].ToString();
                                this.Action = dr["Action"].ToString();
                                this.NewValues = this.ToListValues(dr["NewValues"].ToString(), pIsDefined);
                                this.OldValues = this.ToListValues(dr["OldValues"].ToString(), pIsDefined);
                                this.WhereValues = this.ToListValues(dr["WhereValues"].ToString(), pIsDefined);
                                this.AffectedTable = dr["AffectedTable"].ToString();
                                this.Status = dr["Status"].ToString();
                                this.Remarks = dr["Remarks"].ToString();
                                this.ApprovedBy = dr["ApprovedBy"].ToString();
                                if (dr["ApprovedOn"].ToString() == "") { this.ApprovedOn = null; }
                                else { this.ApprovedOn = Convert.ToDateTime(dr["ApprovedOn"]); }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CTBC.Logs.Write("Fill", ex.Message, "Request List");
            }
        }

        public int Insert()
        {
            int intReturn = 0;
            try
            {
                string strSQL = "";
                strSQL += "INSERT INTO RequestList VALUES (";
                strSQL += "@pRequestCode ";
                strSQL += ",@pModule ";
                strSQL += ",@pAction ";
                strSQL += ",@pNewValues ";
                strSQL += ",@pOldValues ";
                strSQL += ",@pWhereValues ";
                strSQL += ",@pAffectedTable ";
                strSQL += ",@pRemarks ";
                strSQL += ",@pStatus ";
                strSQL += ",@pApprovedBy ";
                strSQL += ",null ";
                strSQL += ")";
                strSQL += " SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY]";

                using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
                {
                    using (SqlCommand cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = strSQL;
                        cmd.Parameters.Add(new SqlParameter("@pRequestCode", this.Request.RequestCode));
                        cmd.Parameters.Add(new SqlParameter("@pModule", this.Module));
                        cmd.Parameters.Add(new SqlParameter("@pAction", this.Action.ToString()));
                        cmd.Parameters.Add(new SqlParameter("@pNewValues", this.ToStringValues(this.NewValues)));
                        cmd.Parameters.Add(new SqlParameter("@pOldValues", this.ToStringValues(this.OldValues)));
                        cmd.Parameters.Add(new SqlParameter("@pWhereValues", this.ToStringValues(this.WhereValues)));
                        cmd.Parameters.Add(new SqlParameter("@pAffectedTable", this.AffectedTable));
                        cmd.Parameters.Add(new SqlParameter("@pRemarks", this.Remarks));
                        cmd.Parameters.Add(new SqlParameter("@pStatus", this.Status));
                        cmd.Parameters.Add(new SqlParameter("@pApprovedBy", string.IsNullOrEmpty(this.ApprovedBy) ? "" : this.ApprovedBy));
                        cn.Open();
                        intReturn = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                CTBC.Logs.Write("Insert", ex.Message, "Request List");
            }
            return intReturn;
        }
        //Return STRING FOR SQL COMMAND
        //private string BuildSQL()
        //{
        // string strReturn = "";
        // int intIndex = 0;
        // try
        // {
        //  Actions action = (Actions)Enum.Parse(typeof(Actions), this.Action);

        //  if (action == Actions.INSERT)
        //  {
        //   strReturn += " UPDATE";
        //   strReturn += " " + this.AffectedTable;
        //   strReturn += " SET Status = @pStatus";
        //   strReturn += " WHERE ";
        //   intIndex = 0;
        //   foreach (RequestList.Values value in this.WhereValues)
        //   {
        //    intIndex++;
        //    strReturn += string.Format(" {0} = {1} {2} ", value.FieldName, "@p" + value.FieldName, intIndex == this.WhereValues.Count ? "" : "AND");
        //   }
        //  }
        //  else if (action == Actions.UPDATE)
        //  {
        //   strReturn += " UPDATE ";
        //   strReturn += string.Format(" {0} ", this.AffectedTable);
        //   strReturn += " SET ";
        //   intIndex = 0;
        //   foreach (RequestList.Values value in this.NewValues)
        //   {
        //    intIndex++;
        //    strReturn += string.Format(" {0} = {1} {2} ", value.FieldName, (string.IsNullOrEmpty(value.Value.ToString()) ? "null" : "@p" + value.FieldName), intIndex == this.NewValues.Count ? "" : ",");
        //   }
        //   strReturn += " WHERE ";
        //   intIndex = 0;
        //   foreach (RequestList.Values value in this.WhereValues)
        //   {
        //    intIndex++;
        //    strReturn += string.Format(" {0} = {1} {2} ", value.FieldName, "@p" + value.FieldName, intIndex == this.WhereValues.Count ? "" : "AND");
        //   }
        //  }
        //  else if (action == Actions.DELETE)
        //  {
        //   strReturn += " UPDATE ";
        //   strReturn += string.Format(" {0} ", this.AffectedTable);
        //   strReturn += " SET ";
        //   intIndex = 0;
        //   foreach (RequestList.Values value in this.NewValues)
        //   {
        //    intIndex++;
        //    strReturn += string.Format(" {0} = {1} {2} ", value.FieldName, "@p" + value.FieldName, intIndex == this.NewValues.Count ? "" : ",");
        //   }
        //   strReturn += " WHERE ";
        //   intIndex = 0;
        //   foreach (RequestList.Values value in this.WhereValues)
        //   {
        //    intIndex++;
        //    strReturn += string.Format(" {0} = {1} {2} ", value.FieldName, "@p" + value.FieldName, intIndex == this.WhereValues.Count ? "" : "AND");
        //   }
        //  }
        // }
        // catch (Exception ex)
        // {
        // }
        // return strReturn;
        //}

        //public int DoProcessRequest(int pRequestCode, bool pIsApproved, string pApprover)
        //{
        // int intReturn = 0;
        // try
        // {
        //  string strSQL = "";
        //  this.RequestListCode = pRequestCode;
        //  this.Fill(false);

        //  strSQL = this.BuildSQL();

        //  using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
        //  {
        //   using (SqlCommand cmd = cn.CreateCommand())
        //   {
        //    cmd.CommandText = strSQL;

        //    RequestList.Actions action = (RequestList.Actions)Enum.Parse(typeof(RequestList.Actions), this.Action);

        //    if (action == Actions.INSERT)
        //    {
        //     cmd.Parameters.Add(new SqlParameter("@pStatus", pIsApproved ? (int)SharedFunctions.Status.Approved : (int)SharedFunctions.Status.Disapproved));

        //     foreach (RequestList.Values value in this.WhereValues)
        //     {
        //      cmd.Parameters.Add(new SqlParameter("@p" + value.FieldName, value.Value));
        //     }
        //     cn.Open();
        //     intReturn = cmd.ExecuteNonQuery();
        //    }
        //    else
        //    {
        //     if (pIsApproved)
        //     {
        //      if (action == Actions.UPDATE)
        //      {
        //       foreach (RequestList.Values value in this.NewValues)
        //       {
        //        cmd.Parameters.Add(new SqlParameter("@p" + value.FieldName, value.Value));
        //       }
        //      }
        //      else if (action == Actions.DELETE)
        //      {
        //       foreach (RequestList.Values value in this.NewValues)
        //       {
        //        cmd.Parameters.Add(new SqlParameter("@p" + value.FieldName, (value.FieldName == "AdjustedIn" || value.FieldName == "AdjustedOut") && value.Value == "" ? System.Data.SqlTypes.SqlDateTime.Null : value.Value));
        //       }
        //      }

        //      foreach (RequestList.Values value in this.WhereValues)
        //      {
        //       cmd.Parameters.Add(new SqlParameter("@p" + value.FieldName, value.Value));
        //      }
        //      cn.Open();
        //      intReturn = cmd.ExecuteNonQuery();
        //     }
        //    }
        //   }
        //  }

        //  if (pIsApproved)
        //  {
        //   this.Status = "1";
        //  }
        //  else
        //  {
        //   this.Status = "2";
        //  }
        //  this.ApprovedBy = pApprover;
        //  this.ApprovedOn = SharedFunctions.ServerDate;
        //  //this.Update(item);
        // }
        // catch (Exception ex)
        // {
        // }
        // return intReturn;
        //}

        public int Update(RequestList pRequestList)
        {
            int intReturn = 0;
            try
            {
                string strSQL = "";
                strSQL += "UPDATE RequestList SET ";
                strSQL += " NewValues = @pNewValues ";
                strSQL += ",OldValues = @pOldValues ";
                strSQL += ",Remarks = @pRemarks ";
                strSQL += ",WhereValues = @pWhereValues ";
                strSQL += ",Status = @pStatus ";
                strSQL += ",ApprovedBy = @pApprovedBy ";
                strSQL += ",ApprovedOn = @pApprovedOn ";
                strSQL += "WHERE RequestListCode = @pRequestListCode";

                using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
                {
                    using (SqlCommand cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = strSQL;
                        cmd.Parameters.Add(new SqlParameter("@pRequestListCode", pRequestList.RequestListCode));
                        cmd.Parameters.Add(new SqlParameter("@pNewValues", ToStringValues(pRequestList.NewValues)));
                        cmd.Parameters.Add(new SqlParameter("@pOldValues", ToStringValues(pRequestList.OldValues)));
                        cmd.Parameters.Add(new SqlParameter("@pWhereValues", ToStringValues(pRequestList.WhereValues)));
                        cmd.Parameters.Add(new SqlParameter("@pStatus", pRequestList.Status));
                        cmd.Parameters.Add(new SqlParameter("@pRemarks", pRequestList.Remarks));
                        cmd.Parameters.Add(new SqlParameter("@pApprovedBy", pRequestList.ApprovedBy));
                        if (pRequestList.ApprovedOn.ToString() == "") { cmd.Parameters.Add(new SqlParameter("@pApprovedOn", System.Data.SqlTypes.SqlDateTime.Null)); }
                        else { cmd.Parameters.Add(new SqlParameter("@pApprovedOn", SharedFunctions.ServerDate)); }
                        cn.Open();
                        intReturn = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                CTBC.Logs.Write("Update", ex.Message, "Request List");
            }
            return intReturn;
        }


        public int ChangeStatus()
        {
            int intReturn = 0;
            try
            {
                using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
                {
                    using (SqlCommand cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = "UPDATE Requests SET Status = @pStatus WHERE RequestListCode = @pRequestListCode";
                        cmd.Parameters.Add(new SqlParameter("@pRequestListCode", this.RequestListCode));
                        cmd.Parameters.Add(new SqlParameter("@pStatus", this.Status));
                        cn.Open();
                        intReturn = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                CTBC.Logs.Write("Change Status", ex.Message, "Request List");
            }
            return intReturn;
        }
        //Change Request List Status 
        public int ChangRequestStatus()
        {
            int intReturn = 0;
            try
            {
                using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
                {
                    using (SqlCommand cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = "UPDATE RequestList SET Status = @pStatus WHERE RequestListCode = @pRequestListCode";
                        cmd.Parameters.Add(new SqlParameter("@pRequestListCode", this.RequestListCode));
                        cmd.Parameters.Add(new SqlParameter("@pStatus", SharedFunctions.Status.Cancelled));
                        cn.Open();
                        intReturn = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                CTBC.Logs.Write("Change Request Status", ex.Message, "Request List");
            }
            return intReturn;
        }
        //End
        public List<RequestList> GetRequestList(int pRequestCode)
        {
            List<RequestList> requests = new List<RequestList>();
            try
            {
                string strSQL = "";
                strSQL += "SELECT ";
                strSQL += "* ";
                strSQL += "FROM RequestList ";
                strSQL += "WHERE RequestCode = @pRequestCode";

                using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
                {
                    using (SqlCommand cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = strSQL;
                        cmd.Parameters.Add(new SqlParameter("@pRequestCode", pRequestCode));
                        cn.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                RequestList req = new RequestList();
                                req.RequestListCode = dr["RequestListCode"].ToString().ToInt();
                                req.Request = new Requests();
                                req.Request.RequestCode = dr["RequestCode"].ToString().ToInt();
                                req.Request.FillInfo();
                                req.Module = dr["Module"].ToString();
                                req.Action = dr["Action"].ToString();
                                req.NewValues = this.ToListValues(dr["NewValues"].ToString());
                                req.OldValues = this.ToListValues(dr["OldValues"].ToString());
                                req.WhereValues = this.ToListValues(dr["WhereValues"].ToString());
                                req.AffectedTable = dr["AffectedTable"].ToString();
                                req.Remarks = dr["Remarks"].ToString();
                                req.Status = dr["Status"].ToString();
                                req.ApprovedBy = dr["ApprovedBy"].ToString();
                                if (dr["ApprovedOn"].ToString() == "") { req.ApprovedOn = null; }
                                else { req.ApprovedOn = Convert.ToDateTime(dr["ApprovedOn"]); }

                                requests.Add(req);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CTBC.Logs.Write("Get Request List", ex.Message, "Request List");
            }
            return requests;
        }

        public List<RequestList> GetRequestList(string pModule, string pCreatedBy)
        {
            List<RequestList> reqList = new List<RequestList>();
            try
            {
                string strSQL = "";
                strSQL += " SELECT * ";
                strSQL += " FROM RequestList";
                strSQL += " WHERE";
                strSQL += " RequestCode IN(SELECT RequestCode FROM Requests WHERE CreatedBy = @pCreatedBy) AND";
                strSQL += " Module = @pModule";

                using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
                {
                    using (SqlCommand cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = strSQL;
                        cmd.Parameters.Add(new SqlParameter("@pModule", pModule));
                        cmd.Parameters.Add(new SqlParameter("@pCreatedBy", pCreatedBy));

                        cn.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                RequestList req = new RequestList();
                                req.RequestListCode = dr["RequestListCode"].ToString().ToInt();
                                req.Request = new Requests();
                                req.Request.RequestCode = dr["RequestCode"].ToString().ToInt();
                                req.Request.Fill();
                                req.Module = dr["Module"].ToString();
                                req.Action = dr["Action"].ToString();
                                req.NewValues = this.ToListValues(dr["NewValues"].ToString());
                                req.OldValues = this.ToListValues(dr["OldValues"].ToString());
                                req.WhereValues = this.ToListValues(dr["WhereValues"].ToString());
                                req.AffectedTable = dr["AffectedTable"].ToString();
                                req.Remarks = dr["Remarks"].ToString();
                                req.Status = dr["Status"].ToString();
                                req.ApprovedBy = dr["ApprovedBy"].ToString();
                                if (dr["ApprovedOn"].ToString() == "") { req.ApprovedOn = null; }
                                else { req.ApprovedOn = Convert.ToDateTime(dr["ApprovedOn"]); }

                                reqList.Add(req);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CTBC.Logs.Write("Get Request List", ex.Message, "Request List");
            }
            return reqList;
        }

        public List<RequestList> GetRequestList(string pModule, int[] pRequestCodes)
        {
            List<RequestList> requests = new List<RequestList>();
            try
            {
                string strSQL = "";
                strSQL += "SELECT ";
                strSQL += "* ";
                strSQL += "FROM RequestList ";
                strSQL += "WHERE ";
                strSQL += "Module = @pModule AND ";
                strSQL += "RequestCode IN(";
                int ctr = 0;
                foreach (int num in pRequestCodes)
                {
                    ctr++;
                    strSQL += "@p" + num.ToString() + (ctr == pRequestCodes.Length ? "" : ",");
                }
                strSQL += ")";

                using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
                {
                    using (SqlCommand cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = strSQL;
                        cmd.Parameters.Add(new SqlParameter("@pModule", pModule));
                        foreach (int num in pRequestCodes)
                        {
                            cmd.Parameters.Add(new SqlParameter("@p" + num.ToString(), num.ToString()));
                        }

                        cn.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                RequestList req = new RequestList();
                                req.RequestListCode = dr["RequestListCode"].ToString().ToInt();
                                req.Request = new Requests();
                                req.Request.RequestCode = dr["RequestCode"].ToString().ToInt();
                                req.Request.Fill();
                                req.Module = dr["Module"].ToString();
                                req.Action = dr["Action"].ToString();
                                req.NewValues = this.ToListValues(dr["NewValues"].ToString());
                                req.OldValues = this.ToListValues(dr["OldValues"].ToString());
                                req.WhereValues = this.ToListValues(dr["WhereValues"].ToString());
                                req.AffectedTable = dr["AffectedTable"].ToString();
                                req.Remarks = dr["Remarks"].ToString();
                                req.Status = dr["Status"].ToString();
                                req.ApprovedBy = dr["ApprovedBy"].ToString();
                                if (dr["ApprovedOn"].ToString() == "") { req.ApprovedOn = null; }
                                else { req.ApprovedOn = Convert.ToDateTime(dr["ApprovedOn"]); }

                                requests.Add(req);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CTBC.Logs.Write("Get Request List", ex.Message, "Request List");
            }
            return requests;
        }

        public int Cancel()
        {
            int intReturn = 0;

            try
            {
                string strSQL1 = "";
                strSQL1 += " UPDATE RequestList";
                strSQL1 += " SET Status = '3'";
                strSQL1 += " WHERE RequestListCode = @pRequestListCode";

                string strSQL2 = "";
                strSQL2 += string.Format(" UPDATE {0}", this.AffectedTable);
                strSQL2 += string.Format(" SET Status = '{0}'", this.Action == ((int)RequestList.Actions.INSERT).ToString() ? "3" : "1");
                strSQL2 += string.Format(" WHERE {0} = @pCode", this.WhereValues.First().FieldName);

                using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
                {
                    using (SqlCommand cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = strSQL1;
                        cmd.Parameters.Add(new SqlParameter("@pRequestListCode", this.RequestListCode));

                        cn.Open();
                        intReturn += cmd.ExecuteNonQuery();

                        cmd.CommandText = strSQL2;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter("@pCode", this.WhereValues.First().Value));
                        intReturn += cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                CTBC.Logs.Write("Cancel", ex.Message, "Request List");
            }
            return intReturn;
        }

        public int CancelAllByModule()
        {
            int intReturn = 0;
            try
            {
                string strSQL1 = "";
                strSQL1 += " UPDATE RequestList";
                strSQL1 += " SET Status = '3'";
                strSQL1 += " WHERE";
                strSQL1 += " AffectedTable = @pAffectedTable";
                strSQL1 += " AND Status = '0'";
                strSQL1 += " AND RequestCode IN(Select RequestCode FROM Requests WHERE CreatedBy = @pCreatedBy)";

                using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
                {
                    using (SqlCommand cmd = cn.CreateCommand())
                    {
                        cn.Open();
                        foreach (RequestList req in new RequestList().GetRequestList(this.Module, this.Request.CreatedBy).Where(x => x.Status == "0"))
                        {
                            string strSQL = "";
                            strSQL += string.Format(" UPDATE {0}", req.AffectedTable);
                            strSQL += string.Format(" SET Status = '{0}'", req.Action == ((int)RequestList.Actions.INSERT).ToString() ? "3" : "0");
                            strSQL += string.Format(" WHERE {0} = @pCode", req.WhereValues.First().FieldName);

                            cmd.CommandText = strSQL;
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter("@pCode", req.WhereValues.First().Value));

                            intReturn += cmd.ExecuteNonQuery();
                        }

                        cmd.CommandText = strSQL1;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter("@pAffectedTable", this.AffectedTable));
                        cmd.Parameters.Add(new SqlParameter("@pCreatedBy", this.Request.CreatedBy));

                        intReturn += cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                CTBC.Logs.Write("Cancel All by Module", ex.Message, "Request List");
            }
            return intReturn;
        }

        public List<string> GetReqModules()
        {
            List<string> lstMods = new List<string>();
            try
            {
                string strSQL = "";
                strSQL += " SELECT DISTINCT(Module) FROM RequestList";

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
                                lstMods.Add(dr["Module"].ToString());
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                CTBC.Logs.Write("Get Request Module", ex.Message, "Request List");
            }
            return lstMods;
        }

        public List<RequestList> RequestListDetails()
        {
            List<RequestList> list = new List<RequestList>();
            string strSQL = "SELECT * FROM REQUESTs R ";
            strSQL += "INNER JOIN RequestList RL ON (RL.RequestCode = R.RequestCode) ";
            strSQL += "WHERE RL.Status = 1 ";
            strSQL += "ORDER BY RL.RequestListCode ";

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
                            list.Add(new RequestList
                            {
                                NewValues = ToListValues(dr["NewValues"].ToString()),
                                WhereValues = ToListValues(dr["WhereValues"].ToString()),
                                RequestListCode = dr["RequestListCode"].ToString().ToInt(),
                                Remarks = dr["Remarks"].ToString(),
                                AffectedTable = dr["AffectedTable"].ToString(),
                                Action = dr["Action"].ToString(),
                                Request = (new Models.Requests
                                {
                                    RequestCode = dr["RequestCode"].ToString().ToInt()
                                })
                            });
                        }
                    }
                }
            }
            List<RequestList> listadd = new List<RequestList>();
            bool addtolist = false;

            foreach (var item in list)
            {
                //var desc = "";     
                // foreach (var cvalue in item.WhereValues)
                // {
                //  if (cvalue.FieldName == "CMSCode")
                //  {
                //   if (item.Action == "2")
                //   {
                //    desc = "CMSCode[" + cvalue.Value.ToString() + "] " + GetCMSCodeDescription(cvalue.Value.ToString().ToInt());
                //   }
                //   else
                //   {
                //    desc = "N/A"; //+ GetCMSCodeDescription(cvalue.Value.ToString().ToInt());
                //   }
                //  }
                //}

                foreach (RequestList.Values subitem in item.NewValues)
                {
                    if (subitem.FieldName == "ModifiedOn" || subitem.FieldName == "ModifiedBy")
                    {
                        addtolist = addtolist ? true : false;
                    }
                    else { addtolist = true; }
                }
                if (addtolist)
                {
                    //var remarks = string.Empty;
                    //var rem = string.Empty;
                    //rem = "Request to ";
                    //rem += item.Action == "2" ? "add" : (item.Action == "1") ? "update" : "delete";
                    //rem += " Service " + servicename;

                    //remarks = (item.AffectedTable == "ServiceOption") ? rem : item.Remarks;

                    listadd.Add(new RequestList
                    {
                        RequestListCode = item.RequestListCode,
                        Remarks = item.Remarks,
                        ConcernedCMSCode = item.Remarks,//desc,
                        NewValues = item.NewValues,
                        Request = (new Models.Requests
                        {
                            RequestCode = item.Request.RequestCode,
                        })
                    });
                    addtolist = false;
                }
            }
            return listadd;
        }

        public string GetCMSCodeDescription(int pCMSCode)
        {
            string strReturn = "";
            try
            {
                using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
                {
                    using (SqlCommand cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT Description From CMSCodes WHERE CMSCode = @pCMSCode";
                        cmd.Parameters.Add(new SqlParameter("@pCMSCode", pCMSCode));
                        cn.Open();
                        strReturn = cmd.ExecuteScalar().ToString();
                    }
                }
            }
            catch (Exception e)
            {
                CTBC.Logs.Write("Get CMS Code", e.Message, "Request List");
            }
            return strReturn;
        }

        public List<Enrollment.RequestListDetails> RequestListCodeDetails()
        {
            List<Enrollment.RequestListDetails> list = new List<Enrollment.RequestListDetails>();
            string strSQL = "SELECT Distinct(R.RequestCode), R.CreatedBy FROM REQUESTs R ";
            strSQL += "INNER JOIN RequestList RL ON (RL.RequestCode = R.RequestCode) ";
            strSQL += "WHERE RL.Status = 1 ";
            try
            {
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
                                list.Add(new Enrollment.RequestListDetails
                                {
                                    RequestCode = dr["RequestCode"].ToString().ToInt(),
                                    ConcernedCMSCode = GetCMSCodeandDescription(dr["RequestCode"].ToString().ToInt()),
                                    RequestsDetails = (new Enrollment.RequestsDetails
                                    {
                                        CreatedBy = dr["CreatedBy"].ToString()
                                    })
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                CTBC.Logs.Write("Request List Code Details", e.Message, "Request List");
            }
            return list;
        }

        public string GetCMSCodeandDescription(int pRequestCode)
        {
            string strReturn = "";
            List<RequestList> list = new List<RequestList>();
            using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
            {
                using (SqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "SELECT WhereValues, Action, AffectedTable, NewValues FROM RequestList WHERE RequestCode = @pRequestCode and Status = 1";
                    cmd.Parameters.Add(new SqlParameter("@pRequestCode", pRequestCode));
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new RequestList
                            {
                                WhereValues = ToListValues(dr["WhereValues"].ToString()),
                                NewValues = ToListValues(dr["NewValues"].ToString()),
                                AffectedTable = dr["AffectedTable"].ToString(),
                                Action = dr["Action"].ToString()
                            });
                        }
                    }
                }
            }

            string Desc = "";
            foreach (var item in list)
            {
                bool alreadyExists = item.NewValues.Any(x => x.FieldName == "Description");
                if (item.AffectedTable == "CMSCodes")
                {
                    if (alreadyExists ? true : false)
                    {
                        foreach (var nitem in item.NewValues)
                        {
                            if (item.Action != "2")
                            {
                                if (nitem.FieldName == "Description")
                                {
                                    Desc = nitem.Value.ToString();
                                }
                            }
                        }
                    }
                    foreach (var subitem in item.WhereValues)
                    {
                        if (subitem.FieldName == "CMSCode")
                        {
                            if (item.Action == "2")
                            {
                                strReturn = "New";
                            }
                            else
                            {
                                Desc = alreadyExists ? Desc : GetCMSCodeDescription(subitem.Value.ToString().ToInt());
                                strReturn = "[" + subitem.Value.ToString() + "]-" + Desc;
                            }
                        }
                    }
                }
            }

            return strReturn;
        }

        public List<Values> GenerateData(object pObject)
        {
            List<Values> list = new List<Values>();
            foreach (var item in pObject.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                if (item.Name != "ServiceOptionDetails")
                {
                    list.Add(new Values
                    {
                        FieldName = item.Name,
                        Value = ConvertValues(item.GetValue(pObject, null), item.Name, true)
                    });
                }
            }

            if (pObject.GetType().Name == "AvailedDetails")
            {
                if (list.Count(x => x.FieldName == "IsActive") > 0)
                {
                    list.Remove(list.Single(x => x.FieldName == "IsActive"));
                }
            }

            return list;
        }

        public List<Values> GenerateWhereValues(object pObject)
        {
            List<Values> list = new List<Values>();
            foreach (var item in pObject.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                if (item.Name == "CMSCode" || item.Name == "ServiceID")
                {
                    list.Add(new Values
                    {
                        FieldName = item.Name,
                        Value = item.GetValue(pObject, null)
                    });
                }

            }
            return list;
        }

        public int InsertRequest(Enrollment.RequestsDetails pDetails)
        {
            int intReturn = 0;
            string strSQL = "INSERT INTO ";
            strSQL += "Requests ";
            strSQL += "VALUES ( ";
            strSQL += "@pCreatedBy ";
            strSQL += ",@pCreatedOn ";
            strSQL += ",null ";
            strSQL += ",null ";
            strSQL += ",@pIPAddress ";
            strSQL += ",@pAssignedApprover) ";
            strSQL += "SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY] ";

            using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
            {
                using (SqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = strSQL;
                    cmd.Parameters.Add(new SqlParameter("@pCreatedBy", pDetails.CreatedBy));
                    cmd.Parameters.Add(new SqlParameter("@pCreatedOn", pDetails.CreatedOn));
                    cmd.Parameters.Add(new SqlParameter("@pIPAddress", pDetails.IPAddress));
                    cmd.Parameters.Add(new SqlParameter("@pAssignedApprover", pDetails.AssignedApprover));
                    cn.Open();
                    intReturn = cmd.ExecuteScalar().ToString().ToInt();
                }
            }
            return intReturn;
        }

        public int InsertRequestList(object pDetails, Enrollment.RequestListDetails pRequestListDetails, bool IsDetailsForInsert, int pCMSCode)
        {
            int intReturn = 0;
            string strSQL2 = "INSERT INTO ";
            strSQL2 += "RequestList ";
            strSQL2 += "VALUES ( ";
            strSQL2 += "@pRequestCode ";
            strSQL2 += ",@pModule ";
            strSQL2 += ",@pAction ";
            strSQL2 += ",@pNewValues ";
            strSQL2 += ",@pOldValues ";
            strSQL2 += ",@pWhereValues ";
            strSQL2 += ",@pAffectedTable ";
            strSQL2 += ",@pRemarks ";
            strSQL2 += ",@pStatus ";
            strSQL2 += ",null ";
            strSQL2 += ",null) ";

            string strSQL = "UPDATE ";
            strSQL += "RequestList ";
            strSQL += "SET  ";
            strSQL += "RequestCode = @pRequestCode ";
            strSQL += ",Module = @pModule ";
            strSQL += ",Action = @pAction ";
            strSQL += ",NewValues = @pNewValues ";
            strSQL += ",OldValues = @pOldValues ";
            strSQL += ",WhereValues = @pWhereValues ";
            strSQL += ",AffectedTable = @pAffectedTable ";
            strSQL += ",Remarks = @pRemarks ";
            strSQL += ",Status = @pStatus ";
            strSQL += "WHERE RequestListCode = @pRequestListCode ";

            SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring);
            cn.Open();
            SqlCommand cmd = cn.CreateCommand();
            cmd.Transaction = cn.BeginTransaction();
            SqlTransaction trans = cmd.Transaction;

            try
            {
                var affectedtable = pRequestListDetails.AffectedTable;
                int serviceid = 0;
                bool hasRequest = false;
                if (affectedtable == "ServicesAvailment")
                {
                    var data = (ServiceOptions.AvailedDetails)pDetails;
                    serviceid = data.ServiceID;
                }
                NewValues = (pRequestListDetails.Action == "1") ? ReturnNewValues(affectedtable, pCMSCode, pDetails) : GenerateData(pDetails);
                OldValues = (IsDetailsForInsert) ? GenerateData(pDetails) : affectedtable == "CMSCode" ? ReturnOldValues(affectedtable, pCMSCode, 0) : ReturnOldValues(affectedtable, pCMSCode, serviceid);
                WhereValues = GenerateWhereValues(pDetails);

                List<RequestList> reqlist = GetRequestListbyRequestCodeForApproval(pRequestListDetails.RequestCode);
                int preqlistcode = 0;
                foreach (var item in reqlist)
                {
                    if (item.AffectedTable == "ServicesAvailment")
                    {
                        foreach (var witem in item.WhereValues)
                        {
                            if (witem.FieldName == "ServiceID")
                            {
                                if (witem.Value.ToInt() == serviceid)
                                {
                                    preqlistcode = item.RequestListCode;
                                    hasRequest = true;
                                }
                            }
                        }
                    }
                }
                if (hasRequest)
                {
                    cmd.CommandText = strSQL;
                    cmd.Parameters.Add(new SqlParameter("@pRequestListCode", preqlistcode));
                }
                else
                {
                    cmd.CommandText = strSQL2;
                }

                var beAdd = "";
                if (affectedtable == "CMSCodes")
                {
                    CMSCode.Details cdetails = (CMSCode.Details)pDetails;
                    beAdd = (pRequestListDetails.Action == "2") ? " " + cdetails.Description : "[" + cdetails.CMSCode + "] " + cdetails.Description;
                }
                cmd.Parameters.Add(new SqlParameter("@pRequestCode", pRequestListDetails.RequestCode));
                cmd.Parameters.Add(new SqlParameter("@pModule", pRequestListDetails.Module));
                cmd.Parameters.Add(new SqlParameter("@pAction", pRequestListDetails.Action));
                cmd.Parameters.Add(new SqlParameter("@pNewValues", ToStringValues(this.NewValues)));
                cmd.Parameters.Add(new SqlParameter("@pOldValues", ToStringValues(this.OldValues)));
                cmd.Parameters.Add(new SqlParameter("@pWhereValues", ToStringValues(this.WhereValues)));
                cmd.Parameters.Add(new SqlParameter("@pAffectedTable", pRequestListDetails.AffectedTable));
                cmd.Parameters.Add(new SqlParameter("@pRemarks", pRequestListDetails.Remarks + beAdd));
                cmd.Parameters.Add(new SqlParameter("@pStatus", pRequestListDetails.Status));
                cmd.ExecuteNonQuery();

                //bool IsForInsert = false;
                if (this.NewValues.Count == 0)
                {
                    trans.Rollback();
                }
                //else if (this.NewValues.Count == 1 || this.NewValues.Count == 2)
                //{
                // if (pRequestListDetails.AffectedTable == "CMSCodes")
                // {
                //  foreach (var item in NewValues)
                //  {
                //   if (item.FieldName != "ModifiedOn" && item.FieldName != "ModifiedBy")
                //   {
                //    IsForInsert = true;
                //   }
                //   else { IsForInsert = IsForInsert? true:false; }
                //  }
                //  if (IsForInsert) { trans.Commit(); } else { trans.Rollback(); }
                // }
                // else { trans.Commit(); }
                //}
                else
                {
                    trans.Commit();
                }
            }
            catch (Exception e)
            {
                trans.Rollback();
                CTBC.Logs.Write("InsertRequestList", e.Message, "RequestList");
            }
            finally
            {
                trans.Dispose();
                trans = null;
                cmd.Dispose();
                cmd = null;
                cn.Close();
                cn.Dispose();
                cn = null;
            }
            return intReturn;
        }

        public bool IsForInsert(object pDetails, Enrollment.RequestListDetails pRequestListDetails, int pCMSCode)
        {
            bool forinsert = false;
            this.NewValues = (pRequestListDetails.Action == "1") ? ReturnNewValues(pRequestListDetails.AffectedTable, pCMSCode, pDetails) : GenerateData(pDetails);
            foreach (var item in NewValues)
            {
                if (item.FieldName != "ModifiedBy" && item.FieldName != "ModifiedOn")
                {
                    forinsert = true;
                }
                else { forinsert = forinsert ? true : false; }
            }
            return forinsert;
        }

        public bool IsServiceForinsert(ServiceOptions.AvailedDetails details, Enrollment.RequestListDetails pRequestListDetails, bool pIsForInsert, int pCMSCode)
        {
            bool forInsert = false;
            Models.ServiceOptions.AvailedDetails a_details = new ServiceOptions.AvailedDetails();
            //Models.ServiceOptions.ServiceOptionDetails s_details = a_details.ServiceOptionDetails;
            a_details.CMSCode = pCMSCode;

            if (pIsForInsert) { a_details.EnrolledBy = details.EnrolledBy; }
            if (pIsForInsert) { a_details.EnrolledOn = details.EnrolledOn; }
            a_details.ModifiedBy = details.ModifiedBy;
            a_details.ModifiedOn = details.ModifiedOn;
            a_details.ServiceID = details.ServiceID;
            a_details.RMID = details.RMID;

            a_details.ServiceName = details.ServiceName;
            a_details.MotherRequiredADB = details.MotherRequiredADB;
            a_details.SubRequiredADB = details.SubRequiredADB;
            a_details.MinNumberEmployee = details.MinNumberEmployee;
            a_details.Status = details.Status;
            pRequestListDetails.AffectedTable = "ServicesAvailment";
            var rem = (pIsForInsert) ? "add" : (pRequestListDetails.Action == "1") ? "update" : "delete";
            pRequestListDetails.Remarks = "Request to " + rem + " Services by ";
            forInsert = new Models.RequestList().IsForInsert(a_details, pRequestListDetails, pCMSCode);

            //s_details.AccountNoServiceType = details.ServiceOptionDetails.AccountNoServiceType;
            //s_details.CMSCode = pCMSCode;
            //s_details.MaxFreeTransaction = details.ServiceOptionDetails.MaxFreeTransaction;
            //s_details.MaxWithdrawalPaidByEmployer = details.ServiceOptionDetails.MaxWithdrawalPaidByEmployer;
            //s_details.MinNumberEmployee = details.ServiceOptionDetails.MinNumberEmployee;
            //s_details.MotherRequiredADB = details.ServiceOptionDetails.MotherRequiredADB.ToString().ToDecimal();
            //s_details.PayrollFrequency = details.ServiceOptionDetails.PayrollFrequency;
            //s_details.Remarks = details.ServiceOptionDetails.Remarks;
            //s_details.ServiceOptionID = details.ServiceOptionDetails.ServiceOptionID;
            //s_details.SubRequiredADB = details.ServiceOptionDetails.SubRequiredADB.ToString().ToDecimal();
            //s_details.WithdrawalFeeAccountNo = details.ServiceOptionDetails.WithdrawalFeeAccountNo;
            //s_details.WithdrawalFeePerTransaction = details.ServiceOptionDetails.WithdrawalFeePerTransaction.ToString().ToDecimal();
            //s_details.ServiceID = a_details.ServiceID;
            //s_details.Status = details.ServiceOptionDetails.Status;
            //pRequestListDetails.AffectedTable = "ServiceOption";
            //var rema = (pIsForInsert) ? "add" : (pRequestListDetails.Action == "1") ? "update" : "delete";
            //pRequestListDetails.Remarks = "Request to " + rema + " Service Details by ";
            //forInsert = forInsert? true : new Models.RequestList().IsForInsert(s_details, pRequestListDetails, pCMSCode);

            //new Models.RequestList().InsertRequestList(s_details, pRequestListDetails, pIsForInsert, details.CMSCode);
            return forInsert;
        }

        public List<Values> ReturnOldValues(string pAffectedTable, int pCMSCode, int pServiceID)
        {
            List<Values> list = new List<Values>();
            string strSQL = "SELECT * ";
            strSQL += "FROM ";
            strSQL += " " + pAffectedTable + " ";
            strSQL += "WHERE CMSCode = @pCMSCode ";
            strSQL += (pAffectedTable == "CMSCodes" ? "" : "AND ServiceID = @pServiceID");
            try
            {
                using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
                {
                    using (SqlCommand cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = strSQL;
                        cmd.Parameters.Add(new SqlParameter("@pCMSCode", pCMSCode));
                        if (pAffectedTable == "ServicesAvailment")
                        {
                            cmd.Parameters.Add(new SqlParameter("@pServiceID", pServiceID));
                        }
                        cn.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                for (var i = 0; i < dr.FieldCount; i++)
                                {
                                    list.Add(new Values
                                    {
                                        FieldName = dr.GetName(i),
                                        Value = dr.GetValue(i)
                                    });
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception e)
            {
                CTBC.Logs.Write("Return Old Values", e.Message, "Request List");
            }
            return list;
        }

        public List<Values> ReturnNewValues(string pAffectedTable, int pCMSCode, object objDetails)
        {
            List<Values> list3 = new List<Values>();
            List<Values> list = new List<Values>();
            string strSQL = "";
            if (pAffectedTable != "ServicesAvailment")
            {
                strSQL += "SELECT * ";
                strSQL += "FROM ";
                strSQL += "" + pAffectedTable + " ";
                strSQL += "WHERE ";
                strSQL += "CMSCode = @pCMSCode ";
            }
            else
            {
                strSQL += "SELECT * ";
                strSQL += "FROM ";
                strSQL += "" + pAffectedTable + " ";
                strSQL += "WHERE ";
                strSQL += "CMSCode = @pCMSCode ";
                strSQL += "AND ServiceID = @pServiceID ";
            }


            try
            {
                using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
                {
                    using (SqlCommand cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = strSQL;
                        if (pAffectedTable != "ServicesAvailment")
                        {
                            cmd.Parameters.Add(new SqlParameter("@pCMSCode", pCMSCode));
                        }
                        else
                        {
                            ServiceOptions.AvailedDetails serv = (ServiceOptions.AvailedDetails)objDetails;
                            cmd.Parameters.Add(new SqlParameter("@pCMSCode", pCMSCode));
                            cmd.Parameters.Add(new SqlParameter("@pServiceID", serv.ServiceID));
                        }
                        cn.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                for (var i = 0; i < dr.FieldCount; i++)
                                {
                                    list.Add(new Values
                                    {
                                        FieldName = dr.GetName(i),
                                        Value = ConvertValues(dr.GetValue(i).ToString(), dr.GetName(i), true)
                                    });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                CTBC.Logs.Write("Return New Values", e.Message, "Request List");
            }

            List<Values> list2 = new List<Values>();
            foreach (var j in objDetails.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                list2.Add(new Values
                {
                    FieldName = j.Name,
                    Value = ConvertValues((j.GetValue(objDetails, null) != null) ? j.GetValue(objDetails, null) : "", j.Name, true)
                    //Value = ConvertValues((j.GetValue(objDetails, null) != null) ? j.GetValue(objDetails, null).ToString() : "", j.Name, true)
                });
            }
            foreach (var item2 in list2)
            {
                foreach (var item in list)
                {
                    if ((item2.FieldName != "CreatedOn" && item2.FieldName != "EnrolledOn" && item2.FieldName != "EnrolledBy" && item2.FieldName != "CreatedBy" && item2.FieldName != "ServiceID" && item2.FieldName != "ServiceOptionID") ? item.FieldName == item2.FieldName : item2.Value == null)
                    {
                        if ((item2.Value != null) ? item.Value.ToString() != item2.Value.ToString() : false)
                        {
                            list3.Add(new Values
                            {
                                FieldName = item2.FieldName,
                                Value = ConvertValues(item2.Value.ToString(), item2.FieldName, true)
                            });
                        }
                    }
                }
            }
            if (list.Count == 0 && list2.Count != 0) { this.Action = "2"; }
            else if (list.Count != 0 && list2.Count == 0) { this.Action = "0"; }
            else if (list.Count == 0 && list2.Count == 0) { this.Action = "1"; }

            return (list.Count != 0 && list2.Count != 0) ? list3 : list2;
        }

        public List<RequestList> GetforApproveByRequesListCode(int pRequestListCode)
        {
            List<RequestList> list = new List<RequestList>();
            string strSQL = "SELECT * FROM ";
            strSQL += "RequestList RL ";
            strSQL += " INNER JOIN Requests R ON (RL.RequestCode = R.RequestCode) ";
            strSQL += "WHERE ";
            strSQL += "RL.RequestCode = @pRequestListCode ";

            using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
            {
                using (SqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = strSQL;
                    cmd.Parameters.Add(new SqlParameter("@pRequestListCode", @pRequestListCode));
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Actions action = (Actions)Enum.Parse(typeof(Actions), dr["Action"].ToString());
                            list.Add(new RequestList
                            {
                                RequestListCode = dr["RequestListCode"].ToString().ToInt(),
                                Action = action.ToString(),
                                AffectedTable = dr["AffectedTable"].ToString(),
                                ApprovedBy = dr["ApprovedBy"].ToString(),
                                ApprovedOn = dr["ApprovedOn"].ToString().ToDateTime(),
                                Module = dr["Module"].ToString(),
                                NewValues = ToListValues(dr["NewValues"].ToString()),
                                OldValues = ToListValues(dr["OldValues"].ToString()),
                                Remarks = dr["Remarks"].ToString(),
                                Status = dr["Status"].ToString(),
                                WhereValues = ToListValues(dr["WhereValues"].ToString())
                            });
                        }
                    }
                }
            }
            return list;
        }

        public int UpdateRequest(Enrollment.RequestsDetails pDetails)
        {
            int intReturn = 0;
            string strSQL = "UPDATE ";
            strSQL += "Requests ";
            strSQL += "SET ";
            strSQL += "ModifiedBy = @pModifiedBy ";
            strSQL += ",ModifiedOn = @pModifiedOn ";
            strSQL += "WHERE ";
            strSQL += "RequestCode = @pRequestCode ";
            try
            {
                using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
                {
                    using (SqlCommand cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = strSQL;
                        cmd.Parameters.Add(new SqlParameter("@pModifiedBy", pDetails.ModifiedBy));
                        cmd.Parameters.Add(new SqlParameter("@pModifiedOn", pDetails.ModifiedOn));
                        cmd.Parameters.Add(new SqlParameter("@pRequestCode", pDetails.RequestCode));
                        cn.Open();
                        intReturn = cmd.ExecuteScalar().ToString().ToInt();
                    }
                }
            }
            catch (Exception e)
            {
                CTBC.Logs.Write("Update Request", e.Message, "Request List");
            }

            return intReturn;
        }

        public int UpdateRequestList(object pDetails, Enrollment.RequestListDetails pRequestListDetails)
        {
            int intReturn = 0;
            string strSQL2 = "INSERT INTO ";
            strSQL2 += "RequestList ";
            strSQL2 += "VALUES ( ";
            strSQL2 += "@pRequestCode ";
            strSQL2 += ",@pModule ";
            strSQL2 += ",@pAction ";
            strSQL2 += ",@pNewValues ";
            strSQL2 += ",@pOldValues ";
            strSQL2 += ",@pWhereValues ";
            strSQL2 += ",@pAffectedTable ";
            strSQL2 += ",@pRemarks ";
            strSQL2 += ",@pStatus ";
            strSQL2 += ",@pApprovedBy ";
            strSQL2 += ",null) ";

            SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring);
            cn.Open();
            SqlCommand cmd = cn.CreateCommand();
            cmd.Transaction = cn.BeginTransaction();
            SqlTransaction trans = cmd.Transaction;

            try
            {
                NewValues = GenerateData(pDetails);
                OldValues = GenerateData(pDetails);
                WhereValues = GenerateWhereValues(pDetails);
                //equestListDetails.RequestCode = InsertRequest(pRequestListDetails.RequestsDetails);
                cmd.CommandText = strSQL2;
                cmd.Parameters.Add(new SqlParameter("@pRequestCode", pRequestListDetails.RequestCode));
                cmd.Parameters.Add(new SqlParameter("@pModule", pRequestListDetails.Module));
                cmd.Parameters.Add(new SqlParameter("@pAction", pRequestListDetails.Action));
                cmd.Parameters.Add(new SqlParameter("@pNewValues", ToStringValues(this.NewValues)));
                cmd.Parameters.Add(new SqlParameter("@pOldValues", ToStringValues(this.OldValues)));
                cmd.Parameters.Add(new SqlParameter("@pWhereValues", ToStringValues(this.WhereValues)));
                cmd.Parameters.Add(new SqlParameter("@pAffectedTable", pRequestListDetails.AffectedTable));
                cmd.Parameters.Add(new SqlParameter("@pRemarks", pRequestListDetails.Remarks + pRequestListDetails.RequestsDetails.CreatedBy));
                cmd.Parameters.Add(new SqlParameter("@pStatus", pRequestListDetails.Status));
                cmd.Parameters.Add(new SqlParameter("@pApprovedBy", pRequestListDetails.ApprovedBy ?? DBNull.Value.ToString()));
                cmd.ExecuteNonQuery();

                trans.Commit();
            }
            catch (Exception e)
            {
                trans.Rollback();
                CTBC.Logs.Write("UpdateRequestList", e.Message, "RequestList");
            }
            finally
            {
                trans.Dispose();
                trans = null;
                cmd.Dispose();
                cmd = null;
                cn.Close();
                cn.Dispose();
                cn = null;
            }
            return intReturn;
        }

        public int ApproveByRequestCode(int pRequestCode)
        {
            int intReturn = 0;
            string strSQl = "UPDATE RequestList ";
            strSQl += "SET Status = 2 ";
            strSQl += "WHERE RequestCode = @pRequestCode ";

            using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
            {
                using (SqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = strSQl;
                    cmd.Parameters.Add(new SqlParameter("@pRequestCode", pRequestCode));
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            return intReturn;
        }

        public List<RequestList> GetRequestListbyRequestCodeForApproval(int pRequestCode)
        {
            int intRequestListCode = 0;
            List<RequestList> list = new List<RequestList>();
            string strSQL = "SELECT * FROM RequestList ";
            strSQL += "WHERE RequestCode = @pRequestCode and Status = 1";
            try
            {
                using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
                {
                    using (SqlCommand cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = strSQL;
                        cmd.Parameters.Add(new SqlParameter("@pRequestCode", pRequestCode));
                        cn.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                intRequestListCode = dr["RequestListCode"].ToString().ToInt();
                                list.Add(new RequestList
                                {
                                    Action = dr["Action"].ToString(),
                                    AffectedTable = dr["AffectedTable"].ToString(),
                                    ApprovedBy = dr["ApprovedBy"].ToString(),
                                    ApprovedOn = dr["ApprovedOn"].ToString().ToDateTime(),
                                    Module = dr["Module"].ToString(),
                                    NewValues = ToListValues(dr["NewValues"].ToString()),
                                    OldValues = ToListValues(dr["OldValues"].ToString()),
                                    WhereValues = ToListValues(dr["WhereValues"].ToString()),
                                    Remarks = dr["Remarks"].ToString(),
                                    RequestListCode = dr["RequestListCode"].ToString().ToInt(),
                                    Status = dr["Status"].ToString(),
                                    Request = new Requests().GetRequestsByRequestListCode(intRequestListCode)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                CTBC.Logs.Write("Get Request List By Request Code for Approval", e.Message, "Request List");
            }
            return list;
        }

        public int ProcessRequest(int pRequestCode, bool pIsApproved, string pApprover, bool pIsForApprover, bool pIsStatus, string pMPersonId, string pSPersonId)
        {
            int intReturn = 0;
            string strSQl = "";
            bool IsForInsert = false;
            int seCMSCode = 0;
            string seDescription = "";
            string seBranchName = "";

            List<RequestList> request = GetRequestListbyRequestCodeForApproval(pRequestCode);

            foreach (var item in request)
            {
                strSQl = sqlBuild(item);
                try
                {
                    using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
                    {
                        using (SqlCommand cmd = cn.CreateCommand())
                        {
                            cmd.CommandText = strSQl;
                            RequestList.Actions action = (RequestList.Actions)Enum.Parse(typeof(RequestList.Actions), item.Action);
                            if (item.AffectedTable == "ServicesAvailment" || item.AffectedTable == "ServiceOption")
                            {
                                if (action == Actions.INSERT)
                                {
                                    //INSERT NEW SERVICE HERE
                                    foreach (RequestList.Values sitem in item.NewValues)
                                    {
                                        if (item.AffectedTable == "ServicesAvailment")
                                        {
                                            if (sitem.FieldName != "ServiceName")
                                            {
                                                cmd.Parameters.Add(new SqlParameter("@p" + sitem.FieldName, sitem.FieldName != "Status" ? sitem.Value : pIsApproved ? (int)SharedFunctions.Status.Approved : (int)SharedFunctions.Status.Disapproved));
                                            }
                                        }
                                        else if (item.AffectedTable == "ServiceOption")
                                        {
                                            if (sitem.FieldName != "ServiceOptionID")
                                            {
                                                cmd.Parameters.Add(new SqlParameter("@p" + sitem.FieldName, sitem.FieldName != "Status" ? sitem.Value : pIsApproved ? (int)SharedFunctions.Status.Approved : (int)SharedFunctions.Status.Disapproved));
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (pIsApproved)
                                    {
                                        if (action == Actions.UPDATE)
                                        {
                                            //UPDATE NEW SERVICES HERE
                                            foreach (RequestList.Values sitem in item.NewValues)
                                            {
                                                if (item.AffectedTable == "ServicesAvailment")
                                                {
                                                    if (sitem.FieldName != "ServiceID" && sitem.FieldName != "CMSCode")
                                                    {
                                                        cmd.Parameters.Add(new SqlParameter("@p" + sitem.FieldName, sitem.FieldName != "Status" ? sitem.Value : pIsApproved ? (int)SharedFunctions.Status.Approved : (int)SharedFunctions.Status.Disapproved));
                                                    }
                                                }
                                                else if (item.AffectedTable == "ServiceOption")
                                                {
                                                    if (sitem.FieldName != "ServiceID" && sitem.FieldName != "CMSCode" && sitem.FieldName != "ServiceOptionID")
                                                    {
                                                        cmd.Parameters.Add(new SqlParameter("@p" + sitem.FieldName, sitem.FieldName != "Status" ? sitem.Value : pIsApproved ? (int)SharedFunctions.Status.Approved : (int)SharedFunctions.Status.Disapproved));
                                                    }
                                                }
                                            }
                                            foreach (RequestList.Values witem in item.WhereValues)
                                            {
                                                cmd.Parameters.Add(new SqlParameter("@p" + witem.FieldName, witem.Value));
                                            }
                                        }
                                        else if (action == Actions.DELETE)
                                        {
                                            //DELETE STATUS FOR SERVICES
                                            foreach (RequestList.Values sitem in item.NewValues)
                                            {
                                                if (item.AffectedTable == "ServicesAvailment")
                                                {
                                                    if (sitem.FieldName != "ServiceID" && sitem.FieldName != "CMSCode")
                                                    {
                                                        cmd.Parameters.Add(new SqlParameter("@p" + sitem.FieldName, sitem.FieldName != "Status" ? sitem.Value : pIsApproved ? (int)SharedFunctions.Status.Approved : (int)SharedFunctions.Status.Disapproved));
                                                    }
                                                }
                                                else if (item.AffectedTable == "ServiceOption")
                                                {
                                                    if (sitem.FieldName != "ServiceOptionID")
                                                    {
                                                        cmd.Parameters.Add(new SqlParameter("@p" + sitem.FieldName, sitem.FieldName != "Status" ? sitem.Value : pIsApproved ? (int)SharedFunctions.Status.Approved : (int)SharedFunctions.Status.Disapproved));
                                                    }
                                                }
                                            }
                                            foreach (RequestList.Values witem in item.WhereValues)
                                            {
                                                cmd.Parameters.Add(new SqlParameter("@p" + witem.FieldName, witem.Value));
                                            }
                                        }
                                    }
                                }
                                cn.Open();
                                intReturn = cmd.ExecuteNonQuery();
                            }
                            else
                            {
                                if (action == Actions.INSERT)
                                {
                                    IsForInsert = true;
                                    action = Actions.UPDATE;
                                }
                                else { IsForInsert = false; }

                                if (action == Actions.INSERT)
                                {
                                    foreach (RequestList.Values value in item.NewValues)
                                    {
                                        if (value.FieldName != "ServiceName" && value.FieldName != "ServiceOptionID" && value.FieldName != "ID")
                                        {
                                            if (item.AffectedTable == "CMSCodes")
                                            {
                                                if (value.FieldName != "CMSCode")
                                                {
                                                    cmd.Parameters.Add(new SqlParameter("@p" + value.FieldName, (value.FieldName != "Status") ? value.Value : (pIsApproved) ? (int)SharedFunctions.Status.Approved : (int)SharedFunctions.Status.Disapproved));
                                                }
                                            }
                                            else
                                            {
                                                cmd.Parameters.Add(new SqlParameter("@p" + value.FieldName, (value.FieldName != "Status") ? value.Value : (pIsApproved) ? (int)SharedFunctions.Status.Approved : (int)SharedFunctions.Status.Disapproved));
                                            }
                                        }
                                    }
                                    cn.Open();
                                    intReturn = cmd.ExecuteNonQuery();

                                }
                                else
                                {
                                    if (pIsApproved)
                                    {
                                        if (action == Actions.UPDATE)
                                        {
                                            foreach (RequestList.Values value in item.NewValues)
                                            {
                                                if (value.FieldName != "ServiceName" && value.FieldName != "ServiceOptionID" && value.FieldName != "CMSCode" && value.FieldName != "BranchName")
                                                {
                                                    if (value.FieldName != "ServiceID")
                                                    {
                                                        if (value.FieldName == "IsActive" || value.FieldName == "IsAutoDebit")
                                                        {
                                                            cmd.Parameters.Add(new SqlParameter("@p" + value.FieldName, (value.Value.ToString() == "True") ? "1" : "0"));
                                                        }
                                                        else
                                                        {
                                                            cmd.Parameters.Add(new SqlParameter("@p" + value.FieldName, (value.FieldName != "Status") ? value.Value : (pIsApproved) ? (int)SharedFunctions.Status.Approved : (int)SharedFunctions.Status.Disapproved));
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else if (action == Actions.DELETE)
                                        {
                                            //foreach (RequestList.Values value in item.NewValues)
                                            //{
                                            // if (value.FieldName != "ServiceName" && value.FieldName != "ServiceOptionID")
                                            // {
                                            //  if (value.FieldName != "ServiceID" && value.FieldName != "CMSCode")
                                            //  {
                                            //   cmd.Parameters.Add(new SqlParameter("@p" + value.FieldName, (value.FieldName != "Status") ? (value.FieldName == "EnrolledOn" && value.FieldName == "ModifiedOn") ? Convert.ToDateTime(value.Value) : value.Value : "3"));
                                            //  }
                                            // }
                                            //}
                                        }
                                        foreach (RequestList.Values value in item.WhereValues)
                                        {
                                            cmd.Parameters.Add(new SqlParameter("@p" + value.FieldName, value.Value));
                                        }
                                    }
                                    cn.Open();
                                    intReturn = cmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                    if (pIsApproved)
                    {
                        item.Status = "2";
                    }
                    else
                    {
                        item.Status = "3";
                    }
                    item.ApprovedBy = pApprover;
                    item.ApprovedOn = DateTime.Now;
                    this.Update(item);
                    if (item.AffectedTable == "CMSCodes" && IsForInsert)
                    {
                        //UPDATE CREATED ON FIELD IN CMS CODES TABLE
                        using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
                        {
                            using (SqlCommand cmd = cn.CreateCommand())
                            {
                                string strSQL = "";
                                strSQL += " UPDATE CMSCodes";
                                strSQL += " SET CreatedOn = @pCreatedOn";
                                strSQL += " WHERE CMSCode = @pCMSCode";

                                cmd.CommandText = strSQL;
                                cmd.Parameters.Add(new SqlParameter("@pCreatedOn", item.ApprovedOn));
                                cmd.Parameters.Add(new SqlParameter("@p" + item.WhereValues.First().FieldName, item.WhereValues.First().Value));

                                cn.Open();
                                cmd.ExecuteNonQuery();
                                cn.Close();
                            }
                        }

                        foreach (RequestList.Values cmscodeitem in item.NewValues)
                        {
                            if (cmscodeitem.FieldName == "CMSCode") { seCMSCode = cmscodeitem.Value.ToInt(); }
                            if (cmscodeitem.FieldName == "BranchName") { seBranchName = cmscodeitem.Value.ToString(); }
                            if (cmscodeitem.FieldName == "Description") { seDescription = cmscodeitem.Value.ToString(); }
                        }
                        SendEmailNotification(seCMSCode, seBranchName, seDescription);
                        IsForInsert = false;
                    }
                }
                catch (Exception e)
                {
                    CTBC.Logs.Write("Process Request", e.Message, "Request List");
                }
            }
            SendEmailRequestNotification(pIsForApprover, pIsStatus, pMPersonId, pSPersonId, pRequestCode);
            return intReturn;
        }

        public DataTable GetRequestListEnrollment(string pCreatedBy, DateTime? pStartDate, DateTime? pEndDate)
        {
            List<RequestList> list = new List<RequestList>();
            DataTable dt = new DataTable();
            string strSQL = "";
            strSQL += "SELECT ";
            strSQL += " REQ.RequestCode ";
            strSQL += " ,REQ.NewValues ";
            strSQL += " ,REQ.OldValues ";
            strSQL += " ,REQ.Remarks ";
            strSQL += " ,REQ.ApprovedBy ";
            strSQL += " ,REQ.ApprovedOn ";
            strSQL += " ,RE.CreatedBy ";
            strSQL += " ,RE.CreatedOn ";
            strSQL += " ,RE.IPAddress ";
            strSQL += " ,RE.AssignedApprover ";
            strSQL += " FROM RequestList REQ ";
            strSQL += " INNER JOIN Requests Re ON RE.RequestCode = REQ.RequestCode ";
            if (pStartDate != null && pEndDate == null)
            {
                strSQL += "	WHERE	REQ.ApprovedOn >= @pStartDate ";
            }
            else if (pEndDate != null && pStartDate == null)
            {
                strSQL += "	WHERE	REQ.ApprovedOn <= @pEndDate ";
            }
            else if (pStartDate != null && pEndDate != null)
            {
                strSQL += "	WHERE REQ.ApprovedOn BETWEEN @pStartDate AND @pEndDate ";
            }
            else
            {
                strSQL += "	WHERE	REQ.ApprovedOn = REQ.ApprovedOn ";
            }
            strSQL += " AND RE.CreatedBy = @pCreatedBy";

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
                        cmd.Parameters.Add(new SqlParameter("@pCreatedBy", pCreatedBy));
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
                CTBC.Logs.Write("GetRequestList", ex.Message, "RequestList");
            }
            return dt;
        }

        public int ProcessRequestList(List<string> pRequestCodelist, bool pIsApproved, string pApprover, bool pIsForApprover, bool pIsStatus, string pMPersonId)
        {
            int intReturn = 0;
            try
            {
                foreach (var item in pRequestCodelist)
                {
                    string requestor = GetRequestorByRequestCode(item.ToInt());
                    ProcessRequest(item.ToInt(), pIsApproved, pApprover, pIsForApprover, pIsStatus, pMPersonId, requestor);
                }
                intReturn = 1;
            }
            catch (Exception e)
            {
                intReturn = 0;
                CTBC.Logs.Write("ProcessRequestList", e.Message, "RequestList");

            }
            return intReturn;
        }

        public string GetRequestorByRequestCode(int pRequestCode)
        {
            string strReturn = "";
            using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
            {
                using (SqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "SELECT CreatedBy FROM Requests WHERE RequestCode = @pRequestCode ";
                    cmd.Parameters.Add(new SqlParameter("@pRequestCode", pRequestCode));
                    cn.Open();
                    strReturn = cmd.ExecuteScalar().ToString();
                }
            }
            return strReturn;
        }

        public int RejectRequest(int pRequestCode, bool pIsNewCMSCode, string pApprover, bool pIsForApprover, bool pIsStatus, string pMPersonId, string pSPersonId)
        {
            int intReturn = 0;
            string strSQL = "UPDATE RequestList ";
            strSQL += "SET Status = 3 ";
            strSQL += ",ApprovedBy = @pApprovedBy ";
            strSQL += ",ApprovedOn = @pApprovedOn ";
            strSQL += "WHERE RequestCode = @pRequestCode ";
            try
            {
                using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
                {
                    using (SqlCommand cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = strSQL;
                        cmd.Parameters.Add(new SqlParameter("@pRequestCode", pRequestCode));
                        cmd.Parameters.Add(new SqlParameter("@pApprovedBy", pApprover));
                        cmd.Parameters.Add(new SqlParameter("@pApprovedOn", SharedFunctions.ServerDate));
                        cn.Open();
                        intReturn = cmd.ExecuteNonQuery();
                    }
                }
                if (pIsNewCMSCode)
                {
                    RejectCMSCode(GetCMSCodebyRequestCode(pRequestCode));
                }
                SendEmailRequestNotification(pIsForApprover, pIsStatus, pMPersonId, pSPersonId, pRequestCode);
            }
            catch (Exception e)
            {
                CTBC.Logs.Write("Reject Request", e.Message, "Request List");
            }
            return intReturn;
        }

        public int RejectRequestList(List<ParamData> pRequestCodeList, string pApprover, bool pIsForApprover, bool pIsStatus, string pMPersonId)
        {
            //bool pIsNewCMSCode
            int intReturn = 0;
            try
            {
                foreach (var item in pRequestCodeList)
                {
                    bool IsNew = false;
                    string requestor = GetRequestorByRequestCode(item.RequestCode);
                    if (item.CMSCode == "New")
                    {
                        IsNew = true;
                    }
                    else { IsNew = false; }
                    RejectRequest(item.RequestCode, IsNew, pApprover, pIsForApprover, pIsStatus, pMPersonId, requestor);
                }
            }
            catch (Exception e)
            {
                CTBC.Logs.Write("Reject Request List", e.Message, "Request List");
            }
            return intReturn;
        }

        public int CancelRequest(int pRequestCode, bool pIsNewCMSCode)
        {
            int intReturn = 0;
            string strSQL = "UPDATE RequestList ";
            strSQL += "SET Status = 3 ";
            strSQL += "WHERE RequestCode = @pRequestCode ";

            using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
            {
                using (SqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = strSQL;
                    cmd.Parameters.Add(new SqlParameter("@pRequestCode", pRequestCode));
                    cn.Open();
                    intReturn = cmd.ExecuteNonQuery();
                }
            }
            if (pIsNewCMSCode)
            {
                CancelCMSCode(GetCMSCodebyRequestCode(pRequestCode));
            }

            return intReturn;
        }

        public int CancelAllRequest(List<ParamData> pRequestCodeList)
        {
            int intReturn = 0;
            foreach (var item in pRequestCodeList)
            {
                bool IsNew = false;
                if (item.CMSCode == "New")
                {
                    IsNew = true;
                }
                else { IsNew = false; }
                CancelRequest(item.RequestCode, IsNew);
            }
            return intReturn;
        }

        public int CancelRequestByRequestListCode(string pRequestListCode)
        {
            int intReturn = 0;
            string strSQL = "UPDATE RequestList ";
            strSQL += "SET Status = 3 ";
            strSQL += "WHERE RequestListCode = @pRequestListCode ";

            using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
            {
                using (SqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = strSQL;
                    cmd.Parameters.Add(new SqlParameter("@pRequestListCode", pRequestListCode));
                    cn.Open();
                    intReturn = cmd.ExecuteNonQuery();
                }
            }

            return intReturn;
        }

        public int RejectCMSCode(int pCMSCode)
        {
            int intReturn = 0;
            string strSQL = "UPDATE CMSCOdes ";
            strSQL += "SET ";
            strSQL += "Status = 5 ";
            strSQL += "WHERE CMSCode = @pCMSCode";

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
            return intReturn;
        }

        public int CancelCMSCode(int pCMSCode)
        {
            int intReturn = 0;
            string strSQL = "UPDATE CMSCOdes ";
            strSQL += "SET ";
            strSQL += "Status = 4 ";
            strSQL += "WHERE CMSCode = @pCMSCode";

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
            return intReturn;
        }

        public string sqlBuild(RequestList pRequestListDetails)
        {
            string strReturn = "";
            int intIndex = 0;
            try
            {
                Actions action = (Actions)Enum.Parse(typeof(Actions), pRequestListDetails.Action);
                if (pRequestListDetails.AffectedTable == "ServicesAvailment" || pRequestListDetails.AffectedTable == "ServiceOption")
                {
                    if (action == Actions.INSERT)
                    {
                        strReturn += " INSERT INTO ";
                        strReturn += " " + pRequestListDetails.AffectedTable;
                        strReturn += " VALUES ( ";
                        foreach (RequestList.Values value in pRequestListDetails.NewValues)
                        {
                            intIndex++;
                            if (pRequestListDetails.AffectedTable == "ServicesAvailment")
                            {
                                if (value.FieldName != "ServiceName")
                                {
                                    strReturn += "@p" + value.FieldName + string.Format((intIndex == pRequestListDetails.NewValues.Count) ? "" : ",");
                                }
                            }
                            else
                            {
                                if (value.FieldName != "ServiceOptionID")
                                {
                                    strReturn += "@p" + value.FieldName + string.Format((intIndex == pRequestListDetails.NewValues.Count) ? "" : ",");
                                }
                            }
                        }
                        strReturn += ")";
                    }
                    else if (action == Actions.UPDATE)
                    {
                        strReturn += " UPDATE ";
                        strReturn += string.Format(" {0} ", pRequestListDetails.AffectedTable);
                        strReturn += " SET ";
                        intIndex = 0;
                        foreach (RequestList.Values value in pRequestListDetails.NewValues)
                        {
                            intIndex++;
                            if (value.FieldName != "ServiceName" && value.FieldName != "ServiceOptionID" && value.FieldName != "ID" && value.FieldName != "BranchName" && value.FieldName != "ServiceID")
                            {
                                if (pRequestListDetails.AffectedTable == "CMSCodes")
                                {
                                    if (value.FieldName != "CMSCode")
                                    {
                                        strReturn += string.Format(" {0} = {1} {2} ", value.FieldName, (string.IsNullOrEmpty(value.Value.ToString()) ? "null" : "@p" + value.FieldName), intIndex == pRequestListDetails.NewValues.Count ? "" : ",");
                                    }
                                }
                                else
                                {
                                    strReturn += string.Format(" {0} = {1} {2} ", value.FieldName, (string.IsNullOrEmpty(value.Value.ToString()) ? "null" : "@p" + value.FieldName), intIndex == pRequestListDetails.NewValues.Count ? "" : ",");
                                }
                            }
                        }
                        strReturn += " WHERE ";
                        intIndex = 0;
                        foreach (RequestList.Values value in pRequestListDetails.WhereValues)
                        {
                            intIndex++;
                            strReturn += string.Format(" {0} = {1} {2} ", value.FieldName, "@p" + value.FieldName, intIndex == pRequestListDetails.WhereValues.Count ? "" : "AND");
                        }
                    }
                    else if (action == Actions.DELETE)
                    {
                        strReturn += " UPDATE ";
                        strReturn += string.Format(" {0} ", pRequestListDetails.AffectedTable);
                        strReturn += " SET ";
                        strReturn += " Status = 3 ";
                        intIndex = 0;
                        //foreach (RequestList.Values value in pRequestListDetails.NewValues)
                        //{
                        // intIndex++;
                        // if (value.FieldName != "ServiceName" && value.FieldName != "ServiceOptionID" && value.FieldName != "CMSCode" && value.FieldName != "ServiceID")
                        // {
                        //  strReturn += string.Format(" {0} = {1} {2} ", value.FieldName, "@p" + value.FieldName, intIndex == pRequestListDetails.NewValues.Count ? "" : ",");
                        // }
                        //}
                        strReturn += " WHERE ";
                        intIndex = 0;
                        foreach (RequestList.Values value in pRequestListDetails.WhereValues)
                        {
                            intIndex++;
                            strReturn += string.Format(" {0} = {1} {2} ", value.FieldName, "@p" + value.FieldName, intIndex == pRequestListDetails.WhereValues.Count ? "" : " AND ");
                        }
                    }
                }
                else
                {
                    if (action == Actions.INSERT)
                    {
                        action = Actions.UPDATE;
                    }

                    if (action == Actions.INSERT)
                    {
                        strReturn += " INSERT INTO";
                        strReturn += " " + pRequestListDetails.AffectedTable;
                        strReturn += " VALUES ( ";
                        intIndex = 0;
                        foreach (RequestList.Values value in pRequestListDetails.NewValues)
                        {
                            intIndex++;
                            //strReturn += string.Format(" {0} = {1} {2} ", value.FieldName, (string.IsNullOrEmpty(value.Value.ToString()) ? "null" : "@p" + value.FieldName), intIndex == pRequestListDetails.NewValues.Count ? "" : ",");
                            if (value.FieldName != "ServiceName" && value.FieldName != "ServiceOptionID" && value.FieldName != "ID" && value.FieldName != "BranchName")
                            {
                                if (pRequestListDetails.AffectedTable == "CMSCodes")
                                {
                                    if (value.FieldName != "CMSCode")
                                    {
                                        strReturn += "@p" + value.FieldName + string.Format((intIndex == pRequestListDetails.NewValues.Count) ? "" : ",");
                                    }
                                }
                                else { strReturn += "@p" + value.FieldName + string.Format((intIndex == pRequestListDetails.NewValues.Count) ? "" : ","); }
                            }
                        }
                        strReturn += " )";
                    }
                    else if (action == Actions.UPDATE)
                    {
                        strReturn += " UPDATE ";
                        strReturn += string.Format(" {0} ", pRequestListDetails.AffectedTable);
                        strReturn += " SET ";
                        intIndex = 0;
                        foreach (RequestList.Values value in pRequestListDetails.NewValues)
                        {
                            intIndex++;
                            if (value.FieldName != "ServiceName" && value.FieldName != "ServiceOptionID" && value.FieldName != "ID" && value.FieldName != "BranchName" && value.FieldName != "ServiceID")
                            {
                                if (pRequestListDetails.AffectedTable == "CMSCodes")
                                {
                                    if (value.FieldName != "CMSCode")
                                    {
                                        strReturn += string.Format(" {0} = {1} {2} ", value.FieldName, (string.IsNullOrEmpty(value.Value.ToString()) ? "null" : "@p" + value.FieldName), intIndex == pRequestListDetails.NewValues.Count ? "" : ",");
                                    }
                                }
                                else
                                {
                                    strReturn += string.Format(" {0} = {1} {2} ", value.FieldName, (string.IsNullOrEmpty(value.Value.ToString()) ? "null" : "@p" + value.FieldName), intIndex == pRequestListDetails.NewValues.Count ? "" : ",");
                                }
                            }
                        }
                        strReturn += " WHERE ";
                        intIndex = 0;
                        foreach (RequestList.Values value in pRequestListDetails.WhereValues)
                        {
                            intIndex++;
                            strReturn += string.Format(" {0} = {1} {2} ", value.FieldName, "@p" + value.FieldName, intIndex == pRequestListDetails.WhereValues.Count ? "" : "AND");
                        }
                    }
                    else if (action == Actions.DELETE)
                    {
                        strReturn += " UPDATE ";
                        strReturn += string.Format(" {0} ", pRequestListDetails.AffectedTable);
                        strReturn += " SET ";
                        strReturn += " Status = 3 ";
                        intIndex = 0;
                        //foreach (RequestList.Values value in pRequestListDetails.NewValues)
                        //{
                        // intIndex++;
                        // if (value.FieldName != "ServiceName" && value.FieldName != "ServiceOptionID" && value.FieldName != "CMSCode" && value.FieldName != "ServiceID")
                        // {
                        //  strReturn += string.Format(" {0} = {1} {2} ", value.FieldName, "@p" + value.FieldName, intIndex == pRequestListDetails.NewValues.Count ? "" : ",");
                        // }
                        //}
                        strReturn += " WHERE ";
                        intIndex = 0;
                        foreach (RequestList.Values value in pRequestListDetails.WhereValues)
                        {
                            intIndex++;
                            strReturn += string.Format(" {0} = {1} {2} ", value.FieldName, "@p" + value.FieldName, intIndex == pRequestListDetails.WhereValues.Count ? "" : " AND ");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CTBC.Logs.Write("SQL Build", ex.Message, "Request List");
            }
            return strReturn;
        }

        public int InsertRequestServiceandDetails(ServiceOptions.AvailedDetails details, Enrollment.RequestListDetails pRequestListDetails, bool pIsForInsert, int pCMSCode)
        {
            int intReturn = 0;
            Models.ServiceOptions.AvailedDetails a_details = new ServiceOptions.AvailedDetails();
            //Models.ServiceOptions.ServiceOptionDetails s_details = a_details.ServiceOptionDetails;
            a_details.CMSCode = pCMSCode;

            if (pIsForInsert) { a_details.EnrolledBy = details.EnrolledBy; }
            if (pIsForInsert) { a_details.EnrolledOn = details.EnrolledOn; }
            a_details.ModifiedBy = details.ModifiedBy;
            a_details.ModifiedOn = details.ModifiedOn;
            a_details.ServiceID = details.ServiceID;
            a_details.RMID = details.RMID;

            a_details.ServiceName = details.ServiceName;
            a_details.MotherRequiredADB = details.MotherRequiredADB;
            a_details.SubRequiredADB = details.SubRequiredADB;
            a_details.MinNumberEmployee = details.MinNumberEmployee;
            a_details.Status = details.Status;
            pRequestListDetails.AffectedTable = "ServicesAvailment";
            var rem = (pIsForInsert) ? "add" : (pRequestListDetails.Action == "1") ? "update" : "delete";
            pRequestListDetails.Remarks = "Request to " + rem + " Service " + details.ServiceName;
            new Models.RequestList().InsertRequestList(a_details, pRequestListDetails, pIsForInsert, details.CMSCode);

            //s_details.AccountNoServiceType = details.ServiceOptionDetails.AccountNoServiceType;
            //s_details.CMSCode = pCMSCode;
            //s_details.MaxFreeTransaction = details.ServiceOptionDetails.MaxFreeTransaction;
            //s_details.MaxWithdrawalPaidByEmployer = details.ServiceOptionDetails.MaxWithdrawalPaidByEmployer;
            //s_details.MinNumberEmployee = details.ServiceOptionDetails.MinNumberEmployee;
            //s_details.MotherRequiredADB = details.ServiceOptionDetails.MotherRequiredADB.ToString().ToDecimal();
            //s_details.PayrollFrequency = details.ServiceOptionDetails.PayrollFrequency;
            //s_details.Remarks = details.ServiceOptionDetails.Remarks;
            //s_details.ServiceOptionID = details.ServiceOptionDetails.ServiceOptionID;
            //s_details.SubRequiredADB = details.ServiceOptionDetails.SubRequiredADB.ToString().ToDecimal();
            //s_details.WithdrawalFeeAccountNo = details.ServiceOptionDetails.WithdrawalFeeAccountNo;
            //s_details.WithdrawalFeePerTransaction = details.ServiceOptionDetails.WithdrawalFeePerTransaction.ToString().ToDecimal();
            //s_details.ServiceID = a_details.ServiceID;
            //s_details.Status = a_details.ServiceOptionDetails.Status;
            //pRequestListDetails.AffectedTable = "ServiceOption";
            //var rema = (pIsForInsert) ? "add" : (pRequestListDetails.Action == "1") ? "update" : "delete";
            //pRequestListDetails.Remarks = "Request to " + rema + " Service Details";
            //new Models.RequestList().InsertRequestList(s_details, pRequestListDetails, pIsForInsert, details.CMSCode);
            return intReturn;
        }

        public List<RequestList> GetRequestByRequestListCode(int pRequestListCode)
        {
            List<RequestList> list = new List<RequestList>();
            string strSQL = "SELECT * ";
            strSQL += "FROM ";
            strSQL += "RequestList ";
            strSQL += "WHERE ";
            strSQL += "RequestListCode = @pRequestListCode ";

            using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
            {
                using (SqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = strSQL;
                    cmd.Parameters.Add(new SqlParameter("@pRequestListCode", pRequestListCode));
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new RequestList
                            {
                                Action = dr["Action"].ToString(),
                                AffectedTable = dr["AffectedTable"].ToString(),
                                ApprovedBy = dr["ApprovedBy"].ToString(),
                                ApprovedOn = dr["ApprovedOn"].ToString().ToDateTime(),
                                Module = dr["Module"].ToString(),
                                NewValues = ToListValues(dr["NewValues"].ToString()),
                                OldValues = ToListValues(dr["OldValues"].ToString()),
                                WhereValues = ToListValues(dr["WhereValues"].ToString()),
                                Remarks = dr["Remarks"].ToString(),
                                RequestListCode = dr["RequestListCode"].ToString().ToInt(),
                                Status = dr["Status"].ToString(),
                                Request = new Requests().GetRequestsByRequestListCode(pRequestListCode)
                            });
                        }
                    }
                }
            }
            return list;
        }
        //EMAIL //REQUEST MAINTENANCE
        //New CMSCode
        public int SendEmailNotification(int pCMSCode, string pBranchName, string pDescription)
        {
            int intReturn = 0;
            string strMailBody = "";
            strMailBody += "Dear " + pBranchName + ",";
            strMailBody += "\r\n";
            strMailBody += "\r\n";
            strMailBody += "This is to request the following:" + "\r\n";
            strMailBody += "1) Please assign CMS Code " + pCMSCode + " to the mother/child accounts under the subject account." + "\r\n";
            strMailBody += "2) Please change the investor type of the known MOTHER ACCOUNT to 0701." + "\r\n";
            strMailBody += "3) Please change the investor type of the known CHILD ACCOUNTS to 0702." + "\r\n" + "\r\n";
            strMailBody += "Please comply immediately." + "\r\n" + "\r\n";
            strMailBody += "Best Regards," + "\r\n";
            strMailBody += "E-Channels Operations/CMS Support Team";
            try
            {
                using (CTBC.Mail.Mailer mail = new CTBC.Mail.Mailer())
                {
                    Models.SharedFunctions.NotificationSettings notificationsettings = new SharedFunctions().GetNoticationSettings();
                    mail.Sender = notificationsettings.Sender; // AFM 20220510
                    //mail.Sender = "ssdu.dev@ctbcbank.com.ph";
                    // mail.CC = "romeo.episcope@ctbcbank.com.ph"; //temp removed
                    //e-channels@ctbcbank.com.ph";
                    //mail.Recipient = ReturnBranchEmail((pBranchName == "") ? "-No Value-" : pBranchName);
                    mail.Recipient = "itp2ba3@ctbcbank.com.ph"; //for testing
                    //mail.Recipient = "itp2dev4@ctbcbank.com.ph"; //for testing
                    mail.Subject = "Request for account maintenance: (" + pCMSCode + ") " + pDescription;
                    mail.Body = strMailBody;
                    mail.SMTPServer = notificationsettings.SMPTPServer; // AFM 20220510
                    //mail.SMTPServer = "172.16.4.52";
                    intReturn = mail.Send().ToInt();
                }
            }
            catch (Exception e)
            {
                CTBC.Logs.Write("Send Email Notification", e.Message, "Request List");
            }
            return intReturn;
        }

        public string GetDetail(int pReqCode)
        {
            List<string> list = new List<string>();
            string strReturn = "";
            string strSQL = "SELECT NewValues FROM RequestList WHERE RequestCode = @pRequestCode";
            using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
            {
                cn.Open();
                using (SqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = strSQL;
                    cmd.Parameters.Add(new SqlParameter("@pRequestCode", pReqCode));
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(dr["NewValues"].ToString());
                        }
                    }
                }
            }
            strReturn = string.Join(";", list);
            return strReturn;
        }
        //EMAIL FOR REQUEST //REQUEST UPDATE APPROVAL
        public int SendEmailRequestNotification(bool pIsForApprover, bool pIsStatus, string pMPersonID, string pSPersonID, int pRequestCode)
        {
            int intReturn = 0;
            SYS_MATRIX.Models.SYS_USERS_MODEL muser = getUserDetails(pMPersonID);
            SYS_MATRIX.Models.SYS_USERS_MODEL suser = getUserDetails(pSPersonID);
            List<RequestList> requestlist = GetRequestListRemarks(pRequestCode);


            string cmscode = "";
            string strMailBody = "Dear " + muser.FullName + "(" + muser.UserID + ")" + ":" + "@" + "@";
            strMailBody += (pIsForApprover ? "A CMMC request filed by " + suser.FullName + "(" + suser.UserID + ")" + " with the details below is for your processing." : "The request you filed with the details below was " + (pIsStatus ? "approved" : "rejected"));
            strMailBody += "@" + "@";
            string nn = GetDetail(pRequestCode);
            cmscode = GetCMSCodebyRequestCode(pRequestCode).ToString(); //
            strMailBody += "Request ID: " + pRequestCode + "@";
            //strMailBody += "CMS Code: " + cmscode + "@";
            strMailBody += "Request Details: " + "@";

            strMailBody += "--------------------------------------------------\n";
            List<RequestList> requests = GetRequestList(pRequestCode);

            foreach (RequestList req in requests)
            {
                strMailBody += "@" + string.Format(req.Remarks, "\n") + "@";

                List<RequestList.Values> newVals = req.NewValues;
                foreach (RequestList.Values val in newVals)
                {
                    strMailBody += string.Format("{0}: {1}\n", val.FieldName, val.Value);
                }
            }

            strMailBody += "--------------------------------------------------\n";


            if (!pIsForApprover)
            {
                strMailBody += "Status: " + (pIsStatus ? "Approved" : "Rejected") + "@" + "@";
                strMailBody += "To create new request or update existing records ";
            }
            else
            {
                strMailBody += "@" + "@" + "To approve or reject this request ";
            }

            strMailBody += "or view additional" + "@";
            strMailBody += "information, please click the link below and login to CMMC system." + "@" + "@";
            strMailBody += "https://cmpapp02.chinatrust.com.ph:8082/" + (pIsForApprover ? "Home" : "Enrollment");
            strMailBody += "@" + "@";
            strMailBody += "Thanks." + "@";
            strMailBody += "Cash Management Monitoring and Charging System." + "@";
            strMailBody += "[System Generated Notification – Please do not reply]";
            strMailBody = strMailBody.Replace("@", "\r\n");

            try
            {
                using (CTBC.Mail.Mailer mail = new CTBC.Mail.Mailer())
                {
                    mail.Sender = "ssdu.dev@ctbcbank.com.ph";
                    mail.CC = suser.Email;
                    mail.Recipient = muser.Email;
                    mail.Subject = "CMMC Request # " + pRequestCode.ToString() + (pIsForApprover ? " requires your attention" : (pIsStatus ? " Approved" : " Rejected"));
                    mail.Body = strMailBody;
                    mail.SMTPServer = "172.16.4.52";
                    intReturn = mail.Send().ToInt();
                }
            }
            catch (Exception e)
            {
                CTBC.Logs.Write("Send Email Request Notification", e.Message, "Request List");
            }
            return intReturn;
        }

        public int SendEmailTermination(bool pIsForApprover, bool pIsStatus, string pMPersonID, string pSPersonID, int pRequestCode)
        {
            int intReturn = 0;
            SYS_MATRIX.Models.SYS_USERS_MODEL muser = getUserDetails(pMPersonID);
            SYS_MATRIX.Models.SYS_USERS_MODEL suser = getUserDetails(pSPersonID);
            List<RequestList> requestlist = GetRequestListRemarks(pRequestCode);

            string cmscode = "";

            string strMailBody = "Dear " + muser.UserID + " - " + muser.FullName + ":" + "@" + "@";
            strMailBody += (pIsForApprover ? "A CMMC request filed by " + suser.UserID + " - " + suser.FullName + " with the details below is for your processing." : "The request you filed with the details below was " + (pIsStatus ? "approved" : "rejected"));
            strMailBody += "@" + "@";


            foreach (var item in requestlist)
            {
                if (item.AffectedTable == "CMSCodes")
                {
                    cmscode = item.Action == "2" ? "New" : GetCMSCodebyRequestCode(pRequestCode).ToString();
                    strMailBody += "Request ID: " + pRequestCode + "@";
                    strMailBody += "CMS Code: " + cmscode + "@";
                    strMailBody += "Request Details: " + "@";
                }
                strMailBody += "\t" + item.Remarks + "@";
            }

            if (!pIsForApprover)
            {
                strMailBody += "Status: " + (pIsStatus ? "Approved" : "Rejected") + "@" + "@";
                strMailBody += "To create new request or update existing records ";
            }
            else
            {
                strMailBody += "@" + "@" + "To approve or reject this request ";
            }

            strMailBody += "or view additional" + "@";
            strMailBody += "information, please click the link below and login to CMMC system." + "@" + "@";
            strMailBody += "https://cmpapp02.chinatrust.com.ph:8082/" + (pIsForApprover ? "Home" : "Enrollment");
            strMailBody += "@" + "@";
            strMailBody += "Thanks." + "@";
            strMailBody += "Cash Management Monitoring and Charging System." + "@";
            strMailBody += "[System Generated Notification – Please do not reply]";
            strMailBody = strMailBody.Replace("@", "\r\n");

            try
            {
                using (CTBC.Mail.Mailer mail = new CTBC.Mail.Mailer())
                {
                    mail.Sender = "ssdu.dev@ctbcbank.com.ph";
                    mail.CC = suser.Email;
                    mail.Recipient = muser.Email;
                    mail.Subject = "CMMC Request # " + pRequestCode.ToString() + (pIsForApprover ? " requires your attention" : (pIsStatus ? " Approved" : " Rejected"));
                    mail.Body = strMailBody;
                    mail.SMTPServer = "172.16.4.52";
                    intReturn = mail.Send().ToInt();
                }
            }
            catch (Exception e)
            {
                CTBC.Logs.Write("Send Email Termination", e.Message, "Request List");
            }
            return intReturn;
        }

        //AFM 202205
        public int SendEmailForUnmanagedAccount(List<int> pCMSCode, List<string> pBranchName)
        {
            int intReturn = 0;
            try
            {
                List<string> distinctbranch = pBranchName.Distinct().ToList();

                for (int iCnt = 0; iCnt < distinctbranch.Count; iCnt++)
                {
                    var result = Enumerable.Range(0, pBranchName.Count)
                    .Where(i => pBranchName[i] == distinctbranch[iCnt])
                    .ToList();

                    string sb = "";
                    sb += "Dear " + distinctbranch[iCnt] + ",";
                    sb += "@" + "@";
                    sb += "Follow up on Accounts that are not yet managed with the following respective CMS Code(s).";
                    sb += "@" + "@";
                    sb += "CMS Codes: " + "@";

                    foreach (var s in result)
                    {
                        sb += "'" + pCMSCode[s].ToString() + "'" + "@";
                    }

                    sb += "@" + "@";
                    sb += "Thanks." + "@";
                    sb += "Cash Management Monitoring and Charging System" + "@";
                    sb += "[System Generated Notification – Please do not reply]";
                    sb = sb.Replace("@", "<br>");

                    SmtpClient smtpClient = new SmtpClient();
                    MailMessage message = new MailMessage();
                    MailAddress fromAddress = new MailAddress("ssdu.dev@ctbcbank.com.ph");
                    MailAddressCollection ccAddress = new MailAddressCollection();
                    smtpClient.Host = "172.16.4.52";
                    message.From = fromAddress;
                    foreach (var item in ccAddress)
                        message.CC.Add(item);
                    //message.To.Add("itp2dev4@ctbcbank.com.ph"); //test email
                    message.To.Add("itp2ba3@ctbcbank.com.ph");
                    //message.To.Add(ReturnBranchEmail((distinctbranch[iCnt] == "") ? "-No Value-" : distinctbranch[iCnt]));
                    message.Subject = "CMMC - CMS Code Unmaintained accounts for " + DateTime.Now.ToString("MM/dd/yyyy");
                    message.IsBodyHtml = true;
                    message.Body = sb;

                    smtpClient.Send(message);
                }
            }
            catch (Exception ex)
            {
                CTBC.Logs.Write("Send Email for Unmaintained Account", ex.Message, "Unmaintained list");
            }

            return intReturn;
        }

        public int SendEmailForAccountMaintenanceStatus(List<string> pCMSCodes)
        {
            int intReturn = 0;
            string strSQL = "SELECT * FROM CMSCodes where CMSCode IN (@pCMSCodes)";
            List<MaintainedAccount> MAccount = GetMaintainedAccount(pCMSCodes);

            using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
            {
                using (SqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = strSQL;
                    cmd.Parameters.Add(new SqlParameter("@pCMSCodes", String.Join(",", pCMSCodes.ToArray())));
                    cn.Open();
                }
            }

            try
            {
                string sb = "";
                sb += "Dear All:";
                sb += "@" + "@";
                sb += "Please refer to generated CMS Codes and status of branch account maintenance as of " + DateTime.Now.ToString("MM/dd/yyyy, hh:ss tt.");
                sb += "@" + "@";
                //sb += getHTML(dtWith, true);
                sb += "@" + "@";
                //sb += getHTML(dtWithOut, false);
                sb += "@" + "@";
                sb += "Kindly follow up the branches concerned to tag the accounts as deemed necessary to enable CMMC to monitor the usage of services availed by the clients.";
                sb += "@" + "@";
                sb += "Thanks." + "@";
                sb += "Cash Management Monitoring and Charging System" + "@";
                sb += "[System Generated Notification – Please do not reply]";
                sb = sb.Replace("@", "<br>");

                SmtpClient smtpClient = new SmtpClient();
                MailMessage message = new MailMessage();
                MailAddress fromAddress = new MailAddress("ssdu.dev@ctbcbank.com.ph");
                MailAddressCollection ccAddress = new MailAddressCollection();
                ccAddress.Add("");
                smtpClient.Host = "172.16.4.52";
                message.From = fromAddress;
                message.To.Add("ssdu.dev@ctbcbank.com.ph");
                message.Subject = "CMMC - CMS Code Account Maintenance status for " + DateTime.Now.ToString("MM/dd/yyyy");
                message.IsBodyHtml = true;
                message.Body = sb;

                // Send SMTP mail
                smtpClient.Send(message);
            }
            catch (Exception e)
            {
                CTBC.Logs.Write("Send Email for Account Maintenance Status", e.Message, "Request List");
            }
            return intReturn;
        }

        public List<MaintainedAccount> GetMaintainedAccount(List<string> pCMSCodes)
        {
            List<MaintainedAccount> maccount = new List<MaintainedAccount>();
            string OracleString = "";
            OracleString += "select Booking_Number, ";
            OracleString += "(select Count(*) FROM fnsonlp.invm where INV_TYPE in ('0701') and booking_number IN (@pCMSCodes))  as MotherCount, ";
            OracleString += "(select Count(*) FROM fnsonlp.invm where INV_TYPE in ('0702') and booking_number IN (@pCMSCodes))  as ChildCount ";
            OracleString += "FROM fnsonlp.invm ";
            OracleString += "WHERE ";
            OracleString += "booking_number IN (@pCMSCodes) ";
            OracleString += "GROUP BY booking_number; ";
            try
            {
                using (OracleConnection cn = new OracleConnection(SharedFunctions.OracleConnection))
                {
                    using (OracleCommand cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = OracleString;
                        cmd.Parameters.Add(new OracleParameter("@pCMSCodes", String.Join(",", pCMSCodes.ToArray())));
                        cn.Open();
                        using (OracleDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                maccount.Add(new MaintainedAccount
                                {
                                    CMSCode = dr["Booking_Number"].ToString(),
                                    MotherCount = dr["MotherCount"].ToInt(),
                                    ChildCount = dr["ChildCount"].ToInt()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                CTBC.Logs.Write("Get Maintained Account", e.Message, "Request List");
            }

            return maccount;
        }

        private static string getHTML(DataTable dt, bool withMother)
        {
            StringBuilder myBuilder = new StringBuilder();

            myBuilder.Append("<table border='1' cellpadding='5' cellspacing='0' ");
            myBuilder.Append("style='border: solid 1px Silver; font-size: x-small;'>");
            myBuilder.Append("<tr><th colspan='16' align='left'>w/" + (withMother ? "" : "o") + " Mother Account(s) Maintained</th></tr>");
            myBuilder.Append("<tr align='left' valign='top'>");
            foreach (DataColumn myColumn in dt.Columns)
            {
                myBuilder.Append("<td align='left' valign='top'>");
                myBuilder.Append(myColumn.ColumnName);
                myBuilder.Append("</td>");
            }
            myBuilder.Append("</tr>");

            foreach (DataRow myRow in dt.Rows)
            {
                myBuilder.Append("<tr align='left' valign='top'>");
                foreach (DataColumn myColumn in dt.Columns)
                {
                    myBuilder.Append("<td align='left' valign='top'>");
                    myBuilder.Append(myRow[myColumn.ColumnName].ToString());
                    myBuilder.Append("</td>");
                }
                myBuilder.Append("</tr>");
            }
            myBuilder.Append("</table>");

            return myBuilder.ToString();
        }

        public SYS_MATRIX.Models.SYS_USERS_MODEL getUserDetails(string pUserID)
        {
            SYS_MATRIX.Models.SYS_USERS_MODEL user = new SYS_MATRIX.Models.SYS_USERS_MODEL();
            string strSQL = "SELECT Email, FullName, UserID FROM SYS_USERS ";
            strSQL += "WHERE UserID = @pUserID";
            try
            {
                using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
                {
                    using (SqlCommand cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = strSQL;
                        cmd.Parameters.Add(new SqlParameter("@pUserID", pUserID));
                        cn.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                user.Email = dr["Email"].ToString();
                                user.FullName = dr["FullName"].ToString();
                                user.UserID = dr["UserID"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                CTBC.Logs.Write("getUserDetails", e.Message, "RequestList");
            }

            return user;
        }

        //   public List<Values> GetValues (int pRequestCode)
        //{
        //    List<Values> List = new List<Values>();
        //    using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
        //    {
        //        using (SqlCommand cmd = cn.CreateCommand())
        //        {
        //            cmd.CommandText = "SELECT NewValues FROM RequestList WHERE RequestCode = @pRequestCode";
        //            cmd.Parameters.Add(new SqlParameter("@pRequestCode", pRequestCode));
        //            cn.Open();
        //            using (SqlDataReader dr = cmd.ExecuteReader())
        //            {
        //                while (dr.Read())
        //                {
        //                    List.Add(new Values
        //                    {
        //                      FieldName = 
        //                    });
        //                }
        //            }
        //        }
        //    }
        //    return List;
        //}

        public List<RequestList> GetRequestListRemarks(int pRequestCode)
        {
            List<RequestList> list = new List<RequestList>();
            using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
            {
                using (SqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Remarks, Action, AffectedTable FROM RequestList WHERE RequestCode = @pRequestCode";
                    cmd.Parameters.Add(new SqlParameter("@pRequestCode", pRequestCode));
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new RequestList
                            {
                                Remarks = dr["Remarks"].ToString(),
                                Action = dr["Action"].ToString(),
                                AffectedTable = dr["AffectedTable"].ToString()
                            });
                        }
                    }
                }
            }
            return list;
        }

        public string ReturnBranchEmail(string pBranchName)
        {
            string strReturn = "";
            string strSQL = "SELECT Email ";
            strSQL += "FROM ";
            strSQL += "Branches ";
            strSQL += "WHERE BranchName = @pBranchName";
            try
            {
                using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
                {
                    using (SqlCommand cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = strSQL;
                        cmd.Parameters.Add(new SqlParameter("@pBranchName", pBranchName));
                        cn.Open();
                        strReturn = cmd.ExecuteScalar().ToString();
                    }
                }
            }
            catch (Exception e)
            {
                CTBC.Logs.Write("ReturnBranchEmail", e.Message, "RequestList");
            }
            return strReturn;
        }

        public int GetCMSCodebyRequestCode(int pRequestCode)
        {
            int intReturn = 0;
            Models.Enrollment.CmsCodeDetails cmscodedetails = new Enrollment.CmsCodeDetails();
            List<RequestList> requestlist = new List<RequestList>();
            string strSQL = "SELECT WhereValues, NewValues, AffectedTable ";
            strSQL += "FROM ";
            strSQL += "RequestList ";
            strSQL += "WHERE ";
            strSQL += "RequestCode = @pRequestCode ";

            try
            {
                using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
                {
                    using (SqlCommand cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = strSQL;
                        cmd.Parameters.Add(new SqlParameter("@pRequestCode", pRequestCode));
                        cn.Open();
                        this.WhereValues = ToListValues(cmd.ExecuteScalar().ToString());
                    }
                }
            }
            catch (Exception e)
            {
                CTBC.Logs.Write("Get CMS Code by Request Code", e.Message, "Request List");
            }
            foreach (var item in this.WhereValues)
            {
                if (item.FieldName == "CMSCode")
                {
                    intReturn = item.Value.ToString().ToInt();
                }
            }
            return intReturn;
        }

        public Enrollment.CmsCodeDetails EditRequest(int RequestCode)
        {

            Enrollment.CmsCodeDetails details = new Enrollment.CmsCodeDetails();
            CMSCode.Details cdetails = new CMSCode.Details();
            cdetails.Description = details.GeneralDetails.Description;

            List<RequestList> requestlist = new List<RequestList>();
            string strSQL = "SELECT WhereValues, NewValues, AffectedTable ";
            strSQL += "FROM ";
            strSQL += "RequestList ";
            strSQL += "WHERE ";
            strSQL += "RequestCode = @pRequestCode ";

            try
            {
                using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
                {
                    using (SqlCommand cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = strSQL;
                        cmd.Parameters.Add(new SqlParameter("@pRequestCode", RequestCode));
                        cn.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                requestlist.Add(new RequestList
                                {
                                    NewValues = ToListValues(dr["NewValues"].ToString()),
                                    AffectedTable = dr["AffectedTable"].ToString()
                                });
                            }
                        }
                    }
                }

                foreach (var item in requestlist)
                {
                    if (item.AffectedTable == "CMSCodes")
                    {
                        foreach (var item2 in item.NewValues)
                        {
                            if (item2.FieldName == "Description") { cdetails.Description = item2.Value.ToString(); }
                            //if (item2.FieldName == "") { cmscodedetails.Tagging = item2.Value.ToString(); }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                CTBC.Logs.Write("Edit Request", ex.Message, "Request List");
            }
            return details;
        }

        public bool IsServiceAlreadyPendingforRequest(int pCMSCode, int pServiceID)
        {
            bool boolReturn = false;
            List<RequestList> reqlist = new List<RequestList>();

            string strSQL = "";
            strSQL += "SELECT WhereValues, Action, AffectedTable FROM RequestList WHERE Status = 1";
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
                            reqlist.Add(new RequestList
                            {
                                WhereValues = ToListValues(dr["WhereValues"].ToString()),
                                AffectedTable = dr["AffectedTable"].ToString(),
                                Action = dr["Action"].ToString()
                            });
                        }
                    }
                }
            }
            bool proceed = false;
            foreach (var subitem in reqlist)
            {
                if (subitem.AffectedTable == "ServicesAvailment" && subitem.Action == "2")
                {
                    foreach (var item in subitem.WhereValues)
                    {
                        if (subitem.WhereValues.Count == 2)
                        {
                            if (item.FieldName == "CMSCode")
                            {
                                if (item.Value.ToString() == pCMSCode.ToString())
                                {
                                    boolReturn = true;
                                }
                                else { boolReturn = false; }
                            }
                            else if (item.FieldName == "ServiceID")
                            {
                                if (item.Value.ToString() == pServiceID.ToString())
                                {
                                    boolReturn = boolReturn ? true : false;
                                }
                                else
                                {
                                    boolReturn = boolReturn ? true : false;
                                }
                            }
                        }
                    }
                }
            }
            return boolReturn;
        }

        public DataTable GetApprove(string pApprover, string pStatus, DateTime? pStartDate = null, DateTime? pEndDate = null)
        {
            List<RequestList> list = new List<RequestList>();
            DataTable dt = new DataTable();
            string strSQL = "";
            strSQL += "SELECT RL.RequestCode, RL.Remarks ";
            strSQL += " ,Status = CASE WHEN RL.Status = '1' THEN 'For Approval' ";
            strSQL += "                WHEN RL.Status = '2' THEN 'Approved' ";
            strSQL += "                WHEN RL.Status = '3' THEN 'Disapproved' ";
            strSQL += "               ELSE '' ";
            strSQL += "             END ";
            strSQL += " ,AffectedTable = CASE WHEN RL.AffectedTable = 'CMSCodes' THEN 'CMS Codes' ";
            strSQL += "                       WHEN RL.AffectedTable = 'ServicesAvailment' THEN 'Services Availment' ";
            strSQL += "                ELSE '' ";
            strSQL += "             END ";
            strSQL += " ,SU.FullName ";
            strSQL += " ,RL.ApprovedBy ";
            strSQL += " ,RL.ApprovedOn ";
            strSQL += " FROM RequestList RL ";
            strSQL += " INNER JOIN SYS_USERS SU ON SU.UserID = RL.ApprovedBy ";
            if (pStartDate != null && pEndDate == null)
            {
                strSQL += "	WHERE	RL.ApprovedOn >= @pStartDate ";
            }
            else if (pEndDate != null && pStartDate == null)
            {
                strSQL += "	WHERE	RL.ApprovedOn <= @pEndDate ";
            }
            else if (pStartDate != null && pEndDate != null)
            {
                strSQL += "	WHERE RL.ApprovedOn BETWEEN @pStartDate AND @pEndDate ";
            }
            else
            {
                strSQL += "	WHERE	RL.ApprovedOn = RL.ApprovedOn ";
            }
            strSQL += " AND RL.Status =  @pStatus ";
            strSQL += " AND RL.ApprovedBy = @pApprover ";

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
                        cmd.Parameters.Add(new SqlParameter("@pApprover", pApprover));
                        cmd.Parameters.Add(new SqlParameter("@pStatus", pStatus));

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
                CTBC.Logs.Write("GetApprove", ex.Message, "RequestList");
            }
            return dt;
        }

        public DataTable GetDisapprove(string pApprover, DateTime? pStartDate = null, DateTime? pEndDate = null)
        {
            List<RequestList> list = new List<RequestList>();
            DataTable dt = new DataTable();
            string strSQL = "";
            strSQL += "SELECT RL.RequestCode, RL.Remarks ";
            strSQL += " ,Status = CASE WHEN RL.Status = '1' THEN 'For Approval' ";
            strSQL += "                WHEN RL.Status = '2' THEN 'Approved' ";
            strSQL += "                WHEN RL.Status = '3' THEN 'Disapproved' ";
            strSQL += "               ELSE '' ";
            strSQL += "             END ";
            strSQL += " ,AffectedTable = CASE WHEN RL.AffectedTable = 'CMSCodes' THEN 'CMS Codes' ";
            strSQL += "                       WHEN RL.AffectedTable = 'ServicesAvailment' THEN 'Services Availment' ";
            strSQL += "               ELSE '' ";
            strSQL += "             END ";
            strSQL += " ,SU.FullName ";
            strSQL += " ,RL.ApprovedBy ";
            strSQL += " ,RL.ApprovedOn ";
            strSQL += " FROM RequestList RL ";
            strSQL += " INNER JOIN SYS_USERS SU ON SU.UserID = RL.ApprovedBy ";
            if (pStartDate != null && pEndDate == null)
            {
                strSQL += "	WHERE	RL.ApprovedOn >= @pStartDate ";
            }
            else if (pEndDate != null && pStartDate == null)
            {
                strSQL += "	WHERE	RL.ApprovedOn <= @pEndDate ";
            }
            else if (pStartDate != null && pEndDate != null)
            {
                strSQL += "	WHERE RL.ApprovedOn BETWEEN @pStartDate AND @pEndDate ";
            }
            else
            {
                strSQL += "	WHERE	RL.ApprovedOn = RL.ApprovedOn ";
            }
            strSQL += " AND RL.Status = '3' ";
            strSQL += " AND RL.ApprovedBy = @pApprover ";

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
                        cmd.Parameters.Add(new SqlParameter("@pApprover", pApprover));

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
                CTBC.Logs.Write("GetDisapprove", ex.Message, "RequestList");
            }
            return dt;
        }

        public List<RequestList> GetRequestForEdit(int pRequestCode)
        {
            List<RequestList> list = new List<RequestList>();
            string strSQL = "SELECT * ";
            strSQL += "FROM ";
            strSQL += "RequestList ";
            strSQL += "WHERE ";
            strSQL += "RequestCode = @pRequestCode AND Status NOT IN (3) ";

            using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
            {
                using (SqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = strSQL;
                    cmd.Parameters.Add(new SqlParameter("@pRequestCode", pRequestCode));
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new RequestList
                            {
                                NewValues = ToListValues(dr["NewValues"].ToString()),
                                OldValues = ToListValues(dr["OldValues"].ToString()),
                                WhereValues = ToListValues(dr["WhereValues"].ToString()),
                                AffectedTable = dr["AffectedTable"].ToString(),
                                Action = dr["Action"].ToString(),
                                RequestListCode = dr["RequestListCode"].ToString().ToInt(),
                                Request = new Requests
                                {
                                    RequestCode = pRequestCode
                                }
                            });
                        }
                    }
                }
            }
            return list;
        }

        public List<string> GetCMSCodesColumnName()
        {
            List<string> list = new List<string>();
            string strSQL = "Select Column_name ";
            strSQL += "FROM " + GetDatabaseName() + ".INFORMATION_SCHEMA.COLUMNS ";
            strSQL += "WHERE TABLE_NAME = N'CMSCodes' ";

            try
            {
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
                                list.Add(dr["Column_Name"].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                CTBC.Logs.Write("GetCMSCodesColumnName", e.Message, "RequestList");
            }
            return list;
        }

        public List<string> GetServicesColumnName()
        {
            List<string> list = new List<string>();

            string strSQL = "Select Column_name ";
            strSQL += "FROM " + GetDatabaseName() + ".INFORMATION_SCHEMA.COLUMNS ";
            strSQL += "WHERE TABLE_NAME = N'ServicesAvailment' ";

            try
            {
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
                                list.Add(dr["Column_Name"].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                CTBC.Logs.Write("Get Services Column Name", e.Message, "Request List");
            }
            return list;
        }

        public int UpdateEditofRequest(List<Enrollment.RequestListDetails> pDetails)
        {
            int intReturn = 0;
            string strSQL = "UPDATE RequestList ";
            strSQL += "SET ";
            strSQL += "NewValues = @pNewValues ";
            strSQL += ",Remarks = @pRemarks ";
            strSQL += "WHERE RequestListCode = @pRequestListCode ";

            foreach (var item in pDetails)
            {
                using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
                {
                    using (SqlCommand cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = strSQL;
                        cmd.Parameters.Add(new SqlParameter("@pRequestListCode", item.RequestListCode));
                        cmd.Parameters.Add(new SqlParameter("@pNewValues", item.NewValues));
                        cmd.Parameters.Add(new SqlParameter("@pRemarks", UpdateRemarks(item.RequestListCode, item)));
                        cn.Open();
                        intReturn = cmd.ExecuteNonQuery();
                    }
                }
            }
            return intReturn;
        }

        public string GetDatabaseName()
        {
            string strReturn = "";
            using (CTBC.Utility.Registry registry = new CTBC.Utility.Registry(CTBC.Utility.Registry.RegistryType.LOCAL_MACHINE, @"SOFTWARE\CTBC\CMMC"))
            {
                strReturn = registry.Read("CONNECTION_DB");
            }
            return strReturn;
        }

        public string UpdateRemarks(int pRequestListCode, Enrollment.RequestListDetails pRequestDetails)
        {
            string strReturn = "";
            RequestList req = new RequestList();
            using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
            {
                using (SqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM RequestList WHERE RequestListCode = @pRequestListCode";
                    cmd.Parameters.Add(new SqlParameter("@pRequestListCode", pRequestListCode));
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            req.NewValues = ToListValues(dr["NewValues"].ToString());
                            req.WhereValues = ToListValues(dr["WhereValues"].ToString());
                            req.Action = dr["Action"].ToString();
                            req.Remarks = dr["Remarks"].ToString();
                        }
                    }
                }
            }
            bool alreadyExists = ToListValues(pRequestDetails.NewValues).Any(x => x.FieldName == "Description");
            string CMSCode = (from c in req.WhereValues
                              where c.FieldName == "CMSCode"
                              select c.Value).FirstOrDefault().ToString();
            if (alreadyExists)
            {
                foreach (var item in ToListValues(pRequestDetails.NewValues))
                {
                    if (item.FieldName == "Description")
                    {
                        var Desc = item.FieldName == "Description" ? item.Value.ToString() : "";
                        strReturn += "Request to ";
                        strReturn += (req.Action == "2") ? "add " + Desc : req.Action == "1" ? "update [" + CMSCode + "] " + Desc : "delete " + CMSCode + "] " + Desc;
                    }
                }
            }
            else
            {
                strReturn = req.Remarks;
            }
            return strReturn;
        }

        public int UpdateServiceRequest(int pRequestCode, Models.Enrollment.CmsCodeDetails pdetails)
        {
            int intReturn = 0;
            bool AServices = false;
            bool ForInsert = true;

            int intCMSCode = GetCMSCodebyRequestCode(pRequestCode);
            List<Models.ServiceOptions.AvailedDetails> list = new Models.ServiceOptions().GetEverything(intCMSCode);
            Enrollment.RequestListDetails req = new Enrollment.RequestListDetails();
            req.Module = "EditCMSCode";
            req.Status = "1";
            req.RequestCode = pRequestCode;
            req.AffectedTable = "ServicesAvailment";
            List<RequestList> reqlist = GetRequestListbyRequestCodeForApproval(pRequestCode);

            if (pdetails.AvailedDetailsList != null)
            {
                var tobeDeleted = (from n in list
                                   where !(from m in pdetails.AvailedDetailsList select m.ServiceID).Contains(n.ServiceID)
                                   select n).ToList();
                var tobeInserted = (from n in pdetails.AvailedDetailsList
                                    where !(from m in list select m.ServiceID).Contains(n.ServiceID)
                                    select n).ToList();
                var forUpdate = (from n in pdetails.AvailedDetailsList
                                 where (from m in list select m.ServiceID).Contains(n.ServiceID)
                                 select n).ToList();
                //update    
                if (forUpdate.Count != 0)
                {
                    foreach (var item in forUpdate)
                    {
                        AServices = AServices == true ? true : new Models.RequestList().IsServiceForinsert(item, req, false, pdetails.GeneralDetails.CMSCode);
                        ForInsert = ForInsert ? true : AServices;
                        //pRequestListDetails.Action = "1";
                        //new Models.ServiceOptions().Update(item);
                        //new Models.RequestList().InsertRequestServiceandDetails(item, pRequestListDetails, false, pdetails.CMSCode);
                    }
                }

                //delete 
                if (tobeDeleted.Count != 0)
                {
                    foreach (var item in tobeDeleted)
                    {
                        req.Action = "0";
                        AServices = AServices == true ? true : new Models.RequestList().IsServiceForinsert(item, req, false, pdetails.GeneralDetails.CMSCode);
                        ForInsert = ForInsert ? true : AServices;
                        //pRequestListDetails.Action = "0";
                        //new Models.ServiceOptions().Update(item);
                        //new Models.RequestList().InsertRequestServiceandDetails(item, pRequestListDetails, false, pdetails.CMSCode);
                    }
                }

                //Insert
                if (tobeInserted.Count != 0)
                {
                    foreach (var item in tobeInserted)
                    {
                        AServices = AServices == true ? true : new Models.RequestList().IsServiceForinsert(item, req, false, pdetails.GeneralDetails.CMSCode);
                        ForInsert = ForInsert ? true : AServices;
                        //pRequestListDetails.Action = "2";
                        //new Models.ServiceOptions().Insert(item);
                        //new Models.RequestList().InsertRequestServiceandDetails(item, pRequestListDetails, true, pdetails.CMSCode);
                    }
                }
            }
            else
            {
                foreach (var item in list)
                {
                    //pRequestListDetails.Action = "0";
                    AServices = AServices == true ? true : new Models.RequestList().IsServiceForinsert(item, req, false, pdetails.GeneralDetails.CMSCode);
                    ForInsert = ForInsert ? true : AServices;
                    //new Models.RequestList().InsertRequestServiceandDetails(item, pRequestListDetails, false, pdetails.CMSCode);
                }
                AServices = AServices ? true : false;
                ForInsert = ForInsert ? true : AServices;
            }

            if (ForInsert)
            {
                if (AServices)
                {
                    if (pdetails.AvailedDetailsList != null)
                    {
                        var tobeDeleted = (from n in list
                                           where !(from m in pdetails.AvailedDetailsList select m.ServiceID).Contains(n.ServiceID)
                                           select n).ToList();
                        var tobeInserted = (from n in pdetails.AvailedDetailsList
                                            where !(from m in list select m.ServiceID).Contains(n.ServiceID)
                                            select n).ToList();
                        var forUpdate = (from n in pdetails.AvailedDetailsList
                                         where (from m in list select m.ServiceID).Contains(n.ServiceID)
                                         select n).ToList();
                        //update    
                        if (forUpdate.Count != 0)
                        {
                            foreach (var item in forUpdate)
                            {
                                req.Action = "1";
                                //new Models.ServiceOptions().Update(item);
                                new Models.RequestList().InsertRequestServiceandDetails(item, req, false, intCMSCode);
                            }
                        }

                        //delete
                        if (tobeDeleted.Count != 0)
                        {
                            foreach (var item in tobeDeleted)
                            {
                                req.Action = "0";
                                //new Models.ServiceOptions().Update(item);
                                new Models.RequestList().InsertRequestServiceandDetails(item, req, false, intCMSCode);
                            }
                        }

                        //Insert
                        if (tobeInserted.Count != 0)
                        {
                            foreach (var item in tobeInserted)
                            {
                                req.Action = "2";
                                //new Models.ServiceOptions().Insert(item);
                                new Models.RequestList().InsertRequestServiceandDetails(item, req, true, intCMSCode);
                            }
                        }
                    }
                    else
                    {
                        if (list.Count != 0)
                        {
                            foreach (var item in list)
                            {
                                req.Action = "0";
                                new Models.RequestList().InsertRequestServiceandDetails(item, req, false, intCMSCode);
                            }
                        }
                    }
                }
            }

            //if (pdetails.AvailedDetailsList != null)
            //{
            // var tobeDeleted = (from n in list
            //                    where !(from m in pdetails.AvailedDetailsList select m.ServiceID).Contains(n.ServiceID)
            //                    select n).ToList();
            // var tobeInserted = (from n in pdetails.AvailedDetailsList
            //                     where !(from m in list select m.ServiceID).Contains(n.ServiceID)
            //                     select n).ToList();
            // var forUpdate = (from n in pdetails.AvailedDetailsList
            //                  where (from m in list select m.ServiceID).Contains(n.ServiceID)
            //                  select n).ToList();

            //Enrollment.RequestListDetails req = new Enrollment.RequestListDetails();
            //req.Module = "EditCMSCode";
            //req.Status = "1";
            //req.RequestCode = pRequestCode;

            //List<RequestList> reqlist = GetRequestListbyRequestCodeForApproval(pRequestCode);

            //List<int> servidlist = new List<int>();

            // foreach(var item in reqlist){
            //  if (item.AffectedTable == "ServicesAvailment"){
            //   foreach(var witem in item.WhereValues){
            //    if(witem.FieldName == "ServiceID"){
            //     servidlist.Add(witem.Value.ToInt());
            //    }       
            //   }      
            //  }
            // }

            //if (tobeDeleted.Count != 0) {
            // foreach(var item in tobeDeleted){
            //  req.Action = "0"; 
            //  bool has = servidlist.Any(x => x == item.ServiceID);
            //  if (has)
            //  {
            //   List<Enrollment.RequestListDetails> ereqlist = new List<Enrollment.RequestListDetails>();
            //   ereqlist.Add(new Enrollment.RequestListDetails
            //   {
            //    AffectedTable = "ServicesAvailment",
            //    RequestListCode = GetRequestListCodeByServiceID(reqlist, item.ServiceID),
            //    NewValues = ToStringValues(GenerateData(item))
            //   });
            //   UpdateEditofRequest(ereqlist);
            //  }
            //  else
            //  {
            //   InsertRequestServiceandDetails(item, req, false, intCMSCode);
            //  }
            // }    
            //}

            //if (tobeInserted.Count != 0) { 
            // foreach(var item in tobeInserted){
            //  req.Action = "2"; 
            //  bool has = servidlist.Any(x => x == item.ServiceID);
            //  if (has)
            //  {
            //   List<Enrollment.RequestListDetails> ereqlist = new List<Enrollment.RequestListDetails>();
            //   ereqlist.Add(new Enrollment.RequestListDetails
            //   {
            //    AffectedTable = "ServicesAvailment",
            //    RequestListCode = GetRequestListCodeByServiceID(reqlist, item.ServiceID),
            //    NewValues = ToStringValues(GenerateData(item))
            //   });
            //   UpdateEditofRequest(ereqlist);
            //  }
            //  else
            //  {
            //   InsertRequestServiceandDetails(item, req, true, intCMSCode);
            //  }
            // }
            //}

            //if (forUpdate.Count != 0)
            //{
            // foreach (var item in forUpdate)
            // {
            //  req.Action = "1";
            //  bool has = servidlist.Any(x => x == item.ServiceID);
            //  if (has){
            //   List<Enrollment.RequestListDetails> ereqlist = new List<Enrollment.RequestListDetails>();
            //   ereqlist.Add(new Enrollment.RequestListDetails{
            //     AffectedTable = "ServicesAvailment",
            //     RequestListCode = GetRequestListCodeByServiceID(reqlist, item.ServiceID),
            //     NewValues = ToStringValues(GenerateData(item))
            //   });
            //   UpdateEditofRequest(ereqlist);
            //  }
            //  else {
            //   foreach(var litem in list){
            //    if(litem.ServiceID == item.ServiceID){
            //     if(litem.MotherRequiredADB != item.MotherRequiredADB){
            //     if (litem.SubRequiredADB != item.SubRequiredADB) {
            //      if (litem.MinNumberEmployee != item.MinNumberEmployee)
            //      {
            //       InsertRequestServiceandDetails(item, req, false, intCMSCode);
            //      }          
            //     }         
            //    }         
            //    }        
            //   }
            //  }
            // }
            //}
            //}
            return intReturn;
        }

        public int GetRequestListCodeByServiceID(List<RequestList> pReqs, int pServiceID)
        {
            int intReturn = 0;
            foreach (var item in pReqs)
            {
                if (item.AffectedTable == "ServicesAvailment")
                {
                    foreach (var witem in item.WhereValues)
                    {
                        if (witem.FieldName == "ServiceID")
                        {
                            if (witem.Value.ToInt() == pServiceID)
                            {
                                intReturn = item.RequestListCode;
                            }
                        }
                    }
                }
            }
            return intReturn;
        }

        public int InsertForTerminationRequest(int pCMSCode, Models.Enrollment.RequestListDetails pReqList)
        {
            int intReturn = 0;

            //Models.RequestList reqlist = new Models.RequestList();
            //reqlist.Action = pReqList.Action;
            //reqlist.AffectedTable = pReqList.AffectedTable;
            //reqlist.Module = pReqList.Module;
            //reqlist.NewValues = ToListValues(pReqList.NewValues);
            //reqlist.OldValues = ToListValues(pReqList.OldValues);
            //reqlist.WhereValues = ToListValues(pReqList.WhereValues);
            //reqlist.Remarks = pReqList.Remarks;
            //reqlist.Status = pReqList.Status;

            string strSQL = "INSERT INTO ";
            strSQL += "RequestList ";
            strSQL += "VALUES ( ";
            strSQL += "@pRequestCode ";
            strSQL += ",@pModule ";
            strSQL += ",@pAction ";
            strSQL += ",@pNewValues ";
            strSQL += ",@pOldValues ";
            strSQL += ",@pWhereValues ";
            strSQL += ",@pAffectedTable ";
            strSQL += ",@pRemarks ";
            strSQL += ",@pStatus ";
            strSQL += ",null ";
            strSQL += ",null) ";

            using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
            {
                using (SqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = strSQL;
                    cmd.Parameters.Add(new SqlParameter("@pRequestCode", pReqList.RequestsDetails.RequestCode));
                    cmd.Parameters.Add(new SqlParameter("@pModule", pReqList.Module));
                    cmd.Parameters.Add(new SqlParameter("@pAction", pReqList.Action));
                    cmd.Parameters.Add(new SqlParameter("@pNewValues", pReqList.NewValues));
                    cmd.Parameters.Add(new SqlParameter("@pOldValues", ToStringValues(ReturnOldValues(pReqList.AffectedTable, pCMSCode, 0))));
                    cmd.Parameters.Add(new SqlParameter("@pWhereValues", pReqList.WhereValues));
                    cmd.Parameters.Add(new SqlParameter("@pAffectedTable", pReqList.AffectedTable));
                    cmd.Parameters.Add(new SqlParameter("@pRemarks", pReqList.Remarks));
                    cmd.Parameters.Add(new SqlParameter("@pStatus", pReqList.Status));
                    cn.Open();
                    intReturn = cmd.ExecuteNonQuery();
                }
            }
            SendEmailTermination(true, false, pReqList.RequestsDetails.AssignedApprover, pReqList.RequestsDetails.CreatedBy, pReqList.RequestsDetails.RequestCode);
            return intReturn;
        }

    }
}