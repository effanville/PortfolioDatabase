using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancePortfolioDatabase
{
    public partial class Portfolio
    {

        private List<Security> fFunds;

        public List<Security> Funds
        {
            get { return fFunds; }
            set { fFunds = value; }
        }

        private List<CashAccount> fBankAccounts;

        public List<CashAccount> BankAccounts
        {
            get { return fBankAccounts; }
            set { fBankAccounts = value; }
        }

        public Portfolio()
        {
        }
    }
}
