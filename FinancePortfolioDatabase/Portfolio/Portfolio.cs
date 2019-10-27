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
            fFunds = new List<Security>();
            fBankAccounts = new List<CashAccount>();
        }

        public List<string> GetSecurityNames()
        {
            var names = new List<string>();
            foreach (var security in Funds)
            {
                names.Add(security.GetName());
            }

            return names;
        }

        public List<NameComp > GetSecurityNamesAndCompanies()
        {
            var namesAndCompanies = new List<NameComp>();
            
            foreach (var security in Funds)
            {
                namesAndCompanies.Add(new NameComp(security.GetName(), security.GetCompany()));
            }

            return namesAndCompanies;
        }

        public List<string> GetBankAccountNames()
        {
            var names = new List<string>();
            foreach (var bankAcc in BankAccounts)
            {
                names.Add(bankAcc.GetName());
            }

            return names;
        }
        public List<NameComp> GetBankAccountNamesAndCompanies()
        {
            var namesAndCompanies = new List<NameComp>();

            foreach (var bankAcc in BankAccounts)
            {
                namesAndCompanies.Add(new NameComp(bankAcc.GetName(), bankAcc.GetCompany()));
            }

            return namesAndCompanies;
        }
    }
}
