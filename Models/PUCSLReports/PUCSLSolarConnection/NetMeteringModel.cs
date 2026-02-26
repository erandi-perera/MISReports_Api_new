using System.Collections.Generic;

namespace MISReports_Api.Models.PUCSLReports.PUCSLSolarConnection
{
    /// <summary>
    /// Single row in the Net Metering report.
    /// Represents data for a tariff category (D, GP, H, I, R, GV).
    /// </summary>
    public class NetMeteringData
    {
        /// <summary>Tariff category code (e.g., "D", "GP", "H", "I", "R", "GV")</summary>
        public string Category { get; set; }

        /// <summary>Year as 2-digit string (e.g., "25")</summary>
        public string Year { get; set; }

        /// <summary>Month as number string (e.g., "9" for September)</summary>
        public string Month { get; set; }

        /// <summary>Number of customers</summary>
        public int NoOfCustomers { get; set; }

        /// <summary>Units Day in kWh</summary>
        public decimal UnitsDayKwh { get; set; }

        /// <summary>Units Peak (Bulk only)</summary>
        public decimal UnitsPeakKwh { get; set; }

        /// <summary>Units Off-Peak (Bulk only)</summary>
        public decimal UnitsOffPeakKwh { get; set; }
    }

    /// <summary>
    /// Response wrapper for Net Metering report.
    /// Contains separate Ordinary and Bulk sections with totals.
    /// </summary>
    public class NetMeteringResponse
    {
        /// <summary>Ordinary section data rows</summary>
        public List<NetMeteringData> Ordinary { get; set; }

        /// <summary>Ordinary total row</summary>
        public NetMeteringData OrdinaryTotal { get; set; }

        /// <summary>Bulk section data rows</summary>
        public List<NetMeteringData> Bulk { get; set; }

        /// <summary>Bulk total row</summary>
        public NetMeteringData BulkTotal { get; set; }

        /// <summary>Error message if any issues occurred</summary>
        public string ErrorMessage { get; set; }

        public NetMeteringResponse()
        {
            Ordinary = new List<NetMeteringData>();
            Bulk = new List<NetMeteringData>();
        }
    }
}