namespace MISReports_Api.Models
{
    public class ActiveCustomerTariffModel
    {
        public string Province { get; set; }
        public string Area { get; set; }
        public string Division { get; set; }
        public string BillCycle { get; set; }
        public string Tariff { get; set; }
        public decimal NoOfCustomers { get; set; }
    }
}