using MISReports_Api.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace MISReports_Api.DAL
{
    public class ProvinceWIPRepository
    {
        public async Task<List<WorkInProgressProvinceModel>> GetProvinceWIP(string compId, DateTime fromDate, DateTime toDate)
        {
            var list = new List<WorkInProgressProvinceModel>();

            string connectionString = ConfigurationManager.ConnectionStrings["DefaultOracle"].ConnectionString;

            using (var conn = new OracleConnection(connectionString))
            {
                await conn.OpenAsync();

                string sql = @"
SELECT
    T1.estimate_no,
    T1.project_no,
    T1.std_cost,
    T1.descr,
    T1.fund_id,
    (SELECT SUBSTR(chrg_gl_cd,8,5) FROM pcjbtypm WHERE dept_id = T1.dept_id AND cat_cd = T1.cat_cd) AS Acc_code,
    T1.cat_cd,
    T1.dept_id,
    (SELECT comp_id FROM gldeptm WHERE dept_id = T1.dept_id) AS Area,
    (CASE WHEN T1.apr_dt1 <= T1.prj_ass_dt THEN NULL ELSE T1.apr_dt1 END) AS soft_close_dt,
    T1.conf_dt,
    (CASE WHEN T2.res_type LIKE '%MAT%' THEN 'MAT' WHEN T2.res_type LIKE 'LAB%' THEN 'LAB' ELSE 'OTH' END) AS resource_type,
    SUM(NVL(T2.commited_cost,0)) AS commited_cost,
    (SELECT comp_nm FROM glcompm WHERE comp_id = :compId) AS cct_name
FROM pcestdmt T2
INNER JOIN pcesthmt T1 ON T2.estimate_no = T1.estimate_no AND T2.dept_id = T1.dept_id
WHERE T1.dept_id IN (
    SELECT dept_id FROM gldeptm WHERE comp_id IN (SELECT comp_id FROM glcompm WHERE parent_id = :compId OR comp_id = :compId)
)
AND T1.conf_dt BETWEEN :fromDate AND :toDate
AND T1.status = 3
GROUP BY 
    T1.estimate_no, T1.project_no, T1.std_cost, T1.descr, T1.fund_id, T1.cat_cd, T1.dept_id,
    T2.res_type, T1.apr_dt1, T1.prj_ass_dt, T1.conf_dt
ORDER BY T1.estimate_no, T1.project_no";

                using (var cmd = new OracleCommand(sql, conn))
                {
                    cmd.BindByName = true;
                    cmd.Parameters.Add("compId", OracleDbType.Varchar2).Value = compId;
                    cmd.Parameters.Add("fromDate", OracleDbType.Date).Value = fromDate;
                    cmd.Parameters.Add("toDate", OracleDbType.Date).Value = toDate;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(new WorkInProgressProvinceModel
                            {
                                EstimateNo = reader["estimate_no"].ToString(),
                                ProjectNo = reader["project_no"].ToString(),
                                StdCost = reader["std_cost"] as decimal?,
                                Description = reader["descr"].ToString(),
                                FundId = reader["fund_id"].ToString(),
                                AccCode = reader["Acc_code"].ToString(),
                                CatCd = reader["cat_cd"].ToString(),
                                DeptId = reader["dept_id"].ToString(),
                                Area = reader["Area"].ToString(),
                                SoftCloseDate = reader["soft_close_dt"] as DateTime?,
                                ConfDate = reader["conf_dt"] as DateTime?,
                                ResourceType = reader["resource_type"].ToString(),
                                CommittedCost = reader["commited_cost"] as decimal? ?? 0,
                                CctName = reader["cct_name"].ToString()
                            });
                        }
                    }
                }
            }

            return list;
        }
    }
}
