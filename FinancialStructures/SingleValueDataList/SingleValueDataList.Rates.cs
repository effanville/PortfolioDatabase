﻿using FinancialStructures.DataStructures;
using FinancialStructures.FinanceFunctionsList;
using System;

namespace FinancialStructures.FinanceStructures
{
    public partial class SingleValueDataList
    {
        /// <summary>
        /// Returns the latest valuation of the CashAccount.
        /// </summary>
        internal virtual DailyValuation LatestValue()
        {
            DateTime latestDate = fValues.LatestDate();
            double latestValue = fValues.LatestValue();

            return new DailyValuation(latestDate, latestValue);
        }

        /// <summary>
        /// Returns the first valuation of the CashAccount.
        /// </summary>
        internal DailyValuation FirstValue()
        {
            return fValues.FirstValuation();
        }

        /// <summary>
        /// Returns the latest earlier valuation of the CashAccount to <paramref name="date"/>.
        /// </summary>
        public DailyValuation Value(DateTime date)
        {
            return fValues.Value(date);
        }


        /// <summary>
        /// returns compound annual rate of security between the two times specified
        /// </summary>
        public double CAR(DateTime earlierTime, DateTime laterTime)
        {
            return FinancialFunctions.CAR(Value(earlierTime), Value(laterTime));
        }

        /// <summary>
        /// Returns the latest earlier valuation of the OldCashAccount to <paramref name="date"/>.
        /// </summary>
        internal virtual DailyValuation NearestEarlierValuation(DateTime date)
        {
            return fValues.NearestEarlierValue(date);
        }

        /// <summary>
        /// Returns earliest valuation after the date specified. 
        /// </summary>
        internal DailyValuation NearestLaterValuation(DateTime date)
        {
            return fValues.NearestLaterValue(date);
        }
    }
}