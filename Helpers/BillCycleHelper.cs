using System.Collections.Generic;

namespace MISReports_Api.Helpers
{
    public static class BillCycleHelper
    {
        public static List<string> Generate24MonthYearStrings(int maxCycle) //If maxCycle = 401, it will generate month-year strings for:401, 400, 399, ..., 378
        {
            List<string> monthYearStrings = new List<string>();

            for (int i = maxCycle; i > maxCycle - 24 && i > 0; i--)
            {
                monthYearStrings.Add(ConvertToMonthYear(i));
            }

            return monthYearStrings;
        }

        public static string ConvertToMonthYear(int billCycle)
        {
            try
            {
                int mnth = (billCycle - 100) % 12;
                int m_mnth = mnth;
                int yr = 97 + (billCycle - 100) / 12;
                string yr1;

                if (mnth == 0)
                {
                    yr -= 1;
                    m_mnth = 12;
                }

                yr1 = (yr % 100).ToString("00");

                switch (m_mnth)
                {
                    case 1: return "Jan " + yr1;
                    case 2: return "Feb " + yr1;
                    case 3: return "Mar " + yr1;
                    case 4: return "Apr " + yr1;
                    case 5: return "May " + yr1;
                    case 6: return "Jun " + yr1;
                    case 7: return "Jul " + yr1;
                    case 8: return "Aug " + yr1;
                    case 9: return "Sep " + yr1;
                    case 10: return "Oct " + yr1;
                    case 11: return "Nov " + yr1;
                    case 12: return "Dec " + yr1;
                    default: return "Unknown";
                }
            }
            catch
            {
                return "Invalid";
            }
        }
    }
}