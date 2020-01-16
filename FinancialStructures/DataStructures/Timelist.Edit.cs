using FinancialStructures.ReportingStructures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FinancialStructures.DataStructures
{
    public partial class TimeList
    {
        /// <summary>
        /// Adds value to the data.
        /// </summary>
        private void AddData(DateTime date, double value, ErrorReports reports)
        {
            var valuation = new DailyValuation(date, value);
            fValues.Add(valuation);
            reports.AddReport($"Added value {value}");
            Sort();
        }

        /// <summary>
        /// Orders the list according to date.
        /// </summary>
        private void Sort()
        {
            if (fValues != null && fValues.Any())
            {
                fValues = fValues.OrderBy(x => x.Day).ToList();
            }
        }

        /// <summary>
        /// Checks if value on <param name="date"/> exists. If exists then index is output.
        /// </summary>
        internal bool ValueExists(DateTime date, out int index)
        {
            if (fValues != null && fValues.Any())
            {
                for (int i = 0; i < fValues.Count; i++)
                {
                    if (fValues[i].Day == date)
                    {
                        index = i;
                        return true;
                    }
                }
            }

            index = -1;
            return false;
        }

        /// <summary>
        /// Adds value to the data only if value of the date doesn't currently exist.
        /// </summary>
        internal bool TryAddValue(DateTime date, double value)
        {
            for (int i = 0; i < fValues.Count; i++)
            {
                if (fValues[i].Day == date)
                {
                    return false;
                }
            }

            AddData(date, value, new ErrorReports());
            return true;
        }

        /// <summary>
        /// Edits data on <paramref name="date"/> and replaces existing value with <paramref name="value"/>.
        /// </summary>
        internal bool TryEditData(DateTime date, double value, ErrorReports reports)
        {
            if (fValues != null && fValues.Any())
            {
                for (int i = 0; i < fValues.Count; i++)
                {
                    if (fValues[i].Day == date)
                    {
                        reports.AddReport($"Editing Data: {date} value changed from {fValues[i].Value} to {value}");
                        fValues[i].Value = value;

                        return true;
                    }
                }
            }

            return false;
        }

        internal bool TryEditData(DateTime oldDate, DateTime newDate, double value, ErrorReports reports)
        {
            if (fValues != null && fValues.Any())
            {
                for (int i = 0; i < fValues.Count; i++)
                {
                    if (fValues[i].Day == oldDate)
                    {
                        reports.AddReport($"Editing Data: {oldDate} value changed from {fValues[i].Value} to {newDate} - {value}");
                        fValues[i].Day = newDate;
                        fValues[i].Value = value;

                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Edits the data on date specified. If data doesn't exist then adds the data.
        /// </summary>
        internal void TryEditDataOtherwiseAdd(DateTime oldDate, DateTime date, double value, ErrorReports reports)
        {
            if (!TryEditData(oldDate, date, value, reports))
            {
                AddData(date, value, reports);
            }
        }

        /// <summary>
        /// Deletes data if exists. If deletes, returns true.
        /// </summary>
        internal bool TryDeleteValue(DateTime date, ErrorReports reports)
        {
            if (fValues != null && fValues.Any())
            {
                for (int i = 0; i < fValues.Count; i++)
                {
                    if (fValues[i].Day == date)
                    {
                        reports.AddReport($"Deleted value: date - {date} and value - {fValues[i].Value}");
                        fValues.RemoveAt(i);
                        return true;
                    }
                }
            }

            reports.AddError($"Deleting Value: Could not find data on date {date}.");
            return false;
        }

        /// <summary>
        /// obtains first instance of the value for the date requested. Returns false if no data.
        /// </summary>
        internal bool TryGetValue(DateTime date, out double value)
        {
            value = 0;
            if (fValues != null && fValues.Any())
            {
                for (int i = 0; i < fValues.Count; i++)
                {
                    if (fValues[i].Day == date)
                    {
                        value = fValues[i].Value;
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Returns linearly interpolated value of the List on the date provided.
        /// </summary>
        internal DailyValuation Value(DateTime date)
        {
            if (fValues == null)
            {
                return new DailyValuation(DateTime.Today, 1.0);
            }
            if (Count() == 1)
            {
                return fValues[0];
            }
            if (date < FirstDate())
            {
                return FirstValuation();
            }
            if (date >= LatestDate())
            {
                return LatestValuation();
            }

            var earlier = NearestEarlierValue(date);
            var later = NearestLaterValue(date);

            double value =earlier.Value +(later.Value - earlier.Value)/(later.Day - earlier.Day).Days * (date - earlier.Day).Days;
            return new DailyValuation(date, value);
        }

        /// <summary>
        /// Returns the DailyValuation on or before the date specified.
        /// </summary>
        internal DailyValuation NearestEarlierValue(DateTime date)
        {
            if (fValues != null && fValues.Any())
            {
                if (date < FirstDate())
                {
                    return null;
                }

                if (Count() == 1)
                {
                    return fValues[0];
                }

                if (date > LatestDate())
                {
                    return LatestValuation();
                }

                // list sorted with earliest at start. First occurence greater than value means 
                // the first value later.
                for (int i = Count() - 1; i > -1; i--)
                {
                    if (date > fValues[i].Day)
                    {
                        return fValues[i];
                    }
                    if (date == fValues[i].Day)
                    {
                        return fValues[i];
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Returns DailyValuation closest to the date but earlier to it. 
        /// If a strictly earlier one cannot be found then return null.
        /// </summary>
        internal DailyValuation RecentPreviousValue(DateTime date)
        {
            if (fValues != null && fValues.Any())
            {
                // Some cases can return early.
                if (Count() == 1 || date <= FirstDate())
                {
                    return null;
                }

                if (date > LatestDate())
                {
                    return LatestValuation();
                }

                // go back in time until find a valuation that is after the date we want
                // Then the value we want is the previous in the vector.
                for (int i = Count() - 1; i > 0; i--)
                {
                    if (date == fValues[i].Day)
                    {
                        return fValues[i - 1];
                    }
                    if (date > fValues[i].Day)
                    {
                        return fValues[i];
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// returns nearest valuation in the timelist to the date provided.
        /// </summary>
        internal bool TryGetNearestEarlierValue(DateTime date, out DailyValuation value)
        {
            value = NearestEarlierValue(date);
            if (value == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// returns nearest valuation in the timelist to the date provided.
        /// </summary>
        internal DailyValuation NearestLaterValue(DateTime date)
        {
            if (fValues != null && fValues.Any())
            {
                if (Count() == 1)
                {
                    return fValues[0];
                }

                if (date > LatestDate())
                {
                    return null;
                }

                if (date < FirstDate())
                {
                    return FirstValuation();
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
            }

            return null;
        }

        /// <summary>
        /// returns nearest valuation in the timelist to the date provided.
        /// </summary>
        internal DailyValuation NearestValue(DateTime date)
        {
            if (fValues != null && fValues.Any())
            {
                if (Count() == 1)
                {
                    return fValues[0];
                }

                if (date > LatestDate())
                {
                    return LatestValuation();
                }

                if (date < FirstDate())
                {
                    return FirstValuation();
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
            }

            return null;
        }

        /// <summary>
        /// Returns the first date held in the vector, or default if cannot find any data
        /// </summary>
        internal DateTime FirstDate()
        {
            if (fValues != null && fValues.Any())
            {
                return fValues[0].Day;
            }

            return new DateTime();
        }

        /// <summary>
        /// Returns first value held, or 0 if no data.
        /// </summary>
        internal double FirstValue()
        {
            if (fValues != null && fValues.Any())
            {
                return fValues[0].Value;
            }
            return 0.0;
        }

        /// <summary>
        /// Returns first pair of date and value, or null if this doesn't exist.
        /// </summary>
        /// <returns></returns>
        internal DailyValuation FirstValuation()
        {
            if (fValues != null && fValues.Any())
            {
                return fValues[0];
            }

            return null;
        }

        /// <summary>
        /// Returns latest date held, or default if no data.
        /// </summary>
        internal DateTime LatestDate()
        {
            if (fValues != null && fValues.Any())
            {
                return fValues[fValues.Count() - 1].Day;
            }
            return new DateTime();
        }

        /// <summary>
        /// Returns latest value, or 0 if no data held.
        /// </summary>
        internal double LatestValue()
        {
            if (fValues != null && fValues.Any())
            {
                return fValues[fValues.Count() - 1].Value;
            }

            return 0.0;
        }

        /// <summary>
        /// Returns a pair of date and value of the most recently held data, or null if no data held.
        /// </summary>
        internal DailyValuation LatestValuation()
        {
            if (fValues != null && fValues.Any())
            {
                return fValues[fValues.Count() - 1];
            }

            return null;
        }

        /// <summary>
        /// returns all valuations on or between the two dates specified, or empty list if none held.
        /// </summary>
        internal List<DailyValuation> GetValuesBetween(DateTime earlierTime, DateTime laterTime)
        {
            var valuesBetween = new List<DailyValuation>();

            foreach (DailyValuation value in fValues)
            {
                if (value.Day >= earlierTime && value.Day <= laterTime)
                {
                    valuesBetween.Add(value);
                }
            }

            return valuesBetween;
        }
    }
}
