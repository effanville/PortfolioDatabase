﻿using System;
using FinancialStructures.FinanceInterfaces;
using FinancialStructures.NamingStructures;

namespace FinancialStructures.Database.Statistics
{
    public static partial class Statistics
    {
        /// <summary>
        /// returns the fraction a held in the account has out of its company.
        /// </summary>
        [Obsolete("This function is currently broken. Requires fixing.")]
        public static double AccountInCompanyFraction(this IPortfolio portfolio, Totals elementType, TwoName names, DateTime date)
        {
            double companyFraction = portfolio.Fraction(elementType, names, date);
            if (companyFraction.Equals(0.0))
            {
                return 0.0;
            }
            return portfolio.Fraction(elementType, names, date) / companyFraction;
        }

        /// <summary>
        /// Returns the fraction of investments in the account out of the portfolio.
        /// </summary>
        public static double Fraction(this IPortfolio portfolio, Totals elementType, TwoName names, DateTime date)
        {
            switch (elementType)
            {
                case Totals.Security:
                {
                    if (portfolio.TryGetSecurity(names, out ISecurity desired))
                    {
                        if (desired.Any())
                        {
                            ICurrency currency = portfolio.Currency(Account.Security, desired);
                            return desired.Value(date, currency).Value / portfolio.TotalValue(Totals.Security, date);
                        }
                    }

                    return double.NaN;
                }
                case Totals.BankAccount:
                {
                    if (portfolio.TryGetAccount(Account.BankAccount, names, out ISingleValueDataList desired))
                    {
                        if (desired is ICashAccount cashAcc)
                        {
                            if (cashAcc.Any())
                            {
                                ICurrency currency = portfolio.Currency(Account.BankAccount, cashAcc);
                                return cashAcc.Value(date, currency).Value / portfolio.TotalValue(Totals.BankAccount, date);
                            }
                        }
                    }

                    return double.NaN;
                }
                case Totals.Sector:
                {
                    return portfolio.TotalValue(Totals.Sector, date, names) / portfolio.TotalValue(date);
                }
                case Totals.SecurityCompany:
                case Totals.BankAccountCompany:
                case Totals.Company:
                {
                    return portfolio.TotalValue(elementType, date, names) / portfolio.TotalValue(Totals.All, date);
                }
                default:
                {
                    return double.NaN;
                }
            }
        }
    }
}
