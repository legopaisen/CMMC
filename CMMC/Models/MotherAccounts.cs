using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Data;
using Newtonsoft.Json;

//AFM 20220425
namespace CMMC.Models
{
    public class MotherAccounts : IDisposable
    {
        public void Dispose() { GC.SuppressFinalize(this); }

        public struct Details
        {
            public string AccountNo { get; set; }
            public string CompanyName { get; set; }
            public string ClientType { get; set; }
            public string Branch { get; set; }
        }
        public Details Fill(int pCMSCode)
        {
            Details details = new Details();
            try
            {
                using (SqlConnection con = new SqlConnection(SharedFunctions.Connectionstring))
                {
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "select CMA.ac_accountno, CMC.cu_name1, branchcode ";
                        cmd.CommandText = "from cmmc_accounts CMA, cmmc_customer CMC, branches BR ";
                        cmd.CommandText += "where CMA.ac_custno = CMS.cu_custNo and CMA.ac_branchcode = BR.branchcode ";
                        cmd.CommandText += "and CMA.ac_investmentType = '0701' ";
                        cmd.CommandText += "and CMA.ac_accountno = @pCMSCode";
                        cmd.Parameters.Add(new SqlParameter("@pCMSCode", pCMSCode));
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                details.AccountNo = reader["ac_accountno"].ToString();
                                details.CompanyName = reader["ac_accountno"].ToString();
                                details.ClientType = reader["ac_accountno"].ToString();
                                details.Branch = reader["ac_accountno"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CTBC.Logs.Write("Fill", ex.Message, "Mother Accounts");
            }

            return details;
        }

    }
}