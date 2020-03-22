﻿using FinancialStructures.Database;
using FinancialStructures.FinanceStructures;
using FinancialStructures.NamingStructures;
using FinancialStructures.ReportLogging;

namespace FinancialStructures.PortfolioAPI
{
    public static class PortfolioDeleteMethods
    {
        /// <summary>
        /// Removes the account from the database if it can.
        /// </summary>
        /// <param name="portfolio">The database to query.</param>
        /// <param name="elementType">The type of account to remove.</param>
        /// <param name="name">The name of the account to remove.</param>
        /// <param name="reportLogger">A report callback.</param>
        /// <returns>Success or failure.</returns>
        public static bool TryRemove(this Portfolio portfolio, AccountType elementType, NameData name, LogReporter reportLogger)
        {
            if (string.IsNullOrEmpty(name.Name) && string.IsNullOrEmpty(name.Company))
            {
                reportLogger.LogDetailed("Critical", "Error", "RemovingData", $"Adding {elementType}: Company `{name.Company}' or name `{name.Name}' cannot both be empty.");
                return false;
            }

            switch (elementType)
            {
                case (AccountType.Security):
                    {
                        foreach (Security sec in portfolio.Funds)
                        {
                            if (sec.GetCompany().Equals(name.Company) && sec.GetName().Equals(name.Name))
                            {
                                portfolio.Funds.Remove(sec);
                                reportLogger.LogDetailed("Detailed", "Report", "DeletingData", $"Security `{name.Company}'-`{name}' removed from the database.");
                                return true;
                            }
                        }

                        break;
                    }
                case (AccountType.Currency):
                    {
                        foreach (var currency in portfolio.Currencies)
                        {
                            if (name.Name == currency.GetName())
                            {
                                reportLogger.LogDetailed("Detailed", "Report", "DeletingData", $"Deleted sector {currency.GetName()}");
                                portfolio.Currencies.Remove(currency);
                                return true;
                            }
                        }

                        break;
                    }
                case (AccountType.BankAccount):
                    {
                        foreach (CashAccount acc in portfolio.BankAccounts)
                        {
                            if (acc.GetCompany() == name.Company && acc.GetName() == name.Name)
                            {
                                portfolio.BankAccounts.Remove(acc);
                                reportLogger.LogDetailed("Detailed", "Report", "DeletingData", $"Deleting Bank Account: Deleted `{name.Company}'-`{name.Name}'.");
                                return true;
                            }
                        }

                        break;
                    }
                case (AccountType.Sector):
                    {
                        foreach (var sector in portfolio.BenchMarks)
                        {
                            if (name.Name.Equals(sector.GetName()))
                            {
                                reportLogger.LogDetailed("Detailed", "Report", "DeletingData", $"Deleted sector {sector.GetName()}");
                                portfolio.BenchMarks.Remove(sector);
                                return true;
                            }
                        }

                        break;
                    }
                default:
                    reportLogger.Log("Error", "DeletingData", $"Editing an Unknown type.");
                    return false;
            }

            reportLogger.Log("Error", "AddingData", $"{elementType.ToString()} - {name.ToString()} could not be found in the database.");
            return false;
        }
    }
}