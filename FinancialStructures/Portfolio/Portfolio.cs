using System.Collections.Generic;

namespace FinancialStructures.FinanceStructures
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

        private List<Currency> fCurrencies;

        public List<Currency> Currencies
        {
            get { return fCurrencies; }
            private set { fCurrencies = value; }
        }

        public Portfolio()
        {
            fFunds = new List<Security>();
            fBankAccounts = new List<CashAccount>();
            fCurrencies = new List<Currency>();
        }
    }
}
