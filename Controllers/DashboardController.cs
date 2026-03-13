using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MISReports_Api.DAL.Dashboard;
using MISReports_Api.Models.Dashboard;
using System.Web.Http;

namespace MISReports_Api.Controllers
{
    [RoutePrefix("api/dashboard")]
    public class DashboardController : ApiController
    {
        private readonly BulkCustomersDao _bulkCustomersDao = new BulkCustomersDao();

        /// <summary>
        /// Get active customer count (cst_st='0')
        /// </summary>
        [HttpGet]
        [Route("customers/active-count")]
        public IHttpActionResult GetActiveCustomerCount()
        {
            try
            {
                if (!_bulkCustomersDao.TestConnection(out string connError))
                {
                    return Ok(new
                    {
                        data = (object)null,
                        errorMessage = "Database connection failed.",
                        errorDetails = connError
                    });
                }

                int count = _bulkCustomersDao.GetActiveCustomerCount();

                return Ok(new
                {
                    data = new { activeCustomerCount = count },
                    errorMessage = (string)null
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    data = (object)null,
                    errorMessage = "Cannot get active customer count.",
                    errorDetails = ex.Message
                });
            }
        }
    }

}

        



        
