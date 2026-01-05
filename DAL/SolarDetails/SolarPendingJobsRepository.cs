//Branch wise Solar Pending Jobs after paid PIV2

using MISReports_Api.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace MISReports_Api.DAL
{
    public class SolarPendingJobsRepository
    {
        private readonly string _connectionString =
            ConfigurationManager.ConnectionStrings["HQOracle"].ConnectionString;

        public List<SolarPendingJobsModel> GetSolarPendingJobsReport(string compId, DateTime fromDate, DateTime toDate)
        {
            var result = new List<SolarPendingJobsModel>();

            string sql = @"
SELECT DISTINCT
    a.dept_id,
    a.application_id,
    a.application_no,
    a.submit_date,
    e.projectno,
    c.piv_date,
    CASE
        WHEN a.application_sub_type IN ('NA') THEN 'Net Accounting'
        WHEN a.application_sub_type IN ('NM') THEN 'Net Metering'
        WHEN a.application_sub_type IN ('NP') THEN 'Net Plus'
        WHEN a.application_sub_type IN ('BA') THEN 'Bulk Net Accounting'
        WHEN a.application_sub_type IN ('BM') THEN 'Bulk Net Metering'
        WHEN a.application_sub_type IN ('BP') THEN 'Bulk Net Plus'
        WHEN a.application_sub_type IN ('AC') THEN 'Net Accounting Conversion'
        WHEN a.application_sub_type IN ('PC') THEN 'Net Plus Conversion'
        WHEN a.application_sub_type IN ('NT') THEN 'Net Metering TOU'
        WHEN a.application_sub_type IN ('AT') THEN 'Net Accounting TOU'
        WHEN a.application_sub_type IN ('PP') THEN 'Net Plus Plus (With Account No.)'
        WHEN a.application_sub_type IN ('PB') THEN 'Bulk Net Plus Plus'
        WHEN a.application_sub_type IN ('PN') THEN 'Net Plus Plus (Without Account No.)'
        ELSE NULL
    END AS application_sub_type_desc,
    c.paid_date,
    (SELECT c2.paid_date
     FROM piv_detail c2
     WHERE c2.reference_type='EST'
       AND TRIM(c2.reference_no)=TRIM(a.application_no)
       AND c2.status IN ('C', 'P','T','M','Y')) AS piv2_paid_date,
    (SELECT existing_acc_no
     FROM WIRING_LAND_DETAIL
     WHERE application_id = a.application_id) AS existing_acc_no,
    CASE
        WHEN c1.status = 33 THEN 'Job No to be created'
        WHEN c1.status = 22 THEN 'Contractor to be Allocated'
        ELSE 'Not Energized'
    END AS status_desc,
    (SELECT comp_nm FROM glcompm WHERE comp_id = :compID) AS COMP_NM
FROM applications a
    INNER JOIN piv_detail c ON TRIM(a.application_no) = TRIM(c.reference_no)
        AND c.reference_type = 'APP'
        AND a.dept_id = c.dept_id
    INNER JOIN application_reference e ON a.application_id = e.application_id
        AND a.dept_id = e.dept_id
    INNER JOIN pcesthtt c1 ON TRIM(e.application_no) = TRIM(c1.estimate_no)
WHERE a.application_type = 'CR'
  AND c1.status IN (33, 22)
  AND a.application_sub_type IN ('NM','NP','NA','BM','BP','BA','NT','AC','PC','PP','PN','PB','AT')
  AND c.status IN ('C', 'P','T','M','Y')
  AND a.dept_id IN (SELECT dept_id
                    FROM gldeptm
                    WHERE comp_id IN (SELECT comp_id
                                      FROM glcompm
                                      WHERE comp_id = :compID OR parent_id = :compID))
  AND a.submit_date >= TO_DATE(:fromDate, 'yyyy/mm/dd')
  AND a.submit_date <= TO_DATE(:toDate, 'yyyy/mm/dd')
  AND TRIM(c1.project_No) NOT IN (SELECT TRIM(PROJECT_NO) FROM spodrcrd)
ORDER BY a.dept_id, e.projectno, a.application_no";

            using (OracleConnection conn = new OracleConnection(_connectionString))
            using (OracleCommand cmd = new OracleCommand(sql, conn))
            {
                cmd.BindByName = true;
                cmd.Parameters.Add("compID", OracleDbType.Varchar2).Value = compId;
                cmd.Parameters.Add("fromDate", OracleDbType.Varchar2).Value = fromDate.ToString("yyyy/MM/dd");
                cmd.Parameters.Add("toDate", OracleDbType.Varchar2).Value = toDate.ToString("yyyy/MM/dd");

                conn.Open();
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var model = new SolarPendingJobsModel
                        {
                            Dept_Id = reader["dept_id"].ToString(),
                            Application_Id = reader["application_id"].ToString(),
                            Application_No = reader["application_no"].ToString(),
                            Submit_Date = reader["submit_date"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["submit_date"]),
                            ProjectNo = reader["projectno"].ToString(),
                            Piv_Date = reader["piv_date"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["piv_date"]),
                            Application_Sub_Type = reader["application_sub_type_desc"].ToString(),
                            Paid_Date = reader["paid_date"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["paid_date"]),
                            Piv2_Paid_Date = reader["piv2_paid_date"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["piv2_paid_date"]),
                            Existing_Acc_No = reader["existing_acc_no"].ToString(),
                            Status = reader["status_desc"].ToString(),
                            Comp_Nm = reader["COMP_NM"].ToString()
                        };
                        result.Add(model);
                    }
                }
            }

            return result;
        }
    }
}