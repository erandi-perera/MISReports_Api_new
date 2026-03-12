using MISReports_Api.DBAccess;
using MISReports_Api.Models;
using System;
using System.Data.OleDb;

namespace MISReports_Api.DAO
{
    public class ActiveCustomerTariffDao
    {
        private readonly DBConnection _dbConnection;

        public ActiveCustomerTariffDao()
        {
            _dbConnection = new DBConnection();
        }

        public ActiveCustomerTariffResponse GetReport(string customerType, string level, int fromCycle, int toCycle)
        {
            var response = new ActiveCustomerTariffResponse
            {
                CustomerType = customerType,
                ReportLevel = level,
                FromCycle = fromCycle.ToString(),
                ToCycle = toCycle.ToString()
            };

            try
            {
                bool isBulk = customerType.Equals("Bulk", StringComparison.OrdinalIgnoreCase);

                using (var conn = _dbConnection.GetConnection(isBulk))
                {
                    conn.Open();

                    string query = BuildQuery(isBulk, level);

                    using (var cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("?", fromCycle);
                        cmd.Parameters.AddWithValue("?", toCycle);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var item = new ActiveCustomerTariffModel
                                {
                                    Province = reader[0]?.ToString() ?? "",
                                    Area = reader[1]?.ToString() ?? "",
                                    Division = reader[2]?.ToString() ?? "",
                                    BillCycle = reader[3]?.ToString() ?? "",
                                    Tariff = reader[4]?.ToString() ?? "",
                                    NoOfCustomers = reader[5] == DBNull.Value ? 0 : Convert.ToDecimal(reader[5])
                                };

                                response.Data.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.ToString();
            }

            return response;
        }

        private string BuildQuery(bool isBulk, string level)
        {
            if (!isBulk)
            {
                switch (level.ToLower())
                {
                    case "area":
                        return @"SELECT '', c.area_code, '', c.calc_cycle, t.tariff_class, SUM(c.cnt)
                                 FROM consmry c, tariff_code t, areas a
                                 WHERE c.tariff_code=t.tariff_code
                                 AND c.area_code=a.area_code
                                 AND (c.calc_cycle >= ? AND c.calc_cycle <= ?)
                                 GROUP BY 1,2,3,4,5
                                 ORDER BY 1,2,3,4,5";

                    case "province":
                        return @"SELECT a.prov_code, '', '', c.calc_cycle, t.tariff_class, SUM(c.cnt)
                                 FROM consmry c, tariff_code t, areas a
                                 WHERE c.tariff_code=t.tariff_code
                                 AND c.area_code=a.area_code
                                 AND (c.calc_cycle >= ? AND c.calc_cycle <= ?)
                                 GROUP BY 1,2,3,4,5
                                 ORDER BY 1,2,3,4,5";

                    case "division":
                        return @"SELECT '', '', a.region, c.calc_cycle, t.tariff_class, SUM(c.cnt)
                                 FROM consmry c, tariff_code t, areas a
                                 WHERE c.tariff_code=t.tariff_code
                                 AND c.area_code=a.area_code
                                 AND (c.calc_cycle >= ? AND c.calc_cycle <= ?)
                                 GROUP BY 1,2,3,4,5
                                 ORDER BY 1,2,3,4,5";

                    default: // Entire CEB
                        return @"SELECT '', 'Entire CEB', '', c.calc_cycle, t.tariff_class, SUM(c.cnt)
                                 FROM consmry c, tariff_code t, areas a
                                 WHERE c.tariff_code=t.tariff_code
                                 AND c.area_code=a.area_code
                                 AND (c.calc_cycle >= ? AND c.calc_cycle <= ?)
                                 GROUP BY 1,2,3,4,5
                                 ORDER BY 1,2,3,4,5";
                }
            }
            else
            {
                switch (level.ToLower())
                {
                    case "area":
                        return @"SELECT p.prov_name, b.area_name, '', a.bill_cycle, a.tariff, SUM(a.no_acc)
                                 FROM account_info a, areas b, provinces p
                                 WHERE a.area_cd=b.area_code
                                 AND b.prov_code=p.prov_code
                                 AND (a.bill_cycle >= ? AND a.bill_cycle <= ?)
                                 AND a.tariff NOT IN ('TM1')
                                 GROUP BY 1,2,3,4,5
                                 ORDER BY 1,2,3,4,5";

                    case "province":
                        return @"SELECT b.region, p.prov_name, '', a.bill_cycle, a.tariff, SUM(a.no_acc)
                                 FROM account_info a, areas b, provinces p
                                 WHERE a.area_cd=b.area_code
                                 AND b.prov_code=p.prov_code
                                 AND (a.bill_cycle >= ? AND a.bill_cycle <= ?)
                                 AND a.tariff NOT IN ('TM1')
                                 GROUP BY 1,2,3,4,5
                                 ORDER BY 1,2,3,4,5";

                    case "division":
                        return @"SELECT '', b.region, '', a.bill_cycle, a.tariff, SUM(a.no_acc)
                                 FROM account_info a, areas b, provinces p
                                 WHERE a.area_cd=b.area_code
                                 AND b.prov_code=p.prov_code
                                 AND (a.bill_cycle >= ? AND a.bill_cycle <= ?)
                                 AND a.tariff NOT IN ('TM1')
                                 GROUP BY 1,2,3,4,5
                                 ORDER BY 1,2,3,4,5";

                    default: // Entire CEB
                        return @"SELECT '', 'Entire CEB', '', a.bill_cycle, a.tariff, SUM(a.no_acc)
                                 FROM account_info a, areas b, provinces p
                                 WHERE a.area_cd=b.area_code
                                 AND b.prov_code=p.prov_code
                                 AND (a.bill_cycle >= ? AND a.bill_cycle <= ?)
                                 AND a.tariff NOT IN ('TM1')
                                 GROUP BY 1,2,3,4,5
                                 ORDER BY 1,2,3,4,5";
                }
            }
        }
    }
}