using System;
using SecurityStatisticsFunctions;
using GlobalHeldData;
using FinancialStructures.Mathematics;
using CompanyStatisticsFunctions;

namespace FinancialStructures.GUIFinanceStructures
{
    public static class SecurityStatsHolderHelper
    {
        public static void AddSecurityStats(SecurityStatsHolder a)
        {
            
            if (a.Name == "Totals" && string.IsNullOrEmpty(a.Company))
            {
                a.LatestVal = MathSupport.Truncate(GlobalData.Finances.AllSecuritiesValue(DateTime.Today));
                a.FundsFraction = 1.0;
                a.Profit = MathSupport.Truncate(GlobalData.Finances.TotalProfit());
                a.CAR3M = MathSupport.Truncate(100 * GlobalData.Finances.IRRPortfolio(DateTime.Today.AddMonths(-3), DateTime.Today)) ;
                a.CAR6M = MathSupport.Truncate(100 * GlobalData.Finances.IRRPortfolio(DateTime.Today.AddMonths(-6), DateTime.Today));
                a.CAR1Y = MathSupport.Truncate(100 * GlobalData.Finances.IRRPortfolio(DateTime.Today.AddMonths(-12), DateTime.Today));
                a.CAR5Y = MathSupport.Truncate(100 * GlobalData.Finances.IRRPortfolio(DateTime.Today.AddMonths(-60), DateTime.Today));
                a.CARTotal = MathSupport.Truncate(100 * GlobalData.Finances.IRRPortfolio(GlobalData.Finances.FirstValueDate(), DateTime.Today));
            }
            else if (a.Name == "Totals" && !string.IsNullOrEmpty(a.Company))
            {
                var c = a.Company;
                a.LatestVal = MathSupport.Truncate(CompanyStatistics.CompanyLatestValue(c));
                a.FundsFraction = MathSupport.Truncate(CompanyStatistics.FundsCompanyFraction(c, DateTime.Today), 4);
                a.Profit = MathSupport.Truncate(CompanyStatistics.CompanyProfit(c));
                a.CAR3M = MathSupport.Truncate(100 * CompanyStatistics.IRRCompany(c, DateTime.Today.AddMonths(-3), DateTime.Today));
                a.CAR6M = MathSupport.Truncate(100 * CompanyStatistics.IRRCompany(c, DateTime.Today.AddMonths(-6), DateTime.Today));
                a.CAR1Y = MathSupport.Truncate(100 * CompanyStatistics.IRRCompany(c, DateTime.Today.AddMonths(-12), DateTime.Today));
                a.CAR5Y = MathSupport.Truncate(100 * CompanyStatistics.IRRCompany(c, DateTime.Today.AddMonths(-60), DateTime.Today));
                a.CARTotal = MathSupport.Truncate(100 * CompanyStatistics.IRRCompanyTotal(c));
            }
            else
            {
                var c = a.Company;
                var n = a.Name;
                a.LatestVal = MathSupport.Truncate(SecurityStatistics.SecurityLatestValue(n, c));
                a.FundsFraction = MathSupport.Truncate(SecurityStatistics.FundsFraction(n, c), 4);
                a.Profit = MathSupport.Truncate(SecurityStatistics.Profit(n, c));
                a.CAR3M = MathSupport.Truncate(100 * SecurityStatistics.SecurityIRRTime(n, c, DateTime.Today.AddMonths(-3), DateTime.Today));
                a.CAR6M = MathSupport.Truncate(100 * SecurityStatistics.SecurityIRRTime(n, c, DateTime.Today.AddMonths(-6), DateTime.Today));
                a.CAR1Y = MathSupport.Truncate(100 * SecurityStatistics.SecurityIRRTime(n, c, DateTime.Today.AddMonths(-12), DateTime.Today));
                a.CAR5Y = MathSupport.Truncate(100 * SecurityStatistics.SecurityIRRTime(n, c, DateTime.Today.AddMonths(-60), DateTime.Today));
                a.CARTotal = MathSupport.Truncate(100 * SecurityStatistics.SecurityIRR(n, c));
            }
        }
    }
}
