using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using CTBC;

namespace SYS_MATRIX.Models
{
 public class SYS_GROUP_MODEL
 {
  public int Group_Code { get; set; }
  public string Group_Name { get; set; }
  public string Description { get; set; }
  public bool IsActive { get; set; }
  public string CreatedBy { get; set; }
  public DateTime CreatedOn { get; set; }
  public string ModifiedBy { get; set; }
  public DateTime ModifiedOn { get; set; }
 }

 public class SYS_GROUP : IDisposable
 {
  public void Dispose() { GC.SuppressFinalize(this); }

  public SYS_GROUP_MODEL Fill(int pGroupCode)
  {
   SYS_GROUP_MODEL group = new SYS_GROUP_MODEL();
   try
   {
    using (SqlConnection cn = new SqlConnection(CMMC.Models.SharedFunctions.Connectionstring))
    {
     using (SqlCommand cmd = cn.CreateCommand())
     {
      cmd.CommandText = "SELECT * FROM SYS_GROUPS WHERE Group_Code = @pGroupCode";
      cmd.Parameters.Add(new SqlParameter("@pGroupCode", pGroupCode));

      cn.Open();
      using (SqlDataReader reader = cmd.ExecuteReader())
      {
       if (reader.Read())
       {
        group.Group_Code = reader["Group_Code"].ToInt();
        group.Group_Name = reader["Group_Name"].ToString();
        group.Description = reader["Description"].ToString();
        group.IsActive = reader["IsActive"].ToString().Equals("1");
        group.CreatedBy = reader["CreatedBy"].ToString();
        group.CreatedOn = reader["CreatedOn"].ToDateTime();
        group.ModifiedBy = reader["ModifiedBy"].ToString();
        if (!string.IsNullOrEmpty(reader["ModifiedOn"].ToString()))
        { group.ModifiedOn = reader["ModifiedOn"].ToDateTime(); }
       }
      }
     }
    }
   }
   catch (Exception ex)
   {
    CTBC.Logs.Write("Fill", ex.Message, "SYS_GROUP");
   }
   return group;
  }

  public int GroupProcessAction(List<int> IDs, bool IsActivate, string DeactivatedBy)
  {
   int intReturn = 0;
   string strSQL = "";

   strSQL += "UPDATE SYS_GROUPS ";
   strSQL += "SET ";
   strSQL += "IsActive = @pIsActive ";
   strSQL += ",ModifiedBy = @pModifiedBy ";
   strSQL += ",ModifiedOn = GETDATE() ";
   strSQL += "WHERE ";
   strSQL += "Group_Code = @pGroupCode ";

   using (SqlConnection cn = new SqlConnection(CMMC.Models.SharedFunctions.Connectionstring))
   {
    using (SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = strSQL;
     cn.Open();
     foreach (int intIDs in IDs)
     {
      cmd.Parameters.Clear();
      cmd.Parameters.Add(new SqlParameter("@pGroupCode", intIDs));
      cmd.Parameters.Add(new SqlParameter("@pIsActive", IsActivate ? '1' : '0'));
      cmd.Parameters.Add(new SqlParameter("@pModifiedBy", DeactivatedBy));
      intReturn += cmd.ExecuteNonQuery();
     }
    }
   }

   return intReturn;
  }

  public int Delete(string pID)
  {
   int intReturn = 0;
   string strSQL = "DELETE SYS_GROUPS WHERE Group_Code = @pCode";

   using (SqlConnection cn = new SqlConnection(CMMC.Models.SharedFunctions.Connectionstring))
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

  public int Insert(SYS_GROUP_MODEL Model)
  {
   int intReturn = 0;
   try
   {
    string strSQL = "";
    strSQL += " INSERT INTO ";
    strSQL += " SYS_GROUPS";
    strSQL += " VALUES(";
    strSQL += " @pGroupName,";
    strSQL += " @pDescription,";
    strSQL += " @pIsActive,";
    strSQL += " @pCreatedBy,";
    strSQL += " GETDATE(),";
    strSQL += " null,";
    strSQL += " null";
    strSQL += " )";

    using (SqlConnection cn = new SqlConnection(CMMC.Models.SharedFunctions.Connectionstring))
    {
     using (SqlCommand cmd = cn.CreateCommand())
     {
      cmd.CommandText = strSQL;
      cmd.Parameters.Add(new SqlParameter("@pGroupName", Model.Group_Name));
      cmd.Parameters.Add(new SqlParameter("@pDescription", Model.Description));
      cmd.Parameters.Add(new SqlParameter("@pIsActive", Model.IsActive ? '1' : '0'));
      cmd.Parameters.Add(new SqlParameter("@pCreatedBy", Model.CreatedBy));

      cn.Open();
      intReturn = cmd.ExecuteNonQuery();
     }
    }
   }
   catch (Exception ex)
   {
    CTBC.Logs.Write("Insert", ex.Message, "SYS_GROUP");
   }
   return intReturn;
  }

  public int Update(SYS_GROUP_MODEL Model)
  {
   int intReturn = 0;
   try
   {
    string strSQL = "";
    strSQL += " UPDATE ";
    strSQL += " SYS_GROUPS";
    strSQL += " SET";
    strSQL += " Group_Name = @pGroupName,";
    strSQL += " Description = @pDescription,";
    strSQL += " IsActive = @pIsActive,";
    strSQL += " ModifiedBy = @pModifiedBy,";
    strSQL += " ModifiedOn = GETDATE()";
    strSQL += " WHERE Group_Code = @pGroupCode";

    using (SqlConnection cn = new SqlConnection(CMMC.Models.SharedFunctions.Connectionstring))
    {
     using (SqlCommand cmd = cn.CreateCommand())
     {
      cmd.CommandText = strSQL;
      cmd.Parameters.Add(new SqlParameter("@pGroupName", Model.Group_Name));
      cmd.Parameters.Add(new SqlParameter("@pDescription", Model.Description));
      cmd.Parameters.Add(new SqlParameter("@pIsActive", Model.IsActive ? '1' : '0'));
      cmd.Parameters.Add(new SqlParameter("@pModifiedBy", Model.ModifiedBy));
      cmd.Parameters.Add(new SqlParameter("@pGroupCode", Model.Group_Code));

      cn.Open();
      intReturn = cmd.ExecuteNonQuery();
     }
    }
   }
   catch (Exception ex)
   {
    CTBC.Logs.Write("Update", ex.Message, "SYS_GROUP");
   }
   return intReturn;
  }

  public List<SYS_GROUP_MODEL> GetList()
  {
   List<SYS_GROUP_MODEL> list = new List<SYS_GROUP_MODEL>();

   string strSQL = "";
   strSQL += "SELECT ";
   strSQL += " Group_Code ";
   strSQL += " ,Group_Name ";
   strSQL += " ,Description ";
   strSQL += " ,IsActive ";
   strSQL += " ,CreatedBy ";
   strSQL += " ,CreatedOn ";
   strSQL += " ,ModifiedBy ";
   strSQL += " ,ModifiedOn ";
   strSQL += " FROM SYS_GROUPS ";

   using (SqlConnection cn = new SqlConnection(CMMC.Models.SharedFunctions.Connectionstring))
   {
    using (SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = strSQL;
     cn.Open();
     using (SqlDataReader dr = cmd.ExecuteReader())
     {
      while (dr.Read())
      {
       list.Add(new SYS_GROUP_MODEL()
       {
        Group_Code = (dr["Group_Code"].ToString()).ToInt(),
        Group_Name = dr["Group_Name"].ToString(),
        Description = dr["Description"].ToString(),
        IsActive = dr["IsActive"].ToString().Equals("1"),
        CreatedBy = dr["CreatedBy"].ToString(),
        CreatedOn = dr["CreatedOn"].ToDateTime(),
        ModifiedBy = dr["ModifiedBy"].ToString(),
        ModifiedOn = dr["ModifiedOn"].ToString().ToDateTime()
       });
      }
     }
    }
   }
   return list;
  }
 }
}