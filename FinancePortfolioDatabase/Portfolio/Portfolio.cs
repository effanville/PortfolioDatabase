using System.Collections.Generic;
using GUIFinanceStructures;

namespace FinanceStructures
{
    public partial class Portfolio
    {

        private List<Security> fFunds;

        public List<Security> Funds
        {
            get { return fFunds; }
            private set { fFunds = value; }
        }

        private List<CashAccount> fBankAccounts;

        public List<CashAccount> BankAccounts
        {
            get { return fBankAccounts; }
            private set { fBankAccounts = value; }
        }

        public Portfolio()
        {
            fFunds = new List<Security>();
            fBankAccounts = new List<CashAccount>();
        }
    }
}
