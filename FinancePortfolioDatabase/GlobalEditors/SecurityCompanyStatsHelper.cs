using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinancePortfolioDatabase;
using GlobalHeldData;

namespace CompanyStatisticsFunctions
{
    /// <summary>
    /// Helper class to get statistics for companies
    /// </summary>
    public static class CompanyStatisticsHelper
    {
        public static double CompanyLatestValue(string company)
        {
            return GlobalData.Finances.CompanyValue(company, DateTime.Today);
        }

        public static double CompanyValue(string company, DateTime date)
        {
            return GlobalData.Finances.CompanyValue(company, date);
        }

        public static double IRRCompany(string company, DateTime earlierTime, DateTime laterTime)
        {
            return GlobalData.Finances.IRRCompany(company, earlierTime, laterTime);
        }

        public static double IRRFunds(DateTime earlierTime, DateTime laterTime)
        {
            return GlobalData.Finances.IRRPortfolio( earlierTime, laterTime);
        }
    }
}
