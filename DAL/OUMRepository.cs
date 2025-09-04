using MISReports_Api.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Odbc;

namespace MISReports_Api.DAL
{
    public class OUMRepository
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["InformixCreditCard"].ConnectionString;

        public int InsertIntoAmex2(List<OUMEmployeeModel> data)
        {
            int count = 0;
            using (var conn = new OdbcConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Clear existing data
                    using (var cmd = new OdbcCommand("DELETE FROM test_amex2", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // Insert new data
                    foreach (var item in data)
                    {
                        using (var cmd = new OdbcCommand())
                        {
                            cmd.Connection = conn;
                            cmd.CommandText = @"INSERT INTO test_amex2 (pdate, o_id, acct_no, cname, bill_amt, tax, tot_amt, authcode, cno) 
                                              VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)";

                            cmd.Parameters.AddWithValue("pdate", item.AuthDate);
                            cmd.Parameters.AddWithValue("o_id", item.OrderId);
                            cmd.Parameters.AddWithValue("acct_no", item.AcctNumber ?? "");
                            cmd.Parameters.AddWithValue("cname", item.BankCode ?? "");
                            cmd.Parameters.AddWithValue("bill_amt", item.BillAmt);
                            cmd.Parameters.AddWithValue("tax", item.TaxAmt);
                            cmd.Parameters.AddWithValue("tot_amt", item.TotAmt);
                            cmd.Parameters.AddWithValue("authcode", item.AuthCode ?? "");
                            cmd.Parameters.AddWithValue("cno", item.CardNo ?? "");

                            count += cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch (OdbcException ex)
                {
                    Console.WriteLine($"Error inserting into Amex2: {ex.Message}");
                    throw;
                }
            }
            return count;
        }

        public void RefreshCrdTemp()
        {
            using (var conn = new OdbcConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Delete existing records from test_crdt_tmp
                    using (var cmd = new OdbcCommand("DELETE FROM test_crdt_tmp", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // Insert from amex2 to crdt_tmp
                    string insertSql = @"INSERT INTO test_crdt_tmp 
                                       SELECT o_id, acct_no, '-', '-', bill_amt, tax, tot_amt, 'S',
                                              authcode, pdate, pdate, 'S', 0, cname, 'CRC', '', '', '', '', '', '',
                                              cno, 'Bil', acct_no, 'RSK', ''
                                       FROM test_amex2";

                    using (var cmd = new OdbcCommand(insertSql, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // Update null values for specific fields
                    using (var cmd = new OdbcCommand(@"UPDATE test_crdt_tmp 
                                                     SET updt_flag = NULL, post_flag = NULL, err_flag = NULL, sms_st = NULL", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // Update payment_type where ref_number is longer than 10 characters  
                    using (var cmd = new OdbcCommand(@"UPDATE test_crdt_tmp SET payment_type = 'PIV' WHERE LENGTH(ref_number) > 10", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (OdbcException ex)
                {
                    Console.WriteLine($"Error refreshing CrdTemp: {ex.Message}");
                    throw;
                }
            }
        }

        public List<OUMCrdTempModel> GetCrdTempRecords()
        {
            var records = new List<OUMCrdTempModel>();
            using (var conn = new OdbcConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string sql = "SELECT * FROM test_crdt_tmp ORDER BY auth_date";

                    using (var cmd = new OdbcCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var record = new OUMCrdTempModel
                            {
                                OrderId = GetSafeInt(reader, "order_id"),
                                AcctNumber = GetSafeString(reader, "acct_number"),
                                CustName = GetSafeString(reader, "custname"),
                                UserName = GetSafeString(reader, "username"),
                                BillAmt = GetSafeDecimal(reader, "bill_amt"),
                                TaxAmt = GetSafeDecimal(reader, "tax_amt"),
                                TotAmt = GetSafeDecimal(reader, "tot_amt"),
                                TrStatus = GetSafeString(reader, "trstatus"),
                                AuthCode = GetSafeString(reader, "authcode"),
                                PmntDate = GetSafeDateTime(reader, "pmnt_date"),
                                AuthDate = GetSafeDateTime(reader, "auth_date"),
                                CebRes = GetSafeString(reader, "cebres"),
                                SerlNo = GetSafeInt(reader, "serl_no"),
                                BankCode = GetSafeString(reader, "bank_code"),
                                BranCode = GetSafeString(reader, "bran_code"),
                                CardNo = GetSafeString(reader, "card_no"),
                                PaymentType = GetSafeString(reader, "payment_type"),
                                RefNumber = GetSafeString(reader, "ref_number"),
                                ReferenceType = GetSafeString(reader, "reference_type")
                            };
                            records.Add(record);
                        }
                    }
                }
                catch (OdbcException ex)
                {
                    Console.WriteLine($"Error retrieving CrdTemp records: {ex.Message}");
                    throw;
                }
            }
            return records;
        }

        public bool ApproveRecords()
        {
            using (var conn = new OdbcConnection(connectionString))
            {
                OdbcTransaction transaction = null;
                try
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();

                    // Insert into test_crdtcdslt
                    using (var cmd = new OdbcCommand("INSERT INTO test_crdtcdslt SELECT * FROM test_crdt_tmp", conn, transaction))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // Insert into backup table
                    using (var cmd = new OdbcCommand("INSERT INTO appadm1.crdt_tmp_backup SELECT * FROM appadm1.test_crdt_tmp", conn, transaction))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // Delete from temp table
                    using (var cmd = new OdbcCommand("DELETE FROM appadm1.test_crdt_tmp", conn, transaction))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    return true;
                }
                catch (OdbcException ex)
                {
                    transaction?.Rollback();
                    Console.WriteLine($"Error approving records: {ex.Message}");
                    throw;
                }
            }
        }

        // Helper methods for safe data conversion
        private string GetSafeString(OdbcDataReader reader, string columnName)
        {
            try
            {
                int ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal)?.Trim();
            }
            catch
            {
                return null;
            }
        }

        private int GetSafeInt(OdbcDataReader reader, string columnName)
        {
            try
            {
                int ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? 0 : Convert.ToInt32(reader.GetValue(ordinal));
            }
            catch
            {
                return 0;
            }
        }

        private decimal GetSafeDecimal(OdbcDataReader reader, string columnName)
        {
            try
            {
                int ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? 0m : Convert.ToDecimal(reader.GetValue(ordinal));
            }
            catch
            {
                return 0m;
            }
        }

        private DateTime GetSafeDateTime(OdbcDataReader reader, string columnName)
        {
            try
            {
                int ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? DateTime.MinValue : Convert.ToDateTime(reader.GetValue(ordinal));
            }
            catch
            {
                return DateTime.MinValue;
            }
        }
    }
}