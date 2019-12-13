using System;

namespace FinancialStructures.GUIFinanceStructures
{
    public class BankAccountStatsHolder : IComparable
    {
        public int CompareTo(object obj)
        {
            if (obj is BankAccountStatsHolder value)
            {
                if (Company == value.Company)
                {
                    return Name.CompareTo(value.Name);
                }

                return Company.CompareTo(value.Company);
            }

            return 0;
        }

        public BankAccountStatsHolder()
        { }
        public BankAccountStatsHolder(string n, string c, double latestValue)
        {
            Name = n;
            Company = c;
            LatestVal = latestValue;
        }
        public string Name { get; set; }
        public string Company { get; set; }
        public double LatestVal { get; set; }
    }
}
