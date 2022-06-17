using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using CTBC;

namespace CMMC.Models
{
 public class ProductTypes : IDisposable
 {
  public void Dispose() { GC.SuppressFinalize(this); }

  public struct ProductDetails
  {
   public string ProductCode { get; set; }
   public string ProductDescription { get; set; }
   public string ProductCurrency { get; set; }
   public string ProductCost { get; set; }
   public string Type { get; set; }
   public string ProductType { get; set; }
   public string ProductGroup { get; set; }
   public string CreatedBy { get; set; }
   public DateTime CreatedOn { get; set; }
   public string ModifiedBy { get; set; }
   public DateTime? ModifiedOn { get; set; }
  }

  public ProductDetails Fill (string pCode)
  {
   ProductDetails productdetails = new ProductDetails();
   using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
   {
    using (SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = "SELECT * FROM CMMC_ProductTypes WHERE PT_ProductCode = @pCode";
     cmd.Parameters.Add(new SqlParameter("@pCode", pCode));
     cn.Open();
     using (SqlDataReader dr = cmd.ExecuteReader())
     {
      if (dr.Read())
      {
       productdetails.ProductCode = dr["PT_ProductCode"].ToString();
       productdetails.ProductDescription = dr["PT_Description"].ToString();
       productdetails.ProductCurrency = dr["PT_Currency"].ToString();
       productdetails.ProductCost = dr["PT_Cost"].ToString();
       productdetails.Type = dr["PT_Type"].ToString();
       productdetails.ProductType= dr["PT_ProductType"].ToString();
       productdetails.ProductGroup = dr["PT_ProductGroup"].ToString();
       productdetails.CreatedBy = dr["PT_CreatedBy"].ToString();
       productdetails.CreatedOn = dr["PT_CreatedOn"].ToString().ToDateTime();
       productdetails.ModifiedBy = dr["PT_ModifiedBy"].ToString();
       productdetails.ModifiedOn = dr["PT_ModifiedOn"].ToString().ToDateTimeParse();
      }
     }
    }
   }
   return productdetails;
  }

  public List<ProductDetails> GetList()
  {
   List<ProductDetails> list = new List<ProductDetails>();
   string strSQL = "";
   strSQL += "SELECT ";
   strSQL += " PT_ProductCode ";
   strSQL += " ,PT_Description ";
   strSQL += " ,PT_Currency ";
   strSQL += " ,PT_Cost ";
   strSQL += " ,PT_Type ";
   strSQL += " ,PT_ProductType ";
   strSQL += " ,PT_ProductGroup ";
   strSQL += " ,PT_CreatedBy ";
   strSQL += " ,PT_CreatedOn ";
   strSQL += " ,PT_ModifiedBy ";
   strSQL += " ,PT_ModifiedOn ";
   strSQL += " FROM CMMC_ProductTypes ";
   strSQL += " ORDER BY PT_ProductCode, PT_Description ";


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
       list.Add(new ProductDetails(){
        ProductCode = dr["PT_ProductCode"].ToString(),
        ProductDescription = dr["PT_Description"].ToString(),
        ProductCurrency = dr["PT_Currency"].ToString(),
        ProductCost = dr["PT_Cost"].ToString(),
        Type = dr["PT_Type"].ToString(),
        ProductType = dr["PT_ProductType"].ToString(),
        ProductGroup = dr["PT_ProductGroup"].ToString(),
        CreatedBy = dr["PT_CreatedBy"].ToString(),
        CreatedOn = dr["PT_CreatedOn"].ToString().ToDateTime(),
        ModifiedBy = dr["PT_ModifiedBy"].ToString(),
        ModifiedOn = dr["PT_ModifiedOn"].ToString().ToDateTimeParse()
       });
      }
     }
    }
   }
   return list;
  }

  public int Insert(ProductDetails pDetails)
  {
   int intReturn = 0;
   string strSQL = "";
   strSQL += "INSERT INTO CMMC_ProductTypes ";
   strSQL += "VALUES ";
   strSQL += "( ";
   strSQL += "@pProductCode ";
   strSQL += " ,@pDescription ";
   strSQL += " ,@pCurrency ";
   strSQL += " ,@pCost ";
   strSQL += " ,@pType ";
   strSQL += " ,@pProductType ";
   strSQL += " ,@pProductGroup ";
   strSQL += " ,@pCreatedBy ";
   strSQL += " ,@pCreatedOn ";
   strSQL += " ,NULL ";
   strSQL += " ,NULL) ";

   if (!this.IsExist(pDetails.ProductCode))
   {
    
     using (SqlConnection cn = new SqlConnection(CMMC.Models.SharedFunctions.Connectionstring))
     {
      using (SqlCommand cmd = cn.CreateCommand())
      {
       cmd.CommandText = strSQL;
       cmd.Parameters.Add(new SqlParameter("@pProductCode", pDetails.ProductCode));
       cmd.Parameters.Add(new SqlParameter("@pDescription", pDetails.ProductDescription));
       cmd.Parameters.Add(new SqlParameter("@pCurrency", pDetails.ProductCurrency));
       cmd.Parameters.Add(new SqlParameter("@pCost", pDetails.ProductCost));
       cmd.Parameters.Add(new SqlParameter("@pType", pDetails.Type));
       cmd.Parameters.Add(new SqlParameter("@pProductType", pDetails.ProductType));
       cmd.Parameters.Add(new SqlParameter("@pProductGroup", pDetails.ProductGroup));
       cmd.Parameters.Add(new SqlParameter("@pCreatedBy", pDetails.CreatedBy));
       cmd.Parameters.Add(new SqlParameter("@pCreatedOn", pDetails.CreatedOn));
       cn.Open();
       intReturn = cmd.ExecuteNonQuery();
      }
     }
    }
   
   
   return intReturn;
  }

  public int Update (ProductDetails pDetails)
  {
   int intReturn = 0;
   string strSQL = "";
   strSQL += "UPDATE CMMC_ProductTypes ";
   strSQL += "SET ";
   //strSQL += "PT_ProductCode = @pProductCode ";
   strSQL += " PT_Description = @pDescription ";
   strSQL += " ,PT_Currency = @pCurrency ";
   strSQL += " ,PT_Cost = @pCost ";
   strSQL += " ,PT_Type = @pType ";
   strSQL += " ,PT_ProductType = @pProductType ";
   strSQL += " ,PT_ProductGroup = @pProductGroup ";
   strSQL += " ,PT_ModifiedBy = @pModifiedBy ";
   strSQL += " ,PT_ModifiedOn = GETDATE() ";
   strSQL += " WHERE ";
   strSQL += " PT_ProductCode = @pProductCode ";

   using (SqlConnection cn = new SqlConnection(SharedFunctions.Connectionstring))
   {
    using (SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = strSQL;
     cmd.Parameters.Add(new SqlParameter("@pDescription", pDetails.ProductDescription));
     cmd.Parameters.Add(new SqlParameter("@pCurrency", pDetails.ProductCurrency));
     cmd.Parameters.Add(new SqlParameter("@pCost", pDetails.ProductCost));
     cmd.Parameters.Add(new SqlParameter("@pType", pDetails.Type));
     cmd.Parameters.Add(new SqlParameter("@pProductType", pDetails.ProductType));
     cmd.Parameters.Add(new SqlParameter("@pModifiedBy", pDetails.ModifiedBy));
     cmd.Parameters.Add(new SqlParameter("@pProductCode", pDetails.ProductCode));
     cmd.Parameters.Add(new SqlParameter("@pProductGroup", pDetails.ProductGroup));
     cn.Open();
     intReturn = cmd.ExecuteNonQuery();
    }
   }
   return intReturn;
  }

  public bool IsExist(string pProductCode)
  {
   bool blnReturn = false;
   using (SqlConnection cn = new SqlConnection(CMMC.Models.SharedFunctions.Connectionstring))
   {
    using (SqlCommand cmd = cn.CreateCommand())
    {
     cmd.CommandText = "SELECT COUNT(*) AS COUNTER FROM CMMC_ProductTypes WHERE PT_ProductCode = @pProductCode";
     cmd.Parameters.Add(new SqlParameter("@pProductCode", pProductCode));
     cn.Open();
     blnReturn = cmd.ExecuteScalar().ToString().ToInt() > 0;
    }
   }
   return blnReturn;
  }

  public int Delete(string pID)
  {
   int intReturn = 0;
   string strSQL = "Delete CMMC_ProductTypes WHERE PT_ProductCode = @pCode";

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

 }
}