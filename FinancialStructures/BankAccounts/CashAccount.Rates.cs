using System;
using FinancialStructures.DataStructures;

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
        public DailyValuation LatestValue()
        {
            DateTime latestDate = fAmounts.GetLatestDate();
            double latestValue = fAmounts.GetLatestValue();

            return new DailyValuation(latestDate, latestValue);
        }

        /// <summary>
        /// Returns the first valuation of the CashAccount.
        /// </summary>
        internal DailyValuation FirstValue()
        {
            DateTime firstDate = fAmounts.GetFirstDate();
            double latestValue = fAmounts.GetFirstValue();

            return new DailyValuation(firstDate, latestValue);
        }

        /// <summary>
        /// Returns the latest earlier valuation of the CashAccount to <paramref name="date"/>.
        /// </summary>
        internal DailyValuation GetNearestEarlierValuation(DateTime date)
        {
            return fAmounts.GetNearestEarlierValue(date);
        }

        /// <summary>
        /// Returns earliest valuation after the date specified. 
        /// </summary>
        internal DailyValuation GetNearestLaterValuation(DateTime date)
        {
            return fAmounts.GetNearestLaterValue(date);
        }
    }
}
