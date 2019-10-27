using System;
using System.Collections.Generic;
using GUIFinanceStructures;

namespace FinanceStructures
{
    public partial class CashAccount
    {
        public bool TryAddValue(DateTime date, double value)
        {
            if (fAmounts.ValueExists(date, out int index))
            {
                return false;
            }

            return fAmounts.TryAddValue(date, value);
        }

        internal List<AccountDayDataView> GetDataForDisplay()
        {
            var output = new List<AccountDayDataView>();
            foreach (var datevalue in fAmounts.Values)
            {
                fAmounts.TryGetValue(datevalue.Day, out double UnitPrice);
                var thisday = new AccountDayDataView(datevalue.Day, UnitPrice);
                output.Add(thisday);
            }

            return output;
        }

        public bool TryEditValue(DateTime date, double value)
        {
            return fAmounts.TryEditData(date, value);
        }

        public bool TryEditNameCompany(string name, string company)
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

        public bool TryDeleteData(DateTime date)
        {
            return fAmounts.TryDeleteValue(date);
        }

        public bool IsEqualTo(CashAccount otherAccount)
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

        public CashAccount Copy()
        {
            return new CashAccount(fName, fCompany, fAmounts);
        }

        public string GetName()
        {
            return fName;
        }

        public string GetCompany()
        {
            return fCompany;
        }
    }
}
