using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancePortfolioDatabase
{
    public partial class CashAccount
    {
        public bool TryAddValue(DateTime date, double value)
        {
            if (Amounts.ValueExists(date, out int index))
            {
                return false;
            }

            Amounts.AddData(date, value);

            return true;
        }

        public bool TryEditValue(DateTime date, double value)
        {
            return Amounts.TryEditData(date, value);
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
            return new CashAccount(fName, fCompany, Amounts);
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
