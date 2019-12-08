using System;
using System.Collections.Generic;
using FinancialStructures.DataStructures;
using FinancialStructures.FinanceStructures;
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

        public static double CompanyProfit(string company)
        {
            return GlobalData.Finances.CompanyProfit(company);
        }

        public static double FundsCompanyFraction(string company, DateTime date)
        {
            return GlobalData.Finances.FundsCompanyFraction(company, date);
        }

        public static List<DailyValuation_Named> GetCompanyInvestments(string company)
        {
            return GlobalData.Finances.GetCompanyInvestments(company);
        }

        public static double IRRCompany(string company, DateTime earlierTime, DateTime laterTime)
        {
            return GlobalData.Finances.IRRCompany(company, earlierTime, laterTime);
        }

        private static List<Security> CompanySecurities(string company)
        {
            return GlobalData.Finances.CompanySecurities(company);
        }

        public static double IRRCompanyTotal(string company)
        {
            DateTime earlierTime = DateTime.Today; 
            DateTime laterTime = DateTime.Today;
            var securities = CompanySecurities(company);
            if (securities.Count == 0)
            {
                return double.NaN;
            }
            foreach (var security in securities)
            {
                if (security.Any())
                {
                    var first = security.FirstValue().Day;
                    var last = security.LatestValue().Day;
                    if (first < earlierTime)
                    {
                        earlierTime = first;
                    }
                    if (last > laterTime)
                    {
                        laterTime = last;
                    }
                }
            }
            return GlobalData.Finances.IRRCompany(company, earlierTime, laterTime);
        }

        public static double IRRFunds(DateTime earlierTime, DateTime laterTime)
        {
            return GlobalData.Finances.IRRPortfolio( earlierTime, laterTime);
        }
    }
}
