using FinancialStructures.DataStructures;
using FinancialStructures.FinanceStructures;
using System;
using System.Collections.Generic;

namespace FinancialStructures.Database
{
    public static partial class PortfolioSecurity
    {
        public static double SecurityLatestValue(this Portfolio portfolio, string name, string company)
        {
            if (!portfolio.TryGetSecurity(name, company, out Security desired) || !desired.Any())
            {
                return double.NaN;
            }
            var currencyName = desired.GetCurrency();
            var currency = portfolio.Currencies.Find(cur => cur.Name == currencyName);
            return desired.LatestValue(currency).Value;
        }

        public static double RecentChange(this Portfolio portfolio, string name, string company)
        {
            if (portfolio.TryGetSecurity(name, company, out Security desired))
            {
                if (desired.Any())
                {
                    var currencyName = desired.GetCurrency();
                    var currency = portfolio.Currencies.Find(cur => cur.Name == currencyName);
                    var needed = desired.LatestValue(currency);
                    return needed.Value - desired.LastEarlierValuation(needed.Day, currency).Value;
                }
            }

            return double.NaN;
        }

        public static double Profit(this Portfolio portfolio, string name, string company)
        {
            if (portfolio.TryGetSecurity(name, company, out Security desired))
            {
                if (desired.Any())
                {
                    var currencyName = desired.GetCurrency();
                    var currency = portfolio.Currencies.Find(cur => cur.Name == currencyName);
                    return desired.LatestValue(currency).Value - desired.TotalInvestment(currency);
                }
            }

            return double.NaN;
        }

        public static double FundsFraction(this Portfolio portfolio, string name, string company)
        {
            if (portfolio.TryGetSecurity(name, company, out Security desired))
            {
                if (desired.Any())
                {
                    var currencyName = desired.GetCurrency();
                    var currency = portfolio.Currencies.Find(cur => cur.Name == currencyName);
                    return desired.LatestValue(currency).Value / portfolio.AllSecuritiesValue(DateTime.Today);
                }
            }

            return double.NaN;
        }

        private static List<DailyValuation_Named> GetSecurityInvestments(this Portfolio portfolio, string name, string company)
        {
            if (portfolio.TryGetSecurity(name, company, out Security desired))
            {
                if (desired.Any())
                {
                    var currencyName = desired.GetCurrency();
                    var currency = portfolio.Currencies.Find(cur => cur.Name == currencyName);
                    return desired.AllInvestmentsNamed(currency);
                }
            }

            return null;
        }

        /// <summary>
        /// If possible, returns the CAR of the security specified.
        /// </summary>
        public static double CAR(this Portfolio portfolio, string name, string company, DateTime earlierTime, DateTime laterTime)
        {
            if (portfolio.TryGetSecurity(name, company, out Security desired))
            {
                if (desired.Any())
                {
                    var currencyName = desired.GetCurrency();
                    var currency = portfolio.Currencies.Find(cur => cur.Name == currencyName);
                    return desired.CAR(earlierTime, laterTime, currency);
                }
            }

            return double.NaN;
        }

        /// <summary>
        /// If possible, returns the IRR of the security specified.
        /// </summary>
        public static double IRR(this Portfolio portfolio, string name, string company)
        {
            if (portfolio.TryGetSecurity(name, company, out Security desired))
            {
                if (desired.Any())
                {
                    var currencyName = desired.GetCurrency();
                    var currency = portfolio.Currencies.Find(cur => cur.Name == currencyName);
                    return desired.IRR(currency);
                }
            }

            return double.NaN;
        }

        /// <summary>
        /// If possible, returns the IRR of the security specified over the time period.
        /// </summary>
        public static double IRR(this Portfolio portfolio, string name, string company, DateTime earlierTime, DateTime laterTime)
        {
            if (portfolio.TryGetSecurity(name, company, out Security desired))
            {
                if (desired.Any())
                {
                    var currencyName = desired.GetCurrency();
                    var currency = portfolio.Currencies.Find(cur => cur.Name == currencyName);
                    return desired.IRRTime(earlierTime, laterTime, currency);
                }
            }

            return double.NaN;
        }
    }
}
