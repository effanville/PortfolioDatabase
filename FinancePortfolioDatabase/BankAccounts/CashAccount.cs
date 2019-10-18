using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancePortfolioDatabase
{
    public partial class CashAccount
    {
        private string fName;

        private string fCompany;

        private TimeList Amounts;

        public CashAccount(string name, string company)
        {
            fName =name;
            fCompany =company;
            Amounts = new TimeList();
        }

        private CashAccount(string name, string company, TimeList amounts)
        {
            fName = name;
            fCompany = company;
            Amounts = amounts;
        }
    }
}
