using System;
using System.Web.Http;
using MISReports_Api.DAL;
using MISReports_Api.Models;

namespace MISReports_Api.Controllers
{
    [RoutePrefix("api/general")]
    public class GeneralController : ApiController
    {
        private readonly RegisteredCustomersBillCycleDao _smsDao = new RegisteredCustomersBillCycleDao();

        [HttpGet]
        [Route("original/smsRegisteredRange")]
        public IHttpActionResult GetSMSRegisteredRange(
            [FromUri] string fromCycle,
            [FromUri] string toCycle,
            [FromUri] string reportType,
            [FromUri] string typeCode = null)
        {
            try
            {
                var request = new SMSUsageRequest
                {
                    FromBillCycle = fromCycle,
                    ToBillCycle = toCycle,
                    ReportType = reportType,
                    TypeCode = typeCode
                };

                var monthlyData = _smsDao.GetSMSCountRange(request);

                // Using the updated model name here
                return Ok(new
                {
                    data = new SMSRegisteredCustomersModel
                    {
                        LocationName = string.IsNullOrEmpty(typeCode) ? "Entire CEB" : typeCode,
                        MonthlyCounts = monthlyData
                    },
                    errorMessage = (string)null
                });
            }
            catch (Exception ex)
            {
                return Ok(new { data = (object)null, errorMessage = ex.Message });
            }
        }
    }
}