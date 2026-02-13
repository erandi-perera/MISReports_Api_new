using MISReports_Api.Models;

namespace MISReports_Api.DAL.Analysis
{
    public class SolarAgeAnalysisRepository
    {
        private readonly SolarAgeAnalysisDao _dao =
            new SolarAgeAnalysisDao();

        public SolarAgeSummaryModel GetSummary(string areaCode, int billCycle)
        {
            return _dao.GetSummary(areaCode, billCycle);
        }
    }
}
