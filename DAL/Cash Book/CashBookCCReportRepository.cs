// DAL/CashBookCCReportRepository.cs
using MISReports_Api.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace MISReports_Api.DAL
{
    /// <summary>
    /// Dedicated repository for Cash Book report by Cost Center (dept_id)
    /// Implements your exact SQL query.
    /// .NET 4.7.2 | C# 7.3
    /// </summary>
    public class CashBookCCReportRepository
    {
        private readonly string _connStr = ConfigurationManager.ConnectionStrings["HQOracle"].ConnectionString;

        public List<CashBookCCReportModel> GetCashBookCCReport(
            string fromDate,
            string toDate,
            string costCenter,
            string payee)
        {
            string oracleFrom = FormatDateForOracle(fromDate);
            string oracleTo = FormatDateForOracle(toDate);

            var result = new List<CashBookCCReportModel>();

            const string sql = @"
SELECT * FROM (
    SELECT
        T1.chq_run,
        T1.chq_dt,
        T1.payee,
        T1.pymt_docno,
        T1.chq_amt,
        T1.chq_no,
        (SELECT dept_nm FROM gldeptm WHERE dept_id = :costctr) AS cct_name
    FROM cbchqdmt T1
    WHERE T1.chq_dt >= TO_DATE(:fromDate, 'YYYY/MM/DD')
      AND T1.chq_dt <= TO_DATE(:toDate,   'YYYY/MM/DD')
      AND T1.dept_id = :costctr
      AND (TRIM(:ee) IS NULL OR UPPER(T1.payee) LIKE '%' || UPPER(TRIM(:ee)) || '%')
    GROUP BY
        T1.chq_run, T1.chq_dt, T1.payee, T1.pymt_docno, T1.chq_amt, T1.chq_no
    ORDER BY
        T1.payee, T1.chq_dt, T1.chq_no
) WHERE ROWNUM <= 5000";

            using (var conn = new OracleConnection(_connStr))
            using (var cmd = new OracleCommand(sql, conn))
            {
                cmd.BindByName = true;

                cmd.Parameters.Add("fromDate", OracleDbType.Varchar2).Value = oracleFrom;
                cmd.Parameters.Add("toDate", OracleDbType.Varchar2).Value = oracleTo;
                cmd.Parameters.Add("costctr", OracleDbType.Varchar2).Value = costCenter?.Trim();
                cmd.Parameters.Add("ee", OracleDbType.Varchar2).Value =
                    string.IsNullOrWhiteSpace(payee) ? (object)DBNull.Value : payee.Trim();

                try
                {
                    conn.Open();
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            result.Add(new CashBookCCReportModel
                            {
                                ChqRun = SafeGetString(rdr, "CHQ_RUN"),
                                ChqDt = SafeGetDateTime(rdr, "CHQ_DT"),
                                Payee = SafeGetString(rdr, "PAYEE"),
                                PymtDocNo = SafeGetString(rdr, "PYMT_DOCNO"),
                                ChqAmt = SafeGetDecimal(rdr, "CHQ_AMT"),
                                ChqNo = SafeGetString(rdr, "CHQ_NO"),
                                CctName = SafeGetString(rdr, "CCT_NAME")
                            });
                        }
                    }
                }
                catch (OracleException oex)
                {
                    throw new Exception($"Oracle error {oex.Number}: {oex.Message}", oex);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error fetching cash book (cost center) report", ex);
                }
            }

            return result;
        }

        private string FormatDateForOracle(string yyyymmdd)
        {
            if (string.IsNullOrWhiteSpace(yyyymmdd) || yyyymmdd.Length != 8)
                return null;
            return $"{yyyymmdd.Substring(0, 4)}/{yyyymmdd.Substring(4, 2)}/{yyyymmdd.Substring(6, 2)}";
        }

        private string SafeGetString(OracleDataReader r, string col)
        {
            int ord = r.GetOrdinal(col);
            return r.IsDBNull(ord) ? null : r.GetString(ord);
        }

        private decimal? SafeGetDecimal(OracleDataReader r, string col)
        {
            int ord = r.GetOrdinal(col);
            return r.IsDBNull(ord) ? (decimal?)null : r.GetDecimal(ord);
        }

        private DateTime? SafeGetDateTime(OracleDataReader r, string col)
        {
            int ord = r.GetOrdinal(col);
            return r.IsDBNull(ord) ? (DateTime?)null : r.GetDateTime(ord);
        }
    }
}