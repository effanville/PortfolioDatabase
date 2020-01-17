using System;

namespace FinancialStructures.DataStructures
{
    /// <summary>
    /// Holds a date and a value to act as the value on that day.
    /// </summary>
    public class DailyValuation : IComparable
    {
        /// <summary>
        /// Method of comparison. Compares dates.
        /// </summary>
        public int CompareTo(object obj)
        {
            DailyValuation a = (DailyValuation)obj;
            return DateTime.Compare(Day, a.Day);
        }

        /// <summary>
        /// Returns a copy of the specified valuation
        /// </summary>
        /// <returns></returns>
        public DailyValuation Copy()
        {
            return new DailyValuation(Day, Value);
        }

        public void SetData(DateTime date, double value)
        {
            Day = date;
            Value = value;
        }

        public void SetDay(DateTime date)
        {
            Day = date;
        }

        public void SetValue(double value)
        {
            Value = value;
        }

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

        public DailyValuation(DailyValuation dailyValue)
            : this(dailyValue.Day, dailyValue.Value)
        { }
    }
}
