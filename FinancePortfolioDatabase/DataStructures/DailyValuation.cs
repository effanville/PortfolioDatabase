using System;

namespace DataStructures
{
    /// <summary>
    /// Holds a date and a value to act as the value on that day.
    /// </summary>
    public class DailyValuation
    {
        /// <summary>
        /// The date for the valuation
        /// </summary>
        public DateTime Day { get; set; }

        /// <summary>
        /// The specific valuation
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// empty constructor.
        /// </summary>
        protected DailyValuation()
        {
        }

        /// <summary>
        /// Standard constructor.
        /// </summary>
        public DailyValuation(DateTime idealDate, double idealValue)
        {
            Day = idealDate;
            Value = idealValue;
        }
    }
}
