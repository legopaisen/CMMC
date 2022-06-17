using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CMMC.Models.Batch_Computation
{
  public class BatchProcessModel
  {
    public int ID { get; set; }
    public int ProcessCode { get; set; }
    public string ProcessName { get; set; }
    public string Description { get; set; }
  }

  public class BatchProcess
  {
    public List<BatchProcessModel> GetList()
    {
      List<BatchProcessModel> models = new List<BatchProcessModel>();

      using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
      {
        using (SqlCommand cmd = cn.CreateCommand())
        {
          cmd.CommandText = " SELECT * FROM BatchProcess";

          cn.Open();
          using (SqlDataReader dr = cmd.ExecuteReader())
          {
            while (dr.Read())
            {
              BatchProcessModel model = new BatchProcessModel();
              model.ID = Convert.ToInt32(dr["ID"]);
              model.ProcessCode = Convert.ToInt32(dr["ProcessCode"]);
              model.ProcessName = Convert.ToString(dr["ProcessName"]);
              model.Description = dr["Description"].ToString();             

              models.Add(model);
            }
          }
          cn.Close();
        }
      }

      return models;
    }

    public BatchProcessModel Fill(int pID)
    {
      BatchProcessModel model = new BatchProcessModel();

      using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
      {
        using (SqlCommand cmd = cn.CreateCommand())
        {
          cmd.CommandText = " SELECT * FROM BatchProcess WHERE ID = @pID";
          cmd.Parameters.Add(new SqlParameter("@pID", pID));

          cn.Open();
          using (SqlDataReader dr = cmd.ExecuteReader())
          {
            while (dr.Read())
            {
              model.ID = Convert.ToInt32(dr["ID"]);
              model.ProcessCode = Convert.ToInt32(dr["ProcessCode"]);
              model.ProcessName = Convert.ToString(dr["ProcessName"]);
              model.Description = dr["Description"].ToString();
            }
          }
          cn.Close();
        }
      }
      return model;      
    }
  }
}