﻿using FinancialStructures.DataStructures;
using FinancialStructures.DisplayStructures;
using FinancialStructures.FinanceFunctionsList;
using FinancialStructures.FinanceInterfaces;
using FinancialStructures.GUIFinanceStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinancialStructures.PortfolioAPI
{
    public static partial class PortfolioStatistics
    {
        public static int LongestName(this IPortfolio portfolio)
        {
            return portfolio.Names(AccountType.Security).Max().Length;
        }

        public static int LongestCompany(this IPortfolio portfolio)
        {
            var companies = portfolio.Companies(AccountType.Security);
            companies.Sort();
            return companies.Select(c => c.Length).Max();
        }

        /// <summary>
        /// Returns the earliest date held in the portfolio.
        /// </summary>
        public static DateTime FirstValueDate(this IPortfolio portfolio)
        {
            var output = DateTime.Today;
            foreach (var sec in portfolio.Funds)
            {
                if (sec.Any())
                {
                    var currencyName = sec.Currency;
                    var currency = portfolio.Currencies.Find(cur => cur.Name == currencyName);
                    var securityEarliest = sec.FirstValue(currency).Day;
                    if (securityEarliest < output)
                    {
                        output = securityEarliest;
                    }
                }
            }
            return output;
        }

        /// <summary>
        /// returns the total profit in the portfolio.
        /// </summary>
        public static double TotalProfit(this IPortfolio portfolio)
        {
            double total = 0;
            foreach (var sec in portfolio.Funds)
            {
                if (sec.Any())
                {
                    var currencyName = sec.Currency;
                    var currency = portfolio.Currencies.Find(cur => cur.Name == currencyName);
                    total += portfolio.Profit(sec.Company, sec.Name);
                }
            }

            return total;
        }

        /// <summary>
        /// returns the total profit in the portfolio.
        /// </summary>
        public static double RecentChange(this IPortfolio portfolio)
        {
            double total = 0;
            foreach (var desired in portfolio.Funds)
            {
                if (desired.Any())
                {
                    total += portfolio.RecentChange(desired.Company, desired.Name);
                }
            }

            return total;
        }

        /// <summary>
        /// If possible, returns the IRR of all securities over the time period.
        /// </summary>
        public static double IRRPortfolio(this IPortfolio portfolio, DateTime earlierTime, DateTime laterTime)
        {
            if (portfolio.NumberOf(AccountType.Security) == 0)
            {
                return double.NaN;
            }
            double earlierValue = 0;
            double laterValue = 0;
            var Investments = new List<DailyValuation>();

            foreach (var security in portfolio.Funds)
            {
                if (security.Any())
                {
                    var currencyName = security.Currency;
                    var currency = portfolio.Currencies.Find(cur => cur.Name == currencyName);
                    earlierValue += security.Value(earlierTime, currency).Value;
                    laterValue += security.Value(laterTime, currency).Value;
                    Investments.AddRange(security.InvestmentsBetween(earlierTime, laterTime, currency));
                }
            }

            return FinancialFunctions.IRRTime(new DailyValuation(earlierTime, earlierValue), Investments, new DailyValuation(laterTime, laterValue));
        }

        /// <summary>
        /// The total value of all securities and bank accounts on the date specified
        /// </summary>
        public static double Value(this IPortfolio portfolio, DateTime date)
        {
            return portfolio.TotalValue(AccountType.Security, date) + portfolio.TotalValue(AccountType.BankAccount, date);
        }

        public static List<DatabaseStatistics> GenerateDatabaseStatistics(this IPortfolio portfolio)
        {
            var names = new List<DatabaseStatistics>();
            foreach (var sec in portfolio.Funds)
            {
                names.Add(new DatabaseStatistics(sec.Company, sec.Name, sec.FirstValue().Day, sec.LatestValue().Day, sec.Count(), (sec.LatestValue().Day - sec.FirstValue().Day).Days / (365 * (double)sec.Count())));
            }
            foreach (var bankAcc in portfolio.BankAccounts)
            {
                names.Add(new DatabaseStatistics(bankAcc.Name, bankAcc.Company, bankAcc.FirstValue().Day, bankAcc.LatestValue().Day, bankAcc.Count(), 365 * (double)bankAcc.Count() / (bankAcc.LatestValue().Day - bankAcc.FirstValue().Day).Days));
            }

            return names;
        }

        public async static Task<List<HistoryStatistic>> GenerateHistoryStats(this IPortfolio portfolio, int daysGap)
        {
            var outputs = new List<HistoryStatistic>();
            var calculationDate = portfolio.FirstValueDate();
            await Task.Run(() => BackGroundTask(calculationDate, portfolio, outputs, daysGap));
            return outputs;
        }

        private static void BackGroundTask(DateTime calculationDate, IPortfolio portfolio, List<HistoryStatistic> outputs, int daysGap)
        {
            while (calculationDate < DateTime.Today)
            {
                var calcuationDateStatistics = new HistoryStatistic(portfolio, calculationDate);
                outputs.Add(calcuationDateStatistics);
                calculationDate = calculationDate.AddDays(daysGap);
            }
            if (calculationDate == DateTime.Today)
            {
                var calcuationDateStatistics = new HistoryStatistic(portfolio, calculationDate);
                outputs.Add(calcuationDateStatistics);
            }
        }
    }
}
