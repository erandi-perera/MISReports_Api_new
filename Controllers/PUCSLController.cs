using MISReports_Api.DAL.PUCSLReports.PUCSLSolarConnection;
using MISReports_Api.Models.SolarInformation;
using MISReports_Api.Models.PUCSLReports.PUCSLSolarConnection;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace MISReports_Api.Controllers
{
    /// <summary>
    /// Controller for PUCSL Solar Connection Summary reports.
    ///
    /// All endpoints accept a JSON POST body (PUCSLRequest):
    /// {
    ///   "reportCategory": "Province" | "Region" | "EntireCEB",
    ///   "typeCode":       "C"  (prov_code or region code; omit for EntireCEB),
    ///   "billCycle":      "202501",
    ///   "reportType":     "FixedSolarData" | "VariableSolarData",
    ///   "solarType":      "NetAccounting" | "NetPlus" | "NetPlusPlus"
    /// }
    /// </summary>
    [RoutePrefix("api")]
    public class PUCSLController : ApiController
    {
        private readonly FixedSolarDataDao _fixedSolarDataDao = new FixedSolarDataDao();
        // Future: private readonly VariableSolarDataDao _variableSolarDataDao = new VariableSolarDataDao();

        // ================================================================
        //  POST  pucslapi/solar-data
        //
        //  Routes to the correct DAO based on request.ReportType.
        //  VariableSolarData returns a placeholder until that DAO exists.
        // ================================================================
        [HttpPost]
        [Route("pucsl/solarConnections")]
        public IHttpActionResult GetSolarData([FromBody] PUCSLRequest request)
        {
            // ── Null body guard ───────────────────────────────────────
            if (request == null)
            {
                return Ok(JObject.FromObject(new
                {
                    data = (object)null,
                    errorMessage = "Request body is required."
                }));
            }

            // ── Validate common fields ────────────────────────────────
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.BillCycle))
                errors.Add("BillCycle is required.");

            if ((request.ReportCategory == PUCSLReportCategory.Province ||
                 request.ReportCategory == PUCSLReportCategory.Region) &&
                string.IsNullOrWhiteSpace(request.TypeCode))
            {
                errors.Add("TypeCode (province or region code) is required for Province/Region report category.");
            }

            if (errors.Count > 0)
            {
                return Ok(JObject.FromObject(new
                {
                    data = (object)null,
                    errorMessage = string.Join(" ", errors)
                }));
            }

            // ── Route by ReportType ───────────────────────────────────
            switch (request.ReportType)
            {
                case PUCSLReportType.FixedSolarData:
                    return ProcessFixedSolarData(request);

                case PUCSLReportType.VariableSolarData:
                    // Placeholder until VariableSolarDataDao is implemented
                    return Ok(JObject.FromObject(new
                    {
                        data = (object)null,
                        errorMessage = "Variable Solar Data report is not yet implemented."
                    }));

                default:
                    return Ok(JObject.FromObject(new
                    {
                        data = (object)null,
                        errorMessage = "Invalid ReportType. Valid values: FixedSolarData, VariableSolarData."
                    }));
            }
        }

        // ================================================================
        //  PRIVATE — Fixed Solar Data
        // ================================================================
        private IHttpActionResult ProcessFixedSolarData(PUCSLRequest request)
        {
            try
            {
                if (!_fixedSolarDataDao.TestConnection(out string connError))
                {
                    return Ok(JObject.FromObject(new
                    {
                        data = (object)null,
                        errorMessage = "Database connection failed.",
                        errorDetails = connError
                    }));
                }

                var data = _fixedSolarDataDao.GetFixedSolarDataReport(request);

                return Ok(JObject.FromObject(new
                {
                    data = data,
                    errorMessage = (string)null
                }));
            }
            catch (Exception ex)
            {
                return Ok(JObject.FromObject(new
                {
                    data = (object)null,
                    errorMessage = "Cannot retrieve Fixed Solar Data report.",
                    errorDetails = ex.Message
                }));
            }
        }
    }
}