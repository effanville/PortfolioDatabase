using System;
using FinanceFunctionsList;

namespace DataStructures
{
    /// <summary>
    /// Sorted list of values, with last value the most recent, and first the oldest.
    /// </summary>
    public partial class TimeList
    {
        /// <summary>
        /// returns the CAR of the timelist between the dates provided.
        /// </summary>
        internal double CAR(DateTime earlierTime, DateTime laterTime)
        {
            return FinancialFunctions.CAR(GetNearestEarlierValue(earlierTime), GetNearestEarlierValue(laterTime));
        }
        /// <summary>
        /// Returns internal rate of return of the values in the TimeList
        /// </summary>
        internal double IRR(DailyValuation latestValue)
        {
            // if have only one investment easy to return the CAR.
            if (Count() == 1)
            {
                return FinancialFunctions.CAR(latestValue, GetFirstValuation());
            }

            return FinancialFunctions.IRR(fValues, latestValue);
        }

        /// <summary>
        /// Returns the internal rate of return between <param name="latestValue"/> and <param name="startValue"/>
        /// </summary>
        internal double IRRTime(DailyValuation startValue, DailyValuation latestValue)
        {
            if (startValue == null || latestValue == null)
            {
                return double.NaN;
            }

            // if have only one investment easy to return the CAR.
            if (Count() == 1)
            {
                return FinancialFunctions.CAR(latestValue, startValue);
            }

            return FinancialFunctions.IRRTime(GetValuesBetween(startValue.Day, latestValue.Day), latestValue, startValue);
        }
    }
}
