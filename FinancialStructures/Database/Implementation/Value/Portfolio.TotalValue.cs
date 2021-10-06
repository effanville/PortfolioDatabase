﻿using System;
using System.Collections.Generic;
using FinancialStructures.FinanceStructures;
using FinancialStructures.NamingStructures;

namespace FinancialStructures.Database.Implementation
{
    public partial class Portfolio
    {
        /// <inheritdoc/>
        public double TotalValue(Totals elementType, TwoName names = null)
        {
            return TotalValue(elementType, DateTime.Today, names);
        }

        /// <inheritdoc/>
        public double TotalValue(Totals elementType, DateTime date, TwoName names = null)
        {
            switch (elementType)
            {
                case Totals.Security:
                {
                    double total = 0;
                    foreach (ISecurity sec in FundsThreadSafe)
                    {
                        if (sec.Any())
                        {
                            ICurrency currency = Currency(Account.Security, sec);
                            total += sec.Value(date, currency).Value;
                        }
                    }

                    return total;
                }
                case Totals.Currency:
                {
                    double total = 0;
                    foreach (ISecurity sec in FundsThreadSafe)
                    {
                        if (sec.Any() && sec.Names.Currency == names.Name)
                        {
                            ICurrency currency = Currency(Account.Security, sec);
                            total += sec.Value(date, currency).Value;
                        }
                    }
                    foreach (IExchangableValueList acc in BankAccountsThreadSafe)
                    {
                        if (acc.Any() && acc.Names.Currency == names.Name)
                        {
                            ICurrency currency = Currency(Account.BankAccount, acc);
                            total += acc.ValuationOnOrBefore(date, currency).Value;
                        }
                    }
                    return total;
                }
                case Totals.BankAccount:
                {
                    double sum = 0;
                    foreach (IExchangableValueList acc in BankAccountsThreadSafe)
                    {
                        if (acc.Any())
                        {
                            ICurrency currency = Currency(Account.BankAccount, acc);
                            sum += acc.ValuationOnOrBefore(date, currency).Value;
                        }
                    }

                    return sum;
                }
                case Totals.SecuritySector:
                {
                    double sum = 0;
                    foreach (ISecurity fund in FundsThreadSafe)
                    {
                        if (fund.IsSectorLinked(names))
                        {
                            sum += fund.ValuationOnOrBefore(date).Value;
                        }
                    }

                    return sum;
                }
                case Totals.BankAccountSector:
                {
                    double sum = 0;

                    foreach (IExchangableValueList fund in BankAccountsThreadSafe)
                    {
                        if (fund.IsSectorLinked(names))
                        {
                            sum += fund.ValuationOnOrBefore(date).Value;
                        }
                    }

                    return sum;
                }
                case Totals.Sector:
                {
                    return TotalValue(Totals.SecuritySector, date, names) + TotalValue(Totals.BankAccountSector, date, names);
                }
                case Totals.All:
                {
                    return TotalValue(Totals.Security, date) + TotalValue(Totals.BankAccount, date);
                }
                case Totals.SecurityCompany:
                {
                    IReadOnlyList<IValueList> securities = CompanyAccounts(Account.Security, names.Company);
                    double value = 0;
                    foreach (ISecurity security in securities)
                    {
                        if (security.Any())
                        {
                            ICurrency currency = Currency(Account.Security, security);
                            value += security.Value(date, currency).Value;
                        }
                    }

                    return value;
                }
                case Totals.BankAccountCompany:
                {
                    IReadOnlyList<IValueList> bankAccounts = CompanyAccounts(Account.BankAccount, names.Company);
                    if (bankAccounts.Count == 0)
                    {
                        return double.NaN;
                    }
                    double value = 0;
                    foreach (IExchangableValueList account in bankAccounts)
                    {
                        if (account != null && account.Any())
                        {
                            ICurrency currency = Currency(Account.BankAccount, account);
                            value += account.ValuationOnOrBefore(date, currency).Value;
                        }
                    }

                    return value;
                }
                case Totals.Company:
                {
                    return TotalValue(Totals.BankAccount, date, names) + TotalValue(Totals.Security, date, names);
                }

                case Totals.CurrencySector:
                case Totals.SecurityCurrency:
                case Totals.BankAccountCurrency:
                case Totals.Benchmark:
                default:
                    break;
            }

            return 0.0;
        }
    }
}
