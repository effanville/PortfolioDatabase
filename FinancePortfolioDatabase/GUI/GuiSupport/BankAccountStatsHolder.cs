using System;
using BankAccountStatisticsFunctions;

namespace GUIFinanceStructures
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
        public BankAccountStatsHolder(string n, string c)
        {
            Name = n;
            Company = c;
            LatestVal = BankAccountStatistics.BankAccountLatestValue(n, c);
        }
        public string Name { get; set; }
        public string Company { get; set; }
        public double LatestVal { get; set; }
    }
}
