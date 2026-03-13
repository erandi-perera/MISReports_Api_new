using MISReports_Api.DAL.Dashboard;
using MISReports_Api.Models.Dashboard;
using System;
using System.Web.Http;

namespace MISReports_Api.Controllers
{
    [RoutePrefix("api/dashboard")]
    public class DashboardController : ApiController
    {
        private readonly OrdinaryCustomersDao _dao = new OrdinaryCustomersDao();

        [HttpGet]
        [Route("ordinary-customers-summary")]
        public IHttpActionResult GetOrdinaryCustomersSummary([FromUri] string billCycle)
        {
            if (string.IsNullOrWhiteSpace(billCycle))
            {
                return Ok(new
                {
                    data = (object)null,
                    errorMessage = "Bill cycle is required."
                });
            }

            try
            {
                if (!_dao.TestConnection(out string connError))
                {
                    return Ok(new
                    {
                        data = (object)null,
                        errorMessage = "Database connection failed.",
                        errorDetails = connError
                    });
                }

                var data = _dao.GetOrdinaryCustomersCount(billCycle);

                return Ok(new
                {
                    data = data,
                    errorMessage = (string)null
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    data = (object)null,
                    errorMessage = "Error retrieving ordinary customers summary.",
                    errorDetails = ex.Message
                });
            }
        }
    }
}