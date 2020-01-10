using FinancialStructures.DataStructures;
using FinancialStructures.FinanceFunctionsList;
using System;
using System.Collections.Generic;

namespace FinancialStructures.FinanceStructures
{
    public partial class Security
    {
        internal double TotalInvestment()
        {
            return fInvestments.Sum();
        }

        /// <summary>
        /// The date and latest value of the security
        /// </summary>
        public DailyValuation LatestValue()
        {
            DailyValuation latestDate = fUnitPrice.GetLatestValuation();
            if (latestDate == null)
            {
                return new DailyValuation(DateTime.Today, 0.0); ;
            }

            double latestValue = latestDate.Value * fShares.GetLatestValue();

            return new DailyValuation(latestDate.Day, latestValue);
        }

        /// <summary>
        /// The date and first value of the security
        /// </summary>
        public DailyValuation FirstValue()
        {
            DailyValuation firstDate = fUnitPrice.GetFirstValuation();
            if (firstDate == null)
            {
                return new DailyValuation(DateTime.MinValue, 0.0); ;
            }

            double latestValue = firstDate.Value * fShares.GetFirstValue();

            return new DailyValuation(firstDate.Day, latestValue);
        }

        /// <summary>
        /// Returns most recent valuation on or before the date specified. 
        /// </summary>
        internal DailyValuation GetLastEarlierValuation(DateTime date)
        {
            DailyValuation val = fUnitPrice.GetLastEarlierValue(date);
            if (val == null)
            {
                return new DailyValuation(date, 0.0);
            }

            double latestValue = fShares.GetLastEarlierValue(date).Value * val.Value;
            return new DailyValuation(date, latestValue);
        }

        /// <summary>
        /// Returns most recent valuation on or before the date specified. 
        /// </summary>
        internal DailyValuation GetNearestEarlierValuation(DateTime date)
        {
            DailyValuation val = fUnitPrice.GetNearestEarlierValue(date);
            if (val == null)
            {
                return new DailyValuation(date, 0.0);
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
            if (val == null)
            {
                return new DailyValuation(date, 0.0);
            }

            double latestValue = fShares.GetNearestLaterValue(date).Value * val.Value;

            return new DailyValuation(date, latestValue);
        }

        /// <summary>
        /// Returns earliest valuation after the date specified. 
        /// </summary>
        internal List<DailyValuation> GetInvestmentsBetween(DateTime earlierDate, DateTime laterDate)
        {
            List<DailyValuation> values = fInvestments.GetValuesBetween(earlierDate, laterDate);

            return values;
        }

        /// <summary>
        /// returns a list of all investments with the name of the security.
        /// </summary>
        public List<DailyValuation_Named> GetAllInvestmentsNamed()
        {
            List<DailyValuation> values = fInvestments.GetValuesBetween(fInvestments.GetFirstDate(), fInvestments.GetLatestDate());
            List<DailyValuation_Named> namedValues = new List<DailyValuation_Named>();
            foreach (var val in values)
            {
                if (val != null && val.Value != 0)
                {
                    namedValues.Add(new DailyValuation_Named(this.fName, this.fCompany, val));
                }
            }
            return namedValues;
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
                return FinancialFunctions.IRRTime(firstTime, invs, latestTime);
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
