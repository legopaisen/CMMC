using System;
using System.Collections.Generic;
using System.Data.OracleClient;
using System.Linq;
using System.Web;

namespace CMMC.Models
{
    public class CIFDetails : IDisposable
    {
        public void Dispose() { GC.SuppressFinalize(this); }

        public struct Details
        {
            public int CIFNo { get; set; }
            public string Name { get; set; }
            public string Add_1 { get; set; }
            public string Add_2 { get; set; }
            public string Add_3 { get; set; }
            public string Add_4 { get; set; }
            public string Add_5 { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string Country { get; set; }
            public string AreaCode { get; set; }
        }

        public Details Fill(string CIFNo)
        {
            Details details = new Details();
            string sQuery = @"select cd.BP_ID, cd.NM1_L01, ba.FRST_LN_OF_ADRS, ba.SCND_LN_OF_ADRS, ba.THRD_LN_OF_ADRS, ba.FOURTH_LN_OF_ADRS, ba.FIFTH_LN_OF_ADRS, 
                                ba.PLC, ba.STATE, ba.CNTRY_CODE, ba.AREA_CODE from BANCS_CORPORATE_DETAILS cd inner join bancs_address ba on ba.bp_id = cd.bp_id  ";
            sQuery += $"where cd.bp_id = '{CIFNo}' ";
            sQuery += $"and EXISTS (select * from CTBC_DEPOSIT_ACCOUNTS DA where DA.CUSTOMER_ID = cd.BP_ID and DA.PRODUCT_ID = '151202' and DA.payroll_code is null)";
            try
            {
                using (OracleConnection cn = new OracleConnection(SharedFunctions.ODSConnectionString))
                {
                    using (OracleCommand cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = sQuery;
                        cn.Open();
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                details.CIFNo = Convert.ToInt32(CIFNo);
                                details.Name = reader["NM1_L01"].ToString();
                                details.Add_1 = reader["FRST_LN_OF_ADRS"].ToString();
                                details.Add_2 = reader["SCND_LN_OF_ADRS"].ToString();
                                details.Add_3 = reader["THRD_LN_OF_ADRS"].ToString();
                                details.Add_4 = reader["FOURTH_LN_OF_ADRS"].ToString();
                                details.Add_5 = reader["FIFTH_LN_OF_ADRS"].ToString();
                                details.City = reader["PLC"].ToString();
                                details.State = reader["STATE"].ToString();
                                details.Country = reader["CNTRY_CODE"].ToString();
                                details.AreaCode = reader["AREA_CODE"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return details;
        }
    }
}