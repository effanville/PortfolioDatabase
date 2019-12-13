using System;

namespace FinancialStructures.GUIFinanceStructures
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
