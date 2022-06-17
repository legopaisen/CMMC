using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CMMC.Models.Batch_Computation
{
  public class BatchScheduleModel
  {
    public int ID { get; set; }
    public List<BatchProcessModel> Processes { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime ProcessDate { get; set; }
    public RunType RunType { get; set; }
    public RunStatus Status { get; set; }
    public string Remarks { get; set; }
    public bool IsActive { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public string ModifiedBy { get; set; }
    public DateTime ModifiedOn { get; set; }
  }

  public enum RunStatus
  {
    PENDING = 0,
    SUCCESS = 1,
    FAILED = 2
  }

  public enum RunType
  {
    REGULAR = 0,
    SCHEDULED = 1,
    MANUAL = 2
  }

  public class BatchSchedule
  {
    public List<BatchScheduleModel> GetList(out long pTotal, int pPageNumber, int pNumberOfItems, string pSearchKey = "", string pRunType = "")
    {
      List<BatchScheduleModel> models = new List<BatchScheduleModel>();
      pTotal = 0;

      string strSQL = "";
      strSQL += " SELECT * FROM BatchSchedule WHERE IsActive = '1'";

      if (pRunType != "3")
      {
        strSQL += " AND RunType = @pRunType";
      }

      if (pSearchKey.Trim().Length > 0)
      {
        strSQL += " AND (ID LIKE '%' + @pSearchKey + '%'";
        strSQL += " OR StartTime LIKE '%' + @pSearchKey + '%'";
        strSQL += " OR ProcessDate LIKE '%' + @pSearchKey + '%'";
      }
      strSQL += " ORDER BY StartTime DESC";
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
              BatchScheduleModel model = new BatchScheduleModel();
              model.ID = Convert.ToInt32(dr["ID"]);

              List<BatchProcessModel> processes = new List<BatchProcessModel>();
              foreach (string str in dr["ProcessIDs"].ToString().Split(':'))
              {
                processes.Add(new BatchProcessModel() { ProcessCode = Convert.ToInt32(str) });
              }

              model.Processes = processes;
              model.StartTime = Convert.ToDateTime(dr["StartTime"]);
              model.ProcessDate = Convert.ToDateTime(dr["ProcessDate"]);
              model.RunType = (RunType)Enum.Parse(typeof(RunType), dr["RunType"].ToString());
              model.Status = (RunStatus)Enum.Parse(typeof(RunStatus), dr["Status"].ToString());
              model.Remarks = Convert.ToString(dr["Remarks"]);
              model.IsActive = Convert.ToString(dr["IsActive"]).Equals("1");
              model.CreatedBy = Convert.ToString(dr["CreatedBy"]);
              model.CreatedOn = Convert.ToDateTime(dr["CreatedOn"]);
              model.ModifiedBy = Convert.ToString(dr["ModifiedBy"]);
              model.ModifiedOn = Convert.ToDateTime(dr["ModifiedOn"]);

              models.Add(model);

            }
          }

          strSQL = "";
          strSQL += " SELECT COUNT(*) FROM BatchSchedule WHERE IsActive = '1'";

          if (pRunType != "3")
          {
            strSQL += " AND RunType = @pRunType";
          }

          if (pSearchKey.Trim().Length > 0)
          {
            strSQL += " AND (ID LIKE '%' + @pSearchKey + '%'";
            strSQL += " OR StartTime LIKE '%' + @pSearchKey + '%'";
            strSQL += " OR ProcessDate LIKE '%' + @pSearchKey + '%'";
          }
          cmd.CommandText = strSQL;
          pTotal = Convert.ToInt32(cmd.ExecuteScalar());

          cn.Close();

        }
      }


      return models;
    }

    public List<BatchScheduleModel> GetList()
    {
      List<BatchScheduleModel> models = new List<BatchScheduleModel>();

      using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
      {
        using (SqlCommand cmd = cn.CreateCommand())
        {
          cmd.CommandText = " SELECT * FROM BatchSchedule WHERE IsActive = '1'";

          cn.Open();
          using (SqlDataReader dr = cmd.ExecuteReader())
          {
            while (dr.Read())
            {
              BatchScheduleModel model = new BatchScheduleModel();
              model.ID = Convert.ToInt32(dr["ID"]);

              List<BatchProcessModel> processes = new List<BatchProcessModel>();
              foreach (string str in dr["ProcessIDs"].ToString().Split(':'))
              {
                processes.Add(new BatchProcessModel() { ProcessCode = Convert.ToInt32(str) });
              }

              model.Processes = processes;
              model.StartTime = Convert.ToDateTime(dr["StartTime"]);
              model.ProcessDate = Convert.ToDateTime(dr["ProcessDate"]);
              model.RunType = (RunType)Enum.Parse(typeof(RunType), dr["RunType"].ToString());
              model.Status = (RunStatus)Enum.Parse(typeof(RunStatus), dr["Status"].ToString());
              model.Remarks = Convert.ToString(dr["Remarks"]);
              model.IsActive = Convert.ToString(dr["IsActive"]).Equals("1");
              model.CreatedBy = Convert.ToString(dr["CreatedBy"]);
              model.CreatedOn = Convert.ToDateTime(dr["CreatedOn"]);
              model.ModifiedBy = Convert.ToString(dr["ModifiedBy"]);
              model.ModifiedOn = Convert.ToDateTime(dr["ModifiedOn"]);


              models.Add(model);

            }
          }

          cn.Close();

        }
      }
      return models;
    }

    public BatchScheduleModel GetLastRun()
    {
      BatchScheduleModel model = new BatchScheduleModel();

      using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
      {
        using (SqlCommand cmd = cn.CreateCommand())
        {
          cmd.CommandText = " SELECT TOP 1 * FROM BatchSchedule WHERE Status <> '0' ORDER BY StartTime DESC";

          cn.Open();
          using (SqlDataReader dr = cmd.ExecuteReader())
          {
            if (dr.Read())
            {
              model.ID = Convert.ToInt32(dr["ID"]);

              List<BatchProcessModel> processes = new List<BatchProcessModel>();
              foreach (string str in dr["ProcessIDs"].ToString().Split(':'))
              {
                processes.Add(new BatchProcessModel() { ProcessCode = Convert.ToInt32(str) });
              }

              model.Processes = processes;
              model.StartTime = Convert.ToDateTime(dr["StartTime"]);
              model.ProcessDate = Convert.ToDateTime(dr["ProcessDate"]);
              model.RunType = (RunType)Enum.Parse(typeof(RunType), dr["RunType"].ToString());
              model.Status = (RunStatus)Enum.Parse(typeof(RunStatus), dr["Status"].ToString());
              model.Remarks = Convert.ToString(dr["Remarks"]);
              model.IsActive = Convert.ToString(dr["IsActive"]).Equals("1");
              model.CreatedBy = Convert.ToString(dr["CreatedBy"]);
              model.CreatedOn = Convert.ToDateTime(dr["CreatedOn"]);
              model.ModifiedBy = Convert.ToString(dr["ModifiedBy"]);
              model.ModifiedOn = Convert.ToDateTime(dr["ModifiedOn"]);
            }
          }


          if (model.ID > 0)
          {
            foreach (BatchProcessModel sched in model.Processes)
            {
              cmd.CommandText = " SELECT * FROM BatchProcess WHERE ProcessCode = @pProcessCode";
              cmd.Parameters.Clear();
              cmd.Parameters.Add(new SqlParameter("@pProcessCode", sched.ProcessCode));

              using (SqlDataReader dr = cmd.ExecuteReader())
              {
                if (dr.Read())
                {
                  sched.ID = Convert.ToInt32(dr["ID"]);
                  sched.ProcessName = dr["ProcessName"].ToString();
                }
              }
            }
          }
          cn.Close();

        }
      }

      return model;
    }

    public BatchScheduleModel GetNextRun()
    {
      BatchScheduleModel model = new BatchScheduleModel();

      using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
      {
        using (SqlCommand cmd = cn.CreateCommand())
        {
          cmd.CommandText = " SELECT TOP 1 * FROM BatchSchedule WHERE Status = '0' AND IsActive = '1' ORDER BY StartTime DESC";

          cn.Open();
          using (SqlDataReader dr = cmd.ExecuteReader())
          {
            if (dr.Read())
            {
              model.ID = Convert.ToInt32(dr["ID"]);

              List<BatchProcessModel> processes = new List<BatchProcessModel>();
              foreach (string str in dr["ProcessIDs"].ToString().Split(':'))
              {
                processes.Add(new BatchProcessModel() { ProcessCode = Convert.ToInt32(str) });
              }

              model.Processes = processes;
              model.StartTime = Convert.ToDateTime(dr["StartTime"]);
              model.ProcessDate = Convert.ToDateTime(dr["ProcessDate"]);
              model.RunType = (RunType)Enum.Parse(typeof(RunType), dr["RunType"].ToString());
              model.Status = (RunStatus)Enum.Parse(typeof(RunStatus), dr["Status"].ToString());
              model.Remarks = Convert.ToString(dr["Remarks"]);
              model.IsActive = Convert.ToString(dr["IsActive"]).Equals("1");
              model.CreatedBy = Convert.ToString(dr["CreatedBy"]);
              model.CreatedOn = Convert.ToDateTime(dr["CreatedOn"]);
              model.ModifiedBy = Convert.ToString(dr["ModifiedBy"]);
              model.ModifiedOn = Convert.ToDateTime(dr["ModifiedOn"]);
            }
          }

          if (model.ID > 0)
          {
            foreach (BatchProcessModel sched in model.Processes)
            {
              cmd.CommandText = " SELECT * FROM BatchProcess WHERE ProcessCode = @pProcessCode";
              cmd.Parameters.Clear();
              cmd.Parameters.Add(new SqlParameter("@pProcessCode", sched.ProcessCode));

              using (SqlDataReader dr = cmd.ExecuteReader())
              {
                if (dr.Read())
                {
                  sched.ID = Convert.ToInt32(dr["ID"]);
                  sched.ProcessName = dr["ProcessName"].ToString();
                }
              }
            }
          }

          cn.Close();

        }
      }

      return model;
    }

    public BatchScheduleModel Fill(int id)
    {
      BatchScheduleModel model = new BatchScheduleModel();

      using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
      {
        using (SqlCommand cmd = cn.CreateCommand())
        {
          cmd.CommandText = " SELECT * FROM BatchSchedule WHERE IsActive = '1' AND ID = @pID";
          cmd.Parameters.Add(new SqlParameter("@pID", id));

          cn.Open();
          using (SqlDataReader dr = cmd.ExecuteReader())
          {
            if (dr.Read())
            {
              model.ID = Convert.ToInt32(dr["ID"]);

              List<BatchProcessModel> processes = new List<BatchProcessModel>();
              foreach (string str in dr["ProcessIDs"].ToString().Split(':'))
              {
                processes.Add(new BatchProcessModel() { ProcessCode = Convert.ToInt32(str) });
              }

              model.Processes = processes;
              model.StartTime = Convert.ToDateTime(dr["StartTime"]);
              model.ProcessDate = Convert.ToDateTime(dr["ProcessDate"]);
              model.RunType = (RunType)Enum.Parse(typeof(RunType), dr["RunType"].ToString());
              model.Status = (RunStatus)Enum.Parse(typeof(RunStatus), dr["Status"].ToString());
              model.Remarks = Convert.ToString(dr["Remarks"]);
              model.IsActive = Convert.ToString(dr["IsActive"]).Equals("1");
              model.CreatedBy = Convert.ToString(dr["CreatedBy"]);
              model.CreatedOn = Convert.ToDateTime(dr["CreatedOn"]);
              model.ModifiedBy = Convert.ToString(dr["ModifiedBy"]);
              model.ModifiedOn = Convert.ToDateTime(dr["ModifiedOn"]);
            }
          }

          foreach (BatchProcessModel sched in model.Processes)
          {
            cmd.CommandText = " SELECT * FROM BatchProcess WHERE ProcessCode = @pProcessCode";
            cmd.Parameters.Clear();
            cmd.Parameters.Add(new SqlParameter("@pProcessCode", sched.ProcessCode));

            using (SqlDataReader dr = cmd.ExecuteReader())
            {
              if (dr.Read())
              {
                sched.ID = Convert.ToInt32(dr["ID"]);
                sched.ProcessName = dr["ProcessName"].ToString();
              }
            }
          }

          cn.Close();

        }
      }

      return model;
    }

    public bool HasPending(DateTime pDate, int[] pProcessCodes)
    {
      bool blnReturn = false;

      string strSQL = "";
      strSQL += " SELECT COUNT(1) FROM BatchSchedule";
      strSQL += " WHERE ProcessDate = @pProcessDate";
      strSQL += " AND IsActive = '1'";
      strSQL += " AND Status NOT IN ('1', '2')";
      strSQL += " AND RunType = '1'";
      strSQL += " AND (";
      for (int i = 0; i < pProcessCodes.Length; i++)
      {
        strSQL += (i > 0 ? " OR" : " " ) + " ':' + ProcessIDs + ':' LIKE '%:' + CONVERT(varchar(10), @pCode" + i.ToString() + ") + ':%'";
      }
      strSQL += " )";

      using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
      {
        using (SqlCommand cmd = cn.CreateCommand())
        {
          cmd.CommandText = strSQL;
          cmd.Parameters.Add(new SqlParameter("@pProcessDate", pDate.ToString("yyyy-MM-dd")));
          for (int i = 0; i < pProcessCodes.Length; i++)
          {
            cmd.Parameters.Add(new SqlParameter("@pCode" + i.ToString(), pProcessCodes[i]));
          }

          cn.Open();
          blnReturn = Convert.ToInt32(cmd.ExecuteScalar()) > 0;
          cn.Close();
        }
      }

      return blnReturn;
    }

    public int Insert(BatchScheduleModel Model)
    {
      int intReturn = 0;

      string strSQL = "";
      strSQL += " INSERT INTO";
      strSQL += " BatchSchedule";
      strSQL += " VALUES(";
      strSQL += "  @pProcessIDs";
      strSQL += " ,@pStartTime";
      strSQL += " ,@pProcessDate";
      strSQL += " ,@pRunType";
      strSQL += " ,'0'";
      strSQL += " ,@pRemarks";
      strSQL += " ,'1'";
      strSQL += " ,@pCreatedBy";
      strSQL += " ,GETDATE()";
      strSQL += " ,@pModifiedBy";
      strSQL += " ,GETDATE()";
      strSQL += " )";

      using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
      {
        using (SqlCommand cmd = cn.CreateCommand())
        {
          cmd.CommandText = strSQL;

          string ids = "";
          foreach (BatchProcessModel id in Model.Processes)
          {
            ids += id.ProcessCode.ToString() + ":";
          }

          cmd.Parameters.Add(new SqlParameter("@pProcessIDs", ids.Trim(new char[] { ':' })));
          cmd.Parameters.Add(new SqlParameter("@pStartTime", Model.StartTime));
          cmd.Parameters.Add(new SqlParameter("@pProcessDate", Model.ProcessDate));
          cmd.Parameters.Add(new SqlParameter("@pRunType", (int)RunType.SCHEDULED));
          cmd.Parameters.Add(new SqlParameter("@pRemarks", Model.Remarks));
          cmd.Parameters.Add(new SqlParameter("@pCreatedBy", Model.CreatedBy));
          cmd.Parameters.Add(new SqlParameter("@pModifiedBy", Model.ModifiedBy));

          cn.Open();
          intReturn = cmd.ExecuteNonQuery();
          cn.Close();
        }
      }
      return intReturn;
    }

    public int Update(BatchScheduleModel Model)
    {
      int intReturn = 0;

      string strSQL = "";
      strSQL += " UPDATE";
      strSQL += " BatchSchedule";
      strSQL += " SET";
      strSQL += "  ProcessIDs = @pProcessIDs";
      strSQL += " ,StartTime = @pStartTime";
      strSQL += " ,ProcessDate = @pProcessDate";
      strSQL += " ,Status = @pStatus";
      strSQL += " ,@pRemarks";
      strSQL += " ,@pModifiedBy";
      strSQL += " ,GETDATE()";
      strSQL += " WHERE ID = @pID";

      using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
      {
        using (SqlCommand cmd = cn.CreateCommand())
        {
          cmd.CommandText = strSQL;

          string ids = "";
          foreach (BatchProcessModel id in Model.Processes)
          {
            ids += id.ProcessCode.ToString() + ":";
          }

          cmd.Parameters.Add(new SqlParameter("@pProcessIDs", ids.Trim(new char[] { ':' })));
          cmd.Parameters.Add(new SqlParameter("@pStartTime", Model.StartTime));
          cmd.Parameters.Add(new SqlParameter("@pProcessDate", Model.ProcessDate));
          cmd.Parameters.Add(new SqlParameter("@pStatus", (int)Model.Status));
          cmd.Parameters.Add(new SqlParameter("@pRemarks", Model.Remarks));
          cmd.Parameters.Add(new SqlParameter("@pModifiedBy", Model.ModifiedBy));
          cmd.Parameters.Add(new SqlParameter("@pID", Model.ID));

          cn.Open();
          intReturn = cmd.ExecuteNonQuery();
          cn.Close();
        }
      }

      return intReturn;
    }

    public int Delete(BatchScheduleModel Model)
    {
      int intReturn = 0;

      string strSQL = "";
      strSQL += " UPDATE";
      strSQL += " BatchSchedule";
      strSQL += " SET";
      strSQL += "  IsActive = '0'";
      strSQL += " ,ModifiedBy = @pModifiedBy";
      strSQL += " ,ModifiedOn = GETDATE()";
      strSQL += " WHERE ID = @pID";

      using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
      {
        using (SqlCommand cmd = cn.CreateCommand())
        {
          cmd.CommandText = strSQL;

          cmd.Parameters.Add(new SqlParameter("@pModifiedBy", Model.ModifiedBy));
          cmd.Parameters.Add(new SqlParameter("@pID", Model.ID));

          cn.Open();
          intReturn = cmd.ExecuteNonQuery();
          cn.Close();
        }
      }

      return intReturn;
    }

    public static DateTime GetPreviousBankingDay(DateTime pCurrentDate, bool pIsScheduleEveryday)
    {
      DateTime previousDay = pCurrentDate; 
      int intDayMinus;
      bool isBankingDay = false;

      int rows = 0;

      try
      {
     
        if (!pIsScheduleEveryday)
        {
          if (pCurrentDate.DayOfWeek == DayOfWeek.Monday) //if equals on monday
          {
            previousDay = pCurrentDate.AddDays(-3); //-3 days
            intDayMinus = 4;
          }
          else
          {
            previousDay = pCurrentDate.AddDays(-1);// - 1 day
            intDayMinus = 2;
          }

          while (!isBankingDay) //not banking day
          {
            isBankingDay = true;

            string strSQL = "";
            strSQL += " SELECT COUNT(*) FROM CMMC_HolidaysTbl";
            strSQL += " WHERE hd_Date = @pDate";

            using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
            {
              using (SqlCommand cmd = cn.CreateCommand())
              {
                cmd.CommandText = strSQL;
                cmd.Parameters.Add(new SqlParameter("@pDate", previousDay.ToString("yyyy-MM-dd")));

                cn.Open();
                rows = Convert.ToInt32(cmd.ExecuteScalar());
                cn.Close();
              }
            }

            if (rows > 0) //rows greater than 0 = true
            {
              previousDay = pCurrentDate.AddDays(-intDayMinus);
              isBankingDay = false;
              intDayMinus += 1;
            }

            if (_isWeekend(previousDay)) //check if the day of batch is weekend = true
            {
              previousDay = pCurrentDate.AddDays(-intDayMinus);
              isBankingDay = false;
              intDayMinus += 1;
            }
          }
        }
        else
        {
          previousDay = pCurrentDate.AddDays(-1);
          isBankingDay = true;
        }
      }
      catch (Exception ex)
      {
       CTBC.Logs.Write("Get Previous Banking Day", ex.Message, "BatchSchedule");

      }

      return previousDay;
    }

    public static DateTime[] GetDatesofWithdrawalChargesToBeGenerated(DateTime pCurrentDate, DateTime pPreviousDate)
    {
      List<DateTime> dates = new List<DateTime>();

      DateTime previousDay = pCurrentDate.AddDays(-1);//DateTime.Now.AddDays(-1);
      bool isBankingDay = false; //default
      int rows = 0;

      string strSQL = "";
      strSQL += " SELECT COUNT(*) FROM CMMC_HolidaysTbl";
      strSQL += " WHERE hd_Date = @pDate";

      using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
      {
        using (SqlCommand cmd = cn.CreateCommand())
        {
          cmd.CommandText = strSQL;
          cmd.Parameters.Add(new SqlParameter("@pDate", previousDay.ToString("yyyy-MM-dd")));

          cn.Open();
          rows = Convert.ToInt32(cmd.ExecuteScalar());
          cn.Close();

        }
      }

      if (rows > 0 || _isWeekend(previousDay)) //row is greater than 0 and isweekend is true
      {
        isBankingDay = false;
        dates = new List<DateTime>() { pPreviousDate };
        return dates.ToArray();
      }

      else //if false
      {
        isBankingDay = true;
      }

      previousDay = pCurrentDate.AddDays(-1);
      dates = new List<DateTime>() { previousDay };

      if (isBankingDay) //if banking day is true
      {
        previousDay = pCurrentDate.AddDays(-2); //the previousday is = to the pcurrentdate -2
        isBankingDay = false; 

        while (!isBankingDay) //is not true
        {
          isBankingDay = false;

          strSQL = "";
          strSQL += " SELECT COUNT(*) FROM CMMC_HolidaysTbl";
          strSQL += " WHERE hd_Date = @pDate"; //check the CMMC_holiday tbl if the date today is = to the previousday variable

          using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
          {
            using (SqlCommand cmd = cn.CreateCommand())
            {
              cmd.CommandText = strSQL;
              cmd.Parameters.Add(new SqlParameter("@pDate", previousDay.ToString("yyyy-MM-dd")));

              cn.Open();
              rows = Convert.ToInt32(cmd.ExecuteScalar());
              cn.Close();
            }
          }

          if (rows > 0 || _isWeekend(previousDay)) //
          {
            dates.Add(previousDay);
            previousDay = previousDay.AddDays(-1);
          }
          else
          {
            isBankingDay = true;
          }
        }
      }

      return dates.ToArray();
    }


   //check if weekend
    private static bool _isWeekend(DateTime pDate)
    {
      bool blnReturn = false;

      blnReturn = pDate.DayOfWeek == DayOfWeek.Sunday || pDate.DayOfWeek == DayOfWeek.Saturday;

      return blnReturn;
    }

  }
}