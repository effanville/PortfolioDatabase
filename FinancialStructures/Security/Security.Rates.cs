using FinancialStructures.DataStructures;
using FinancialStructures.FinanceFunctionsList;
using System;
using System.Collections.Generic;

namespace FinancialStructures.FinanceStructures
{
    public partial class Security
    {
        internal double TotalInvestment(Currency currency = null)
        {
            var investments = GetInvestmentsBetween(FirstValue().Day, LatestValue().Day, currency);
            double sum = 0;
            foreach (var investment in investments)
            {
                sum += investment.Value;
            }
            return sum;
        }

        /// <summary>
        /// The date and latest value of the security
        /// </summary>
        public DailyValuation LatestValue(Currency currency = null)
        {
            DailyValuation latestDate = fUnitPrice.GetLatestValuation();
            if (latestDate == null)
            {
                return new DailyValuation(DateTime.Today, 0.0); ;
            }

            double currencyValue = currency == null ? 1.0 : currency.GetNearestEarlierValuation(latestDate.Day).Value;
            double latestValue = latestDate.Value * fShares.GetLatestValue() * currencyValue;

            return new DailyValuation(latestDate.Day, latestValue);
        }

        /// <summary>
        /// The date and first value of the security
        /// </summary>
        public DailyValuation FirstValue(Currency currency = null)
        {
            DailyValuation firstDate = fUnitPrice.GetFirstValuation();
            if (firstDate == null)
            {
                return new DailyValuation(DateTime.MinValue, 0.0); ;
            }
            double currencyValue = currency == null ? 1.0 : currency.GetNearestEarlierValuation(firstDate.Day).Value;
            double latestValue = firstDate.Value * fShares.GetFirstValue() *currencyValue;

            return new DailyValuation(firstDate.Day, latestValue);
        }

        /// <summary>
        /// Returns most recent valuation on or before the date specified. 
        /// </summary>
        internal DailyValuation GetLastEarlierValuation(DateTime date, Currency currency = null)
        {
            DailyValuation val = fUnitPrice.GetLastEarlierValue(date);
            if (val == null)
            {
                return new DailyValuation(date, 0.0);
            }

            double currencyValue = currency == null ? 1.0 : currency.GetNearestEarlierValuation(val.Day).Value;
            double latestValue = fShares.GetLastEarlierValue(date).Value * val.Value * currencyValue;
            return new DailyValuation(date, latestValue);
        }

        /// <summary>
        /// Returns most recent valuation on or before the date specified. 
        /// </summary>
        internal DailyValuation GetNearestEarlierValuation(DateTime date, Currency currency = null)
        {
            DailyValuation val = fUnitPrice.GetNearestEarlierValue(date);
            if (val == null)
            {
                return new DailyValuation(date, 0.0);
            }

            double currencyValue = currency == null ? 1.0 : currency.GetNearestEarlierValuation(val.Day).Value;
            double latestValue = fShares.GetNearestEarlierValue(date).Value * val.Value * currencyValue;
            return new DailyValuation(date, latestValue);
        }

        /// <summary>
        /// Returns earliest valuation after the date specified. 
        /// </summary>
        internal DailyValuation GetNearestLaterValuation(DateTime date, Currency currency = null)
        {
            DailyValuation val = fUnitPrice.GetNearestLaterValue(date);
            if (val == null)
            {
                return new DailyValuation(date, 0.0);
            }
            double currencyValue = currency == null ? 1.0 : currency.GetNearestEarlierValuation(val.Day).Value;
            double latestValue = fShares.GetNearestLaterValue(date).Value * val.Value * currencyValue;

            return new DailyValuation(date, latestValue);
        }

        /// <summary>
        /// Returns earliest valuation after the date specified. 
        /// </summary>
        internal List<DailyValuation> GetInvestmentsBetween(DateTime earlierDate, DateTime laterDate, Currency currency = null)
        {
            List<DailyValuation> values = fInvestments.GetValuesBetween(earlierDate, laterDate);
            foreach (var val in values)
            {
                double currencyValue = currency == null ? 1.0 : currency.GetNearestEarlierValuation(val.Day).Value;
                val.Value *= currencyValue;
            }

            return values;
        }

        /// <summary>
        /// returns a list of all investments with the name of the security.
        /// </summary>
        public List<DailyValuation_Named> GetAllInvestmentsNamed(Currency currency = null)
        {
            List<DailyValuation> values = fInvestments.GetValuesBetween(fInvestments.GetFirstDate(), fInvestments.GetLatestDate());
            List<DailyValuation_Named> namedValues = new List<DailyValuation_Named>();
            foreach (var val in values)
            {
                if (val != null && val.Value != 0)
                {
                    double currencyValue = currency == null ? 1.0 : currency.GetNearestEarlierValuation(val.Day).Value;
                    val.Value *= currencyValue;
                    namedValues.Add(new DailyValuation_Named(this.fName, this.fCompany, val));
                }
            }
            return namedValues;
        }

        /// <summary>
        /// returns compound annual rate of security between the two times specified
        /// </summary>
        internal double CAR(DateTime earlierTime, DateTime laterTime, Currency currency = null)
        {
            return FinancialFunctions.CAR(GetNearestEarlierValuation(earlierTime, currency), GetNearestEarlierValuation(laterTime, currency));
        }

        /// <summary>
        /// Internal rate of return of the investment over the past timelength months
        /// </summary>
        internal double IRRTime(DateTime earlierDate, DateTime laterDate, Currency currency = null)
        {
            if (Any())
            {
                var invs = GetInvestmentsBetween(earlierDate, laterDate, currency);
                var latestTime = GetNearestEarlierValuation(laterDate, currency);
                var firstTime = GetNearestEarlierValuation(earlierDate, currency);
                return FinancialFunctions.IRRTime(firstTime, invs, latestTime);
            }
            return double.NaN;
        }

        /// <summary>
        /// Internal rate of return of the investment over entire history
        /// </summary>
        internal double IRR(Currency currency = null)
        {
            return IRRTime(FirstValue().Day, LatestValue().Day, currency);
        }
    }
}
