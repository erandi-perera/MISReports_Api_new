namespace MISReports_Api.Models
{
    public class SolarAgeSummaryModel
    {
        public int Age_0_1 { get; set; }
        public int Age_1_2 { get; set; }
        public int Age_2_3 { get; set; }
        public int Age_3_4 { get; set; }
        public int Age_4_5 { get; set; }
        public int Age_5_6 { get; set; }
        public int Age_6_7 { get; set; }
        public int Age_7_8 { get; set; }
        public int Age_Above_8 { get; set; }
        public int AgreementDateNull { get; set; }

        public string ErrorMessage { get; set; }
    }
}
