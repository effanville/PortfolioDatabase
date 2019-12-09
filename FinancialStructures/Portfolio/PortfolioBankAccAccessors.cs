using System;
using System.Collections.Generic;
using FinancialStructures.GUIFinanceStructures;
using FinancialStructures.ReportingStructures;

namespace FinancialStructures.FinanceStructures
{
    public partial class Portfolio
    {
        public List<string> GetBankAccountNames()
        {
            var names = new List<string>();
            foreach (var bankAcc in BankAccounts)
            {
                names.Add(bankAcc.GetName());
            }

            return names;
        }

        public List<BankAccountStatsHolder> GenerateBankAccountStatistics()
        {
            var names = new List<BankAccountStatsHolder>();
            foreach (var bankAcc in BankAccounts)
            {
                names.Add(new BankAccountStatsHolder(bankAcc.GetName(), bankAcc.GetCompany(), bankAcc.LatestValue().Value));
            }

            return names;
        }

        public List<string> GetBankAccountCompanyNames()
        {
            var companies = new List<string>();
            foreach (var bankAcc in BankAccounts)
            {
                if (companies.IndexOf(bankAcc.GetCompany()) == -1)
                {
                    companies.Add(bankAcc.GetCompany());
                }
            }
            companies.Sort();

            return companies;
        }

        public List<NameComp> GetBankAccountNamesAndCompanies()
        {
            var namesAndCompanies = new List<NameComp>();

            foreach (var bankAcc in BankAccounts)
            {
                namesAndCompanies.Add(new NameComp(bankAcc.GetName(), bankAcc.GetCompany(), false));
            }

            return namesAndCompanies;
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

        public bool TryGetAccountData(string name, string company, out List<AccountDayDataView> data)
        {
            data = new List<AccountDayDataView>();
            foreach (CashAccount acc in BankAccounts)
            {
                if (acc.GetName() == name && acc.GetCompany() == company)
                {
                    data = acc.GetDataForDisplay();
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

        public bool TryAddBankAccountFromName(string name, string company, ErrorReports reports)
        {
            if (name == null || company == null)
            {
                reports.AddError("Name or Company provided were null.");
                return false;
            }
            if (DoesBankAccountExistFromName(name, company))
            {
                return false;
            }

            var NewAccount = new CashAccount(name, company);
            BankAccounts.Add(NewAccount);
            return true;
        }

        public bool TryRemoveBankAccount(string name, string company, ErrorReports reports)
        {
            foreach (CashAccount acc in BankAccounts)
            {
                if (acc.GetCompany() == company && acc.GetName() == name)
                {
                    BankAccounts.Remove(acc);
                    reports.AddWarning($"Deleting Bank Account: Deleted `{company}'-`{name}'.");
                    return true;
                }
            }
            reports.AddError($"Deleting Bank Account: Could not find account `{company}'-`{name}'.");
            return false;
        }

        public bool TryAddDataToBankAccount(string name, string company, DateTime date, double value)
        {
            for (int accountIndex = 0; accountIndex < BankAccounts.Count; accountIndex++)
            {
                if (BankAccounts[accountIndex].GetCompany() == company && BankAccounts[accountIndex].GetName() == name)
                {
                    // now edit data
                    return BankAccounts[accountIndex].TryAddValue(date, value);
                }
            }

            return false;
        }

        public bool TryEditBankAccount(string name, string company, DateTime date, double value, ErrorReports reports)
        {
            for (int AccountIndex = 0; AccountIndex < BankAccounts.Count; AccountIndex++)
            {
                if (BankAccounts[AccountIndex].GetCompany() == company && BankAccounts[AccountIndex].GetName() == name)
                {
                    // now edit data
                    return BankAccounts[AccountIndex].TryEditValue(date, value, reports);
                }
            }

            reports.AddError($"Editing BankAccount Data: Could not find bank account `{company}'-`{name}'.");
            return false;
        }

        public bool TryEditCashAcountNameCompany(string name, string company, string newName, string newCompany, ErrorReports reports)
        {
            for (int AccountIndex = 0; AccountIndex < Funds.Count; AccountIndex++)
            {
                if (BankAccounts[AccountIndex].GetCompany() == company && BankAccounts[AccountIndex].GetName() == name)
                {
                    // now edit data
                    return BankAccounts[AccountIndex].EditNameCompany(newName, newCompany);
                }
            }
            reports.AddError($"Renaming BankAccount: Could not find bank account `{company}'-`{name}'.");
            return false;
        }

        public bool TryDeleteBankAccountData(string name, string company, DateTime date, ErrorReports reports)
        {
            for (int AccountIndex = 0; AccountIndex < BankAccounts.Count; AccountIndex++)
            {
                if (BankAccounts[AccountIndex].GetCompany() == company && BankAccounts[AccountIndex].GetName() == name)
                {
                    // now edit data
                    return BankAccounts[AccountIndex].TryDeleteData(date, reports);
                }
            }

            reports.AddError($"Deleting Bank Account Data: Could not find bank account `{company}'-`{name}'.");
            return false;
        }
    }
}
