﻿using FinancialStructures.Database;
using FinancialStructures.NamingStructures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FinancialStructures.PortfolioAPI
{
    public static partial class PortfolioData
    {
        /// <summary>
        /// Returns a list of all companes of the desired type in the databse.
        /// </summary>
        /// <param name="portfolio">Database to query.</param>
        /// <param name="elementType">Type of object to search for.</param>
        /// <param name="reportLogger">Report callback. (not used)</param>
        /// <returns>List of names of the desired type.</returns>
        public static List<string> Companies(this Portfolio portfolio, AccountType elementType, Action<string, string, string> reportLogger)
        {
            return portfolio.NameData(elementType, reportLogger).Select(NameData => NameData.Company).Distinct().ToList();
        }

        /// <summary>
        /// Returns a list of all names of the desired type in the databse.
        /// </summary>
        /// <param name="portfolio">Database to query.</param>
        /// <param name="elementType">Type of object to search for.</param>
        /// <param name="reportLogger">Report callback. (not used)</param>
        /// <returns>List of names of the desired type.</returns>
        public static List<string> Names(this Portfolio portfolio, AccountType elementType, Action<string, string, string> reportLogger)
        {
            return portfolio.NameData(elementType, reportLogger).Select(NameData => NameData.Name).ToList();
        }

        /// <summary>
        /// Returns a list of all namedata in the databse.
        /// </summary>
        /// <param name="portfolio">Database to query.</param>
        /// <param name="elementType">Type of object to search for.</param>
        /// <param name="reportLogger">Report callback. (not used)</param>
        /// <returns>List of names of the desired type.</returns>
        public static List<NameCompDate> NameData(this Portfolio portfolio, AccountType elementType, Action<string, string, string> reportLogger)
        {
            var namesAndCompanies = new List<NameCompDate>();
            switch (elementType)
            {
                case (AccountType.Security):
                    {
                        foreach (var security in portfolio.Funds)
                        {
                            DateTime date = DateTime.MinValue;
                            if (security.Any())
                            {
                                date = security.LatestValue().Day;
                            }

                            namesAndCompanies.Add(new NameCompDate(security.GetCompany(), security.GetName(), security.GetCurrency(), security.GetUrl(), security.GetSectors(), date));
                        }
                        break;
                    }
                case (AccountType.Currency):
                    {
                        if (portfolio.Currencies != null)
                        {
                            foreach (var currency in portfolio.Currencies)
                            {
                                DateTime date = DateTime.MinValue;
                                if (currency.Any())
                                {
                                    date = currency.LatestValue().Day;
                                }

                                namesAndCompanies.Add(new NameCompDate(string.Empty, currency.GetName(), string.Empty, currency.GetUrl(), new List<string>(), date));
                            }
                        }
                        break;
                    }
                case (AccountType.BankAccount):
                    {
                        foreach (var bankAcc in portfolio.BankAccounts)
                        {
                            DateTime date = DateTime.MinValue;
                            if (bankAcc.Any())
                            {
                                date = bankAcc.LatestValue().Day;
                            }

                            namesAndCompanies.Add(new NameCompDate(bankAcc.GetCompany(), bankAcc.GetName(), bankAcc.GetCurrency(), string.Empty, bankAcc.GetSectors(), date));
                        }
                        break;
                    }
                case (AccountType.Sector):
                    {
                        foreach (var benchMark in portfolio.BenchMarks)
                        {
                            DateTime date = DateTime.MinValue;
                            if (benchMark.Any())
                            {
                                date = benchMark.LatestValue().Day;
                            }

                            namesAndCompanies.Add(new NameCompDate(benchMark.GetCompany(), benchMark.GetName(), string.Empty, string.Empty, new List<string>(), date));
                        }
                        break;
                    }
                default:
                    break;
            }

            return namesAndCompanies;
        }
    }
}
