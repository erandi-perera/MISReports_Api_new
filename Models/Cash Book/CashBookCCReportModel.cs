// Models/CashBookCCReportModel.cs
using System;

namespace MISReports_Api.Models
{
    public class CashBookCCReportModel
    {
        public string ChqRun { get; set; }
        public DateTime? ChqDt { get; set; }
        public string Payee { get; set; }
        public string PymtDocNo { get; set; }
        public decimal? ChqAmt { get; set; }
        public string ChqNo { get; set; }
        public string CctName { get; set; } // from gldeptm.dept_nm
    }
}