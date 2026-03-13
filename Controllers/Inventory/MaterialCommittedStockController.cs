using MISReports_Api.DAL;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace MISReports_Api.Controllers
{
    [RoutePrefix("api/materialcommittedstock")]
    public class MaterialCommittedStockController : ApiController
    {
        private readonly MaterialCommittedStockRepository _repository = new MaterialCommittedStockRepository();

        // GET api/materialcommittedstock/get?compId=01&matCode=D02
        [HttpGet]
        [Route("get")]
        public async Task<IHttpActionResult> GetMaterialCommittedStock(string compId, string matCode = null)
        {
            if (string.IsNullOrWhiteSpace(compId))
                return BadRequest("compId is required.");

            try
            {
                var result = await _repository.GetMaterialCommittedStock(compId.Trim(), matCode?.Trim());

                var response = new
                {
                    data = result,
                    errorMessage = (string)null
                };

                return Ok(JObject.Parse(JsonConvert.SerializeObject(response)));
            }
            catch (Exception ex)
            {
                var errorResponse = new
                {
                    data = (object)null,
                    errorMessage = "Cannot get Material Committed Stock data.",
                    errorDetails = ex.Message
                };

                return Ok(JObject.Parse(JsonConvert.SerializeObject(errorResponse)));
            }
        }
    }
}