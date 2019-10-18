using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancePortfolioDatabase
{
    public partial class Portfolio
    {
        public bool DoesAccountExist(CashAccount Account)
        {
            foreach (CashAccount acc in BankAccounts)
            {
                if (acc.IsEqualTo(Account))
                {
                    return true;
                }
            }

            return false;
        }

        public bool DoesBankAccountExistFromName(string name, string company)
        {
            foreach (CashAccount acc in BankAccounts)
            {
                if (acc.GetName() == name && acc.GetCompany() == company)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Outputs a copy of the BankAccount if it exists.
        /// </summary>
        public bool TryGetBankAccount(string name, string company, out CashAccount desired)
        {
            foreach (CashAccount sec in BankAccounts)
            {
                if (sec.GetName() == name && sec.GetCompany() == company)
                {
                    desired = sec.Copy();
                    return true;
                }
            }

            desired = null;
            return false;
        }

        public bool TryAddBankAccount(CashAccount NewAccount)
        {
            if (DoesAccountExist(NewAccount))
            {
                return false;
            }

            BankAccounts.Add(NewAccount);
            return true;
        }

        public bool TryAddBankAccountFromName(string name, string company)
        {
            if (DoesBankAccountExistFromName(name, company))
            {
                return false;
            }

            var NewAccount = new CashAccount(name, company);
            BankAccounts.Add(NewAccount);
            return true;
        }

        public bool TryRemoveBankAccount(string name, string company)
        {
            foreach (CashAccount acc in BankAccounts)
            {
                if (acc.GetCompany() == company && acc.GetName() == name)
                {
                    BankAccounts.Remove(acc);
                    return true;
                }
            }

            return false;
        }

        public bool TryEditBankAccount(string name, string company, DateTime date, double value)
        {
            for (int AccountIndex = 0; AccountIndex < BankAccounts.Count; AccountIndex++)
            {
                if (BankAccounts[AccountIndex].GetCompany() == company && BankAccounts[AccountIndex].GetName() == name)
                {
                    // now edit data
                    return BankAccounts[AccountIndex].TryEditValue(date, value);
                }
            }

            return false;
        }

        public bool TryEditCashAcountNameCompany(string name, string company, string newName, string newCompany)
        {
            for (int AccountIndex = 0; AccountIndex < Funds.Count; AccountIndex++)
            {
                if (BankAccounts[AccountIndex].GetCompany() == company && BankAccounts[AccountIndex].GetName() == name)
                {
                    // now edit data
                    return BankAccounts[AccountIndex].TryEditNameCompany(newName, newCompany);
                }
            }

            return false;
        }
    }
}
