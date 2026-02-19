using MISReports_Api.DAL.Analysis;
using Newtonsoft.Json.Linq;
using System;
using System.Web.Http;

namespace MISReports_Api.Controllers
{
    [RoutePrefix("api/solar-age-analysis")]
    public class SolarAgeAnalysisController : ApiController
    {
        private readonly SolarAgeAnalysisRepository _repo =
            new SolarAgeAnalysisRepository();

        /// <summary>
        /// Get solar age analysis summary
        /// </summary>
        /// <param name="areaCode">Area code</param>
        /// <param name="billCycle">Billing cycle</param>
        [HttpGet]
        [Route("GetSummary")]
        public IHttpActionResult GetSummary(string areaCode, int billCycle)
        {
            if (string.IsNullOrWhiteSpace(areaCode))
                return BadRequest("Area code is required");

            if (billCycle <= 0)
                return BadRequest("Invalid bill cycle");

            try
            {
                var result = _repo.GetSummary(areaCode, billCycle);

                return Ok(JObject.FromObject(new
                {
                    data = result,
                    errorMessage = result.ErrorMessage
                }));
            }
            catch (Exception ex)
            {
                return Ok(JObject.FromObject(new
                {
                    data = (object)null,
                    errorMessage = ex.Message
                }));
            }
        }
    }
}
