using MISReports_Api.Models.Shared;
using MISReports_Api.DBAccess;
using System;
using System.Collections.Generic;
using System.Data.OleDb;

namespace MISReports_Api.DAL.Shared
{
    public class ProvinceOrdinaryDao
    {
        private readonly DBConnection _dbConnection = new DBConnection();

        public bool TestConnection(out string errorMessage)
        {
            return _dbConnection.TestConnection(out errorMessage);
        }

        public List<ProvinceModel> GetProvince()
        {
            var provinceList = new List<ProvinceModel>();

            using (var conn = _dbConnection.GetConnection(false))
            {
                try
                {
                    conn.Open();

                    string sql = "Select * from prov_servers where prov_code not in('0','Z')";

                    using (var cmd = new OleDbCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var province = new ProvinceModel
                            {
                                ProvinceCode = reader[1]?.ToString().Trim(),
                                ProvinceName = reader[0]?.ToString().Trim()
                            };

                            provinceList.Add(province);
                        }
                    }
                }
                catch (OleDbException ex)
                {
                    throw new Exception("Error retrieving province data: " + ex.Message, ex);
                }
            }

            return provinceList;
        }
    }
}
