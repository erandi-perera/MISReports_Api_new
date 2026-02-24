using System;
using System.Collections.Generic;

namespace MISReports_Api.Models.PUCSLReports.PUCSLSolarConnection
{
    /// <summary>
    /// Model for Raw Data for Solar Report (Report 4).
    /// Returns Net Metering (net_type='1') energy flow data with separate Ordinary and Bulk sections with totals.
    /// 
    /// IMPORTANT DIFFERENCE:
    /// - Ordinary: Only totals (no peak/off-peak breakdown)
    /// - Bulk: Has peak/off-peak breakdown
    /// </summary>
    public class RawDataForSolarResponse
    {
        public List<RawSolarDataOrdinary> Ordinary { get; set; }
        public RawSolarDataOrdinary OrdinaryTotal { get; set; }
        public List<RawSolarDataBulk> Bulk { get; set; }
        public RawSolarDataBulk BulkTotal { get; set; }
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// Ordinary Raw Solar Data - NO peak/off-peak breakdown.
    /// SQL: units_in (export), units_out (import), bf_units, cf_units
    /// </summary>
    public class RawSolarDataOrdinary
    {
        public string TariffCategory { get; set; }
        public string Year { get; set; }
        public string Month { get; set; }

        // Import/Export totals (no peak/off-peak breakdown)
        public decimal Import { get; set; }          // units_out
        public decimal Export { get; set; }          // units_in
        public decimal BroughtForward { get; set; }  // bf_units
        public decimal CarryForward { get; set; }    // cf_units
    }

    /// <summary>
    /// Bulk Raw Solar Data - HAS peak/off-peak breakdown.
    /// SQL: imp_kwd_units, imp_kwp_units, imp_kwo_units, exp_kwd_units, exp_kwp_units, exp_kwo_units, bf_units, cf_units
    /// </summary>
    public class RawSolarDataBulk
    {
        public string TariffCategory { get; set; }
        public string Year { get; set; }
        public string Month { get; set; }

        // Import - Day breakdown
        public decimal ImportDay { get; set; }       // imp_kwd_units
        public decimal ImportPeak { get; set; }      // imp_kwp_units
        public decimal ImportOffPeak { get; set; }   // imp_kwo_units

        // Export - Day breakdown
        public decimal ExportDay { get; set; }       // exp_kwd_units
        public decimal ExportPeak { get; set; }      // exp_kwp_units
        public decimal ExportOffPeak { get; set; }   // exp_kwo_units

        // Forward units
        public decimal BroughtForward { get; set; }  // bf_units
        public decimal CarryForward { get; set; }    // cf_units
    }
}