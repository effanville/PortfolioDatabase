using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancePortfolioDatabase
{
    public partial class TimeList
    {
        /// <summary>
        /// returns the CAR of the timelist between the dates provided.
        /// </summary>
        public double CAR(DateTime earlierTime, DateTime laterTime)
        {
            return FinancialFunctions.CAR(GetNearestEarlierValue(earlierTime), GetNearestEarlierValue(laterTime));
        }
        /// <summary>
        /// Returns internal rate of return of the values in the TimeList
        /// </summary>
        public double IRR(DailyValuation latestValue)
        {
            // if have only one investment easy to return the CAR.
            if (Count() == 1)
            {
                return FinancialFunctions.CAR(latestValue, GetFirstValuation());
            }

            return FinancialFunctions.IRR(fValues, latestValue);
        }

        public double IRRTime(DailyValuation latestValue, DailyValuation startValue)
        {
            // if have only one investment easy to return the CAR.
            if (Count() == 1)
            {
                return FinancialFunctions.CAR(latestValue, startValue);
            }

            return FinancialFunctions.IRRTime(GetValuesBetween(startValue.Day, latestValue.Day), latestValue, startValue);
        }
    }
}
