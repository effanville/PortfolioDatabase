﻿using System;
using FinancialStructures.FinanceInterfaces;

namespace FinancialStructures.Database.Statistics
{
    public static partial class PortfolioStatisticGenerators
    {
        /// <summary>
        /// Returns the latest date held in the portfolio.
        /// </summary>
        /// <param name="portfolio">The database to query</param>
        /// <param name="elementType">The type of element to search for. All searches for Bank accounts and securities.</param>
        /// <param name="name">An ancillary name to use in the case of Sectors</param>
        /// <returns></returns>
        public static DateTime LatestDate(this IPortfolio portfolio, Totals elementType, string name = null)
        {
            DateTime output = DateTime.Today;
            switch (elementType)
            {
                case Totals.Security:
                {
                    foreach (ISecurity sec in portfolio.Funds)
                    {
                        if (sec.Any())
                        {
                            DateTime securityEarliest = sec.LatestValue().Day;
                            if (securityEarliest > output)
                            {
                                output = securityEarliest;
                            }
                        }
                    }
                    break;
                }
                case Totals.SecurityCompany:
                {
                    foreach (ISecurity sec in portfolio.CompanySecurities(name))
                    {
                        if (sec.LatestValue().Day > output)
                        {
                            output = sec.LatestValue().Day;
                        }
                    }

                    return output;
                }
                case Totals.Sector:
                {
                    foreach (ISecurity sec in portfolio.SectorSecurities(name))
                    {
                        if (sec.LatestValue().Day > output)
                        {
                            output = sec.LatestValue().Day;
                        }
                    }
                    break;
                }
                case Totals.BankAccount:
                {
                    foreach (ICashAccount cashAccount in portfolio.BankAccounts)
                    {
                        if (cashAccount.LatestValue().Day > output)
                        {
                            output = cashAccount.LatestValue().Day;
                        }
                    }

                    break;
                }
                case Totals.Benchmark:
                {
                    foreach (ICashAccount cashAccount in portfolio.BenchMarks)
                    {
                        if (cashAccount.LatestValue().Day > output)
                        {
                            output = cashAccount.LatestValue().Day;
                        }
                    }

                    break;
                }
                case Totals.Currency:
                {
                    foreach (ICashAccount cashAccount in portfolio.Currencies)
                    {
                        if (cashAccount.LatestValue().Day > output)
                        {
                            output = cashAccount.LatestValue().Day;
                        }
                    }

                    break;
                }
                case Totals.All:
                {
                    var earlySecurity = portfolio.FirstValueDate(Totals.Security);
                    var earlyBank = portfolio.FirstValueDate(Totals.BankAccount);
                    output = earlySecurity > earlyBank ? earlySecurity : earlyBank;
                    break;
                }
            }

            return output;
        }
    }
}