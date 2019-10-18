using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancePortfolioDatabase
{
    /// <summary>
    /// Sorted list of values, with last value the most recent, and first the oldest.
    /// </summary>
    public partial class TimeList
    {
        private List<DailyValuation> fValues;

        public TimeList(List<DailyValuation> values)
        {
            fValues = values;
        }

        public TimeList()
        {
        }

        /// <summary>
        /// Adds value to the data.
        /// </summary>
        public void AddData(DateTime date, double value)
        {
            var valuation = new DailyValuation(date, value);
            fValues.Add(valuation);
            Sort();
        }

        private void Sort()
        {
            if (fValues.Count() > 1)
            {
                fValues = fValues.OrderBy(x => x.Day).ToList();
            }
        }

        public bool ValueExists(DateTime date, out int index)
        {
            for (int i = 0; i < fValues.Count; i++)
            {
                if (fValues[i].Day == date)
                {
                    index = i;
                    return true;
                }
            }

            index = -1;
            return false;
        }

        /// <summary>
        /// Adds value to the data only if value of the date doesn't currently exist.
        /// </summary>
        public bool TryAddValue(DateTime date, double value)
        {
            for (int i = 0; i < fValues.Count; i++)
            {
                if (fValues[i].Day == date)
                {
                    return false;
                }
            }

            var valuation = new DailyValuation(date, value);
            fValues.Add(valuation);
            Sort();

            return false;
        }

        public bool TryEditData(DateTime date, double value)
        {
            for (int i = 0; i < fValues.Count; i++)
            {
                if (fValues[i].Day == date)
                {
                    fValues[i].Value = value;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// obtains first instance of the value for the date requested. Returns false if no data.
        /// </summary>
        public bool TryGetValue(DateTime date, out double value)
        {
            value = 0;

            for (int i = 0; i < fValues.Count; i++)
            {
                if (fValues[i].Day == date)
                {
                    value = fValues[i].Value;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// returns nearest valuation in the timelist to the date provided.
        /// </summary>
        public DailyValuation GetNearestEarlierValue(DateTime date)
        {
            // empty so return null
            if (Count() == 0)
            {
                return null;
            }
            if (Count() == 1)
            {
                return fValues[0];
            }

            if (date > GetLatestDate())
            {
                return GetLatestValuation();
            }

            if (date < GetLatestDate())
            {
                return GetFirstValuation();
            }

            // list sorted with earliest at start. First occurence greater than value means 
            // the first value later.
            for (int i = 0; i < Count(); i++)
            {
                if (date < fValues[i].Day)
                {
                    return fValues[i - 1];
                }
            }

            // something has gone wrong.
            return null;
        }

        /// <summary>
        /// returns nearest valuation in the timelist to the date provided.
        /// </summary>
        public DailyValuation GetNearestLaterValue(DateTime date)
        {
            // empty so return null
            if (Count() == 0)
            {
                return null;
            }
            if (Count() == 1)
            {
                return fValues[0];
            }

            if (date > GetLatestDate())
            {
                return GetLatestValuation();
            }

            if (date < GetLatestDate())
            {
                return GetFirstValuation();
            }

            // list sorted with earliest at start. First occurence greater than value means 
            // the first value later.
            for (int i = 0; i < Count(); i++)
            {
                if (date < fValues[i].Day)
                {
                    return fValues[i];
                }
            }

            // something has gone wrong.
            return null;
        }

        /// <summary>
        /// returns nearest valuation in the timelist to the date provided.
        /// </summary>
        public DailyValuation GetNearestValue(DateTime date)
        {
            // empty so return null
            if (Count() == 0)
            {
                return null;
            }
            if (Count() == 1)
            {
                return fValues[0];
            }

            if (date > GetLatestDate())
            {
                return GetLatestValuation();
            }

            if (date < GetLatestDate())
            {
                return GetFirstValuation();
            }

            // list sorted with earliest at start. First occurence greater than value means 
            // the first value later.
            for (int i = 0; i < Count(); i++)
            {
                if (date < fValues[i].Day)
                {
                    if (fValues[i].Day - date < date - fValues[i - 1].Day)
                    {
                        return fValues[i];
                    }

                    return fValues[i - 1];
                }
            }

            // something has gone wrong.
            return null;
        }

        /// <summary>
        /// Returns the first date held in the vector
        /// </summary>
        public DateTime GetFirstDate()
        {
            if (fValues.Count() != 0)
            {
                return fValues[0].Day;
            }
            return new DateTime();
        }

        public double GetFirstValue()
        {
            if (fValues.Count() != 0)
            {
                return fValues[0].Value;
            }
            return -1;
        }

        public DailyValuation GetFirstValuation()
        {
            if (fValues.Count() != 0)
            {
                return fValues[0];
            }

            return null;
        }

        public DateTime GetLatestDate()
        {
            if (fValues.Count() != 0)
            {
                return fValues[fValues.Count() - 1].Day;
            }
            return new DateTime();
        }

        public double GetLatestValue()
        {
            if (fValues.Count() != 0)
            {
                return fValues[fValues.Count() - 1].Value;
            }
            return -1;
        }

        public DailyValuation GetLatestValuation()
        {
            if (fValues.Count() != 0)
            {
                return fValues[fValues.Count() - 1];
            }
            return null;
        }


        public int Count()
        {
            return fValues.Count;
        }

        /// <summary>
        /// returns all valuations between the two times specified.
        /// </summary>
        public List<DailyValuation> GetValuesBetween(DateTime earlierTime, DateTime laterTime)
        {
            var valuesBetween = new List<DailyValuation>();

            foreach (DailyValuation value in fValues)
            {
                if (value.Day > earlierTime && value.Day < laterTime)
                {
                    valuesBetween.Add(value);
                }
            }

            return valuesBetween;
        }
    }
}
