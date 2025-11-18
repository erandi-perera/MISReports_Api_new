// Controllers/CashBookCCReportController.cs
using MISReports_Api.DAL;
using MISReports_Api.Models;
using System;
using System.Linq;
using System.Web.Http;

namespace MISReports_Api.Controllers
{
    [RoutePrefix("api/cashbook")]
    public class CashBookCCReportController : ApiController
    {
        private readonly CashBookCCReportRepository _repo = new CashBookCCReportRepository();

        // PATH: /api/cashbook/ccreport/20220101/20251231/DEPT001/LTL
        [HttpGet]
        [Route("ccreport/{fromDate:regex(^\\d{8}$)}/{toDate:regex(^\\d{8}$)}/{costCenter}/{payee?}")]
        public IHttpActionResult GetReport(string fromDate, string toDate, string costCenter, string payee = null)
        {
            return ExecuteQuery(fromDate, toDate, costCenter, payee);
        }

        // QUERY: ?fromDate=20220101&toDate=20251231&costCenter=DEPT001&payee=LTL
        [HttpGet]
        [Route("ccreport")]
        public IHttpActionResult GetQuery([FromUri] string fromDate, [FromUri] string toDate, [FromUri] string costCenter, [FromUri] string payee = null)
        {
            return ExecuteQuery(fromDate, toDate, costCenter, payee);
        }

        private IHttpActionResult ExecuteQuery(string fromDate, string toDate, string costCenter, string payee)
        {
            try
            {
                // Validate dates
                if (!IsValidDate(fromDate) || !IsValidDate(toDate))
                    return BadRequest("Dates must be in YYYYMMDD format (e.g., 20220101).");

                if (string.IsNullOrWhiteSpace(costCenter))
                    return BadRequest("costCenter is required.");

                payee = (payee ?? "").Trim();
                if (payee.Length > 0 && payee.Length < 3)
                    return BadRequest("Payee must be at least 3 characters.");

                var data = _repo.GetCashBookCCReport(fromDate, toDate, costCenter.Trim(), payee);
                var totalAmt = data.Sum(x => x.ChqAmt ?? 0m);
                const int MAX_RECORDS = 5000;

                var summary = new
                {
                    fromDate,
                    toDate,
                    costCenter = costCenter.Trim(),
                    payeeFilter = payee,
                    totalRecords = data.Count,
                    totalAmount = totalAmt
                };

                if (data.Count >= MAX_RECORDS)
                {
                    return Ok(new
                    {
                        success = false,
                        message = $"Too many records ({data.Count}). Result capped at {MAX_RECORDS}. Use narrower filters.",
                        data = new object[0],
                        summary = new
                        {
                            summary.fromDate,
                            summary.toDate,
                            summary.costCenter,
                            summary.payeeFilter,
                            summary.totalRecords,
                            summary.totalAmount,
                            warning = "RESULT_CAPPED"
                        }
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = data.Any() ? "Data retrieved successfully" : "No records found",
                    data,
                    summary
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}\n{ex.StackTrace}");
                return InternalServerError(new Exception($"Database error: {ex.Message}", ex));
            }
        }

        private bool IsValidDate(string s)
        {
            if (s == null || s.Length != 8) return false;
            if (!int.TryParse(s.Substring(0, 4), out int y) || y < 1900 || y > 2100) return false;
            if (!int.TryParse(s.Substring(4, 2), out int m) || m < 1 || m > 12) return false;
            if (!int.TryParse(s.Substring(6, 2), out int d) || d < 1 || d > 31) return false;
            return true;
        }
    }
}