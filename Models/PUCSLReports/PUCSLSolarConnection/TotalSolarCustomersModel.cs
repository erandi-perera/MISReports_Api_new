using System;

namespace MISReports_Api.Models.PUCSLReports.PUCSLSolarConnection
{
    /// <summary>
    /// Model for Total No of Solar Customers Report (Report 3).
    /// Tracks solar customer counts and units by net_type (scheme type).
    /// 
    /// Data is separated into:
    /// - Ordinary: Regular billing customers
    /// - Bulk: Bulk billing customers
    /// 
    /// Each has 4 net_type categories:
    /// - Net Metering (net_type='1')
    /// - Net Accounting (net_type='2' OR net_type='5')
    /// - Net Plus (net_type='3')
    /// - Net Plus Plus (net_type='4')
    /// </summary>
    public class TotalSolarCustomersModel
    {
        // Tariff category identifier
        public string TariffCategory { get; set; }

        // ===== ORDINARY DATA =====

        // Net Metering (net_type='1')
        public int OrdinaryNetMeteringCustomers { get; set; }
        public decimal OrdinaryNetMeteringUnits { get; set; }

        // Net Accounting (net_type='2' OR net_type='5')
        public int OrdinaryNetAccountingCustomers { get; set; }
        public decimal OrdinaryNetAccountingUnits { get; set; }

        // Net Plus (net_type='3')
        public int OrdinaryNetPlusCustomers { get; set; }
        public decimal OrdinaryNetPlusUnits { get; set; }

        // Net Plus Plus (net_type='4')
        public int OrdinaryNetPlusPlusCustomers { get; set; }
        public decimal OrdinaryNetPlusPlusUnits { get; set; }

        // ===== BULK DATA =====

        // Net Metering (net_type='1')
        public int BulkNetMeteringCustomers { get; set; }
        public decimal BulkNetMeteringUnits { get; set; }

        // Net Accounting (net_type='2' OR net_type='5')
        public int BulkNetAccountingCustomers { get; set; }
        public decimal BulkNetAccountingUnits { get; set; }

        // Net Plus (net_type='3')
        public int BulkNetPlusCustomers { get; set; }
        public decimal BulkNetPlusUnits { get; set; }

        // Net Plus Plus (net_type='4')
        public int BulkNetPlusPlusCustomers { get; set; }
        public decimal BulkNetPlusPlusUnits { get; set; }

        // Error handling
        public string ErrorMessage { get; set; }
    }
}