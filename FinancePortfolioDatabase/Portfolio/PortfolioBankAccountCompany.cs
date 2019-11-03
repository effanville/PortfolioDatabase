using System;
using System.Collections.Generic;
using System.Linq;

namespace FinanceStructures
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
    }
}
