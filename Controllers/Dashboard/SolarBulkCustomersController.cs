using MISReports_Api.DAL.Dashboard;
using MISReports_Api.Models.Dashboard;
using System;
using System.Web.Http;

namespace MISReports_Api.Controllers.Dashboard
{
    [RoutePrefix("api/dashboard/solar-bulk-customers")]
    public class SolarBulkCustomersController : ApiController
    {
        private readonly SolarBulkCustomersDao _solarBulkCustomersDao = new SolarBulkCustomersDao();

        [HttpGet]
        [Route("summary")]
        public IHttpActionResult GetSummary()
        {
            try
            {
                if (!_solarBulkCustomersDao.TestConnection(out string connError))
                {
                    return Ok(new
                    {
                        data = (object)null,
                        errorMessage = "Database connection failed.",
                        errorDetails = connError
                    });
                }

                var data = _solarBulkCustomersDao.GetSummary();

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
                    errorMessage = "Error retrieving solar bulk customers summary.",
                    errorDetails = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("count")]
        public IHttpActionResult GetTotalCustomersCount()
        {
            return GetCountResponse(
                _solarBulkCustomersDao.GetTotalCustomersCount,
                "Error retrieving total solar bulk customers count.");
        }

        [HttpGet]
        [Route("count/net-type-1")]
        public IHttpActionResult GetNetType1CustomersCount()
        {
            return GetCountResponse(
                _solarBulkCustomersDao.GetNetType1CustomersCount,
                "Error retrieving net type 1 solar bulk customers count.");
        }

        [HttpGet]
        [Route("count/net-type-2")]
        public IHttpActionResult GetNetType2CustomersCount()
        {
            return GetCountResponse(
                _solarBulkCustomersDao.GetNetType2CustomersCount,
                "Error retrieving net type 2 solar bulk customers count.");
        }

        [HttpGet]
        [Route("count/net-type-3")]
        public IHttpActionResult GetNetType3CustomersCount()
        {
            return GetCountResponse(
                _solarBulkCustomersDao.GetNetType3CustomersCount,
                "Error retrieving net type 3 solar bulk customers count.");
        }

        [HttpGet]
        [Route("count/net-type-4")]
        public IHttpActionResult GetNetType4CustomersCount()
        {
            return GetCountResponse(
                _solarBulkCustomersDao.GetNetType4CustomersCount,
                "Error retrieving net type 4 solar bulk customers count.");
        }

        private IHttpActionResult GetCountResponse(
            Func<SolarBulkCustomersCount> countGetter,
            string fallbackErrorMessage)
        {
            try
            {
                if (!_solarBulkCustomersDao.TestConnection(out string connError))
                {
                    return Ok(new
                    {
                        data = (object)null,
                        errorMessage = "Database connection failed.",
                        errorDetails = connError
                    });
                }

                var data = countGetter();

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
    }
}