using MISReports_Api.DAO;
using System.Web.Http;

namespace MISReports_Api.Controllers
{
    [RoutePrefix("api/active-customer-tariff")]
    public class ActiveCustomerTariffController : ApiController
    {
        private readonly ActiveCustomerTariffDao _dao;

        public ActiveCustomerTariffController()
        {
            _dao = new ActiveCustomerTariffDao();
        }

        [HttpGet]
        [Route("report")]
        public IHttpActionResult GetReport(
            string customerType,
            string level,
            int fromCycle,
            int toCycle)
        {
            // Validation
            if (string.IsNullOrEmpty(customerType))
                return BadRequest("Customer type is required.");

            if (string.IsNullOrEmpty(level))
                return BadRequest("Report level is required.");

            if (fromCycle <= 0 || toCycle <= 0)
                return BadRequest("Invalid bill cycle range.");

            if (fromCycle > toCycle)
                return BadRequest("FromCycle cannot be greater than ToCycle.");

            try
            {
                var result = _dao.GetReport(
                    customerType,
                    level,
                    fromCycle,
                    toCycle
                );

                if (!string.IsNullOrEmpty(result.ErrorMessage))
                {
                    return InternalServerError(
                        new System.Exception(result.ErrorMessage)
                    );
                }

                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}