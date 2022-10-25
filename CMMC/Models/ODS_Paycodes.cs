using System;
using System.Collections.Generic;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CMMC.Models
{
    public class ODS_Paycodes : IDisposable
    {
        public void Dispose() { GC.SuppressFinalize(this); }

        public struct Details
        {
            public string Payroll_Code { get; set; }
            public string Account_Number { get; set; }
        }

        public List<Details> GetList()
        {
            List<Details> list = new List<Details>();
            string sQuery = "select PAYROLL_CODE from CTBC_DEPOSIT_ACCOUNTS where PAYROLL_CODE IS NOT NULL";
            using (OracleConnection con = new OracleConnection(SharedFunctions.ODSConnectionString))
            {
                using (OracleCommand cmd = con.CreateCommand())
                {
                    con.Open();
                    cmd.CommandText = sQuery;
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Details()
                            {
                                Payroll_Code = reader.GetString(0)
                            });
                        }
                    }
                }
            }
            return list;
        }

        public List<Details> GetListAccount()
        {
            List<Details> list = new List<Details>();
            DateTime dtEnrolled = new DateTime();
            string sQuery = "select ACCOUNT_NUMBER, PAYROLL_CODE from CTBC_DEPOSIT_ACCOUNTS ";
            sQuery += $"where PAYROLL_CODE is not null and ACCOUNT_NUMBER is not null ";
            try
            {
                using (OracleConnection cn = new OracleConnection(SharedFunctions.ODSConnectionString))
                {
                    using (OracleCommand cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = sQuery;
                        cn.Open();
                        using(OracleDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(new Details()
                                {
                                    Account_Number = reader.GetString(0),
                                    Payroll_Code = reader.GetString(1)
                                });
                            }
                        }
                        
                    }
                }
            }
            
            catch (Exception ex) { }

            return list;
        }
    }
}