﻿using System.Collections.Generic;
using FinancialStructures.DataStructures;
using FinancialStructures.FinanceInterfaces;
using FinancialStructures.NamingStructures;

namespace FinancialStructures.Database.Statistics
{
    /// <summary>
    /// Class containing static extension methods for statistics for a portfolio.
    /// </summary>
    public static partial class Statistics
    {
        /// <summary>
        /// Returns a list of all the investments in the given type.
        /// </summary>
        /// <param name="portfolio"></param>
        /// <param name="accountType"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        public static List<DayValue_Named> TotalInvestments(this IPortfolio portfolio, Totals accountType, TwoName Name = null)
        {
            switch (accountType)
            {
                case Totals.Security:
                {
                    List<DayValue_Named> output = new List<DayValue_Named>();
                    List<string> companies = portfolio.Companies(Account.Security);
                    companies.Sort();
                    foreach (string comp in companies)
                    {
                        output.AddRange(portfolio.TotalInvestments(Totals.SecurityCompany, Name));
                    }
                    output.Sort();
                    return output;
                }
                case Totals.SecurityCompany:
                {
                    List<DayValue_Named> output = new List<DayValue_Named>();
                    foreach (ISecurity sec in portfolio.CompanySecurities(Name.Company))
                    {
                        ICurrency currency = portfolio.Currency(Account.Security, sec);
                        output.AddRange(sec.AllInvestmentsNamed(currency));
                    }

                    return output;
                }
                case Totals.SecuritySector:
                {
                    List<DayValue_Named> output = new List<DayValue_Named>();
                    foreach (ISecurity security in portfolio.SectorSecurities(Name.Name))
                    {
                        output.AddRange(security.AllInvestmentsNamed());
                    }

                    return output;
                }
                case Totals.All:
                {
                    return portfolio.TotalInvestments(Totals.Security);
                }
                default:
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Returns the investments in the object specified
        /// </summary>
        /// <param name="portfolio">The database to query.</param>
        /// <param name="accountType">The type of account to look for.</param>
        /// <param name="names">The name of the account.</param>
        /// <returns></returns>
        public static List<DayValue_Named> Investments(this IPortfolio portfolio, Account accountType, TwoName names)
        {
            switch (accountType)
            {
                case Account.Security:
                {
                    if (portfolio.TryGetSecurity(names, out ISecurity desired))
                    {
                        if (desired.Any())
                        {
                            ICurrency currency = portfolio.Currency(Account.Security, desired);
                            return desired.AllInvestmentsNamed(currency);
                        }
                    }

                    return null;
                }
                default:
                {
                    return null;
                }
            }
        }
    }
}