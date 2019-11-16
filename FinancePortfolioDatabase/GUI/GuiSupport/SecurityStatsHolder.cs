using System;
using SecurityStatisticsFunctions;
using GlobalHeldData;
using mathematics;

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
            
            if (n == "Totals")
            {
                LatestVal = MathSupport.Truncate(GlobalData.Finances.AllSecuritiesValue(DateTime.Today),100);
                Profit = MathSupport.Truncate(GlobalData.Finances.TotalProfit());
                CAR3M = MathSupport.Truncate(100 * GlobalData.Finances.IRRPortfolio(DateTime.Today.AddMonths(-3), DateTime.Today)) ;
                CAR6M = MathSupport.Truncate(100 * GlobalData.Finances.IRRPortfolio(DateTime.Today.AddMonths(-6), DateTime.Today));
                CAR1Y = MathSupport.Truncate(100 * GlobalData.Finances.IRRPortfolio(DateTime.Today.AddMonths(-12), DateTime.Today));
                CAR5Y = MathSupport.Truncate(100 * GlobalData.Finances.IRRPortfolio(DateTime.Today.AddMonths(-60), DateTime.Today));
                CARTotal = MathSupport.Truncate(100 * GlobalData.Finances.IRRPortfolio(GlobalData.Finances.FirstValueDate(), DateTime.Today));
            }
            else
            {
                LatestVal = MathSupport.Truncate(SecurityStatistics.SecurityLatestValue(n, c));
                Profit = MathSupport.Truncate(SecurityStatistics.Profit(n, c));
                CAR3M = Math.Truncate(100 * SecurityStatistics.SecurityIRRTime(n, c, DateTime.Today.AddMonths(-3), DateTime.Today));
                CAR6M = Math.Truncate(100 * SecurityStatistics.SecurityIRRTime(n, c, DateTime.Today.AddMonths(-6), DateTime.Today));
                CAR1Y = Math.Truncate(100 * SecurityStatistics.SecurityIRRTime(n, c, DateTime.Today.AddMonths(-12), DateTime.Today));
                CAR5Y = Math.Truncate(100 * SecurityStatistics.SecurityIRRTime(n, c, DateTime.Today.AddMonths(-60), DateTime.Today));
                CARTotal = Math.Truncate(100 * SecurityStatistics.SecurityIRR(n, c));
            }
        }
        public string Name { get; set; }
        public string Company { get; set; }
        public double LatestVal { get; set; }

        public double Profit { get; set; }
        public double CAR3M { get; set; }
        public double CAR6M { get; set; }
        public double CAR1Y { get; set; }
        public double CAR5Y { get; set; }
        public double CARTotal { get; set; }
    }
}
