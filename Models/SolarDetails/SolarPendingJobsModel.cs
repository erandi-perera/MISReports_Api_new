//Branch wise Solar Pending Jobs after paid PIV2

using System;

namespace MISReports_Api.Models
{
    public class SolarPendingJobsModel
    {
        public string Dept_Id { get; set; }
        public string Application_Id { get; set; }
        public string Application_No { get; set; }
        public DateTime? Submit_Date { get; set; }
        public string ProjectNo { get; set; }
        public DateTime? Piv_Date { get; set; }
        public string Application_Sub_Type { get; set; } // Translated name (e.g. Net Metering)
        public DateTime? Paid_Date { get; set; }
        public DateTime? Piv2_Paid_Date { get; set; } // EST paid date
        public string Existing_Acc_No { get; set; }
        public string Status { get; set; } // Job No to be created / Contractor to be Allocated / Not Energized
        public string Comp_Nm { get; set; }
    }
}