using MISReports_Api.DBAccess;
using MISReports_Api.Models.Analysis;
using System;
using System.Collections.Generic;
using System.Data.OleDb;

namespace MISReports_Api.DAL.Analysis
{
    public class SolarAgeCategoryDao
    {
        public List<SolarAgeCategoryDetailModel> GetByCategory(
            string areaCode,
            int billCycle,
            string category)
        {
            var list = new List<SolarAgeCategoryDetailModel>();
            string ageCondition;

            // -------- SAME AGE LOGIC AS SUMMARY --------
            if (category == "LE1")
                ageCondition = "m.agrmnt_date >= ADD_MONTHS(TRUNC(SYSDATE), -12)";
            else if (category == "1-2")
                ageCondition = "m.agrmnt_date < ADD_MONTHS(TRUNC(SYSDATE), -12) AND m.agrmnt_date >= ADD_MONTHS(TRUNC(SYSDATE), -24)";
            else if (category == "2-3")
                ageCondition = "m.agrmnt_date < ADD_MONTHS(TRUNC(SYSDATE), -24) AND m.agrmnt_date >= ADD_MONTHS(TRUNC(SYSDATE), -36)";
            else if (category == "3-4")
                ageCondition = "m.agrmnt_date < ADD_MONTHS(TRUNC(SYSDATE), -36) AND m.agrmnt_date >= ADD_MONTHS(TRUNC(SYSDATE), -48)";
            else if (category == "4-5")
                ageCondition = "m.agrmnt_date < ADD_MONTHS(TRUNC(SYSDATE), -48) AND m.agrmnt_date >= ADD_MONTHS(TRUNC(SYSDATE), -60)";
            else if (category == "5-6")
                ageCondition = "m.agrmnt_date < ADD_MONTHS(TRUNC(SYSDATE), -60) AND m.agrmnt_date >= ADD_MONTHS(TRUNC(SYSDATE), -72)";
            else if (category == "6-7")
                ageCondition = "m.agrmnt_date < ADD_MONTHS(TRUNC(SYSDATE), -72) AND m.agrmnt_date >= ADD_MONTHS(TRUNC(SYSDATE), -84)";
            else if (category == "7-8")
                ageCondition = "m.agrmnt_date < ADD_MONTHS(TRUNC(SYSDATE), -84) AND m.agrmnt_date >= ADD_MONTHS(TRUNC(SYSDATE), -96)";
            else if (category == "GT8")
                ageCondition = "m.agrmnt_date < ADD_MONTHS(TRUNC(SYSDATE), -96)";
            else if (category == "NULL")
                ageCondition = "m.agrmnt_date IS NULL";
            else
                throw new Exception("Invalid age category");

            DBConnection db = new DBConnection();

            /* ============================
               STEP 1: GET PROV SERVER
               ============================ */
            string svr = "";

            using (OleDbConnection connection = db.Billsmrydb())
            {
                string sql = @"
                    SELECT prov_b_svr
                    FROM prov_servers p, areas a
                    WHERE a.prov_code = p.prov_code
                    AND a.area_code = ?";

                using (OleDbCommand command = new OleDbCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("area_code", areaCode);

                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                            svr = reader[0].ToString().Trim();
                    }
                }
            }

            if (string.IsNullOrEmpty(svr))
                throw new Exception("Billing server not found for area " + areaCode);

            /* ============================
               STEP 2: CONNECT billing@SVR
               ============================ */
            string dbName = "billing@" + svr;

            using (OleDbConnection conn = db.Provdb(dbName))
            {
                string sql = @"
SELECT
    n.acct_number,
    n.net_type,
    c.cust_fname,
    c.cust_lname,
    c.address_1,
    c.address_2,
    c.address_3,
    m.agrmnt_date
FROM netmtcons n
JOIN electric_" + areaCode + @" e ON n.acct_number = e.acct_number
JOIN netmeter m ON n.acct_number = m.acct_number
JOIN customers c ON n.acct_number = c.acct_number
WHERE n.bill_cycle = ?
  AND n.area_code = ?
  AND e.cust_status <> '9'
  AND " + ageCondition + @"
ORDER BY m.agrmnt_date";

                using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("bill_cycle", billCycle);
                    cmd.Parameters.AddWithValue("area_code", areaCode);

                    try
                    {
                        using (OleDbDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                list.Add(new SolarAgeCategoryDetailModel
                                {
                                    AccountNumber = dr["acct_number"].ToString(),
                                    NetType = Convert.ToInt32(dr["net_type"]),
                                    NetTypeName = GetNetTypeName(dr["net_type"].ToString()),
                                    CustomerName = dr["cust_fname"] + " " + dr["cust_lname"],
                                    Address1 = dr["address_1"].ToString(),
                                    Address2 = dr["address_2"].ToString(),
                                    Address3 = dr["address_3"].ToString(),
                                    AgreementDate = dr["agrmnt_date"] == DBNull.Value
                                        ? (DateTime?)null
                                        : Convert.ToDateTime(dr["agrmnt_date"])
                                });
                            }
                        }
                    }
                    catch (OleDbException ex)
                    {
                        if (ex.Message.Contains("-111"))
                            return list; // no rows
                        throw;
                    }
                }
            }

            return list;
        }

        private string GetNetTypeName(string netType)
        {
            if (netType == "1") return "Net Metering";
            if (netType == "2") return "Net Accounting";
            if (netType == "3") return "Net Plus";
            if (netType == "4") return "Net Plus Plus";
            if (netType == "5") return "Convert from Net Metering to Net Accounting";
            return "Unknown";
        }
    }
}
