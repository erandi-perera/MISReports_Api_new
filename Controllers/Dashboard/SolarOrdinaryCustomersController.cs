using MISReports_Api.DAL.Dashboard;
using MISReports_Api.Models.Dashboard;
using System;
using System.Web.Http;

namespace MISReports_Api.Controllers.Dashboard
{
    [RoutePrefix("api/dashboard/solar-ordinary-customers")]
    public class SolarOrdinaryCustomersController : ApiController
    {
        private readonly SolarOrdinaryCustomersDao _solarOrdinaryCustomersDao = new SolarOrdinaryCustomersDao();

        [HttpGet]
        [Route("billcycle/max")]
        public IHttpActionResult GetMaxBillCycle()
        {
            try
            {
                if (!_solarOrdinaryCustomersDao.TestConnection(out string connError))
                {
                    return Ok(new
                    {
                        data = (object)null,
                        errorMessage = "Database connection failed.",
                        errorDetails = connError
                    });
                }

                var maxBillCycle = _solarOrdinaryCustomersDao.GetLatestBillCycle();

                return Ok(new
                {
                    data = new { billCycle = maxBillCycle },
                    errorMessage = string.IsNullOrWhiteSpace(maxBillCycle) ? "No bill cycle found in netmtcons." : (string)null
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    data = (object)null,
                    errorMessage = "Error retrieving max bill cycle.",
                    errorDetails = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("count")]
        public IHttpActionResult GetTotalCustomersCount([FromUri] string billCycle = null)
        {
            return GetCustomersCountResponse(
                billCycle,
                _solarOrdinaryCustomersDao.GetTotalCustomersCount,
                "Error retrieving total customers count.");
        }

        [HttpGet]
        [Route("count/net-type-1")]
        public IHttpActionResult GetNetType1CustomersCount([FromUri] string billCycle = null)
        {
            return GetCustomersCountResponse(
                billCycle,
                _solarOrdinaryCustomersDao.GetNetMeteringCustomersCount,
            "Error retrieving net type 1 customers count.");
        }

        [HttpGet]
        [Route("count/net-type-2")]
        public IHttpActionResult GetNetType2CustomersCount([FromUri] string billCycle = null)
        {
            return GetCustomersCountResponse(
                billCycle,
                _solarOrdinaryCustomersDao.GetNetAccountingCustomersCount,
            "Error retrieving net type 2 customers count.");
        }

        [HttpGet]
        [Route("count/net-type-3")]
        public IHttpActionResult GetNetType3CustomersCount([FromUri] string billCycle = null)
        {
            return GetCustomersCountResponse(
                billCycle,
                _solarOrdinaryCustomersDao.GetNetPlusCustomersCount,
            "Error retrieving net type 3 customers count.");
        }

        [HttpGet]
        [Route("count/net-type-4")]
        public IHttpActionResult GetNetType4CustomersCount([FromUri] string billCycle = null)
        {
            return GetCustomersCountResponse(
                billCycle,
                _solarOrdinaryCustomersDao.GetNetPlusPlusCustomersCount,
            "Error retrieving net type 4 customers count.");
        }

        private IHttpActionResult GetCustomersCountResponse(
            string billCycle,
            Func<string, SolarOrdinaryCustomersCount> countGetter,
            string fallbackErrorMessage)
        {
            try
            {
                if (!_solarOrdinaryCustomersDao.TestConnection(out string connError))
                {
                    return Ok(new
                    {
                        data = (object)null,
                        errorMessage = "Database connection failed.",
                        errorDetails = connError
                    });
                }

                var normalizedBillCycle = NormalizeBillCycle(billCycle);
                var data = countGetter(normalizedBillCycle);

                return Ok(new
                {
                    data = data,
                    errorMessage = string.IsNullOrWhiteSpace(data.ErrorMessage) ? (string)null : data.ErrorMessage
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    data = (object)null,
                    errorMessage = fallbackErrorMessage,
                    errorDetails = ex.Message
                });
            }
        }

        private string NormalizeBillCycle(string billCycle)
        {
            if (string.IsNullOrWhiteSpace(billCycle))
            {
                return null;
            }

            var normalized = billCycle.Trim();

            if ((normalized.StartsWith("{") && normalized.EndsWith("}")) ||
                normalized.Equals("null", StringComparison.OrdinalIgnoreCase) ||
                normalized.Equals("undefined", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return normalized;
        }
    }
}