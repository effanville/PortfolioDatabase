using FinancialStructures.DataStructures;
using FinancialStructures.FinanceFunctionsList;
using System;
using System.Collections.Generic;

namespace FinancialStructures.FinanceStructures
{
    public partial class Portfolio
    {
        public bool DoesCompanyExist(string company)
        {
            foreach (Security sec in Funds)
            {
                if (sec.GetCompany() == company)
                {
                    return true;
                }
            }

            return false;
        }

        public List<Security> CompanySecurities(string company)
        {
            var securities = new List<Security>();
            foreach (var sec in Funds)
            {
                if (sec.GetCompany() == company)
                {
                    securities.Add(sec);
                }
            }
            securities.Sort();
            return securities;
        }

        public List<DailyValuation_Named> GetCompanyInvestments(string company)
        {
            var output = new List<DailyValuation_Named>();
            foreach (var sec in CompanySecurities(company))
            {
                var currencyName = sec.GetCurrency();
                var currency = Currencies.Find(cur => cur.Name == currencyName);
                output.AddRange(sec.AllInvestmentsNamed(currency));
            }

            return output;
        }

        public double CompanyValue(string company, DateTime date)
        {
            var securities = CompanySecurities(company);
            double value = 0;
            foreach (var security in securities)
            {
                if (security.Any())
                {
                    var currencyName = security.GetCurrency();
                    var currency = Currencies.Find(cur => cur.Name == currencyName);
                    value += security.NearestEarlierValuation(date, currency).Value;
                }
            }

            return value;
        }

        public double CompanyProfit(string company)
        {
            var securities = CompanySecurities(company);
            double value = 0;
            foreach (var security in securities)
            {
                if (security.Any())
                {
                    var currencyName = security.GetCurrency();
                    var currency = Currencies.Find(cur => cur.Name == currencyName);
                    value += security.LatestValue(currency).Value - security.TotalInvestment(currency);
                }
            }

            return value;
        }

        public double FundsCompanyFraction(string company, DateTime date)
        {
            return CompanyValue(company, date) / AllSecuritiesValue(date);
        }

        /// <summary>
        /// If possible, returns the IRR of all securities in the company specified over the time period.
        /// </summary>
        public double IRRCompany(string company, DateTime earlierTime, DateTime laterTime)
        {
            var securities = CompanySecurities(company);
            if (securities.Count == 0)
            {
                return double.NaN;
            }
            double earlierValue = 0;
            double laterValue = 0;
            var Investments = new List<DailyValuation>();

            foreach (var security in securities)
            {
                if (security.Any())
                {
                    var currencyName = security.GetCurrency();
                    var currency = Currencies.Find(cur => cur.Name == currencyName);
                    earlierValue += security.NearestEarlierValuation(earlierTime, currency).Value;
                    laterValue += security.NearestEarlierValuation(laterTime, currency).Value;
                    Investments.AddRange(security.InvestmentsBetween(earlierTime, laterTime, currency));
                }
            }

            return FinancialFunctions.IRRTime(new DailyValuation(earlierTime, earlierValue), Investments, new DailyValuation(laterTime, laterValue));
        }
    }
}
