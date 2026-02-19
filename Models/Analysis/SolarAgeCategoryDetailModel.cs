using System;

namespace MISReports_Api.Models.Analysis
{
    public class SolarAgeCategoryDetailModel
    {
        public string AccountNumber { get; set; }
        public int NetType { get; set; }
        public string NetTypeName { get; set; }
        public string CustomerName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public DateTime? AgreementDate { get; set; }
    }
}
