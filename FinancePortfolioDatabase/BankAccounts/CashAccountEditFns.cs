using System;
using System.Collections.Generic;
using GUIFinanceStructures;

namespace FinanceStructures
{
    /// <summary>
    /// Here contains various editors for the CashAccount class.
    /// </summary>
    public partial class CashAccount
    {
        /// <summary>
        /// Adds <param name="value"> to amounts on <paramref name="date"/> if data doesnt exist.
        /// </summary>
        internal bool TryAddValue(DateTime date, double value)
        {
            if (fAmounts.ValueExists(date, out int _))
            {
                return false;
            }

            return fAmounts.TryAddValue(date, value);
        }

        /// <summary>
        /// Produces a list of data for GUI visualisation.
        /// </summary>
        internal List<AccountDayDataView> GetDataForDisplay()
        {
            var output = new List<AccountDayDataView>();
            foreach (var datevalue in fAmounts.GetValuesBetween(fAmounts.GetFirstDate(), fAmounts.GetLatestDate()))
            {
                fAmounts.TryGetValue(datevalue.Day, out double UnitPrice);
                var thisday = new AccountDayDataView(datevalue.Day, UnitPrice, false);
                output.Add(thisday);
            }

            return output;
        }

        /// <summary>
        /// Edits value if value exists. Does nothing if it doesn't exist.
        /// </summary>
        internal bool TryEditValue(DateTime date, double value)
        {
            return fAmounts.TryEditData(date, value);
        }

        /// <summary>
        /// Edits the name and company of the CashAccount.
        /// </summary>
        internal bool EditNameCompany(string name, string company)
        {
            if (fCompany != company)
            {
                fCompany = company;
            }
            if (fName != name)
            {
                fName = name;
            }

            return true;
        }

        /// <summary>
        /// Removes data on <paramref name="date"/> if it exists.
        /// </summary>
        internal bool TryDeleteData(DateTime date)
        {
            return fAmounts.TryDeleteValue(date);
        }

        /// <summary>
        /// Checks if another account with the same name and company exists. 
        /// </summary>
        internal bool IsEqualTo(CashAccount otherAccount)
        {
            if (otherAccount.GetName() != fName)
            {
                return false;
            }
            if (otherAccount.GetCompany() != fCompany)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Produces a copy of the specified CashAccount.
        /// </summary>
        internal CashAccount Copy()
        {
            return new CashAccount(fName, fCompany, fAmounts);
        }

        /// <summary>
        /// Returns the name of the CashAccount.
        /// </summary>
        internal string GetName()
        {
            return fName;
        }

        /// <summary>
        /// Returns the company of the CashAccount.
        /// </summary>
        internal string GetCompany()
        {
            return fCompany;
        }
    }
}
