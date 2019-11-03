using System;
using GlobalHeldData;

namespace CompanyStatisticsFunctions
{
    /// <summary>
    /// Helper class to get statistics for companies
    /// </summary>
    public static class CompanyStatistics
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
