using System;

namespace MISReports_Api.Models
{
    public class WorkInProgressProvinceModel
    {
        public string EstimateNo { get; set; }
        public string ProjectNo { get; set; }
        public decimal? StdCost { get; set; }
        public string Description { get; set; }
        public string FundId { get; set; }
        public string AccCode { get; set; }
        public string CatCd { get; set; }
        public string DeptId { get; set; }
        public string Area { get; set; }
        public DateTime? SoftCloseDate { get; set; }
        public DateTime? ConfDate { get; set; }
        public string ResourceType { get; set; }
        public decimal CommittedCost { get; set; }
        public string CctName { get; set; }
    }
}
