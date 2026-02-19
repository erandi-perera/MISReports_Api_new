using System;
using System.Configuration;
using System.Data.OleDb;

namespace MISReports_Api.DBAccess
{
    public class DBConnection
    {
        private readonly string bulkConnectionString;
        private readonly string ordinaryConnectionString;
        public OleDbConnection Provdb(string DBName)
        {
            try
            {
                string connectionstring = "Provider='Ifxoledbc.2';password=run10times;User ID=appadm1; Data Source='" + DBName + "'";
                OleDbConnection connection = new OleDbConnection(connectionstring);
                connection.Open();
                return connection;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public DBConnection()
        {
            try
            {
                var bulkConn = ConfigurationManager.ConnectionStrings["InformixBulkConnection"];
                var ordinaryConn = ConfigurationManager.ConnectionStrings["InformixConnection"];

                if (bulkConn == null)
                    throw new ConfigurationErrorsException("InformixBulkConnection is missing in web.config");

                if (ordinaryConn == null)
                    throw new ConfigurationErrorsException("InformixConnection is missing in web.config");

                bulkConnectionString = bulkConn.ConnectionString;
                ordinaryConnectionString = ordinaryConn.ConnectionString;

                if (string.IsNullOrWhiteSpace(bulkConnectionString))
                    throw new ConfigurationErrorsException("InformixBulkConnection is empty");

                if (string.IsNullOrWhiteSpace(ordinaryConnectionString))
                    throw new ConfigurationErrorsException("InformixConnection is empty");
            }
            catch
            {
                throw;
            }
        }

        // =====================================================
        // Ordinary / Bulk Informix
        // =====================================================
        public OleDbConnection GetConnection(bool useBulkConnection = true)
        {
            string connString = useBulkConnection
                ? bulkConnectionString
                : ordinaryConnectionString;

            if (string.IsNullOrWhiteSpace(connString))
                throw new InvalidOperationException("Connection string is not initialized");

            return new OleDbConnection(connString);
        }

        // =====================================================
        // Solar Age – Dynamic Informix Connection
        // =====================================================
        public OleDbConnection GetSolarAgeConnection(string server)
        {
            if (string.IsNullOrWhiteSpace(server))
                throw new ArgumentException("Server name cannot be empty");

            string connName = $"SolarAge_{server}";

            var connSetting = ConfigurationManager.ConnectionStrings[connName];

            if (connSetting == null)
                throw new ConfigurationErrorsException(
                    $"Connection string '{connName}' not found in web.config");

            if (string.IsNullOrWhiteSpace(connSetting.ConnectionString))
                throw new ConfigurationErrorsException(
                    $"Connection string '{connName}' is empty");

            return new OleDbConnection(connSetting.ConnectionString);
        }

        // =====================================================
        // Test Connections
        // =====================================================
        public bool TestConnection(out string errorMessage, bool useBulkConnection = true)
        {
            errorMessage = null;

            try
            {
                using (var conn = GetConnection(useBulkConnection))
                {
                    conn.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public bool TestSolarAgeConnection(string server, out string errorMessage)
        {
            errorMessage = null;

            try
            {
                using (var conn = GetSolarAgeConnection(server))
                {
                    conn.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        // =====================================================
        // Properties (Backward Compatibility)
        // =====================================================
        public string BulkConnectionString => bulkConnectionString;

        public string OrdinaryConnectionString => ordinaryConnectionString;




        // =====================================================
        // Fixed Billsmry DB connection
        // =====================================================
        public OleDbConnection Billsmrydb()
        {
            try
            {
                string connectionstring = "Provider='Ifxoledbc.2';password=payquery;User ID=payquery;Data Source='billsmry@hqinfdb10'";
                OleDbConnection connection = new OleDbConnection(connectionstring);
                connection.Open();
                return connection;
            }
            catch (Exception ex)
            {
                throw new Exception("Billsmrydb connection failed: " + ex.Message);
            }
        }


    }
}
