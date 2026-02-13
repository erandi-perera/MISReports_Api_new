using MISReports_Api.Models.Analysis;
using System.Collections.Generic;

namespace MISReports_Api.DAL.Analysis
{
    public class SolarAgeCategoryRepository
    {
        private readonly SolarAgeCategoryDao _dao =
            new SolarAgeCategoryDao();

        public List<SolarAgeCategoryDetailModel> GetByCategory(
            string areaCode,
            int billCycle,
            string category)
        {
            return _dao.GetByCategory(areaCode, billCycle, category);
        }
    }
}
