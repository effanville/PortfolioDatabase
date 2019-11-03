using System;
using System.Collections.Generic;
using FinanceFunctionsList;
using DataStructures;

namespace FinanceStructures
{
    public partial class Security
    {
        /// <summary>
        /// The date and latest value of the security
        /// </summary>
        internal DailyValuation LatestValue()
        {
            DateTime latestDate = fUnitPrice.GetLatestDate();
            double latestValue = fUnitPrice.GetLatestValue() * fShares.GetLatestValue();

            return new DailyValuation(latestDate, latestValue);
        }

        /// <summary>
        /// The date and first value of the security
        /// </summary>
        internal DailyValuation FirstValue()
        {
            DateTime firstDate = fUnitPrice.GetFirstDate();
            double latestValue = fUnitPrice.GetFirstValue() * fShares.GetFirstValue();

            return new DailyValuation(firstDate, latestValue);
        }

        /// <summary>
        /// Returns most recent valuation before the date specified. 
        /// </summary>
        internal DailyValuation GetNearestEarlierValuation(DateTime date)
        {
            DailyValuation val = fUnitPrice.GetNearestEarlierValue(date);
            if (val == null)
            {
                return null;
            }

            double latestValue = fShares.GetNearestEarlierValue(date).Value * val.Value;
            return new DailyValuation(date, latestValue);
        }

        /// <summary>
        /// Returns earliest valuation after the date specified. 
        /// </summary>
        internal DailyValuation GetNearestLaterValuation(DateTime date)
        {
            DailyValuation val = fUnitPrice.GetNearestLaterValue(date);
            double latestValue = fShares.GetNearestLaterValue(date).Value * val.Value;

            return new DailyValuation(date, latestValue);
        }

        /// <summary>
        /// Returns earliest valuation after the date specified. 
        /// </summary>
        internal List<DailyValuation> GetInvestmentsBetween(DateTime earlierDate, DateTime laterDate)
        {
            List<DailyValuation> values = fInvestments.GetValuesBetween(earlierDate,laterDate);

            return values;
        }

        /// <summary>
        /// returns compound annual rate of security between the two times specified
        /// </summary>
        internal double CAR(DateTime earlierTime, DateTime laterTime)
        {
            return FinancialFunctions.CAR(GetNearestEarlierValuation(earlierTime), GetNearestEarlierValuation(laterTime));
        }

        /// <summary>
        /// Internal rate of return of the investment over the past timelength months
        /// </summary>
        internal double IRRTime(DateTime earlierDate, DateTime laterDate)
        {
            if (Any())
            {
                var invs = GetInvestmentsBetween(earlierDate, laterDate);
                var latestTime = GetNearestEarlierValuation(laterDate);
                var firstTime = GetNearestEarlierValuation(earlierDate);
                return FinancialFunctions.IRRTime(invs, latestTime, firstTime);
            }
            return double.NaN;
        }

        /// <summary>
        /// Internal rate of return of the investment over entire history
        /// </summary>
        internal double IRR()
        {
            return IRRTime(FirstValue().Day, LatestValue().Day);
        }
    }
}
