﻿using System.Collections.Generic;
using FinancialStructures.FinanceStructures;
using FinancialStructures.NamingStructures;
using Common.Structure.DataStructures;

namespace FinancialStructures.Database.Statistics
{
    public static partial class Statistics
    {
        /// <summary>
        /// returns the total profit in the portfolio.
        /// </summary>
        public static double RecentChange(this IPortfolio portfolio, Totals elementType = Totals.Security, TwoName names = null)
        {
            switch (elementType)
            {
                case Totals.All:
                {
                    return portfolio.RecentChange(Totals.Security) + portfolio.RecentChange(Totals.BankAccount);
                }
                case Totals.Security:
                {
                    double total = 0;
                    foreach (ISecurity desired in portfolio.FundsThreadSafe)
                    {
                        if (desired.Any())
                        {
                            total += portfolio.RecentChange(elementType.ToAccount(), desired.Names);
                        }
                    }

                    return total;
                }
                case Totals.SecurityCompany:
                {
                    double total = 0;

                    IReadOnlyList<IValueList> securities = portfolio.CompanyAccounts(Account.Security, names?.Company);
                    if (securities.Count == 0)
                    {
                        return double.NaN;
                    }

                    foreach (ISecurity desired in securities)
                    {
                        if (desired.Any())
                        {
                            total += portfolio.RecentChange(elementType.ToAccount(), desired.Names);
                        }
                    }

                    return total;
                }
                case Totals.BankAccount:
                {
                    double total = 0.0;
                    foreach (ICashAccount cashAccount in portfolio.BankAccountsThreadSafe)
                    {
                        total += portfolio.RecentChange(elementType.ToAccount(), cashAccount.Names);
                    }

                    return total;
                }
                case Totals.SecuritySector:
                {
                    double total = 0;
                    foreach (ISecurity desired in portfolio.FundsThreadSafe)
                    {
                        if (desired.IsSectorLinked(names) && desired.Any())
                        {
                            total += portfolio.RecentChange(elementType.ToAccount(), desired.Names);
                        }
                    }

                    return total;
                }
                case Totals.BankAccountSector:
                {
                    double total = 0;
                    foreach (ICashAccount desired in portfolio.BankAccountsThreadSafe)
                    {
                        if (desired.IsSectorLinked(names) && desired.Any())
                        {
                            total += portfolio.RecentChange(elementType.ToAccount(), desired.Names);
                        }
                    }

                    return total;
                }
                case Totals.Sector:
                {
                    return portfolio.RecentChange(Totals.BankAccountSector, names) + portfolio.RecentChange(Totals.SecuritySector, names);
                }
                default:
                {
                    return 0.0;
                }
            }
        }

        /// <summary>
        /// returns the change between the most recent two valuations of the security.
        /// </summary>
        public static double RecentChange(this IPortfolio portfolio, Account elementType, TwoName names)
        {
            switch (elementType)
            {
                case Account.Security:
                {
                    if (portfolio.TryGetAccount(Account.Security, names, out IValueList security))
                    {
                        if (security.Any())
                        {
                            var desired = security as ISecurity;
                            ICurrency currency = portfolio.Currency(Account.Security, desired);
                            DailyValuation needed = desired.LatestValue(currency);
                            if (needed.Value > 0)
                            {
                                return needed.Value - desired.RecentPreviousValue(needed.Day, currency).Value;
                            }

                            return 0.0;
                        }
                    }

                    break;
                }
                case Account.BankAccount:
                {
                    if (portfolio.TryGetAccount(elementType, names, out IValueList desired))
                    {
                        if (desired.Any())
                        {
                            var cashAcc = (ICashAccount)desired;
                            ICurrency currency = portfolio.Currency(elementType, cashAcc);
                            DailyValuation needed = cashAcc.LatestValue(currency);
                            if (needed.Value > 0)
                            {
                                return needed.Value - cashAcc.RecentPreviousValue(needed.Day, currency).Value;
                            }

                            return 0.0;
                        }
                    }
                    break;
                }
                default:
                {
                    if (portfolio.TryGetAccount(elementType, names, out IValueList desired))
                    {
                        if (desired.Any())
                        {
                            DailyValuation needed = desired.LatestValue();
                            if (needed.Value > 0)
                            {
                                return needed.Value - desired.RecentPreviousValue(needed.Day).Value;
                            }

                            return 0.0;
                        }
                    }
                    break;
                }
            }

            return double.NaN;
        }
    }
}
