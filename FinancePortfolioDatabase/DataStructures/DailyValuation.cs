using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancePortfolioDatabase
{
    /// <summary>
    /// Holds a date and a value to act as the value on that day.
    /// </summary>
    public class DailyValuation
    {
        public DateTime Day { get; set; }

        public double Value { get; set; }

        public DailyValuation()
        {
        }

        public DailyValuation(DateTime idealDate, double idealValue)
        {
            Day = idealDate;
            Value = idealValue;
        }
    }
}
