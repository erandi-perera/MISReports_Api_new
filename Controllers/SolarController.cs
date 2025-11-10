using MISReports_Api.DAL.SolarInformation.SolarProgressClarification;
using MISReports_Api.DAL.SolarInformation.SolarPVConnections;
using MISReports_Api.DAL.SolarInformation.SolarPaymentRetail;
using MISReports_Api.DAL.SolarInformation.SolarPVCapacity;
using MISReports_Api.DAL.SolarInformation.SolarConnectionDetails;
using MISReports_Api.DAL.SolarInformation;
using MISReports_Api.DAL.Shared;
using MISReports_Api.Models.SolarInformation;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Linq;

namespace MISReports_Api.Controllers
{
    [RoutePrefix("solarapi")]
    public class SolarController : ApiController
    {
        private readonly AreasDao _areasDao = new AreasDao();
        private readonly ProvinceDao _provinceDao = new ProvinceDao();
        private readonly RegionDao _regionDao = new RegionDao();
        private readonly BillCycleDao _billCycleDao = new BillCycleDao();
        private readonly PVBillCycleDao _pvBillCycleDao = new PVBillCycleDao();
        private readonly DetailedDao _detailedDao = new DetailedDao();
        private readonly SummaryDao _summaryDao = new SummaryDao();
        private readonly PVConnectionDao _pvConnectionDao = new PVConnectionDao();
        private readonly PVBulkConnectionDao _pvBulkConnectionDao = new PVBulkConnectionDao();
        private readonly OrdinaryDetailedDao _ordinaryDetailedDao = new OrdinaryDetailedDao();
        private readonly OrdinarySummaryDao _ordinarySummaryDao = new OrdinarySummaryDao();
        private readonly ProvinceOrdinaryDao _provinceOrdinaryDao = new ProvinceOrdinaryDao();
        private readonly RegionOrdinaryDao _regionOrdinaryDao = new RegionOrdinaryDao();
        private readonly BillCycleOrdinaryDao _billCycleOrdinaryDao = new BillCycleOrdinaryDao();
        private readonly BillCycleRetailDao _billCycleRetailDao = new BillCycleRetailDao();
        private readonly RetailDetailedDao _retailDetailedDao = new RetailDetailedDao();
        private readonly OrdSummaryDao _ordSummaryDao = new OrdSummaryDao();
        private readonly BulkSummaryDao _bulkSummaryDao = new BulkSummaryDao();
        private readonly PVCapacityBillCycleDao _pVCapacityBillCycleDao = new PVCapacityBillCycleDao();
        private readonly PVCapacityOrdinaryDao _pvCapacityOrdinaryDao = new PVCapacityOrdinaryDao();
        private readonly PVCapacityBulkDao _pvCapacityBulkDao = new PVCapacityBulkDao();
        private readonly PVCapacitySummaryDao _pvCapacitySummaryDao = new PVCapacitySummaryDao();
        private readonly SolarPaymentBulkDao _solarPaymentBulkDao = new SolarPaymentBulkDao();
        private readonly SolarReadingRetailDetailedDao _solarReadingDetailedDao = new SolarReadingRetailDetailedDao();
        private readonly SolarReadingRetailSummaryDao _solarReadingSummaryDao = new SolarReadingRetailSummaryDao();
        private readonly SolarReadingUsageBulkDao _solarReadingUsageBulkDao = new SolarReadingUsageBulkDao();

        [HttpGet]
        [Route("areas")]
        public IHttpActionResult GetAreas()
        {
            try
            {
                if (!_areasDao.TestConnection(out string connError))
                {
                    return Ok(JObject.FromObject(new
                    {
                        data = (object)null,
                        errorMessage = "Database connection failed.",
                        errorDetails = connError
                    }));
                }

                var areas = _areasDao.GetAreas();

                return Ok(JObject.FromObject(new
                {
                    data = areas,
                    errorMessage = (string)null
                }));
            }
            catch (Exception ex)
            {
                return Ok(JObject.FromObject(new
                {
                    data = (object)null,
                    errorMessage = "Cannot get areas data.",
                    errorDetails = ex.Message
                }));
            }
        }

        [HttpGet]
        [Route("province")]
        public IHttpActionResult GetProvince()
        {
            try
            {
                if (!_provinceDao.TestConnection(out string connError))
                {
                    return Ok(JObject.FromObject(new
                    {
                        data = (object)null,
                        errorMessage = "Database connection failed.",
                        errorDetails = connError
                    }));
                }

                var province = _provinceDao.GetProvince();

                return Ok(JObject.FromObject(new
                {
                    data = province,
                    errorMessage = (string)null
                }));
            }
            catch (Exception ex)
            {
                return Ok(JObject.FromObject(new
                {
                    data = (object)null,
                    errorMessage = "Cannot get province data.",
                    errorDetails = ex.Message
                }));
            }
        }

        [HttpGet]
        [Route("ordinary/province")]
        public IHttpActionResult GetOrdinaryProvince()
        {
            try
            {
                if (!_provinceOrdinaryDao.TestConnection(out string connError))
                {
                    return Ok(JObject.FromObject(new
                    {
                        data = (object)null,
                        errorMessage = "Database connection failed.",
                        errorDetails = connError
                    }));
                }

                var province = _provinceOrdinaryDao.GetProvince();

                return Ok(JObject.FromObject(new
                {
                    data = province,
                    errorMessage = (string)null
                }));
            }
            catch (Exception ex)
            {
                return Ok(JObject.FromObject(new
                {
                    data = (object)null,
                    errorMessage = "Cannot get province data.",
                    errorDetails = ex.Message
                }));
            }
        }

        [HttpGet]
        [Route("region")]
        public IHttpActionResult GetRegion()
        {
            try
            {
                if (!_regionDao.TestConnection(out string connError))
                {
                    return Ok(JObject.FromObject(new
                    {
                        data = (object)null,
                        errorMessage = "Database connection failed.",
                        errorDetails = connError
                    }));
                }

                var region = _regionDao.GetRegion();

                return Ok(JObject.FromObject(new
                {
                    data = region,
                    errorMessage = (string)null
                }));
            }
            catch (Exception ex)
            {
                return Ok(JObject.FromObject(new
                {
                    data = (object)null,
                    errorMessage = "Cannot get region data.",
                    errorDetails = ex.Message
                }));
            }
        }

        [HttpGet]
        [Route("ordinary/region")]
        public IHttpActionResult GetOrdinaryRegion()
        {
            try
            {
                if (!_regionOrdinaryDao.TestConnection(out string connError))
                {
                    return Ok(JObject.FromObject(new
                    {
                        data = (object)null,
                        errorMessage = "Database connection failed.",
                        errorDetails = connError
                    }));
                }

                var region = _regionOrdinaryDao.GetRegion();

                return Ok(JObject.FromObject(new
                {
                    data = region,
                    errorMessage = (string)null
                }));
            }
            catch (Exception ex)
            {
                return Ok(JObject.FromObject(new
                {
                    data = (object)null,
                    errorMessage = "Cannot get region data.",
                    errorDetails = ex.Message
                }));
            }
        }

        [HttpGet]
        [Route("billcycle/max")]
        public IHttpActionResult GetMaxBillCycle()
        {
            try
            {
                var result = _billCycleDao.GetLast24BillCycles();//From netmtchg table in InformixBulkConnection database

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
                    errorMessage = "Cannot get max bill cycle",
                    errorDetails = ex.Message
                }));
            }
        }

        [HttpGet]
        [Route("bill-cycle")]
        public IHttpActionResult GetPVBillCycle()
        {
            try
            {
                var result = _pvBillCycleDao.GetLast24BillCycles();//From netmtcons table in InformixBulkConnection database

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
                    errorMessage = "Cannot get max bill cycle",
                    errorDetails = ex.Message
                }));
            }
        }

        [HttpGet]
        [Route("ordinary/bill-cycle")]
        public IHttpActionResult GetOrdinaryBillCycle()
        {
            try
            {
                var result = _billCycleOrdinaryDao.GetLast24BillCycles();//From netmtchg table in InformixConnection database

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
                    errorMessage = "Cannot get max bill cycle",
                    errorDetails = ex.Message
                }));
            }
        }

        [HttpGet]
        [Route("retail/billcycle")]
        public IHttpActionResult GetRetailBillCycle()
        {
            try
            {
                var result = _billCycleRetailDao.GetLast24BillCycles();//From netmtcons table in InformixConnection database

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
                    errorMessage = "Cannot get max bill cycle",
                    errorDetails = ex.Message
                }));
            }
        }

        [HttpGet]
        [Route("solar-progress/detailed")]
        public IHttpActionResult GetDetailedReport(
            [FromUri] string billCycle,
            [FromUri] string reportType,
            [FromUri] string typeCode = null)
        {
            var validationErrors = new List<string>();

            if (string.IsNullOrWhiteSpace(billCycle))
                validationErrors.Add("Bill cycle is required.");

            if (string.IsNullOrWhiteSpace(reportType))
                validationErrors.Add("Report type is required.");

            if (validationErrors.Count > 0)
            {
                return Ok(new
                {
                    data = (object)null,
                    errorMessage = string.Join("; ", validationErrors)
                });
            }

            var request = new SolarProgressRequest
            {
                BillCycle = billCycle
            };

            switch (reportType.ToLower())
            {
                case "area":
                    request.ReportType = SolarReportType.Area;
                    request.AreaCode = typeCode;
                    break;
                case "province":
                    request.ReportType = SolarReportType.Province;
                    request.ProvCode = typeCode;
                    break;
                case "region":
                    request.ReportType = SolarReportType.Region;
                    request.Region = typeCode;
                    break;
                case "entireceb":
                    request.ReportType = SolarReportType.EntireCEB;
                    break;
                default:
                    return Ok(new
                    {
                        data = (object)null,
                        errorMessage = "Invalid report type.",
                        errorDetails = "Valid types: Area, Province, Region, EntireCEB."
                    });
            }

            return ProcessDetailedRequest(request);
        }

        [HttpGet]
        [Route("solar-progress/summary")]
        public IHttpActionResult GetSummaryReport(
            [FromUri] string billCycle,
            [FromUri] string reportType,
            [FromUri] string typeCode = null)
        {
            var validationErrors = new List<string>();

            if (string.IsNullOrWhiteSpace(billCycle))
                validationErrors.Add("Bill cycle is required.");

            if (string.IsNullOrWhiteSpace(reportType))
                validationErrors.Add("Report type is required.");

            if (validationErrors.Count > 0)
            {
                return Ok(new
                {
                    data = (object)null,
                    errorMessage = string.Join("; ", validationErrors)
                });
            }

            var request = new SolarProgressRequest
            {
                BillCycle = billCycle
            };

            switch (reportType.ToLower())
            {
                case "area":
                    request.ReportType = SolarReportType.Area;
                    request.AreaCode = typeCode;
                    break;
                case "province":
                    request.ReportType = SolarReportType.Province;
                    request.ProvCode = typeCode;
                    break;
                case "region":
                    request.ReportType = SolarReportType.Region;
                    request.Region = typeCode;
                    break;
                case "entireceb":
                    request.ReportType = SolarReportType.EntireCEB;
                    break;
                default:
                    return Ok(new
                    {
                        data = (object)null,
                        errorMessage = "Invalid report type.",
                        errorDetails = "Valid types: Area, Province, Region, EntireCEB."
                    });
            }

            return ProcessSummaryRequest(request);
        }

        [HttpGet]
        [Route("solar-progress/ordinary/detailed")]
        public IHttpActionResult GetOrdinaryDetailedReport(
            [FromUri] string billCycle,
            [FromUri] string reportType,
            [FromUri] string typeCode = null)
        {
            var validationErrors = new List<string>();

            if (string.IsNullOrWhiteSpace(billCycle))
                validationErrors.Add("Bill cycle is required.");

            if (string.IsNullOrWhiteSpace(reportType))
                validationErrors.Add("Report type is required.");

            if (validationErrors.Count > 0)
            {
                return Ok(new
                {
                    data = (object)null,
                    errorMessage = string.Join("; ", validationErrors)
                });
            }

            var request = new SolarProgressRequest
            {
                BillCycle = billCycle
            };

            switch (reportType.ToLower())
            {
                case "area":
                    request.ReportType = SolarReportType.Area;
                    request.AreaCode = typeCode;
                    break;
                case "province":
                    request.ReportType = SolarReportType.Province;
                    request.ProvCode = typeCode;
                    break;
                case "region":
                    request.ReportType = SolarReportType.Region;
                    request.Region = typeCode;
                    break;
                case "entireceb":
                    request.ReportType = SolarReportType.EntireCEB;
                    break;
                default:
                    return Ok(new
                    {
                        data = (object)null,
                        errorMessage = "Invalid report type.",
                        errorDetails = "Valid types: Area, Province, Region, EntireCEB."
                    });
            }

            return ProcessOrdinaryDetailedRequest(request);
        }

        private IHttpActionResult ProcessOrdinaryDetailedRequest(SolarProgressRequest request)
        {
            try
            {
                if (!_ordinaryDetailedDao.TestConnection(out string connError))
                {
                    return Ok(new
                    {
                        data = (object)null,
                        errorMessage = "Database connection failed.",
                        errorDetails = connError
                    });
                }

                string typeValidationError = ValidateRequestParameters(request);
                if (!string.IsNullOrEmpty(typeValidationError))
                {
                    return Ok(new
                    {
                        data = (object)null,
                        errorMessage = typeValidationError,
                        errorDetails = "Invalid request parameters."
                    });
                }

                var data = _ordinaryDetailedDao.GetOrdinaryDetailedReport(request);

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
                    errorMessage = "Cannot get solar progress data.",
                    errorDetails = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("solar-progress/ordinary/summary")]
        public IHttpActionResult GetOrdinarySummaryReport(
            [FromUri] string billCycle,
            [FromUri] string reportType,
            [FromUri] string typeCode = null)
        {
            var validationErrors = new List<string>();

            if (string.IsNullOrWhiteSpace(billCycle))
                validationErrors.Add("Bill cycle is required.");

            if (string.IsNullOrWhiteSpace(reportType))
                validationErrors.Add("Report type is required.");

            if (validationErrors.Count > 0)
            {
                return Ok(new
                {
                    data = (object)null,
                    errorMessage = string.Join("; ", validationErrors)
                });
            }

            var request = new SolarProgressRequest
            {
                BillCycle = billCycle
            };

            switch (reportType.ToLower())
            {
                case "area":
                    request.ReportType = SolarReportType.Area;
                    request.AreaCode = typeCode;
                    break;
                case "province":
                    request.ReportType = SolarReportType.Province;
                    request.ProvCode = typeCode;
                    break;
                case "region":
                    request.ReportType = SolarReportType.Region;
                    request.Region = typeCode;
                    break;
                case "entireceb":
                    request.ReportType = SolarReportType.EntireCEB;
                    break;
                default:
                    return Ok(new
                    {
                        data = (object)null,
                        errorMessage = "Invalid report type.",
                        errorDetails = "Valid types: Area, Province, Region, EntireCEB."
                    });
            }

            return ProcessOrdinarySummaryRequest(request);
        }

        private IHttpActionResult ProcessOrdinarySummaryRequest(SolarProgressRequest request)
        {
            try
            {
                if (!_ordinarySummaryDao.TestConnection(out string connError))
                {
                    return Ok(new
                    {
                        data = (object)null,
                        errorMessage = "Database connection failed.",
                        errorDetails = connError
                    });
                }

                string typeValidationError = ValidateRequestParameters(request);
                if (!string.IsNullOrEmpty(typeValidationError))
                {
                    return Ok(new
                    {
                        data = (object)null,
                        errorMessage = typeValidationError,
                        errorDetails = "Invalid request parameters."
                    });
                }

                var data = _ordinarySummaryDao.GetSummaryReport(request);

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
                    errorMessage = "Cannot get solar progress data.",
                    errorDetails = ex.Message
                });
            }
        }

        private IHttpActionResult ProcessDetailedRequest(SolarProgressRequest request)
        {
            try
            {
                if (!_detailedDao.TestConnection(out string connError))
                {
                    return Ok(new
                    {
                        data = (object)null,
                        errorMessage = "Database connection failed.",
                        errorDetails = connError
                    });
                }

                string typeValidationError = ValidateRequestParameters(request);
                if (!string.IsNullOrEmpty(typeValidationError))
                {
                    return Ok(new
                    {
                        data = (object)null,
                        errorMessage = typeValidationError,
                        errorDetails = "Invalid request parameters."
                    });
                }

                var data = _detailedDao.GetDetailedReport(request);

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
                    errorMessage = "Cannot get solar progress data.",
                    errorDetails = ex.Message
                });
            }
        }

        private IHttpActionResult ProcessSummaryRequest(SolarProgressRequest request)
        {
            try
            {
                if (!_summaryDao.TestConnection(out string connError))
                {
                    return Ok(new
                    {
                        data = (object)null,
                        errorMessage = "Database connection failed.",
                        errorDetails = connError
                    });
                }

                string typeValidationError = ValidateRequestParameters(request);
                if (!string.IsNullOrEmpty(typeValidationError))
                {
                    return Ok(new
                    {
                        data = (object)null,
                        errorMessage = typeValidationError,
                        errorDetails = "Invalid request parameters."
                    });
                }

                var data = _summaryDao.GetSummaryReport(request);

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
                    errorMessage = "Cannot get solar progress data.",
                    errorDetails = ex.Message
                });
            }
        }

        /// <summary>
        /// Common validation method for report type parameters
        /// </summary>
        private string ValidateReportTypeParameters(
            SolarReportType reportType,
            string areaCode,
            string provCode,
            string region)
        {
            switch (reportType)
            {
                case SolarReportType.Area:
                    if (string.IsNullOrEmpty(areaCode))
                        return "Area code is required for Area report type.";
                    break;
                case SolarReportType.Province:
                    if (string.IsNullOrEmpty(provCode))
                        return "Province code is required for Province report type.";
                    break;
                case SolarReportType.Region:
                    if (string.IsNullOrEmpty(region))
                        return "Region is required for Region report type.";
                    break;
                case SolarReportType.EntireCEB:
                    break;
                default:
                    return "Invalid report type specified.";
            }

            return null;
        }

        private string ValidateRequestParameters(SolarProgressRequest request)
        {
            return ValidateReportTypeParameters(
                request.ReportType,
                request.AreaCode,
                request.ProvCode,
                request.Region
            );
        }

        [HttpGet]
        [Route("pv-connections")]
        public IHttpActionResult GetPVConnections(
            [FromUri] string billCycle = null,
            [FromUri] string calcCycle = null,
            [FromUri] string cycleType = "A", // A for bill_cycle, C for calc_cycle
            [FromUri] string reportType = "entireceb",
            [FromUri] string typeCode = null)
        {
            var validationErrors = new List<string>();

            // Validate cycle parameters
            if (cycleType == "A" && string.IsNullOrWhiteSpace(billCycle))
                validationErrors.Add("Bill cycle is required when cycle type is 'A'.");

            if (cycleType == "C" && string.IsNullOrWhiteSpace(calcCycle))
                validationErrors.Add("Calc cycle is required when cycle type is 'C'.");

            if (cycleType != "A" && cycleType != "C")
                validationErrors.Add("Cycle type must be 'A' (bill cycle) or 'C' (calc cycle).");

            if (validationErrors.Count > 0)
            {
                return Ok(new
                {
                    data = (object)null,
                    errorMessage = string.Join("; ", validationErrors)
                });
            }

            var request = new SolarPVConnectionRequest
            {
                BillCycle = billCycle,
                CalcCycle = calcCycle,
                CycleType = cycleType
            };

            switch (reportType.ToLower())
            {
                case "area":
                    request.ReportType = SolarReportType.Area;
                    request.AreaCode = typeCode;
                    break;
                case "province":
                    request.ReportType = SolarReportType.Province;
                    request.ProvCode = typeCode;
                    break;
                case "region":
                    request.ReportType = SolarReportType.Region;
                    request.Region = typeCode;
                    break;
                case "entireceb":
                    request.ReportType = SolarReportType.EntireCEB;
                    break;
                default:
                    return Ok(new
                    {
                        data = (object)null,
                        errorMessage = "Invalid report type.",
                        errorDetails = "Valid types: Area, Province, Region, EntireCEB."
                    });
            }

            return ProcessPVConnectionRequest(request);
        }

        [HttpGet]
        [Route("pv-bulkconnections")]
        public IHttpActionResult GetPVBulkConnections(
            [FromUri] string billCycle = null,
            [FromUri] string calcCycle = null,
            [FromUri] string cycleType = "A", // A for bill_cycle, C for calc_cycle
            [FromUri] string reportType = "entireceb",
            [FromUri] string typeCode = null)
        {
            var validationErrors = new List<string>();

            // Validate cycle parameters
            if (cycleType == "A" && string.IsNullOrWhiteSpace(billCycle))
                validationErrors.Add("Bill cycle is required when cycle type is 'A'.");

            if (cycleType == "C" && string.IsNullOrWhiteSpace(calcCycle))
                validationErrors.Add("Calc cycle is required when cycle type is 'C'.");

            if (cycleType != "A" && cycleType != "C")
                validationErrors.Add("Cycle type must be 'A' (bill cycle) or 'C' (calc cycle).");

            if (validationErrors.Count > 0)
            {
                return Ok(new
                {
                    data = (object)null,
                    errorMessage = string.Join("; ", validationErrors)
                });
            }

            var request = new SolarPVBulkConnectionRequest
            {
                BillCycle = billCycle,
                CalcCycle = calcCycle,
                CycleType = cycleType
            };

            switch (reportType.ToLower())
            {
                case "area":
                    request.ReportType = SolarReportType.Area;
                    request.AreaCode = typeCode;
                    break;
                case "province":
                    request.ReportType = SolarReportType.Province;
                    request.ProvCode = typeCode;
                    break;
                case "region":
                    request.ReportType = SolarReportType.Region;
                    request.Region = typeCode;
                    break;
                case "entireceb":
                    request.ReportType = SolarReportType.EntireCEB;
                    break;
                default:
                    return Ok(new
                    {
                        data = (object)null,
                        errorMessage = "Invalid report type.",
                        errorDetails = "Valid types: Area, Province, Region, EntireCEB."
                    });
            }

            return ProcessPVBulkConnectionRequest(request);
        }

        private IHttpActionResult ProcessPVBulkConnectionRequest(SolarPVBulkConnectionRequest request)
        {
            try
            {
                if (!_pvConnectionDao.TestConnection(out string connError))
                {
                    return Ok(new
                    {
                        data = (object)null,
                        errorMessage = "Database connection failed.",
                        errorDetails = connError
                    });
                }

                string typeValidationError = ValidatePVBulkConnectionParameters(request);
                if (!string.IsNullOrEmpty(typeValidationError))
                {
                    return Ok(new
                    {
                        data = (object)null,
                        errorMessage = typeValidationError,
                        errorDetails = "Invalid request parameters."
                    });
                }

                var data = _pvBulkConnectionDao.GetPVConnections(request);

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
                    errorMessage = "Cannot get PV connections data.",
                    errorDetails = ex.Message
                });
            }
        }

        private string ValidatePVBulkConnectionParameters(SolarPVBulkConnectionRequest request)
        {
            return ValidateReportTypeParameters(
                request.ReportType,
                request.AreaCode,
                request.ProvCode,
                request.Region
            );
        }



        private IHttpActionResult ProcessPVConnectionRequest(SolarPVConnectionRequest request)
        {
            try
            {
                if (!_pvConnectionDao.TestConnection(out string connError))
                {
                    return Ok(new
                    {
                        data = (object)null,
                        errorMessage = "Database connection failed.",
                        errorDetails = connError
                    });
                }

                string typeValidationError = ValidatePVConnectionParameters(request);
                if (!string.IsNullOrEmpty(typeValidationError))
                {
                    return Ok(new
                    {
                        data = (object)null,
                        errorMessage = typeValidationError,
                        errorDetails = "Invalid request parameters."
                    });
                }

                var data = _pvConnectionDao.GetPVConnections(request);

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
                    errorMessage = "Cannot get PV connections data.",
                    errorDetails = ex.Message
                });
            }
        }

        private string ValidatePVConnectionParameters(SolarPVConnectionRequest request)
        {
            return ValidateReportTypeParameters(
                request.ReportType,
                request.AreaCode,
                request.ProvCode,
                request.Region
            );
        }

        [HttpGet]
        [Route("retail/detailed")]
        public IHttpActionResult GetRetailDetailedReport(
            [FromUri] string billCycle = null,
            [FromUri] string calcCycle = null,
            [FromUri] string cycleType = "A", // A for bill_cycle, C for calc_cycle
            [FromUri] string netType = "1",   // Net type filter (1, 2, 3, 4, 5)
            [FromUri] string reportType = "entireceb",
            [FromUri] string typeCode = null)
        {
            var validationErrors = new List<string>();

            // Validate cycle parameters
            if (cycleType == "A" && string.IsNullOrWhiteSpace(billCycle))
                validationErrors.Add("Bill cycle is required when cycle type is 'A'.");

            if (cycleType == "C" && string.IsNullOrWhiteSpace(calcCycle))
                validationErrors.Add("Calc cycle is required when cycle type is 'C'.");

            if (cycleType != "A" && cycleType != "C")
                validationErrors.Add("Cycle type must be 'A' (bill cycle) or 'C' (calc cycle).");

            // Validate net type
            if (string.IsNullOrWhiteSpace(netType))
                validationErrors.Add("Net type is required.");
            else if (!new[] { "1", "2", "3", "4", "5" }.Contains(netType))
                validationErrors.Add("Net type must be 1, 2, 3, 4, or 5.");

            if (validationErrors.Count > 0)
            {
                return Ok(new
                {
                    data = (object)null,
                    errorMessage = string.Join("; ", validationErrors)
                });
            }

            var request = new RetailDetailedRequest
            {
                BillCycle = billCycle,
                CalcCycle = calcCycle,
                CycleType = cycleType,
                NetType = netType
            };

            switch (reportType.ToLower())
            {
                case "area":
                    request.ReportType = SolarReportType.Area;
                    request.AreaCode = typeCode;
                    break;
                case "province":
                    request.ReportType = SolarReportType.Province;
                    request.ProvCode = typeCode;
                    break;
                case "region":
                    request.ReportType = SolarReportType.Region;
                    request.Region = typeCode;
                    break;
                case "entireceb":
                    request.ReportType = SolarReportType.EntireCEB;
                    break;
                default:
                    return Ok(new
                    {
                        data = (object)null,
                        errorMessage = "Invalid report type.",
                        errorDetails = "Valid types: Area, Province, Region, EntireCEB."
                    });
            }

            return ProcessRetailDetailedRequest(request);
        }

        private IHttpActionResult ProcessRetailDetailedRequest(RetailDetailedRequest request)
        {
            try
            {
                if (!_retailDetailedDao.TestConnection(out string connError))
                {
                    return Ok(new
                    {
                        data = (object)null,
                        errorMessage = "Database connection failed.",
                        errorDetails = connError
                    });
                }

                string typeValidationError = ValidateRetailDetailedParameters(request);
                if (!string.IsNullOrEmpty(typeValidationError))
                {
                    return Ok(new
                    {
                        data = (object)null,
                        errorMessage = typeValidationError,
                        errorDetails = "Invalid request parameters."
                    });
                }

                var data = _retailDetailedDao.GetRetailDetailedReport(request);

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
                    errorMessage = "Cannot get retail detailed report data.",
                    errorDetails = ex.Message
                });
            }
        }

        private string ValidateRetailDetailedParameters(RetailDetailedRequest request)
        {
            return ValidateReportTypeParameters(
                request.ReportType,
                request.AreaCode,
                request.ProvCode,
                request.Region
            );
        }

        [HttpGet]
        [Route("retail/summary")]
        public IHttpActionResult GetRetailSummaryReport(
            [FromUri] string billCycle = null,
            [FromUri] string calcCycle = null,
            [FromUri] string cycleType = "A") // A for bill_cycle, C for calc_cycle
        {
            var validationErrors = new List<string>();

            // Validate cycle parameters
            if (cycleType == "A" && string.IsNullOrWhiteSpace(billCycle))
                validationErrors.Add("Bill cycle is required when cycle type is 'A'.");

            if (cycleType == "C" && string.IsNullOrWhiteSpace(calcCycle))
                validationErrors.Add("Calc cycle is required when cycle type is 'C'.");

            if (cycleType != "A" && cycleType != "C")
                validationErrors.Add("Cycle type must be 'A' (bill cycle) or 'C' (calc cycle).");

            if (validationErrors.Count > 0)
            {
                return Ok(new
                {
                    data = (object)null,
                    errorMessage = string.Join("; ", validationErrors)
                });
            }

            var request = new RetailSummaryRequest
            {
                BillCycle = billCycle,
                CalcCycle = calcCycle,
                CycleType = cycleType
            };

            return ProcessRetailSummaryRequest(request);
        }

        private IHttpActionResult ProcessRetailSummaryRequest(RetailSummaryRequest request)
        {
            try
            {
                if (!_ordSummaryDao.TestConnection(out string connError))
                {
                    return Ok(new
                    {
                        data = (object)null,
                        errorMessage = "Database connection failed.",
                        errorDetails = connError
                    });
                }

                var data = _ordSummaryDao.GetRetailSummaryReport(request);

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
                    errorMessage = "Cannot get retail summary report data.",
                    errorDetails = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("retail/summary-bulk")]
        public IHttpActionResult GetRetailBulkSummaryReport([FromUri] string billCycle)
        {
            var validationErrors = new List<string>();

            // Validate bill cycle parameter
            if (string.IsNullOrWhiteSpace(billCycle))
                validationErrors.Add("Bill cycle is required.");

            if (validationErrors.Count > 0)
            {
                return Ok(new
                {
                    data = (object)null,
                    errorMessage = string.Join("; ", validationErrors)
                });
            }

            var request = new RetailSummaryRequest
            {
                BillCycle = billCycle,
                CycleType = "A" // Bulk only uses bill cycle
            };

            return ProcessRetailBulkSummaryRequest(request);
        }

        private IHttpActionResult ProcessRetailBulkSummaryRequest(RetailSummaryRequest request)
        {
            try
            {
                if (!_bulkSummaryDao.TestConnection(out string connError))
                {
                    return Ok(new
                    {
                        data = (object)null,
                        errorMessage = "Database connection failed.",
                        errorDetails = connError
                    });
                }

                var data = _bulkSummaryDao.GetRetailBulkSummaryReport(request);

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
                    errorMessage = "Cannot get retail bulk summary report data.",
                    errorDetails = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("solarPVCapacity/billcycle/max")]
        public IHttpActionResult GetPVCapacityMaxBillCycle()
        {
            try
            {
                var result = _pVCapacityBillCycleDao.GetLast24BillCycles();//From netprogrs table in InformixConnection database

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
                    errorMessage = "Cannot get max bill cycle",
                    errorDetails = ex.Message
                }));
            }
        }

        [HttpGet]
        [Route("solarPVCapacity/ordinary")]
        public IHttpActionResult GetPVCapacityOrdinaryReport(
            [FromUri] string billCycle,
            [FromUri] string reportType,
            [FromUri] string typeCode = null)
        {
            var validationErrors = new List<string>();

            if (string.IsNullOrWhiteSpace(billCycle))
                validationErrors.Add("Bill cycle is required.");

            if (string.IsNullOrWhiteSpace(reportType))
                validationErrors.Add("Report type is required.");

            if (validationErrors.Count > 0)
            {
                return Ok(new
                {
                    data = (object)null,
                    errorMessage = string.Join("; ", validationErrors)
                });
            }

            var request = new SolarProgressRequest
            {
                BillCycle = billCycle
            };

            switch (reportType.ToLower())
            {
                case "area":
                    request.ReportType = SolarReportType.Area;
                    request.AreaCode = typeCode;
                    break;
                case "province":
                    request.ReportType = SolarReportType.Province;
                    request.ProvCode = typeCode;
                    break;
                case "region":
                    request.ReportType = SolarReportType.Region;
                    request.Region = typeCode;
                    break;
                case "entireceb":
                    request.ReportType = SolarReportType.EntireCEB;
                    break;
                default:
                    return Ok(new
                    {
                        data = (object)null,
                        errorMessage = "Invalid report type.",
                        errorDetails = "Valid types: Area, Province, Region, EntireCEB."
                    });
            }

            return ProcessPVCapacityOrdinaryRequest(request);
        }

        private IHttpActionResult ProcessPVCapacityOrdinaryRequest(SolarProgressRequest request)
        {
            try
            {
                if (!_pvCapacityOrdinaryDao.TestConnection(out string connError))
                {
                    return Ok(new
                    {
                        data = (object)null,
                        errorMessage = "Database connection failed.",
                        errorDetails = connError
                    });
                }

                string typeValidationError = ValidateRequestParameters(request);
                if (!string.IsNullOrEmpty(typeValidationError))
                {
                    return Ok(new
                    {
                        data = (object)null,
                        errorMessage = typeValidationError,
                        errorDetails = "Invalid request parameters."
                    });
                }

                var data = _pvCapacityOrdinaryDao.GetPVCapacityReport(request);

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
                    errorMessage = "Cannot get PV capacity data.",
                    errorDetails = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("solarPVCapacity/bulk")]
        public IHttpActionResult GetPVCapacityBulkReport(
            [FromUri] string billCycle,
            [FromUri] string reportType,
            [FromUri] string typeCode = null)
        {
            var validationErrors = new List<string>();

            if (string.IsNullOrWhiteSpace(billCycle))
                validationErrors.Add("Bill cycle is required.");

            if (string.IsNullOrWhiteSpace(reportType))
                validationErrors.Add("Report type is required.");

            if (validationErrors.Count > 0)
            {
                return Ok(new
                {
                    data = (object)null,
                    errorMessage = string.Join("; ", validationErrors)
                });
            }

            var request = new SolarProgressRequest
            {
                BillCycle = billCycle
            };

            switch (reportType.ToLower())
            {
                case "area":
                    request.ReportType = SolarReportType.Area;
                    request.AreaCode = typeCode;
                    break;
                case "province":
                    request.ReportType = SolarReportType.Province;
                    request.ProvCode = typeCode;
                    break;
                case "region":
                    request.ReportType = SolarReportType.Region;
                    request.Region = typeCode;
                    break;
                case "entireceb":
                    request.ReportType = SolarReportType.EntireCEB;
                    break;
                default:
                    return Ok(new
                    {
                        data = (object)null,
                        errorMessage = "Invalid report type.",
                        errorDetails = "Valid types: Area, Province, Region, EntireCEB."
                    });
            }

            return ProcessPVCapacityBulkRequest(request);
        }

        private IHttpActionResult ProcessPVCapacityBulkRequest(SolarProgressRequest request)
        {
            try
            {
                if (!_pvCapacityBulkDao.TestConnection(out string connError))
                {
                    return Ok(new
                    {
                        data = (object)null,
                        errorMessage = "Database connection failed.",
                        errorDetails = connError
                    });
                }

                string typeValidationError = ValidateRequestParameters(request);
                if (!string.IsNullOrEmpty(typeValidationError))
                {
                    return Ok(new
                    {
                        data = (object)null,
                        errorMessage = typeValidationError,
                        errorDetails = "Invalid request parameters."
                    });
                }

                var data = _pvCapacityBulkDao.GetPVCapacityBulkReport(request);

                // Check if data is empty due to null generation capacity
                if (data == null || data.Count == 0)
                {
                    return Ok(new
                    {
                        data = (object)null,
                        errorMessage = "No data available. This may be due to null generation capacity records for the specified bill cycle.",
                        errorDetails = "Please check the data for the bill cycle or contact the administrator."
                    });
                }

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
                    errorMessage = "Cannot get PV capacity bulk data.",
                    errorDetails = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("solarPVCapacity/summary")]
        public IHttpActionResult GetPVCapacitySummaryReport(
            [FromUri] string billCycle,
            [FromUri] string reportType,
            [FromUri] string typeCode = null)
        {
            var validationErrors = new List<string>();

            if (string.IsNullOrWhiteSpace(billCycle))
                validationErrors.Add("Bill cycle is required.");

            if (string.IsNullOrWhiteSpace(reportType))
                validationErrors.Add("Report type is required.");

            if (validationErrors.Count > 0)
            {
                return Ok(new
                {
                    data = (object)null,
                    errorMessage = string.Join("; ", validationErrors)
                });
            }

            var request = new SolarProgressRequest
            {
                BillCycle = billCycle
            };

            switch (reportType.ToLower())
            {
                case "area":
                    request.ReportType = SolarReportType.Area;
                    request.AreaCode = typeCode;
                    break;
                case "province":
                    request.ReportType = SolarReportType.Province;
                    request.ProvCode = typeCode;
                    break;
                case "region":
                    request.ReportType = SolarReportType.Region;
                    request.Region = typeCode;
                    break;
                case "entireceb":
                    request.ReportType = SolarReportType.EntireCEB;
                    break;
                default:
                    return Ok(new
                    {
                        data = (object)null,
                        errorMessage = "Invalid report type.",
                        errorDetails = "Valid types: Area, Province, Region, EntireCEB."
                    });
            }

            return ProcessPVCapacitySummaryRequest(request);
        }

        private IHttpActionResult ProcessPVCapacitySummaryRequest(SolarProgressRequest request)
        {
            try
            {
                if (!_pvCapacitySummaryDao.TestConnection(out string connError))
                {
                    return Ok(new
                    {
                        data = (object)null,
                        errorMessage = "Database connection failed.",
                        errorDetails = connError
                    });
                }

                string typeValidationError = ValidateRequestParameters(request);
                if (!string.IsNullOrEmpty(typeValidationError))
                {
                    return Ok(new
                    {
                        data = (object)null,
                        errorMessage = typeValidationError,
                        errorDetails = "Invalid request parameters."
                    });
                }

                var data = _pvCapacitySummaryDao.GetPVCapacitySummaryReport(request);

                // Check if data is empty due to null generation capacity
                if (data == null || data.Count == 0)
                {
                    return Ok(new
                    {
                        data = (object)null,
                        errorMessage = "No data available. This may be due to null generation capacity records or no data for the specified criteria.",
                        errorDetails = "Please check the data for the bill cycle or contact the administrator."
                    });
                }

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
                    errorMessage = "Cannot get PV capacity summary data.",
                    errorDetails = ex.Message
                });
            }
        }
        [HttpGet]
        [Route("solarPayment/bulk")]
        public IHttpActionResult GetSolarPaymentBulkReport(
            [FromUri] string billCycle,
            [FromUri] string netType = "1",   // Net type filter (1, 2, 3, 4, 5)
            [FromUri] string reportType = "entireceb",
            [FromUri] string typeCode = null)
        {
            var validationErrors = new List<string>();

            // Validate bill cycle parameter
            if (string.IsNullOrWhiteSpace(billCycle))
                validationErrors.Add("Bill cycle is required.");

            // Validate net type
            if (string.IsNullOrWhiteSpace(netType))
                validationErrors.Add("Net type is required.");
            else if (!new[] { "1", "2", "3", "4", "5" }.Contains(netType))
                validationErrors.Add("Net type must be 1, 2, 3, 4, or 5.");

            if (validationErrors.Count > 0)
            {
                return Ok(new
                {
                    data = (object)null,
                    errorMessage = string.Join("; ", validationErrors)
                });
            }

            var request = new SolarPaymentBulkRequest
            {
                BillCycle = billCycle,
                NetType = netType
            };

            switch (reportType.ToLower())
            {
                case "area":
                    request.ReportType = SolarReportType.Area;
                    request.AreaCode = typeCode;
                    break;
                case "province":
                    request.ReportType = SolarReportType.Province;
                    request.ProvCode = typeCode;
                    break;
                case "region":
                    request.ReportType = SolarReportType.Region;
                    request.Region = typeCode;
                    break;
                case "entireceb":
                    request.ReportType = SolarReportType.EntireCEB;
                    break;
                default:
                    return Ok(new
                    {
                        data = (object)null,
                        errorMessage = "Invalid report type.",
                        errorDetails = "Valid types: Area, Province, Region, EntireCEB."
                    });
            }

            return ProcessSolarPaymentBulkRequest(request);
        }

        private IHttpActionResult ProcessSolarPaymentBulkRequest(SolarPaymentBulkRequest request)
        {
            try
            {
                if (!_solarPaymentBulkDao.TestConnection(out string connError))
                {
                    return Ok(new
                    {
                        data = (object)null,
                        errorMessage = "Database connection failed.",
                        errorDetails = connError
                    });
                }

                string typeValidationError = ValidateSolarPaymentBulkParameters(request);
                if (!string.IsNullOrEmpty(typeValidationError))
                {
                    return Ok(new
                    {
                        data = (object)null,
                        errorMessage = typeValidationError,
                        errorDetails = "Invalid request parameters."
                    });
                }

                var data = _solarPaymentBulkDao.GetSolarPaymentBulkReport(request);

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
                    errorMessage = "Cannot get solar payment bulk report data.",
                    errorDetails = ex.Message
                });
            }
        }

        private string ValidateSolarPaymentBulkParameters(SolarPaymentBulkRequest request)
        {
            return ValidateReportTypeParameters(
                request.ReportType,
                request.AreaCode,
                request.ProvCode,
                request.Region
            );
        }


        [HttpGet]
        [Route("solarConnectionDetails/retail/detailed")]
        public IHttpActionResult GetSolarConnectionDetailsRetailDetailedReport(
            [FromUri] string cycleType = "A",     // A = BillCycle, B = CalcCycle
            [FromUri] string billCycle = null,
            [FromUri] string calcCycle = null,
            [FromUri] string netType = "1",
            [FromUri] string reportType = "entireceb",
            [FromUri] string typeCode = null)
        {
            var validationErrors = new List<string>();

            // Validate cycle parameters
            if (cycleType == "A" && string.IsNullOrWhiteSpace(billCycle))
                validationErrors.Add("Bill cycle is required when cycle type is 'A'.");
            if (cycleType == "B" && string.IsNullOrWhiteSpace(calcCycle))
                validationErrors.Add("Calc cycle is required when cycle type is 'B'.");

            // Validate net type
            if (string.IsNullOrWhiteSpace(netType))
                validationErrors.Add("Net type is required.");
            else if (!new[] { "1", "2", "3", "4", "5" }.Contains(netType))
                validationErrors.Add("Net type must be 1, 2, 3, 4, or 5.");

            if (validationErrors.Count > 0)
            {
                return Ok(new
                {
                    data = (object)null,
                    errorMessage = string.Join("; ", validationErrors)
                });
            }

            var request = new RetailDetailedRequest
            {
                CycleType = cycleType,
                BillCycle = billCycle,
                CalcCycle = calcCycle,
                NetType = netType
            };

            switch (reportType.ToLower())
            {
                case "area":
                    request.ReportType = SolarReportType.Area;
                    request.AreaCode = typeCode;
                    break;
                case "province":
                    request.ReportType = SolarReportType.Province;
                    request.ProvCode = typeCode;
                    break;
                case "region":
                    request.ReportType = SolarReportType.Region;
                    request.Region = typeCode;
                    break;
                case "entireceb":
                    request.ReportType = SolarReportType.EntireCEB;
                    break;
                default:
                    return Ok(new
                    {
                        data = (object)null,
                        errorMessage = "Invalid report type.",
                        errorDetails = "Valid types: Area, Province, Region, EntireCEB."
                    });
            }

            return ProcessSolarReadingDetailedRequest(request);
        }

        private IHttpActionResult ProcessSolarReadingDetailedRequest(RetailDetailedRequest request)
        {
            try
            {
                if (!_solarReadingDetailedDao.TestConnection(out string connError))
                {
                    return Ok(new
                    {
                        data = (object)null,
                        errorMessage = "Database connection failed.",
                        errorDetails = connError
                    });
                }

                var data = _solarReadingDetailedDao.GetSolarReadingDetailedReport(request);

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
                    errorMessage = "Cannot get solar reading detailed report data.",
                    errorDetails = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("solarConnectionDetails/retail/summary")]
        public IHttpActionResult GetSolarConnectionDetailsRetailSummaryReport(
            [FromUri] string cycleType = "A",     // A = BillCycle, B = CalcCycle
            [FromUri] string billCycle = null,
            [FromUri] string calcCycle = null,
            [FromUri] string netType = "1",
            [FromUri] string reportType = "entireceb",
            [FromUri] string typeCode = null)
        {
            var validationErrors = new List<string>();

            // Validate cycle parameters
            if (cycleType == "A" && string.IsNullOrWhiteSpace(billCycle))
                validationErrors.Add("Bill cycle is required when cycle type is 'A'.");
            if (cycleType == "B" && string.IsNullOrWhiteSpace(calcCycle))
                validationErrors.Add("Calc cycle is required when cycle type is 'B'.");

            // Validate net type
            if (string.IsNullOrWhiteSpace(netType))
                validationErrors.Add("Net type is required.");
            else if (!new[] { "1", "2", "3", "4", "5" }.Contains(netType))
                validationErrors.Add("Net type must be 1, 2, 3, 4, or 5.");

            if (validationErrors.Count > 0)
            {
                return Ok(new
                {
                    data = (object)null,
                    errorMessage = string.Join("; ", validationErrors)
                });
            }

            var request = new RetailDetailedRequest
            {
                CycleType = cycleType,
                BillCycle = billCycle,
                CalcCycle = calcCycle,
                NetType = netType
            };

            switch (reportType.ToLower())
            {
                case "area":
                    request.ReportType = SolarReportType.Area;
                    request.AreaCode = typeCode;
                    break;
                case "province":
                    request.ReportType = SolarReportType.Province;
                    request.ProvCode = typeCode;
                    break;
                case "region":
                    request.ReportType = SolarReportType.Region;
                    request.Region = typeCode;
                    break;
                case "entireceb":
                    request.ReportType = SolarReportType.EntireCEB;
                    break;
                default:
                    return Ok(new
                    {
                        data = (object)null,
                        errorMessage = "Invalid report type.",
                        errorDetails = "Valid types: Area, Province, Region, EntireCEB."
                    });
            }

            return ProcessSolarReadingSummaryRequest(request);
        }

        private IHttpActionResult ProcessSolarReadingSummaryRequest(RetailDetailedRequest request)
        {
            try
            {
                if (!_solarReadingSummaryDao.TestConnection(out string connError))
                {
                    return Ok(new
                    {
                        data = (object)null,
                        errorMessage = "Database connection failed.",
                        errorDetails = connError
                    });
                }

                var data = _solarReadingSummaryDao.GetSolarReadingSummaryReport(request);

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
                    errorMessage = "Cannot get solar reading summary report data.",
                    errorDetails = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("solarConnectionDetails/bulk")]
        public IHttpActionResult GetSolarConnectionDetailsBulkReport(
            [FromUri] string addedBillCycle = null,
            [FromUri] string billCycle = null,
            [FromUri] string netType = "1",
            [FromUri] string reportType = "entireceb",
            [FromUri] string typeCode = null)
        {
            var validationErrors = new List<string>();

            // Validate required parameters
            if (string.IsNullOrWhiteSpace(addedBillCycle))
                validationErrors.Add("Added bill cycle (added_blcy) is required.");

            if (string.IsNullOrWhiteSpace(billCycle))
                validationErrors.Add("Bill cycle is required.");

            // Validate net type
            if (string.IsNullOrWhiteSpace(netType))
                validationErrors.Add("Net type is required.");
            else if (!new[] { "1", "2", "3", "4", "5" }.Contains(netType))
                validationErrors.Add("Net type must be 1, 2, 3, 4, or 5.");

            if (validationErrors.Count > 0)
            {
                return Ok(new
                {
                    data = (object)null,
                    errorMessage = string.Join("; ", validationErrors)
                });
            }

            var request = new BulkUsageRequest
            {
                AddedBillCycle = addedBillCycle,
                BillCycle = billCycle,
                NetType = netType
            };

            switch (reportType.ToLower())
            {
                case "area":
                    request.ReportType = SolarReportType.Area;
                    request.AreaCode = typeCode;
                    if (string.IsNullOrWhiteSpace(typeCode))
                    {
                        return Ok(new
                        {
                            data = (object)null,
                            errorMessage = "Area code is required for area report type."
                        });
                    }
                    break;
                case "province":
                    request.ReportType = SolarReportType.Province;
                    request.ProvCode = typeCode;
                    if (string.IsNullOrWhiteSpace(typeCode))
                    {
                        return Ok(new
                        {
                            data = (object)null,
                            errorMessage = "Province code is required for province report type."
                        });
                    }
                    break;
                case "region":
                    request.ReportType = SolarReportType.Region;
                    request.Region = typeCode;
                    if (string.IsNullOrWhiteSpace(typeCode))
                    {
                        return Ok(new
                        {
                            data = (object)null,
                            errorMessage = "Region is required for region report type."
                        });
                    }
                    break;
                case "entireceb":
                    request.ReportType = SolarReportType.EntireCEB;
                    break;
                default:
                    return Ok(new
                    {
                        data = (object)null,
                        errorMessage = "Invalid report type.",
                        errorDetails = "Valid types: Area, Province, Region, EntireCEB."
                    });
            }

            return ProcessSolarReadingUsageBulkRequest(request);
        }

        // 3. Add this helper method:

        private IHttpActionResult ProcessSolarReadingUsageBulkRequest(BulkUsageRequest request)
        {
            try
            {
                if (!_solarReadingUsageBulkDao.TestConnection(out string connError))
                {
                    return Ok(new
                    {
                        data = (object)null,
                        errorMessage = "Database connection failed.",
                        errorDetails = connError
                    });
                }

                var data = _solarReadingUsageBulkDao.GetSolarReadingUsageBulkReport(request);

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
                    errorMessage = "Cannot get solar reading usage bulk report data.",
                    errorDetails = ex.Message
                });
            }
        }

    }
}