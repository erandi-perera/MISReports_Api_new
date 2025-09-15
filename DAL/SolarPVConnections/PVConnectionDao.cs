// Optimized PVConnectionDao.cs
using MISReports_Api.DBAccess;
using MISReports_Api.Models.SolarInformation;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.OleDb;

namespace MISReports_Api.DAL.SolarPVConnections
{
    public class PVConnectionDao
    {
        private readonly DBConnection _dbConnection = new DBConnection();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public bool TestConnection(out string errorMessage)
        {
            return _dbConnection.TestConnection(out errorMessage, false);
        }

        public List<SolarPVConnectionModel> GetPVConnections(SolarPVConnectionRequest request)
        {
            var results = new List<SolarPVConnectionModel>();

            try
            {
                logger.Info("=== START GetPVConnections ===");
                logger.Info($"Request: {request.CycleType}Cycle={request.BillCycle ?? request.CalcCycle}, ReportType={request.ReportType}");

                string sql = BuildPVConnectionQuery(request);
                logger.Debug($"Generated SQL: {sql}");

                using (var conn = _dbConnection.GetConnection(false))
                {
                    conn.Open();

                    using (var cmd = new OleDbCommand(sql, conn))
                    {
                        // Set command timeout for large queries
                        cmd.CommandTimeout = 300; // 5 minutes

                        AddParameters(cmd, request);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var pvConnection = MapPVConnectionFromReader(reader);
                                results.Add(pvConnection);
                            }
                        }
                    }
                }

                logger.Info($"=== END GetPVConnections (Success) - {results.Count} records ===");
                return results;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error occurred while fetching PV connections");
                throw;
            }
        }

        private string BuildPVConnectionQuery(SolarPVConnectionRequest request)
        {
            string cycleField = request.CycleType == "A" ? "n.bill_cycle" : "n.calc_cycle";
            string prnCycleField = "c.bill_cycle"; // prn_dat_1 typically uses bill_cycle

            string baseQuery = @"SELECT 
               n.area_code,
               n.acct_number,
               n.net_type,
               n.units_out,
               n.units_in,
               n.gen_cap,
               n.tariff_code,
               n.agrmnt_date,
               a.area_name,
               a.region,
               p.prov_name,
               c.cust_fname,
               c.cust_lname,
               c.crnt_depot,
               c.substn_code
        FROM netmtcons n
        JOIN areas a ON n.area_code = a.area_code
        JOIN provinces p ON a.prov_code = p.prov_code
        JOIN prn_dat_1 c ON n.acct_number = c.acct_number 
        WHERE 1=1";

            // Add the cycle condition for both tables
            baseQuery += " AND " + cycleField + " = ?";

            // If using calc_cycle for netmtcons, we might still need bill_cycle for prn_dat_1
            if (request.CycleType == "C")
            {
                // You may need to adjust this based on your data relationship
                baseQuery += " AND " + prnCycleField + " = ?";
            }
            else
            {
                baseQuery += " AND " + prnCycleField + " = ?";
            }

            switch (request.ReportType)
            {
                case SolarReportType.Area:
                    baseQuery += " AND n.area_code = ?";
                    baseQuery += " ORDER BY n.acct_number";
                    break;

                case SolarReportType.Province:
                    baseQuery += " AND a.prov_code = ?";
                    baseQuery += " ORDER BY n.acct_number";
                    break;

                case SolarReportType.Region:
                    baseQuery += " AND a.region = ?";
                    baseQuery += " ORDER BY n.acct_number";
                    break;

                case SolarReportType.EntireCEB:
                default:
                    baseQuery += " ORDER BY a.region, p.prov_name, a.area_name, n.acct_number";
                    break;
            }

            return baseQuery;
        }

        private void AddParameters(OleDbCommand cmd, SolarPVConnectionRequest request)
        {
            string cycleValue = request.CycleType == "A" ? request.BillCycle : request.CalcCycle;

            // Add cycle parameters
            cmd.Parameters.AddWithValue("@cycle1", cycleValue);
            cmd.Parameters.AddWithValue("@cycle2", cycleValue);

            // Add specific filters based on report type
            switch (request.ReportType)
            {
                case SolarReportType.Area:
                    if (!string.IsNullOrEmpty(request.AreaCode))
                    {
                        cmd.Parameters.AddWithValue("@areaCode", request.AreaCode);
                    }
                    break;
                case SolarReportType.Province:
                    if (!string.IsNullOrEmpty(request.ProvCode))
                    {
                        cmd.Parameters.AddWithValue("@provCode", request.ProvCode);
                    }
                    break;
                case SolarReportType.Region:
                    if (!string.IsNullOrEmpty(request.Region))
                    {
                        cmd.Parameters.AddWithValue("@region", request.Region);
                    }
                    break;
            }
        }

        private SolarPVConnectionModel MapPVConnectionFromReader(OleDbDataReader reader)
        {
            var model = new SolarPVConnectionModel();

            try
            {
                // Basic mapping - all data comes from the single query
                model.AccountNumber = GetColumnValue(reader, "acct_number");
                model.Area = GetColumnValue(reader, "area_name");
                model.Province = GetColumnValue(reader, "prov_name");
                model.Division = GetColumnValue(reader, "region");
                model.PanelCapacity = GetDecimalValue(reader, "gen_cap");
                model.EnergyExported = GetDecimalValue(reader, "units_out");
                model.EnergyImported = GetDecimalValue(reader, "units_in");
                model.Tariff = GetColumnValue(reader, "tariff_code");
                model.AgreementDate = GetColumnValue(reader, "agrmnt_date");
                var depot = GetColumnValue(reader, "crnt_depot") ?? string.Empty;

                var substn = GetColumnValue(reader, "substn_code") ?? string.Empty;
                model.SinNumber = depot + substn;

                // Map customer name
                string firstName = GetColumnValue(reader, "cust_fname") ?? "";
                string lastName = GetColumnValue(reader, "cust_lname") ?? "";
                model.CustomerName = $"{firstName} {lastName}".Trim();

                // Map net type to customer type
                string netType = GetColumnValue(reader, "net_type");
                model.CustomerType = MapNetTypeToCustomerType(netType);

                // Calculate B/F and C/F units (business logic needed here)
                model.BFUnits = 0; // TODO: Implement business logic
                model.CFUnits = model.BFUnits + model.EnergyExported - model.EnergyImported;//INCORRECT data
                model.UnitsForLossReduction = 0; // TODO: Implement business logic

                model.ErrorMessage = string.Empty;
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Error mapping PV connection data for account {model.AccountNumber}");
                model.ErrorMessage = ex.Message;
            }

            return model;
        }

        private string MapNetTypeToCustomerType(string netType)
        {
            if (string.IsNullOrEmpty(netType))
                return "Unknown";

            switch (netType.Trim())
            {
                case "1":
                    return "Net Metering";
                case "2":
                    return "Net Accounting";
                case "3":
                    return "Net Plus";
                case "4":
                    return "Net Plus Plus";
                case "5":
                    return "Convert from Net Metering to Net Accounting";
                default:
                    return "Unknown";
            }

        }

        // Helper methods
        private string GetColumnValue(OleDbDataReader reader, string columnName)
        {
            try
            {
                var value = reader[columnName];
                return value == DBNull.Value ? null : value.ToString()?.Trim();
            }
            catch (IndexOutOfRangeException)
            {
                logger.Warn($"Column '{columnName}' not found in result set");
                return null;
            }
        }

        private decimal GetDecimalValue(OleDbDataReader reader, string columnName)
        {
            try
            {
                var value = reader[columnName];
                return value == DBNull.Value ? 0 : Convert.ToDecimal(value);
            }
            catch (IndexOutOfRangeException)
            {
                logger.Warn($"Column '{columnName}' not found in result set");
                return 0;
            }
            catch (FormatException ex)
            {
                logger.Warn(ex, $"Invalid decimal format in column '{columnName}'");
                return 0;
            }
        }
    }
}