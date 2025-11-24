// File: Repositories/ProvincePIVbankRepository.cs
using MISReports_Api.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace MISReports_Api.Repositories
{
    public class ProvincePIVbankRepository
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["HQOracle"].ConnectionString;

        // Updated Repository Method (Replace GetReport)
        public List<ProvincePIVbankModel> GetReport(string compId, DateTime fromDate, DateTime toDate)
        {
            var result = new List<ProvincePIVbankModel>();

            string sql = @"
        SELECT
            c.dept_id AS CostCenter,
            c.piv_no AS PivNo,
            c.piv_receipt_no AS PivReceiptNo,
            c.piv_date AS PivDate,
            c.paid_date AS PaidDate,
            c.cheque_no AS ChequeNo,
            c.bank_check_no AS BankCheckNo,
            c.paid_amount AS GrandTotal,
            CASE
                WHEN c.payment_mode = 'R' THEN 'Credit Card'
                WHEN c.payment_mode = 'Q' THEN 'Cheque'
                WHEN c.payment_mode = 'D' THEN 'Bank Draft'
                WHEN c.payment_mode = 'C' THEN 'Cash'
                ELSE c.payment_mode
            END AS PaymentMode,
            (SELECT dept_nm FROM gldeptm WHERE dept_id = c.dept_id) AS CCT_NAME,
            (SELECT comp_nm FROM glcompm WHERE comp_id = :compId) AS COMPANY_NAME,
            a.account_code AS AccountCode,
            a.amount AS Amount
        FROM piv_detail c
        INNER JOIN piv_amount a ON c.piv_no = a.piv_no AND c.dept_id = a.dept_id
        WHERE TRIM(c.status) IN ('Q', 'P', 'F')
          AND c.paid_dept_id = '000.00'
          AND c.paid_date >= :fromDate
          AND c.paid_date < :toDatePlusOne
          AND c.dept_id IN (
              SELECT x.dept_id
              FROM gldeptm x
              WHERE x.comp_id IN (
                  SELECT comp_id FROM glcompm
                  WHERE parent_id = :compId OR comp_id = :compId
              )
          )
        ORDER BY c.dept_id, c.piv_no, a.account_code";

            // This query returns ONE row per account split — which is correct
            // But we will group in C# below to ensure clean output if needed
            using (var con = new OracleConnection(_connectionString))
            using (var cmd = new OracleCommand(sql, con))
            {
                cmd.Parameters.Add(new OracleParameter("compId", OracleDbType.Varchar2) { Value = compId });
                cmd.Parameters.Add(new OracleParameter("fromDate", OracleDbType.Date) { Value = fromDate.Date });
                cmd.Parameters.Add(new OracleParameter("toDatePlusOne", OracleDbType.Date) { Value = toDate.AddDays(1).Date });

                con.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    var pivDict = new Dictionary<string, ProvincePIVbankModel>();

                    while (reader.Read())
                    {
                        string key = $"{reader["CostCenter"]}|{reader["PivNo"]}|{reader["PaidDate"]:yyyyMMdd}";

                        if (!pivDict.TryGetValue(key, out var model))
                        {
                            model = new ProvincePIVbankModel
                            {
                                CostCenter = reader["CostCenter"].ToString() ?? "",
                                PivNo = reader["PivNo"].ToString() ?? "",
                                PivReceiptNo = reader["PivReceiptNo"].ToString() ?? "",
                                PivDate = reader["PivDate"] as DateTime?,
                                PaidDate = reader["PaidDate"] as DateTime?,
                                ChequeNo = reader["ChequeNo"].ToString() ?? "",
                                BankCheckNo = reader["BankCheckNo"].ToString() ?? "",
                                GrandTotal = Convert.ToDecimal(reader["GrandTotal"]),
                                PaymentMode = reader["PaymentMode"].ToString() ?? "",
                                CCT_NAME = reader["CCT_NAME"].ToString() ?? "",
                                COMPANY_NAME = reader["COMPANY_NAME"].ToString() ?? "",
                                AccountCode = reader["AccountCode"].ToString() ?? "",
                                Amount = Convert.ToDecimal(reader["Amount"])
                            };
                            pivDict[key] = model;
                        }
                        else
                        {
                            // Already exists → just update account/amount (in case of multiple lines)
                            model.AccountCode = reader["AccountCode"].ToString() ?? "";
                            model.Amount = Convert.ToDecimal(reader["Amount"]);
                        }
                    }

                    result.AddRange(pivDict.Values);
                }
            }

            return result;
        }
    }
}