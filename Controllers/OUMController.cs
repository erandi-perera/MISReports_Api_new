// Controllers/OUMController.cs
using MISReports_Api.DAL;
using MISReports_Api.Models;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace MISReports_Api.Controllers
{
    [RoutePrefix("api/oum")]
    public class OUMController : ApiController
    {
        private readonly OUMRepository _oumRepository = new OUMRepository();

        [HttpPost]
        [Route("upload")]
        public async Task<IHttpActionResult> UploadExcelFile()
        {
            try
            {
                // Check if request contains multipart/form-data
                if (!Request.Content.IsMimeMultipartContent())
                {
                    return Ok(JObject.FromObject(new OUMUploadResponseModel
                    {
                        RecordsInserted = 0,
                        Message = null,
                        ErrorMessage = "Invalid request format. Please use multipart/form-data.",
                        Data = null
                    }));
                }

                var provider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);

                var fileContent = provider.Contents.FirstOrDefault(x => x.Headers.ContentDisposition.Name.Trim('"') == "file");

                if (fileContent == null)
                {
                    return Ok(JObject.FromObject(new OUMUploadResponseModel
                    {
                        RecordsInserted = 0,
                        Message = null,
                        ErrorMessage = "No file found in the request.",
                        Data = null
                    }));
                }

                var fileBytes = await fileContent.ReadAsByteArrayAsync();
                var fileName = fileContent.Headers.ContentDisposition.FileName?.Trim('"');

                if (fileBytes.Length == 0)
                {
                    return Ok(JObject.FromObject(new OUMUploadResponseModel
                    {
                        RecordsInserted = 0,
                        Message = null,
                        ErrorMessage = "File is empty.",
                        Data = null
                    }));
                }

                // Process Excel file
                var employeeData = new List<OUMEmployeeModel>();

                using (var stream = new MemoryStream(fileBytes))
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];

                    for (int row = 1; row <= worksheet.Dimension.End.Row; row++)
                    {
                        try
                        {
                            var employee = new OUMEmployeeModel
                            {
                                AuthDate = DateTime.Parse(worksheet.Cells[row, 1].Text),
                                OrderId = int.Parse(worksheet.Cells[row, 2].Text),
                                AcctNumber = worksheet.Cells[row, 3].Text,
                                BankCode = worksheet.Cells[row, 4].Text,
                                BillAmt = decimal.Parse(worksheet.Cells[row, 5].Text),
                                TaxAmt = decimal.Parse(worksheet.Cells[row, 6].Text),
                                TotAmt = decimal.Parse(worksheet.Cells[row, 7].Text),
                                AuthCode = worksheet.Cells[row, 8].Text,
                                CardNo = worksheet.Cells[row, 9].Text
                            };
                            employeeData.Add(employee);
                        }
                        catch (Exception ex)
                        {
                            // Log row-specific error but continue processing
                            Console.WriteLine($"Error processing row {row}: {ex.Message}");
                        }
                    }
                }

                if (employeeData.Count == 0)
                {
                    return Ok(JObject.FromObject(new OUMUploadResponseModel
                    {
                        RecordsInserted = 0,
                        Message = null,
                        ErrorMessage = "No valid data found in the Excel file.",
                        Data = null
                    }));
                }

                // Insert into database
                var insertedRows = _oumRepository.InsertIntoAmex2(employeeData);

                // Refresh CrdTemp table
                _oumRepository.RefreshCrdTemp();

                // Get updated records
                var records = _oumRepository.GetCrdTempRecords();

                var response = new OUMUploadResponseModel
                {
                    RecordsInserted = insertedRows,
                    Message = insertedRows == 1 ? $"Successfully inserted {insertedRows} record" : $"Successfully inserted {insertedRows} records",
                    ErrorMessage = null,
                    Data = records
                };

                return Ok(JObject.FromObject(response));
            }
            catch (Exception ex)
            {
                return Ok(JObject.FromObject(new OUMUploadResponseModel
                {
                    RecordsInserted = 0,
                    Message = null,
                    ErrorMessage = "Error processing file: " + ex.Message,
                    Data = null
                }));
            }
        }

        [HttpGet]
        [Route("records")]
        public IHttpActionResult GetRecords()
        {
            try
            {
                var records = _oumRepository.GetCrdTempRecords();

                var response = new OUMRecordsResponseModel
                {
                    Records = records,
                    TotalRecords = records.Count,
                    ErrorMessage = null
                };

                return Ok(JObject.FromObject(new
                {
                    data = response,
                    errorMessage = (string)null
                }));
            }
            catch (Exception ex)
            {
                return Ok(JObject.FromObject(new
                {
                    data = (object)null,
                    errorMessage = "Cannot get OUM records.",
                    errorDetails = ex.Message
                }));
            }
        }

        [HttpPost]
        [Route("approve")]
        public IHttpActionResult ApproveRecords()
        {
            try
            {
                var success = _oumRepository.ApproveRecords();

                var response = new OUMApproveResponseModel
                {
                    Success = success,
                    Message = success ? "Records successfully approved and moved to production." : "Failed to approve records.",
                    ErrorMessage = null,
                    RecordsProcessed = success ? _oumRepository.GetCrdTempRecords().Count : 0
                };

                return Ok(JObject.FromObject(new
                {
                    data = response,
                    errorMessage = (string)null
                }));
            }
            catch (Exception ex)
            {
                return Ok(JObject.FromObject(new
                {
                    data = new OUMApproveResponseModel
                    {
                        Success = false,
                        Message = "Transaction failed",
                        ErrorMessage = ex.Message,
                        RecordsProcessed = 0
                    },
                    errorMessage = "Cannot approve OUM records.",
                    errorDetails = ex.Message
                }));
            }
        }

        [HttpPost]
        [Route("refresh")]
        public IHttpActionResult RefreshCrdTemp()
        {
            try
            {
                _oumRepository.RefreshCrdTemp();
                var records = _oumRepository.GetCrdTempRecords();

                return Ok(JObject.FromObject(new
                {
                    data = new
                    {
                        message = "CrdTemp table refreshed successfully",
                        recordCount = records.Count,
                        records = records
                    },
                    errorMessage = (string)null
                }));
            }
            catch (Exception ex)
            {
                return Ok(JObject.FromObject(new
                {
                    data = (object)null,
                    errorMessage = "Cannot refresh CrdTemp table.",
                    errorDetails = ex.Message
                }));
            }
        }
    }
}