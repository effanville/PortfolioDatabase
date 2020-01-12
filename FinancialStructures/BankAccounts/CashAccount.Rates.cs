using FinancialStructures.DataStructures;
using System;

namespace FinancialStructures.FinanceStructures
{
    /// <summary>
    /// Provides rates and date evaluations to use with a CashAccount.
    /// </summary>
    public partial class CashAccount
    {
        /// <summary>
        /// Returns the latest valuation of the CashAccount.
        /// </summary>
        public DailyValuation LatestValue(Currency currency = null)
        {
            DailyValuation latestDate = fAmounts.GetLatestValuation();
            if (latestDate == null)
            {
                return new DailyValuation(DateTime.Today, 0.0); ;
            }

            double currencyValue = currency == null ? 1.0 : currency.GetNearestEarlierValuation(latestDate.Day).Value;
            double latestValue = latestDate.Value * currencyValue;

            return new DailyValuation(latestDate.Day, latestValue);
        }

        /// <summary>
        /// Returns the first valuation of the CashAccount.
        /// </summary>
        internal DailyValuation FirstValue(Currency currency = null)
        {
            DailyValuation firstDate = fAmounts.GetFirstValuation();
            if (firstDate == null)
            {
                return new DailyValuation(DateTime.Today, 0.0); ;
            }

            double currencyValue = currency == null ? 1.0 : currency.GetNearestEarlierValuation(firstDate.Day).Value;
            double latestValue = firstDate.Value * currencyValue;

            return new DailyValuation(firstDate.Day, latestValue);
        }

        /// <summary>
        /// Returns the latest earlier valuation of the CashAccount to <paramref name="date"/>.
        /// </summary>
        internal DailyValuation GetNearestEarlierValuation(DateTime date, Currency currency = null)
        {
            var value = fAmounts.GetNearestEarlierValue(date);
            double currencyValue = currency == null ? 1.0 : currency.GetNearestEarlierValuation(value.Day).Value;
            value.Value *= currencyValue;
            return value;
        }

        /// <summary>
        /// Returns earliest valuation after the date specified. 
        /// </summary>
        internal DailyValuation GetNearestLaterValuation(DateTime date, Currency currency = null)
        {
            var value = fAmounts.GetNearestLaterValue(date);
            double currencyValue = currency == null ? 1.0 : currency.GetNearestEarlierValuation(value.Day).Value;
            value.Value *= currencyValue;
            return value;
        }
    }
}
