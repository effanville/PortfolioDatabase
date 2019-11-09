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
            Value = UnitPrice * ShareNo;
            Investment = investment;
        }

        public DateTime Date { get; set; }
        public double UnitPrice { get; set; }

        public double ShareNo { get; set; }

        public double Value { get; set; }

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
            LatestVal = Math.Truncate(100*SecurityStatistics.SecurityLatestValue(n, c))/100;
            Profit = Math.Truncate(100*SecurityStatistics.Profit(n, c))/100;
            CAR3M = Math.Truncate(10000 * SecurityStatistics.SecurityIRRTime(n, c, DateTime.Today.AddMonths(-3), DateTime.Today))/100;
            CAR6M = Math.Truncate(10000 * SecurityStatistics.SecurityIRRTime(n, c, DateTime.Today.AddMonths(-6), DateTime.Today))/100;
            CAR1Y = Math.Truncate(10000 * SecurityStatistics.SecurityIRRTime(n, c, DateTime.Today.AddMonths(-12), DateTime.Today))/100;
            CAR5Y = Math.Truncate(10000 * SecurityStatistics.SecurityIRRTime(n, c, DateTime.Today.AddMonths(-60), DateTime.Today))/100;
            CARTotal = Math.Truncate(10000 * SecurityStatistics.SecurityIRR(n, c))/100;
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
