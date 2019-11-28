using FinancialStructures.DataStructures;
using FinancialStructures.FinanceStructures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FinancialStructures.FinanceStructures
{
    public partial class Portfolio
    {
        public List<CashAccount> CompanyBankAccounts(string company)
        {
            var accounts = new List<CashAccount>();
            foreach (var acc in BankAccounts)
            {
                if (acc.GetCompany() == company)
                {
                    accounts.Add(acc);
                }
            }
            accounts.Sort();
            return accounts;
        }

        public List<DailyValuation_Named> GenerateBankAccountStatistics(string company)
        {
            var namesAndCompanies = new List<DailyValuation_Named>();

            foreach (var account in BankAccounts)
            {
                if (account.GetCompany() == company)
                {
                    namesAndCompanies.Add(new DailyValuation_Named(account.GetName(), account.GetCompany(), account.LatestValue().Day, account.LatestValue().Value));
                }
            }

            namesAndCompanies.Sort();

            namesAndCompanies.Add(new DailyValuation_Named("Totals", company, DateTime.Today, BankAccountCompanyValue(company, DateTime.Today)));
            return namesAndCompanies;
        }

        public double BankAccountCompanyValue(string company, DateTime date)
        {
            var bankAccounts = CompanyBankAccounts(company);
            if (bankAccounts.Count() == 0)
            {
                return double.NaN;
            }
            double value = 0;
            foreach (var account in bankAccounts)
            {
                value += account.GetNearestEarlierValuation(date).Value;
            }

            return value;
        }
        public double BankAccountValue(DateTime date)
        {
            if (BankAccounts.Count() == 0)
            {
                return double.NaN;
            }
            double value = 0;
            foreach (var account in BankAccounts)
            {
                value += account.GetNearestEarlierValuation(date).Value;
            }

            return value;
        }
    }
}
