using MISReports_Api.DBAccess;
using MISReports_Api.Models;
using System;
using System.Data;
using System.Data.OleDb;

namespace MISReports_Api.DAL.Analysis
{
    public class SolarAgeAnalysisDao
    {
        public SolarAgeSummaryModel GetSummary(string areaCode, int billCycle)
        {
            var model = new SolarAgeSummaryModel();

            try
            {
                DBConnection db = new DBConnection();

                /* ============================
                   STEP 1: GET PROV SERVER
                   ============================ */
                string svr = "";

                using (
                    OleDbConnection connection = db.Billsmrydb()) // ✅ Connect to fixed Billsmry DB
                {
                    string sql = $@"
                        SELECT prov_b_svr
                        FROM prov_servers p, areas a
                        WHERE a.prov_code = p.prov_code
                        AND a.area_code = '{areaCode}'";

                    using (OleDbCommand command = new OleDbCommand(sql, connection))
                    {
                        using (OleDbDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                svr = reader[0].ToString().Trim();
                            }
                        }
                    }
                }

                if (string.IsNullOrEmpty(svr))
                    throw new Exception("Billing server not found for area " + areaCode);

                /* ============================
                   STEP 2: CONNECT BILLING@<SVR>
                   ============================ */
                string dbName = "billing@" + svr;

                using (OleDbConnection billingConn = db.Provdb(dbName))
                {
                    string solarSql = $@"
                        SELECT m.agrmnt_date
                        FROM netmtcons n
                        JOIN electric_{areaCode} e ON n.acct_number = e.acct_number
                        JOIN netmeter m ON n.acct_number = m.acct_number
                        WHERE n.bill_cycle = ?
                        AND n.area_code = ?
                        AND e.cust_status <> '9'";

                    using (OleDbCommand cmd = new OleDbCommand(solarSql, billingConn))
                    {
                        cmd.Parameters.AddWithValue("bill_cycle", billCycle);
                        cmd.Parameters.AddWithValue("area_code", areaCode);

                        using (OleDbDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                if (dr.IsDBNull(0))
                                {
                                    model.AgreementDateNull++;
                                    continue;
                                }

                                DateTime agrDate = dr.GetDateTime(0);
                                int days = (DateTime.Now - agrDate).Days;

                                if (days <= 365) model.Age_0_1++;
                                else if (days <= 730) model.Age_1_2++;
                                else if (days <= 1095) model.Age_2_3++;
                                else if (days <= 1460) model.Age_3_4++;
                                else if (days <= 1825) model.Age_4_5++;
                                else if (days <= 2190) model.Age_5_6++;
                                else if (days <= 2555) model.Age_6_7++;
                                else if (days <= 2920) model.Age_7_8++;
                                else model.Age_Above_8++;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                model.ErrorMessage = ex.Message;
            }

            return model;
        }
    }
}
