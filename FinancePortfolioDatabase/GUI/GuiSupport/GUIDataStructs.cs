using System;
using SecurityStatisticsFunctions;
using BankAccountStatisticsFunctions;

namespace GUIFinanceStructures
{
    public class NameComp
    {
        public NameComp()
        { }
        public NameComp(string n, string c)
        {
            Name = n;
            Company = c;
        }
        public string Name { get; set; }
        public string Company { get; set; }
    }

    public class BasicDayDataView
    {
        public BasicDayDataView()
        { }

        public BasicDayDataView(DateTime date, double unitPrice, double shareNo, double investment)
        {
            Date = date;
            UnitPrice = unitPrice;
            ShareNo = shareNo;
            Investment = investment;
        }

        public DateTime Date { get; set; }
        public double UnitPrice { get; set; }

        public double ShareNo { get; set; }

        public double Investment { get; set; }
    }

    public class AccountDayDataView
    {
        public AccountDayDataView()
        { }

        public AccountDayDataView(DateTime date, double unitPrice)
        {
            Date = date;
            Amount = unitPrice;
        }

        public DateTime Date { get; set; }
        public double Amount { get; set; }
    }

    public class SecurityStatsHolder
    {
        public SecurityStatsHolder()
        { }
        public SecurityStatsHolder(string n, string c)
        {
            Name = n;
            Company = c;
            LatestVal = SecurityStatistics.SecurityLatestValue(n, c);
            CARTotal = 100*SecurityStatistics.SecurityIRR(n, c);
        }
        public string Name { get; set; }
        public string Company { get; set; }
        public double LatestVal { get; set; }

        public double CARTotal { get; set; }
    }

    public class BankAccountStatsHolder
    {
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
