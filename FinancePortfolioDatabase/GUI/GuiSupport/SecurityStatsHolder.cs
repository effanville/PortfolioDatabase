using System;
using SecurityStatisticsFunctions;
using GlobalHeldData;
using mathematics;
using CompanyStatisticsFunctions;

namespace GUIFinanceStructures
{
    public class SecurityStatsHolder : IComparable
    {
        public int CompareTo(object obj)
        {
            if (obj is SecurityStatsHolder value)
            {
                if (Company == value.Company)
                {
                    return Name.CompareTo(value.Name);
                }

                return Company.CompareTo(value.Company);
            }

            return 0;
        }

        public SecurityStatsHolder()
        { }
        public SecurityStatsHolder(string n, string c)
        {
            Name = n;
            Company = c;
            
            if (n == "Totals" && string.IsNullOrEmpty(c))
            {
                LatestVal = MathSupport.Truncate(GlobalData.Finances.AllSecuritiesValue(DateTime.Today));
                FundsFraction = 1.0;
                Profit = MathSupport.Truncate(GlobalData.Finances.TotalProfit());
                CAR3M = MathSupport.Truncate(100 * GlobalData.Finances.IRRPortfolio(DateTime.Today.AddMonths(-3), DateTime.Today)) ;
                CAR6M = MathSupport.Truncate(100 * GlobalData.Finances.IRRPortfolio(DateTime.Today.AddMonths(-6), DateTime.Today));
                CAR1Y = MathSupport.Truncate(100 * GlobalData.Finances.IRRPortfolio(DateTime.Today.AddMonths(-12), DateTime.Today));
                CAR5Y = MathSupport.Truncate(100 * GlobalData.Finances.IRRPortfolio(DateTime.Today.AddMonths(-60), DateTime.Today));
                CARTotal = MathSupport.Truncate(100 * GlobalData.Finances.IRRPortfolio(GlobalData.Finances.FirstValueDate(), DateTime.Today));
            }
            else if (n =="Totals" && !string.IsNullOrEmpty(c))
            {
                LatestVal = MathSupport.Truncate(CompanyStatistics.CompanyLatestValue( c));
                FundsFraction = MathSupport.Truncate(CompanyStatistics.FundsCompanyFraction(c, DateTime.Today), 4);
                Profit = MathSupport.Truncate(CompanyStatistics.CompanyProfit(c));
                CAR3M = MathSupport.Truncate(100 * CompanyStatistics.IRRCompany(c, DateTime.Today.AddMonths(-3), DateTime.Today));
                CAR6M = MathSupport.Truncate(100 * CompanyStatistics.IRRCompany(c, DateTime.Today.AddMonths(-6), DateTime.Today));
                CAR1Y = MathSupport.Truncate(100 * CompanyStatistics.IRRCompany(c, DateTime.Today.AddMonths(-12), DateTime.Today));
                CAR5Y = MathSupport.Truncate(100 * CompanyStatistics.IRRCompany(c, DateTime.Today.AddMonths(-60), DateTime.Today));
                CARTotal = MathSupport.Truncate(100 * CompanyStatistics.IRRCompanyTotal(c));
            }
            else
            {
                LatestVal = MathSupport.Truncate(SecurityStatistics.SecurityLatestValue(n, c));
                FundsFraction = MathSupport.Truncate(SecurityStatistics.FundsFraction(n, c), 4);
                Profit = MathSupport.Truncate(SecurityStatistics.Profit(n, c));
                CAR3M = MathSupport.Truncate(100 * SecurityStatistics.SecurityIRRTime(n, c, DateTime.Today.AddMonths(-3), DateTime.Today));
                CAR6M = MathSupport.Truncate(100 * SecurityStatistics.SecurityIRRTime(n, c, DateTime.Today.AddMonths(-6), DateTime.Today));
                CAR1Y = MathSupport.Truncate(100 * SecurityStatistics.SecurityIRRTime(n, c, DateTime.Today.AddMonths(-12), DateTime.Today));
                CAR5Y = MathSupport.Truncate(100 * SecurityStatistics.SecurityIRRTime(n, c, DateTime.Today.AddMonths(-60), DateTime.Today));
                CARTotal = MathSupport.Truncate(100 * SecurityStatistics.SecurityIRR(n, c));
            }
        }

        public string Company { get; set; }
        public string Name { get; set; }
        
        public double LatestVal { get; set; }
        public double FundsFraction { get; set; }
        public double Profit { get; set; }
        public double CAR3M { get; set; }
        public double CAR6M { get; set; }
        public double CAR1Y { get; set; }
        public double CAR5Y { get; set; }
        public double CARTotal { get; set; }
    }
}
