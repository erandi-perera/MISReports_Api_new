using MISReports_Api.DBAccess;
using MISReports_Api.Models.SolarInformation;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.OleDb;

namespace MISReports_Api.DAL.SolarPVConnections
{
    public class PVBulkConnectionDao
    {
        private readonly DBConnection _dbConnection = new DBConnection();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public bool TestConnection(out string errorMessage)
        {
            return _dbConnection.TestConnection(out errorMessage, true);
        }

        public List<SolarPVBulkConnectionModel> GetPVConnections(SolarPVBulkConnectionRequest request)
        {
            var results = new List<SolarPVBulkConnectionModel>();

            try
            {
                logger.Info("=== START GetPVConnections ===");
                logger.Info($"Request: {request.CycleType}Cycle={request.BillCycle ?? request.CalcCycle}, ReportType={request.ReportType}");

                string sql = BuildPVBulkConnectionQuery(request);
                logger.Debug($"Generated SQL: {sql}");

                using (var conn = _dbConnection.GetConnection(true))
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

        private string BuildPVBulkConnectionQuery(SolarPVBulkConnectionRequest request)
        {
            // Use calc_cycle if requested, otherwise bill_cycle
            string cycleField = request.CycleType == "A" ? "n.bill_cycle" : "n.bill_cycle";
            string prnCycleField = "n.bill_cycle"; // keeping reference aligned with first example

            string baseQuery = @"
        SELECT 
            n.acc_nbr,
            n.net_type,
            n.imp_kwo_units,
            n.imp_kwd_units,
            n.imp_kwp_units,
            n.exp_kwo_units,
            n.exp_kwd_units,
            n.exp_kwp_units,
            n.bill_kwo_units,
            n.bill_kwd_units,
            n.bill_kwp_units,
            n.gen_cap,
            n.tariff,
            n.cf_units,
            n.bf_units,
            e.agrmnt_date,
            c.name,
            i.dp_code,
            i.cnnct_trpnl,
            i.inst_id,
            a.area_name,
            a.region,
            p.prov_name
        FROM netmtcons n
        JOIN netmeter e ON n.acc_nbr = e.acc_nbr
        JOIN customer c ON n.acc_nbr = c.acc_nbr
        JOIN inst_info i ON c.inst_id = i.inst_id
        JOIN areas a ON a.area_code = n.area_cd
        JOIN provinces p ON a.prov_code = p.prov_code
        WHERE 1=1";

            // Cycle condition
            baseQuery += " AND " + cycleField + " = ?";

            // Add condition for prnCycle (still bill_cycle in this schema)
            baseQuery += " AND " + prnCycleField + " = ?";

            // Report type specific filters
            switch (request.ReportType)
            {
                case SolarReportType.Area:
                    baseQuery += " AND n.area_cd = ?";
                    baseQuery += " ORDER BY n.acc_nbr";
                    break;

                case SolarReportType.Province:
                    baseQuery += " AND p.prov_code = ?";
                    baseQuery += " ORDER BY n.acc_nbr";
                    break;

                case SolarReportType.Region:
                    baseQuery += " AND a.region = ?";
                    baseQuery += " ORDER BY n.acc_nbr";
                    break;

                case SolarReportType.EntireCEB:
                default:
                    baseQuery += " ORDER BY a.region, p.prov_name, a.area_name, n.acc_nbr";
                    break;
            }

            return baseQuery;
        }




        private void AddParameters(OleDbCommand cmd, SolarPVBulkConnectionRequest request)
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

        private SolarPVBulkConnectionModel MapPVConnectionFromReader(OleDbDataReader reader)
        {
            var model = new SolarPVBulkConnectionModel();

            try
            {
                model.AccountNumber = GetColumnValue(reader, "acc_nbr");
                model.Area = GetColumnValue(reader, "area_name");
                model.Province = GetColumnValue(reader, "prov_name");
                model.Division = GetColumnValue(reader, "region");
                model.PanelCapacity = GetDecimalValue(reader, "gen_cap");
                model.EnergyExported = GetDecimalValue(reader, "exp_kwo_units");
                model.EnergyImported = GetDecimalValue(reader, "imp_kwo_units");
                model.Tariff = GetColumnValue(reader, "tariff");
                model.AgreementDate = GetColumnValue(reader, "agrmnt_date");
                model.SinNumber = GetColumnValue(reader, "dp_code") ?? GetColumnValue(reader, "cnnct_trpnl");

                // Single "name" field from customer table
                model.CustomerName = GetColumnValue(reader, "name");

                // Institution info
                //model.InstitutionName = GetColumnValue(reader, "inst_name");
                //model.InstitutionId = GetColumnValue(reader, "inst_id");

                // Customer type
                string netType = GetColumnValue(reader, "net_type");
                model.CustomerType = MapNetTypeToCustomerType(netType);

                // B/F and C/F units
                model.BFUnits = GetColumnValue(reader, "bf_units");
                model.CFUnits = GetColumnValue(reader, "cf_units");

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