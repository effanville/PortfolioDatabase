using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FinancePortfolioDatabase
{
    public partial class Security
    {
        /// <summary>
        /// The date and latest value of the security
        /// </summary>
        public DailyValuation LatestValue()
        {
            DateTime latestDate = fUnitPrice.GetLatestDate();
            double latestValue = fUnitPrice.GetLatestValue() * fUnitPrice.GetLatestValue();

            return new DailyValuation(latestDate, latestValue);
        }

        /// <summary>
        /// The date and first value of the security
        /// </summary>
        public DailyValuation FirstValue()
        {
            DateTime firstDate = fUnitPrice.GetFirstDate();
            double latestValue = fUnitPrice.GetFirstValue() * fUnitPrice.GetFirstValue();

            return new DailyValuation(firstDate, latestValue);
        }

        /// <summary>
        /// Returns most recent valuation before the date specified. 
        /// </summary>
        public DailyValuation GetNearestEarlierValuation(DateTime date)
        {
            DailyValuation val = fUnitPrice.GetNearestEarlierValue(date);
            double latestValue = fShares.GetNearestEarlierValue(date).Value * val.Value;

            return new DailyValuation(date, latestValue);
        }

        /// <summary>
        /// Returns earliest valuation after the date specified. 
        /// </summary>
        public DailyValuation GetNearestLaterValuation(DateTime date)
        {
            DailyValuation val = fUnitPrice.GetNearestLaterValue(date);
            double latestValue = fShares.GetNearestLaterValue(date).Value * val.Value;

            return new DailyValuation(date, latestValue);
        }

        /// <summary>
        /// Returns earliest valuation after the date specified. 
        /// </summary>
        public List<DailyValuation> GetInvestmentsBetween(DateTime earlierDate, DateTime laterDate)
        {
            List<DailyValuation> values = fInvestments.GetValuesBetween(earlierDate,laterDate);

            return values;
        }

        /// <summary>
        /// returns compound annual rate of security between the two times specified
        /// </summary>
        public double CAR(DateTime earlierTime, DateTime laterTime)
        {
            return FinancialFunctions.CAR(GetNearestEarlierValuation(earlierTime), GetNearestEarlierValuation(laterTime));
        }

        /// <summary>
        /// Internal rate of return of the investment over the past timelength months
        /// </summary>
        public double IRRTime(DateTime earlierDate, DateTime laterDate)
        {
            return FinancialFunctions.IRRTime(GetInvestmentsBetween(earlierDate, laterDate), GetNearestEarlierValuation(laterDate), GetNearestEarlierValuation(earlierDate));
        }

        /// <summary>
        /// Internal rate of return of the investment over entire history
        /// </summary>
        public double IRR()
        {
            return IRRTime(FirstValue().Day, LatestValue().Day);
        }
    }
}
