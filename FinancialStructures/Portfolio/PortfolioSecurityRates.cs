using FinancialStructures.DataStructures;
using System;
using System.Collections.Generic;

namespace FinancialStructures.FinanceStructures
{
    public partial class Portfolio
    {
        public double RecentChange(string name, string company)
        {
            if (TryGetSecurity(name, company, out Security desired))
            {
                if (desired.Any())
                {
                    var currencyName = desired.GetCurrency();
                    var currency = Currencies.Find(cur => cur.Name == currencyName);
                    var needed = desired.LatestValue(currency);
                    return needed.Value - desired.LastEarlierValuation(needed.Day, currency).Value;
                }
            }

            return double.NaN;
        }

        public double Profit(string name, string company)
        {
            if (TryGetSecurity(name, company, out Security desired))
            {
                if (desired.Any())
                {
                    var currencyName = desired.GetCurrency();
                    var currency = Currencies.Find(cur => cur.Name == currencyName);
                    return desired.LatestValue(currency).Value - desired.TotalInvestment(currency);
                }
            }

            return double.NaN;
        }

        public double FundsFraction(string name, string company)
        {
            if (TryGetSecurity(name, company, out Security desired))
            {
                if (desired.Any())
                {
                    var currencyName = desired.GetCurrency();
                    var currency = Currencies.Find(cur => cur.Name == currencyName);
                    return desired.LatestValue(currency).Value / AllSecuritiesValue(DateTime.Today);
                }
            }

            return double.NaN;
        }

        private List<DailyValuation_Named> GetSecurityInvestments(string name, string company)
        {
            if (TryGetSecurity(name, company, out Security desired))
            {
                if (desired.Any())
                {
                    var currencyName = desired.GetCurrency();
                    var currency = Currencies.Find(cur => cur.Name == currencyName);
                    return desired.AllInvestmentsNamed(currency);
                }
            }

            return null;
        }

        /// <summary>
        /// If possible, returns the CAR of the security specified.
        /// </summary>
        public double CAR(string name, string company, DateTime earlierTime, DateTime laterTime)
        {
            if (TryGetSecurity(name, company, out Security desired))
            {
                if (desired.Any())
                {
                    var currencyName = desired.GetCurrency();
                    var currency = Currencies.Find(cur => cur.Name == currencyName);
                    return desired.CAR(earlierTime, laterTime, currency);
                }
            }

            return double.NaN;
        }

        /// <summary>
        /// If possible, returns the IRR of the security specified.
        /// </summary>
        public double IRR(string name, string company)
        {
            if (TryGetSecurity(name, company, out Security desired))
            {
                if (desired.Any())
                {
                    var currencyName = desired.GetCurrency();
                    var currency = Currencies.Find(cur => cur.Name == currencyName);
                    return desired.IRR(currency);
                }
            }

            return double.NaN;
        }

        /// <summary>
        /// If possible, returns the IRR of the security specified over the time period.
        /// </summary>
        public double IRR(string name, string company, DateTime earlierTime, DateTime laterTime)
        {
            if (TryGetSecurity(name, company, out Security desired))
            {
                if (desired.Any())
                {
                    var currencyName = desired.GetCurrency();
                    var currency = Currencies.Find(cur => cur.Name == currencyName);
                    return desired.IRRTime(earlierTime, laterTime, currency);
                }
            }

            return double.NaN;
        }
    }
}
