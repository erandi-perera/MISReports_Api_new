using MISReports_Api.DAL.Analysis;
using Newtonsoft.Json.Linq;
using System;
using System.Web.Http;

namespace MISReports_Api.Controllers
{
    [RoutePrefix("api/solar-age-category")]
    public class SolarAgeCategoryController : ApiController
    {
        private readonly SolarAgeCategoryRepository _repo =
            new SolarAgeCategoryRepository();

        [HttpGet, Route("age-below-1")]
        public IHttpActionResult AgeBelow1(string areaCode, int billCycle)
            => Execute(areaCode, billCycle, "LE1");

        [HttpGet, Route("age-1-to-2")]
        public IHttpActionResult Age1To2(string areaCode, int billCycle)
            => Execute(areaCode, billCycle, "1-2");

        [HttpGet, Route("age-2-to-3")]
        public IHttpActionResult Age2To3(string areaCode, int billCycle)
            => Execute(areaCode, billCycle, "2-3");

        [HttpGet, Route("age-3-to-4")]
        public IHttpActionResult Age3To4(string areaCode, int billCycle)
            => Execute(areaCode, billCycle, "3-4");

        [HttpGet, Route("age-4-to-5")]
        public IHttpActionResult Age4To5(string areaCode, int billCycle)
            => Execute(areaCode, billCycle, "4-5");

        [HttpGet, Route("age-5-to-6")]
        public IHttpActionResult Age5To6(string areaCode, int billCycle)
            => Execute(areaCode, billCycle, "5-6");

        [HttpGet, Route("age-6-to-7")]
        public IHttpActionResult Age6To7(string areaCode, int billCycle)
            => Execute(areaCode, billCycle, "6-7");

        [HttpGet, Route("age-7-to-8")]
        public IHttpActionResult Age7To8(string areaCode, int billCycle)
            => Execute(areaCode, billCycle, "7-8");

        [HttpGet, Route("age-above-8")]
        public IHttpActionResult AgeAbove8(string areaCode, int billCycle)
            => Execute(areaCode, billCycle, "GT8");

        [HttpGet, Route("agreement-date-null")]
        public IHttpActionResult AgreementDateNull(string areaCode, int billCycle)
            => Execute(areaCode, billCycle, "NULL");

        private IHttpActionResult Execute(
            string areaCode,
            int billCycle,
            string category)
        {
            if (string.IsNullOrWhiteSpace(areaCode))
                return BadRequest("Area code is required");

            if (billCycle <= 0)
                return BadRequest("Invalid bill cycle");

            try
            {
                var result = _repo.GetByCategory(areaCode, billCycle, category);

                return Ok(JObject.FromObject(new
                {
                    data = result,
                    count = result.Count,
                    errorMessage = ""
                }));
            }
            catch (Exception ex)
            {
                return Ok(JObject.FromObject(new
                {
                    data = (object)null,
                    count = 0,
                    errorMessage = ex.Message
                }));
            }
        }
    }
}
