﻿using FinancialStructures.Database;
using FinancialStructures.FinanceStructures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FinancialStructures.PortfolioAPI
{
    public static partial class PortfolioValues
    {
        /// <summary>
        /// Calculates the value held in the company.
        /// </summary>
        /// <param name="portfolio">The database to query.</param>
        /// <param name="elementType">The type of account to find.</param>
        /// <param name="company">The company name to search for.</param>
        /// <param name="date">The date to calculate value on.</param>
        /// <returns>The value held in the company.</returns>
        public static double CompanyValue(this Portfolio portfolio, AccountType elementType, string company, DateTime date)
        {
            switch (elementType)
            {
                case (AccountType.Security):
                    {
                        var securities = portfolio.CompanySecurities(company);
                        double value = 0;
                        foreach (var security in securities)
                        {
                            if (security.Any())
                            {
                                var currency = Currency(portfolio, AccountType.Security, security);
                                value += security.Value(date, currency).Value;
                            }
                        }

                        return value;
                    }
                case (AccountType.Currency):
                    {
                        return 0.0;
                    }
                case (AccountType.BankAccount):
                    {
                        var bankAccounts = portfolio.CompanyBankAccounts(company);
                        if (bankAccounts.Count() == 0)
                        {
                            return double.NaN;
                        }
                        double value = 0;
                        foreach (var account in bankAccounts)
                        {
                            if (account != null && account.Any())
                            {
                                var currency = Currency(portfolio, AccountType.BankAccount, account);
                                value += account.NearestEarlierValuation(date, currency).Value;
                            }
                        }

                        return value;
                    }
                case (AccountType.Sector):
                    {
                        break;
                    }
                default:
                    break;
            }

            return 0.0;
        }

        private static List<CashAccount> CompanyBankAccounts(this Portfolio portfolio, string company)
        {
            var accounts = new List<CashAccount>();
            foreach (var acc in portfolio.GetBankAccounts())
            {
                if (acc.GetCompany() == company)
                {
                    accounts.Add(acc);
                }
            }

            accounts.Sort();
            return accounts;
        }
    }
}
