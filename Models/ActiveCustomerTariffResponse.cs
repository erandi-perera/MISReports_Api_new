using System.Collections.Generic;

namespace MISReports_Api.Models
{
    public class ActiveCustomerTariffResponse
    {
        public string CustomerType { get; set; }
        public string ReportLevel { get; set; }
        public string FromCycle { get; set; }
        public string ToCycle { get; set; }

        public List<ActiveCustomerTariffModel> Data { get; set; }

        public string ErrorMessage { get; set; }

        public ActiveCustomerTariffResponse()
        {
            Data = new List<ActiveCustomerTariffModel>();
        }
    }
}