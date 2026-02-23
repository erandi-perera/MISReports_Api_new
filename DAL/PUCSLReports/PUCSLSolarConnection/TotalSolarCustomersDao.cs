using MISReports_Api.DBAccess;
using MISReports_Api.Helpers;
using MISReports_Api.Models.PUCSLReports.PUCSLSolarConnection;
using MISReports_Api.Models.SolarInformation;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.OleDb;

namespace MISReports_Api.DAL.PUCSLReports.PUCSLSolarConnection
{
    /// <summary>
    /// DAO for PUCSL Total No of Solar Customers Report (Report 3 from PDF).
    /// 
    /// Groups data by tariff_class and net_type instead of tariff_code.
    /// Returns customer counts and units_out for each net_type category:
    ///   - Net Metering: net_type='1'
    ///   - Net Accounting: net_type IN ('2','5')
    ///   - Net Plus: net_type='3'
    ///   - Net Plus Plus: net_type='4'
    ///
    /// Ordinary data -> InformixConnection     (GetConnection(false))
    /// Bulk data     -> InformixBulkConnection (GetConnection(true))
    /// </summary>
    public class TotalSolarCustomersDao
    {
        private readonly DBConnection _dbConnection = new DBConnection();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public bool TestConnection(out string errorMessage)
        {
            return _dbConnection.TestAllConnections(out errorMessage);
        }

        // ================================================================
        //  PUBLIC ENTRY POINT
        // ================================================================
        public List<TotalSolarCustomersModel> GetTotalSolarCustomersReport(PUCSLRequest request)
        {
            var results = new List<TotalSolarCustomersModel>();

            try
            {
                logger.Info("=== START GetTotalSolarCustomersReport ===");
                logger.Info($"Category={request.ReportCategory}, TypeCode={request.TypeCode}, " +
                            $"BillCycle={request.BillCycle}");
                logger.Info("NOTE: SolarType parameter is ignored for this report - returns all net types");

                SolarReportType reportType = MapReportType(request.ReportCategory);

                // Get unique tariff classes from ordinary database (excluding GV1UV and GV1SH)
                var tariffClasses = GetTariffClasses(request.BillCycle);
                if (tariffClasses.Count == 0)
                {
                    logger.Warn("No tariff classes found.");
                    return results;
                }

                foreach (var tariffClass in tariffClasses)
                {
                    var model = new TotalSolarCustomersModel
                    {
                        TariffCategory = tariffClass
                    };

                    // ── ORDINARY ─────────────────────────────────────────────

                    // Net Metering (net_type='1')
                    var ordNetMetering = GetOrdByNetType(reportType, request.TypeCode,
                        request.BillCycle, tariffClass, "1", false);
                    model.OrdinaryNetMeteringCustomers = ordNetMetering.Customers;
                    model.OrdinaryNetMeteringUnits = ordNetMetering.Units;

                    // Net Accounting (net_type='2' OR net_type='5')
                    var ordNetAccounting = GetOrdByNetType(reportType, request.TypeCode,
                        request.BillCycle, tariffClass, "2", true);
                    model.OrdinaryNetAccountingCustomers = ordNetAccounting.Customers;
                    model.OrdinaryNetAccountingUnits = ordNetAccounting.Units;

                    // Net Plus (net_type='3')
                    var ordNetPlus = GetOrdByNetType(reportType, request.TypeCode,
                        request.BillCycle, tariffClass, "3", false);
                    model.OrdinaryNetPlusCustomers = ordNetPlus.Customers;
                    model.OrdinaryNetPlusUnits = ordNetPlus.Units;

                    // Net Plus Plus (net_type='4')
                    var ordNetPlusPlus = GetOrdByNetType(reportType, request.TypeCode,
                        request.BillCycle, tariffClass, "4", false);
                    model.OrdinaryNetPlusPlusCustomers = ordNetPlusPlus.Customers;
                    model.OrdinaryNetPlusPlusUnits = ordNetPlusPlus.Units;

                    // ── BULK ──────────────────────────────────────────────────

                    // Bulk database uses padded province codes
                    string bulkTypeCode = request.TypeCode;
                    if (reportType == SolarReportType.Province && !string.IsNullOrEmpty(request.TypeCode) && request.TypeCode.Length == 1)
                    {
                        bulkTypeCode = request.TypeCode.PadLeft(2, '0');
                    }

                    // Get bulk tariff codes for this tariff class
                    var bulkTariffs = GetBulkTariffsForClass(tariffClass, request.BillCycle);

                    if (bulkTariffs.Count > 0)
                    {
                        // Net Metering (net_type='1')
                        var bulkNetMetering = GetBulkByNetType(reportType, bulkTypeCode,
                            request.BillCycle, bulkTariffs, "1");
                        model.BulkNetMeteringCustomers = bulkNetMetering.Customers;
                        model.BulkNetMeteringUnits = bulkNetMetering.Units;

                        // Net Accounting (net_type='2')
                        var bulkNetAccounting = GetBulkByNetType(reportType, bulkTypeCode,
                            request.BillCycle, bulkTariffs, "2");
                        model.BulkNetAccountingCustomers = bulkNetAccounting.Customers;
                        model.BulkNetAccountingUnits = bulkNetAccounting.Units;

                        // Net Plus (net_type='3')
                        var bulkNetPlus = GetBulkByNetType(reportType, bulkTypeCode,
                            request.BillCycle, bulkTariffs, "3");
                        model.BulkNetPlusCustomers = bulkNetPlus.Customers;
                        model.BulkNetPlusUnits = bulkNetPlus.Units;

                        // Net Plus Plus (net_type='4')
                        var bulkNetPlusPlus = GetBulkByNetType(reportType, bulkTypeCode,
                            request.BillCycle, bulkTariffs, "4");
                        model.BulkNetPlusPlusCustomers = bulkNetPlusPlus.Customers;
                        model.BulkNetPlusPlusUnits = bulkNetPlusPlus.Units;
                    }

                    model.ErrorMessage = string.Empty;
                    results.Add(model);
                }

                // ── SPECIAL HANDLING FOR GV1 (GP-3 and GP-4) ─────────────────
                var gv1Model = new TotalSolarCustomersModel
                {
                    TariffCategory = "GV1"
                };

                // Ordinary GV1 data (tariff_type IN ('GP-3','GP-4'))
                var gv1OrdNetMetering = GetOrdGV1ByNetType(reportType, request.TypeCode,
                    request.BillCycle, "1");
                gv1Model.OrdinaryNetMeteringCustomers = gv1OrdNetMetering.Customers;
                gv1Model.OrdinaryNetMeteringUnits = gv1OrdNetMetering.Units;

                var gv1OrdNetAccounting = GetOrdGV1ByNetType(reportType, request.TypeCode,
                    request.BillCycle, "2");
                gv1Model.OrdinaryNetAccountingCustomers = gv1OrdNetAccounting.Customers;
                gv1Model.OrdinaryNetAccountingUnits = gv1OrdNetAccounting.Units;

                var gv1OrdNetPlus = GetOrdGV1ByNetType(reportType, request.TypeCode,
                    request.BillCycle, "3");
                gv1Model.OrdinaryNetPlusCustomers = gv1OrdNetPlus.Customers;
                gv1Model.OrdinaryNetPlusUnits = gv1OrdNetPlus.Units;

                var gv1OrdNetPlusPlus = GetOrdGV1ByNetType(reportType, request.TypeCode,
                    request.BillCycle, "4");
                gv1Model.OrdinaryNetPlusPlusCustomers = gv1OrdNetPlusPlus.Customers;
                gv1Model.OrdinaryNetPlusPlusUnits = gv1OrdNetPlusPlus.Units;

                // Bulk GV1 - no special handling needed, treated same as other categories
                // (GV1 may or may not exist in bulk, handled by GetBulkTariffsForClass)

                gv1Model.ErrorMessage = string.Empty;
                results.Add(gv1Model);

                logger.Info($"=== END GetTotalSolarCustomersReport — {results.Count} rows ===");
                return results;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error in GetTotalSolarCustomersReport");
                throw;
            }
        }

        // ================================================================
        //  TARIFF CLASSES
        // ================================================================

        /// <summary>
        /// Gets distinct tariff_class values from netmtcons for the given bill cycle.
        /// Excludes GV1UV and GV1SH classes as per PDF.
        /// </summary>
        private List<string> GetTariffClasses(string calcCycle)
        {
            var classes = new List<string>();
            try
            {
                using (var conn = _dbConnection.GetConnection(false))
                {
                    conn.Open();
                    const string sql =
                        "SELECT c.tariff_class FROM netmtcons a, tariff_code c " +
                        "WHERE a.calc_cycle=? AND a.net_type IN ('1','2','3','4','5') " +
                        "AND a.tariff_code=c.tariff_code " +
                        "AND tariff_class NOT IN ('GV1UV','GV1SH') " +
                        "GROUP BY c.tariff_class ORDER BY c.tariff_class";

                    using (var cmd = new OleDbCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("?", calcCycle);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var tariffClass = reader[0]?.ToString().Trim();
                                if (!string.IsNullOrEmpty(tariffClass))
                                {
                                    classes.Add(tariffClass);
                                }
                            }
                        }
                    }
                }
                logger.Info($"Found {classes.Count} tariff classes");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "GetTariffClasses EXCEPTION");
            }
            return classes;
        }

        // ================================================================
        //  ORDINARY — By Net Type
        // ================================================================

        private NetTypeData GetOrdByNetType(SolarReportType rt, string typeCode,
            string calcCycle, string tariffClass, string netType, bool isNetAccounting)
        {
            var data = new NetTypeData();
            try
            {
                using (var conn = _dbConnection.GetConnection(false))
                {
                    conn.Open();

                    string netCond = isNetAccounting
                        ? "(n.net_type='2' OR n.net_type='5')"
                        : "n.net_type=?";

                    string sql;
                    OleDbCommand cmd = new OleDbCommand { Connection = conn };

                    switch (rt)
                    {
                        case SolarReportType.Province:
                            sql = $"SELECT t.tariff_class, COUNT(n.acct_number), COALESCE(SUM(n.units_out),0) " +
                                  $"FROM netmtcons n, tariff_code t, areas a " +
                                  $"WHERE {netCond} AND n.tariff_code=t.tariff_code " +
                                  $"AND n.calc_cycle=? AND tariff_class=? " +
                                  $"AND a.area_code=n.area_code AND a.prov_code=? " +
                                  $"GROUP BY 1 ORDER BY 1";
                            cmd.CommandText = sql;
                            if (!isNetAccounting) cmd.Parameters.AddWithValue("?", netType);
                            cmd.Parameters.AddWithValue("?", calcCycle);
                            cmd.Parameters.AddWithValue("?", tariffClass);
                            cmd.Parameters.AddWithValue("?", typeCode);
                            break;

                        case SolarReportType.Region:
                            sql = $"SELECT t.tariff_class, COUNT(n.acct_number), COALESCE(SUM(n.units_out),0) " +
                                  $"FROM netmtcons n, tariff_code t, areas a " +
                                  $"WHERE {netCond} AND n.tariff_code=t.tariff_code " +
                                  $"AND n.calc_cycle=? AND tariff_class=? " +
                                  $"AND a.area_code=n.area_code AND a.region=? " +
                                  $"GROUP BY 1 ORDER BY 1";
                            cmd.CommandText = sql;
                            if (!isNetAccounting) cmd.Parameters.AddWithValue("?", netType);
                            cmd.Parameters.AddWithValue("?", calcCycle);
                            cmd.Parameters.AddWithValue("?", tariffClass);
                            cmd.Parameters.AddWithValue("?", typeCode);
                            break;

                        default: // EntireCEB
                            sql = $"SELECT t.tariff_class, COUNT(n.acct_number), COALESCE(SUM(n.units_out),0) " +
                                  $"FROM netmtcons n, tariff_code t " +
                                  $"WHERE {netCond} AND n.tariff_code=t.tariff_code " +
                                  $"AND n.calc_cycle=? AND tariff_class=? " +
                                  $"GROUP BY 1 ORDER BY 1";
                            cmd.CommandText = sql;
                            if (!isNetAccounting) cmd.Parameters.AddWithValue("?", netType);
                            cmd.Parameters.AddWithValue("?", calcCycle);
                            cmd.Parameters.AddWithValue("?", tariffClass);
                            break;
                    }

                    using (cmd)
                    using (var reader = cmd.ExecuteReader())
                        if (reader.Read())
                        {
                            data.Customers = reader[1] == DBNull.Value ? 0 : Convert.ToInt32(reader[1]);
                            data.Units = reader[2] == DBNull.Value ? 0 : Convert.ToDecimal(reader[2]);
                        }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"GetOrdByNetType class={tariffClass} netType={netType}");
            }
            return data;
        }

        // ================================================================
        //  ORDINARY — GV1 Special Handling (GP-3 and GP-4)
        // ================================================================

        private NetTypeData GetOrdGV1ByNetType(SolarReportType rt, string typeCode,
            string calcCycle, string netType)
        {
            var data = new NetTypeData();
            try
            {
                using (var conn = _dbConnection.GetConnection(false))
                {
                    conn.Open();

                    string netCond = (netType == "2")
                        ? "(n.net_type='2' OR n.net_type='5')"
                        : "n.net_type=?";

                    string sql;
                    OleDbCommand cmd = new OleDbCommand { Connection = conn };

                    switch (rt)
                    {
                        case SolarReportType.Province:
                            sql = $"SELECT 'GV1', COUNT(n.acct_number), COALESCE(SUM(n.units_out),0) " +
                                  $"FROM netmtcons n, tariff_code c, areas a " +
                                  $"WHERE {netCond} AND n.tariff_code=c.tariff_code " +
                                  $"AND n.calc_cycle=? AND tariff_type IN ('GP-3','GP-4') " +
                                  $"AND a.area_code=n.area_code AND a.prov_code=?";
                            cmd.CommandText = sql;
                            if (netType != "2") cmd.Parameters.AddWithValue("?", netType);
                            cmd.Parameters.AddWithValue("?", calcCycle);
                            cmd.Parameters.AddWithValue("?", typeCode);
                            break;

                        case SolarReportType.Region:
                            sql = $"SELECT 'GV1', COUNT(n.acct_number), COALESCE(SUM(n.units_out),0) " +
                                  $"FROM netmtcons n, tariff_code c, areas a " +
                                  $"WHERE {netCond} AND n.tariff_code=c.tariff_code " +
                                  $"AND n.calc_cycle=? AND tariff_type IN ('GP-3','GP-4') " +
                                  $"AND a.area_code=n.area_code AND a.region=?";
                            cmd.CommandText = sql;
                            if (netType != "2") cmd.Parameters.AddWithValue("?", netType);
                            cmd.Parameters.AddWithValue("?", calcCycle);
                            cmd.Parameters.AddWithValue("?", typeCode);
                            break;

                        default: // EntireCEB
                            sql = $"SELECT 'GV1', COUNT(n.acct_number), COALESCE(SUM(n.units_out),0) " +
                                  $"FROM netmtcons n, tariff_code c " +
                                  $"WHERE {netCond} AND n.tariff_code=c.tariff_code " +
                                  $"AND n.calc_cycle=? AND tariff_type IN ('GP-3','GP-4')";
                            cmd.CommandText = sql;
                            if (netType != "2") cmd.Parameters.AddWithValue("?", netType);
                            cmd.Parameters.AddWithValue("?", calcCycle);
                            break;
                    }

                    using (cmd)
                    using (var reader = cmd.ExecuteReader())
                        if (reader.Read())
                        {
                            data.Customers = reader[1] == DBNull.Value ? 0 : Convert.ToInt32(reader[1]);
                            data.Units = reader[2] == DBNull.Value ? 0 : Convert.ToDecimal(reader[2]);
                        }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"GetOrdGV1ByNetType netType={netType}");
            }
            return data;
        }

        // ================================================================
        //  BULK — Get Tariffs for Class
        // ================================================================

        private List<string> GetBulkTariffsForClass(string tariffClass, string billCycle)
        {
            var tariffs = new List<string>();
            try
            {
                using (var conn = _dbConnection.GetConnection(true))
                {
                    conn.Open();
                    // Get distinct tariff codes from bulk that match this tariff_class
                    // We need to join or query based on tariff naming convention
                    // For simplicity, assume tariff in bulk DB matches tariff_class prefix
                    const string sql =
                        "SELECT tariff FROM netmtcons " +
                        "WHERE bill_cycle=? AND net_type IN ('1','2','3','4') " +
                        "GROUP BY tariff ORDER BY tariff ASC";

                    using (var cmd = new OleDbCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("?", billCycle);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var tariff = reader[0]?.ToString().Trim();
                                // Match tariff to tariff_class (e.g., "DM2" matches "D1", "GP2" matches "GP1")
                                if (!string.IsNullOrEmpty(tariff) && TariffMatchesClass(tariff, tariffClass))
                                {
                                    tariffs.Add(tariff);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"GetBulkTariffsForClass class={tariffClass}");
            }
            return tariffs;
        }

        /// <summary>
        /// Matches bulk tariff code to ordinary tariff_class.
        /// E.g., DM1, DM2 → D1; GP1, GP2, GP3 → GP1; I1, I2, I3 → I1
        /// </summary>
        private bool TariffMatchesClass(string tariff, string tariffClass)
        {
            // Remove numbers and compare prefix
            string tariffPrefix = System.Text.RegularExpressions.Regex.Replace(tariff, @"\d", "");
            string classPrefix = System.Text.RegularExpressions.Regex.Replace(tariffClass, @"\d", "");

            return tariffPrefix.Equals(classPrefix, StringComparison.OrdinalIgnoreCase);
        }

        // ================================================================
        //  BULK — By Net Type
        // ================================================================

        private NetTypeData GetBulkByNetType(SolarReportType rt, string typeCode,
            string billCycle, List<string> tariffs, string netType)
        {
            var data = new NetTypeData();
            try
            {
                using (var conn = _dbConnection.GetConnection(true))
                {
                    conn.Open();

                    string inClause = string.Join(",", tariffs.ConvertAll(t => "?"));
                    string sql;
                    OleDbCommand cmd = new OleDbCommand { Connection = conn };

                    // Net Metering (net_type='1') doesn't need netmeter join
                    if (netType == "1")
                    {
                        switch (rt)
                        {
                            case SolarReportType.Province:
                                sql = $"SELECT COUNT(n.acc_nbr), COALESCE(SUM(n.exp_kwd_units),0) " +
                                      $"FROM netmtcons n, areas a " +
                                      $"WHERE bill_cycle=? AND n.net_type=? " +
                                      $"AND a.area_code=n.area_cd AND a.prov_code=? " +
                                      $"AND tariff IN ({inClause})";
                                cmd.CommandText = sql;
                                cmd.Parameters.AddWithValue("?", billCycle);
                                cmd.Parameters.AddWithValue("?", netType);
                                cmd.Parameters.AddWithValue("?", typeCode);
                                foreach (var t in tariffs) cmd.Parameters.AddWithValue("?", t);
                                break;

                            case SolarReportType.Region:
                                sql = $"SELECT COUNT(n.acc_nbr), COALESCE(SUM(n.exp_kwd_units),0) " +
                                      $"FROM netmtcons n, areas a " +
                                      $"WHERE bill_cycle=? AND n.net_type=? " +
                                      $"AND a.area_code=n.area_cd AND a.region=? " +
                                      $"AND n.tariff IN ({inClause})";
                                cmd.CommandText = sql;
                                cmd.Parameters.AddWithValue("?", billCycle);
                                cmd.Parameters.AddWithValue("?", netType);
                                cmd.Parameters.AddWithValue("?", typeCode);
                                foreach (var t in tariffs) cmd.Parameters.AddWithValue("?", t);
                                break;

                            default: // EntireCEB
                                sql = $"SELECT COUNT(acc_nbr), COALESCE(SUM(exp_kwd_units),0) " +
                                      $"FROM netmtcons " +
                                      $"WHERE bill_cycle=? AND net_type=? " +
                                      $"AND tariff IN ({inClause})";
                                cmd.CommandText = sql;
                                cmd.Parameters.AddWithValue("?", billCycle);
                                cmd.Parameters.AddWithValue("?", netType);
                                foreach (var t in tariffs) cmd.Parameters.AddWithValue("?", t);
                                break;
                        }
                    }
                    else
                    {
                        // Net Accounting, Net Plus, Net Plus Plus need netmeter join and rate filter
                        switch (rt)
                        {
                            case SolarReportType.Province:
                                sql = $"SELECT COUNT(n.acc_nbr), COALESCE(SUM(n.exp_kwd_units),0) " +
                                      $"FROM netmtcons n, areas a, netmeter m " +
                                      $"WHERE n.net_type=? AND bill_cycle=? " +
                                      $"AND m.acc_nbr=n.acc_nbr AND rate NOT IN ('0') " +
                                      $"AND a.area_code=n.area_cd AND a.prov_code=? " +
                                      $"AND n.tariff IN ({inClause})";
                                cmd.CommandText = sql;
                                cmd.Parameters.AddWithValue("?", netType);
                                cmd.Parameters.AddWithValue("?", billCycle);
                                cmd.Parameters.AddWithValue("?", typeCode);
                                foreach (var t in tariffs) cmd.Parameters.AddWithValue("?", t);
                                break;

                            case SolarReportType.Region:
                                sql = $"SELECT COUNT(n.acc_nbr), COALESCE(SUM(n.exp_kwd_units),0) " +
                                      $"FROM netmtcons n, areas a, netmeter m " +
                                      $"WHERE n.net_type=? AND bill_cycle=? " +
                                      $"AND m.acc_nbr=n.acc_nbr AND rate NOT IN ('0') " +
                                      $"AND a.area_code=n.area_cd AND a.region=? " +
                                      $"AND n.tariff IN ({inClause})";
                                cmd.CommandText = sql;
                                cmd.Parameters.AddWithValue("?", netType);
                                cmd.Parameters.AddWithValue("?", billCycle);
                                cmd.Parameters.AddWithValue("?", typeCode);
                                foreach (var t in tariffs) cmd.Parameters.AddWithValue("?", t);
                                break;

                            default: // EntireCEB
                                sql = $"SELECT COUNT(n.acc_nbr), COALESCE(SUM(n.exp_kwd_units),0) " +
                                      $"FROM netmtcons n, netmeter m " +
                                      $"WHERE n.net_type=? AND bill_cycle=? " +
                                      $"AND m.acc_nbr=n.acc_nbr AND rate NOT IN ('0') " +
                                      $"AND tariff IN ({inClause})";
                                cmd.CommandText = sql;
                                cmd.Parameters.AddWithValue("?", netType);
                                cmd.Parameters.AddWithValue("?", billCycle);
                                foreach (var t in tariffs) cmd.Parameters.AddWithValue("?", t);
                                break;
                        }
                    }

                    using (cmd)
                    using (var reader = cmd.ExecuteReader())
                        if (reader.Read())
                        {
                            data.Customers = reader[0] == DBNull.Value ? 0 : Convert.ToInt32(reader[0]);
                            data.Units = reader[1] == DBNull.Value ? 0 : Convert.ToDecimal(reader[1]);
                        }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"GetBulkByNetType netType={netType}");
            }
            return data;
        }

        // ================================================================
        //  UTILITY
        // ================================================================

        private SolarReportType MapReportType(PUCSLReportCategory cat)
        {
            switch (cat)
            {
                case PUCSLReportCategory.Province: return SolarReportType.Province;
                case PUCSLReportCategory.Region: return SolarReportType.Region;
                default: return SolarReportType.EntireCEB;
            }
        }

        // ================================================================
        //  HELPER CLASSES
        // ================================================================

        private class NetTypeData
        {
            public int Customers { get; set; }
            public decimal Units { get; set; }
        }
    }
}