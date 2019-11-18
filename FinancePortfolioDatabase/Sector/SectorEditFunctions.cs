
using GUIFinanceStructures;
using System;
using System.Collections.Generic;

namespace FinanceStructures
{
    /// <summary>
    /// General edit functions for a sector.
    /// </summary>
    public partial class Sector
    {
        public string GetName()
        {
            return fName;
        }

        internal List<AccountDayDataView> GetDataForDisplay()
        {
            var output = new List<AccountDayDataView>();
            if (fValues.Any())
            {
                foreach (var datevalue in fValues.GetValuesBetween(fValues.GetFirstDate(), fValues.GetLatestDate()))
                {
                    fValues.TryGetValue(datevalue.Day, out double UnitPrice);
                    var thisday = new AccountDayDataView(datevalue.Day, UnitPrice);
                    output.Add(thisday);
                }
            }

            return output;
        }

        internal bool TryEditName(string name)
        {
            if (name != fName)
            {
                fName = name;
            }

            return true;
        }

        internal bool DoesDataExist(DateTime date, out int index)
        {
            return fValues.ValueExists(date, out index);
        }

        internal bool TryAddData(DateTime date, double value)
        {
            if (DoesDataExist(date, out int _))
            {
                return false;
            }

            return fValues.TryAddValue(date, value);
        }

        internal bool TryEditData(DateTime date, double value)
        {
            if (DoesDataExist(date, out int i))
            {
                return fValues.TryEditData(date, value);
            }

            return false;
        }

        internal bool TryDeleteData(DateTime date, double value)
        {
            if (value > 0)
            {
                return fValues.TryDeleteValue(date);
            }
            return false;
        }
    }
}
