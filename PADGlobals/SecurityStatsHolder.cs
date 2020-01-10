using CompanyStatisticsFunctions;
using FinancialStructures.FinanceStructures;
using FinancialStructures.Mathematics;
using GlobalHeldData;
using SecurityStatisticsFunctions;
using System;

namespace FinancialStructures.GUIFinanceStructures
{
    public static class SecurityStatsHolderHelper
    {
        public static void AddSecurityStats(SecurityStatsHolder a)
        {

            if (a.Name == "Totals" && string.IsNullOrEmpty(a.Company))
            {
                a.LatestVal = MathSupport.Truncate(GlobalData.Finances.AllSecuritiesValue(DateTime.Today));
                a.RecentChange = 0;
                a.FundsFraction = 1.0;
                a.Profit = MathSupport.Truncate(GlobalData.Finances.TotalProfit());
                a.CAR3M = MathSupport.Truncate(100 * GlobalData.Finances.IRRPortfolio(DateTime.Today.AddMonths(-3), DateTime.Today));
                a.CAR6M = MathSupport.Truncate(100 * GlobalData.Finances.IRRPortfolio(DateTime.Today.AddMonths(-6), DateTime.Today));
                a.CAR1Y = MathSupport.Truncate(100 * GlobalData.Finances.IRRPortfolio(DateTime.Today.AddMonths(-12), DateTime.Today));
                a.CAR5Y = MathSupport.Truncate(100 * GlobalData.Finances.IRRPortfolio(DateTime.Today.AddMonths(-60), DateTime.Today));
                a.CARTotal = MathSupport.Truncate(100 * GlobalData.Finances.IRRPortfolio(GlobalData.Finances.FirstValueDate(), DateTime.Today));
            }
            else if (a.Name == "Totals" && !string.IsNullOrEmpty(a.Company))
            {
                var c = a.Company;
                a.LatestVal = MathSupport.Truncate(CompanyStatistics.CompanyLatestValue(c));
                a.RecentChange = 0;
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
                a.RecentChange = MathSupport.Truncate(SecurityStatistics.RecentChange(n, c));
                a.FundsFraction = MathSupport.Truncate(SecurityStatistics.FundsFraction(n, c), 4);
                a.Profit = MathSupport.Truncate(SecurityStatistics.Profit(n, c));
                a.CAR3M = MathSupport.Truncate(100 * SecurityStatistics.SecurityIRRTime(n, c, DateTime.Today.AddMonths(-3), DateTime.Today));
                a.CAR6M = MathSupport.Truncate(100 * SecurityStatistics.SecurityIRRTime(n, c, DateTime.Today.AddMonths(-6), DateTime.Today));
                a.CAR1Y = MathSupport.Truncate(100 * SecurityStatistics.SecurityIRRTime(n, c, DateTime.Today.AddMonths(-12), DateTime.Today));
                a.CAR5Y = MathSupport.Truncate(100 * SecurityStatistics.SecurityIRRTime(n, c, DateTime.Today.AddMonths(-60), DateTime.Today));
                a.CARTotal = MathSupport.Truncate(100 * SecurityStatistics.SecurityIRR(n, c));
            }
        }

        public static void AddSectorStats(SecurityStatsHolder a)
        { 
            if (a.Company == "BenchMark")
            {
                Sector chosenSector = null;
                foreach (var sector in GlobalData.BenchMarks)
                {
                    if (a.Name == sector.GetName())
                    {
                        chosenSector = sector.Copy();
                    }
                }
                if (chosenSector != null)
                {
                    a.LatestVal = MathSupport.Truncate(chosenSector.LatestValue().Value);
                    a.FundsFraction = 0.0;
                    a.Profit = 0.0;
                    a.CAR3M = MathSupport.Truncate(100 * chosenSector.CAR(DateTime.Today.AddMonths(-3), DateTime.Today));
                    a.CAR6M = MathSupport.Truncate(100 * chosenSector.CAR(DateTime.Today.AddMonths(-6), DateTime.Today));
                    a.CAR1Y = MathSupport.Truncate(100 * chosenSector.CAR(DateTime.Today.AddMonths(-12), DateTime.Today));
                    a.CAR5Y = MathSupport.Truncate(100 * chosenSector.CAR(DateTime.Today.AddMonths(-60), DateTime.Today));
                    a.CARTotal = MathSupport.Truncate(100 * chosenSector.CAR(GlobalData.Finances.FirstValueDate(), DateTime.Today));
                }
            }
            else
            {
                a.LatestVal = MathSupport.Truncate(GlobalData.Finances.SectorValue(a.Name, DateTime.Today));
                a.FundsFraction = MathSupport.Truncate(GlobalData.Finances.SectorFraction(a.Name, DateTime.Today), 4);
                a.Profit = MathSupport.Truncate(GlobalData.Finances.SectorProfit(a.Name));
                a.CAR3M = MathSupport.Truncate(100 * GlobalData.Finances.IRRSector(a.Name, DateTime.Today.AddMonths(-3), DateTime.Today));
                a.CAR6M = MathSupport.Truncate(100 * GlobalData.Finances.IRRSector(a.Name, DateTime.Today.AddMonths(-6), DateTime.Today));
                a.CAR1Y = MathSupport.Truncate(100 * GlobalData.Finances.IRRSector(a.Name, DateTime.Today.AddMonths(-12), DateTime.Today));
                a.CAR5Y = MathSupport.Truncate(100 * GlobalData.Finances.IRRSector(a.Name, DateTime.Today.AddMonths(-60), DateTime.Today));
                a.CARTotal = MathSupport.Truncate(100 * GlobalData.Finances.IRRSector(a.Name, GlobalData.Finances.FirstValueDate(), DateTime.Today));
            }
        }
    }
}
