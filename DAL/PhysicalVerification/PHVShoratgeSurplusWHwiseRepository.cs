using MISReports_Api.Models.PhysicalVerification;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace MISReports_Api.DAL.PhysicalVerification
{
    public class PHVShoratgeSurplusWHwiseRepository
    {
        private readonly string _connectionString =
            ConfigurationManager.ConnectionStrings["HQOracle"].ConnectionString;

        public async Task<List<PHVShoratgeSurplusWHwiseModel>> GetShortageSurplusWHwiseAsync(
            string deptId,
            int repYear,
            int repMonth,
            string warehouseCode)
        {
            var result = new List<PHVShoratgeSurplusWHwiseModel>();

            var fromDate = new DateTime(repYear, repMonth, 1);
            var toDate = fromDate.AddMonths(1);

            string sql = @"
                SELECT
    TRIM(T1.DOC_NO) AS DOC_NO,
    TRIM(T1.MAT_CD) AS MAT_CD,
    T2.MAT_NM,
    T4.UOM_CD,
    TRIM(T4.GRADE_CD) AS GRADE_CD,
    T5.UNIT_COST AS UNIT_PRICE,
    T1.QTY_ON_HAND,
    T1.CNTED_QTY,

    CASE WHEN T1.ADJ_QTY > 0 THEN T1.ADJ_QTY END AS SURPLUS_QTY,
    CASE WHEN T1.ADJ_QTY < 0 THEN T1.ADJ_QTY END AS SHORTAGE_QTY,

    (T1.QTY_ON_HAND * T5.UNIT_COST) AS STOCK_BOOK,
    (T1.CNTED_QTY * T5.UNIT_COST) AS PHYSICAL_BOOK,

    CASE WHEN T1.ADJ_QTY > 0 THEN T1.ADJ_QTY * T5.UNIT_COST END AS SURPLUS_AMT,
    CASE WHEN T1.ADJ_QTY < 0 THEN T1.ADJ_QTY * T5.UNIT_COST END AS SHORTAGE_AMT,

    (SELECT DEPT_NM FROM GLDEPTM WHERE TRIM(DEPT_ID) = TRIM(:dept_id)) AS CCT_NAME
FROM INPHVDMT T1
JOIN INPOSTMT T5
    ON TRIM(T1.DOC_NO)   = TRIM(T5.DOC_NO)
   AND TRIM(T1.DOC_PF)   = TRIM(T5.DOC_PF)
   AND TRIM(T1.DEPT_ID)  = TRIM(T5.DEPT_ID)
   AND TRIM(T1.MAT_CD)   = TRIM(T5.MAT_CD)
   AND TRIM(T1.GRADE_CD) = TRIM(T5.GRADE_CD)
JOIN INMATM T2
    ON TRIM(T5.MAT_CD) = TRIM(T2.MAT_CD)
JOIN INPHVHMT T3
    ON TRIM(T3.DOC_NO)   = TRIM(T1.DOC_NO)
   AND TRIM(T3.DOC_PF)   = TRIM(T1.DOC_PF)
   AND TRIM(T3.DEPT_ID)  = TRIM(T1.DEPT_ID)
   AND TRIM(T3.BATCH_ID) = TRIM(T1.BATCH_ID)
JOIN INWRHMTM T4
    ON TRIM(T5.DEPT_ID)  = TRIM(T4.DEPT_ID)
   AND TRIM(T5.MAT_CD)   = TRIM(T4.MAT_CD)
   AND TRIM(T5.GRADE_CD) = TRIM(T4.GRADE_CD)
   AND TRIM(T3.WRH_CD)   = TRIM(T4.WRH_CD)
WHERE
    T1.ADJ_QTY <> 0
    AND TRIM(T1.DEPT_ID) = TRIM(:dept_id)
    AND T3.PHV_DT >= :from_date
    AND T3.PHV_DT <  :to_date
    AND TRIM(T3.WRH_CD)  = TRIM(:wrh_cd)
ORDER BY
    TRIM(T3.WRH_CD),
    TRIM(T1.MAT_CD),
    TRIM(T4.GRADE_CD)
";

            using (var conn = new OracleConnection(_connectionString))
            using (var cmd = new OracleCommand(sql, conn))
            {
                cmd.BindByName = true;

                cmd.Parameters.Add("dept_id", OracleDbType.Varchar2).Value = deptId;
                cmd.Parameters.Add("from_date", OracleDbType.Date).Value = fromDate;
                cmd.Parameters.Add("to_date", OracleDbType.Date).Value = toDate;
                cmd.Parameters.Add("wrh_cd", OracleDbType.Varchar2).Value = warehouseCode;

                await conn.OpenAsync();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        result.Add(new PHVShoratgeSurplusWHwiseModel
                        {
                            DocumentNo = reader["DOC_NO"]?.ToString(),
                            MaterialCode = reader["MAT_CD"]?.ToString(),
                            MaterialName = reader["MAT_NM"]?.ToString(),
                            UomCode = reader["UOM_CD"]?.ToString(),
                            GradeCode = reader["GRADE_CD"]?.ToString(),

                            UnitPrice = reader["UNIT_PRICE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["UNIT_PRICE"]),
                            QtyOnHand = reader["QTY_ON_HAND"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["QTY_ON_HAND"]),
                            CountedQty = reader["CNTED_QTY"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CNTED_QTY"]),

                            SurplusQty = reader["SURPLUS_QTY"] == DBNull.Value ? null : (decimal?)Convert.ToDecimal(reader["SURPLUS_QTY"]),
                            ShortageQty = reader["SHORTAGE_QTY"] == DBNull.Value ? null : (decimal?)Convert.ToDecimal(reader["SHORTAGE_QTY"]),

                            StockBook = reader["STOCK_BOOK"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["STOCK_BOOK"]),
                            PhysicalBook = reader["PHYSICAL_BOOK"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["PHYSICAL_BOOK"]),

                            SurplusAmount = reader["SURPLUS_AMT"] == DBNull.Value ? null : (decimal?)Convert.ToDecimal(reader["SURPLUS_AMT"]),
                            ShortageAmount = reader["SHORTAGE_AMT"] == DBNull.Value ? null : (decimal?)Convert.ToDecimal(reader["SHORTAGE_AMT"]),

                            CostCentreName = reader["CCT_NAME"]?.ToString()
                        });
                    }
                }
            }

            return result;
        }
    }
}
