using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CMMC.Models.Batch_Computation
{
  public class BatchRunHistoryModel
  {
    public int ID { get; set; }
    public BatchProcessModel Task { get; set; }
    public int RunGroupID { get; set; }
    public DateTime DateProcessed { get; set; }
    public string RunType { get; set; }
    public int Status { get; set; }
    public string RunBy { get; set; }
    public DateTime RunDate { get; set; }
    public DateTime? EndDate { get; set; }
  }

  public class BatchRunHistory
  {
    public static BatchRunHistoryModel GetCurrentlyRunningProcess()
    {
      BatchRunHistoryModel model = new BatchRunHistoryModel();

      string strSQL = "";
      strSQL += " SELECT TOP 1";
      strSQL += "  r.ID";
      strSQL += " ,r.TaskID";
      strSQL += " ,r.RunGroupID";
      strSQL += " ,p.ProcessName";
      strSQL += " ,r.DateProcessed";
      strSQL += " ,r.RunType";
      strSQL += " ,r.Status";
      strSQL += " ,r.RunBy";
      strSQL += " ,r.RunDate";
      strSQL += " ,r.EndDate";
      strSQL += " FROM BatchRunHistory r";
      strSQL += " LEFT JOIN BatchProcess p ON r.TaskID = p.ProcessCode";
      strSQL += " WHERE Status = '2' ORDER BY RunDate DESC";
      
      using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
      {
        using (SqlCommand cmd = cn.CreateCommand())
        {
          cmd.CommandText = strSQL;

          cn.Open();
          using (SqlDataReader dr = cmd.ExecuteReader())
          {
            if (dr.Read())
            {
              model.ID = Convert.ToInt32(dr["ID"]);
              model.Task = new BatchProcessModel()
              {
                ProcessCode = Convert.ToInt32(dr["TaskID"]),
                ProcessName = dr["ProcessName"].ToString()
              };
              model.RunGroupID = Convert.ToInt32(dr["RunGroupID"]);
              model.DateProcessed = Convert.ToDateTime(dr["DateProcessed"]);
              model.RunType = dr["RunType"].ToString();
              model.Status = Convert.ToInt32(dr["Status"]);
              model.RunBy = dr["RunBy"].ToString();
              model.RunDate = Convert.ToDateTime(dr["RunDate"]);

              if (string.IsNullOrEmpty(Convert.ToString(dr["EndDate"])))
              {
                model.EndDate = null;
              }
              else
              {
                model.EndDate = Convert.ToDateTime(dr["EndDate"]);
              }
              
            }
            else
            {
              model = null;
            }
          }
          cn.Close();
        }
      }

      return model;
    }

    public List<BatchRunHistoryModel> GetList(out long pTotal, int pPageNumber, int pNumberOfItems, string pSearchKey = "", string pRunType = "")
    {
      List<BatchRunHistoryModel> models = new List<BatchRunHistoryModel>();
      pTotal = 0;

      string strSQL = "";
      strSQL += " SELECT";
      strSQL += "  r.ID";
      strSQL += " ,r.TaskID";
      strSQL += " ,r.RunGroupID";
      strSQL += " ,p.ProcessName";
      strSQL += " ,r.DateProcessed";
      strSQL += " ,r.RunType";
      strSQL += " ,r.Status";
      strSQL += " ,r.RunBy";
      strSQL += " ,r.RunDate";
      strSQL += " ,r.EndDate";
      strSQL += " FROM BatchRunHistory r";
      strSQL += " LEFT JOIN BatchProcess p ON r.TaskID = p.ProcessCode";

      if (pSearchKey.Trim().Length > 0 || pRunType != "A")
      {
        strSQL += " WHERE";
      }

      if (pRunType != "A")
      {
        strSQL += " RunType = @pRunType";
      }

      if (pSearchKey.Trim().Length > 0)
      {
        strSQL += (pRunType != "A" ? " " : " AND") + " (r.TaskID LIKE '%' + @pSearchKey + '%'";
        strSQL += " OR p.ProcessName LIKE '%' + @pSearchKey + '%'";
        strSQL += " OR CONVERT(varchar(10), r.DateProcessed) LIKE '%' + @pSearchKey + '%'";
        strSQL += " OR CONVERT(varchar(10), r.RunDate) LIKE '%' + @pSearchKey + '%'";
        strSQL += " OR CONVERT(varchar(10), r.EndDate) LIKE '%' + @pSearchKey + '%'";
        strSQL += " OR r.RunType LIKE '%' + @pSearchKey + '%'";
        strSQL += " OR r.RunBy LIKE '%' + @pSearchKey + '%'";
        strSQL += " )";
      }

      strSQL += " ORDER BY r.RunDate DESC";
      strSQL += " OFFSET (@pPageNumber)*@pNumberOfItems ROWS ";
      strSQL += " FETCH NEXT @pNumberOfItems ROWS ONLY";

      using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
      {
        using (SqlCommand cmd = cn.CreateCommand())
        {
          cmd.CommandText = strSQL;
          cmd.Parameters.Add(new SqlParameter("@pPageNumber", pPageNumber));
          cmd.Parameters.Add(new SqlParameter("@pNumberOfItems", pNumberOfItems));
          cmd.Parameters.Add(new SqlParameter("@pRunType", pRunType));
          if (pSearchKey.Trim().Length > 0)
          {
            cmd.Parameters.Add(new SqlParameter("@pSearchKey", pSearchKey));
          }

          cn.Open();
          using (SqlDataReader dr = cmd.ExecuteReader())
          {
            while (dr.Read())
            {
              BatchRunHistoryModel model = new BatchRunHistoryModel();

              model.ID = Convert.ToInt32(dr["ID"]);
              model.Task = new BatchProcessModel()
              {
                ProcessCode = Convert.ToInt32(dr["TaskID"]),
                ProcessName = dr["ProcessName"].ToString()
              };
              model.RunGroupID = Convert.ToInt32(dr["RunGroupID"]);
              model.DateProcessed = Convert.ToDateTime(dr["DateProcessed"]);
              model.RunType = dr["RunType"].ToString();
              model.Status = Convert.ToInt32(dr["Status"]);
              model.RunBy = dr["RunBy"].ToString();
              model.RunDate = Convert.ToDateTime(dr["RunDate"]);
              if (string.IsNullOrEmpty(Convert.ToString(dr["EndDate"])))
              {
                model.EndDate = null;
              }
              else
              {
                model.EndDate = Convert.ToDateTime(dr["EndDate"]);
              }
              models.Add(model);
            }
          }

          strSQL = "";
          strSQL += " SELECT COUNT(*)";
          strSQL += " FROM BatchRunHistory r";
          strSQL += " LEFT JOIN BatchProcess p ON r.TaskID = p.ProcessCode";

          if (pSearchKey.Trim().Length > 0 || pRunType != "A")
          {
            strSQL += " WHERE";
          }

          if (pRunType != "A")
          {
            strSQL += " RunType = @pRunType";
          }

          if (pSearchKey.Trim().Length > 0)
          {
            strSQL += (pRunType != "A" ? " " : " AND") + " (r.TaskID LIKE '%' + @pSearchKey + '%'";
            strSQL += " OR p.ProcessName LIKE '%' + @pSearchKey + '%'";
            strSQL += " OR CONVERT(varchar(10), r.RunDate) LIKE '%' + @pSearchKey + '%'";
            strSQL += " OR CONVERT(varchar(10), r.EndDate) LIKE '%' + @pSearchKey + '%'";
            strSQL += " OR r.RunType LIKE '%' + @pSearchKey + '%'";
            strSQL += " OR r.RunBy LIKE '%' + @pSearchKey + '%'";
            strSQL += ")";
          }

          cmd.CommandText = strSQL;
          pTotal = Convert.ToInt32(cmd.ExecuteScalar());

          cn.Close();
        }
      }
      return models;
    }

    public static List<BatchRunHistoryModel> GetList()
    {
      List<BatchRunHistoryModel> models = new List<BatchRunHistoryModel>();

      string strSQL = "";
      strSQL += " SELECT";
      strSQL += "  r.ID";
      strSQL += " ,r.TaskID";
      strSQL += " ,r.RunGroupID";
      strSQL += " ,p.ProcessName";
      strSQL += " ,r.DateProcessed";
      strSQL += " ,r.RunType";
      strSQL += " ,r.Status";
      strSQL += " ,r.RunBy";
      strSQL += " ,r.RunDate";
      strSQL += " ,r.EndDate";
      strSQL += " FROM BatchRunHistory r";
      strSQL += " LEFT JOIN BatchProcess p ON r.TaskID = p.ProcessCode";
      strSQL += " ORDER BY r.RunDate DESC";

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
              BatchRunHistoryModel model = new BatchRunHistoryModel();

              model.ID = Convert.ToInt32(dr["ID"]);
              model.Task = new BatchProcessModel()
              {
                ProcessCode = Convert.ToInt32(dr["TaskID"]),
                ProcessName = dr["ProcessName"].ToString()
              };
              model.RunGroupID = Convert.ToInt32(dr["RunGroupID"]);
              model.DateProcessed = Convert.ToDateTime(dr["DateProcessed"]);
              model.RunType = dr["RunType"].ToString();
              model.Status = Convert.ToInt32(dr["Status"]);
              model.RunBy = dr["RunBy"].ToString();
              model.RunDate = Convert.ToDateTime(dr["RunDate"]);
              if (string.IsNullOrEmpty(Convert.ToString(dr["EndDate"])))
              {
                model.EndDate = null;
              }
              else
              {
                model.EndDate = Convert.ToDateTime(dr["EndDate"]);
              }

              models.Add(model);
            }
          }
          cn.Close();
        }
      }
      return models;
    }

    public static bool HasFailedRun()
    {
      bool blnReturn = false; //DEFAULT

      string strSQL = "";
      strSQL += " SELECT COUNT(1) FROM BatchRunHistory WHERE Status = '1'"; //Count all BatchRUnHistory where Status is 1

      using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
      {
        using (SqlCommand cmd = cn.CreateCommand())
        {
          cmd.CommandText = strSQL;

          cn.Open();
          blnReturn = Convert.ToInt32(cmd.ExecuteScalar()) > 0; //if COUNT is more than 0 = true.. if not false
          cn.Close();
        }
      }

      return blnReturn;
    }

    public static List<BatchRunHistoryModel> GetFailedRun(int pRunGroupID)
    {
      List<BatchRunHistoryModel> models = new List<BatchRunHistoryModel>();

      string strSQL = "";
      strSQL += " SELECT";
      strSQL += "  r.ID";
      strSQL += " ,r.TaskID";
      strSQL += " ,r.RunGroupID";
      strSQL += " ,p.ProcessName";
      strSQL += " ,r.DateProcessed";
      strSQL += " ,r.RunType";
      strSQL += " ,r.Status";
      strSQL += " ,r.RunBy";
      strSQL += " ,r.RunDate";
      strSQL += " ,r.EndDate";
      strSQL += " FROM BatchRunHistory r";
      strSQL += " LEFT JOIN BatchProcess p ON r.TaskID = p.ProcessCode";
      strSQL += " WHERE RunGroupID = @pRunGroupID";
      strSQL += " ORDER BY r.ID";

      using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
      {
        using (SqlCommand cmd = cn.CreateCommand())
        {
          cmd.CommandText = strSQL;
          cmd.Parameters.Add(new SqlParameter("@pRunGroupID", pRunGroupID));

          cn.Open();
          using (SqlDataReader dr = cmd.ExecuteReader())
          {
            while (dr.Read())
            {
              BatchRunHistoryModel model = new BatchRunHistoryModel();

              model.ID = Convert.ToInt32(dr["ID"]);
              model.Task = new BatchProcessModel()
              {
                ProcessCode = Convert.ToInt32(dr["TaskID"]),
                ProcessName = dr["ProcessName"].ToString()
              };
              model.RunGroupID = Convert.ToInt32(dr["RunGroupID"]);
              model.DateProcessed = Convert.ToDateTime(dr["DateProcessed"]);
              model.RunType = dr["RunType"].ToString();
              model.Status = Convert.ToInt32(dr["Status"]);
              model.RunBy = dr["RunBy"].ToString();
              model.RunDate = Convert.ToDateTime(dr["RunDate"]);
              if (string.IsNullOrEmpty(Convert.ToString(dr["EndDate"])))
              {
                model.EndDate = null;
              }
              else
              {
                model.EndDate = Convert.ToDateTime(dr["EndDate"]);
              }

              models.Add(model);
            }
          }
          cn.Close();

        }
      }

      return models;
    }
  }
}