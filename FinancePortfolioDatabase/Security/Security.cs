using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancePortfolioDatabase
{
    public partial class Security
    {
        private string fName;

        private string fCompany;

        private TimeList fShares;

        private TimeList fUnitPrice;

        private TimeList fInvestments;

        public Security()
        {
            fShares = new TimeList();
            fUnitPrice = new TimeList();
            fInvestments = new TimeList();
        }

        public Security(string name, string company)
        {
            fName = name;
            fCompany = company;
            fShares = new TimeList();
            fUnitPrice = new TimeList();
            fInvestments = new TimeList();
        }

        private Security(string name, string company, TimeList shares, TimeList prices, TimeList investments)
        {
            fName = name;
            fCompany = company;
            fShares = shares;
            fUnitPrice = prices;
            fInvestments = investments;
        }
    }
}
