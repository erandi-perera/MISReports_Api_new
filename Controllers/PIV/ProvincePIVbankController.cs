// File: Controllers/ProvincePIVbankController.cs
using MISReports_Api.Models;
using MISReports_Api.Repositories;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace MISReports_Api.Controllers
{
    [RoutePrefix("api/provincepivbank")]
    public class ProvincePIVbankController : ApiController
    {
        private readonly ProvincePIVbankRepository _repository = new ProvincePIVbankRepository();

        // GET api/provincepivbank/get?compId=001&fromDate=20250101&toDate=20251231
        [HttpGet]
        [Route("get")]
        public IHttpActionResult Get(string compId, string fromDate, string toDate)
        {
            if (string.IsNullOrWhiteSpace(compId) || string.IsNullOrWhiteSpace(fromDate) || string.IsNullOrWhiteSpace(toDate))
                return BadRequest("compId, fromDate and toDate are required.");

            if (!DateTime.TryParseExact(fromDate, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime fromDt) ||
                !DateTime.TryParseExact(toDate, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime toDt))
                return BadRequest("Date format must be yyyyMMdd (example: 20250101)");

            try
            {
                List<ProvincePIVbankModel> data = _repository.GetReport(compId, fromDt, toDt);
                return Ok(data);
            }
            catch (Exception ex)
            {
                // In production: use your logging framework (NLog, log4net, etc.)
                return InternalServerError(ex);
            }
        }
    }
}