using MISReports_Api.DAL.PhysicalVerification;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace MISReports_Api.Controllers
{
    [RoutePrefix("api/phv-shortage-surplus-whwise")]
    public class PHVShoratgeSurplusWHwiseController : ApiController
    {
        private readonly PHVShoratgeSurplusWHwiseRepository _repository;

        public PHVShoratgeSurplusWHwiseController()
        {
            _repository = new PHVShoratgeSurplusWHwiseRepository();
        }

        [HttpGet]
        [Route("list")]
        public async Task<IHttpActionResult> GetShortageSurplusWHwise(
            string deptId,
            int repYear,
            int repMonth,
            string warehouseCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(deptId) ||
                    string.IsNullOrWhiteSpace(warehouseCode))
                {
                    return BadRequest("Department Id and Warehouse Code are required.");
                }

                var data = await _repository.GetShortageSurplusWHwiseAsync(
                    deptId.Trim(),
                    repYear,
                    repMonth,
                    warehouseCode.Trim());

                return Json(data);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
