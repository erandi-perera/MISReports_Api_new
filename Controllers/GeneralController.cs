using System;
using System.Collections.Generic;
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

        [HttpGet]
        [Route("metadata/areas")]
        public IHttpActionResult GetAreas() => SafeExecute(() => _smsDao.GetAreaList());

        [HttpGet]
        [Route("metadata/provinces")]
        public IHttpActionResult GetProvinces() => SafeExecute(() => _smsDao.GetProvinceList());

        [HttpGet]
        [Route("metadata/regions")]
        public IHttpActionResult GetRegions() => SafeExecute(() => _smsDao.GetRegionList());

        [HttpGet]
        [Route("metadata/bill-cycles")]
        public IHttpActionResult GetBillCycles() => SafeExecute(() => _smsDao.GetRecentBillCycles());

        // Helper to reduce code repetition
        private IHttpActionResult SafeExecute<T>(Func<T> action)
        {
            try
            {
                return Ok(new { data = action(), errorMessage = (string)null });
            }
            catch (Exception ex)
            {
                return Ok(new { data = (object)null, errorMessage = ex.Message });
            }
        }
    }
}