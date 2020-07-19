﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinancialStructures.FinanceInterfaces;
using FinancialStructures.StatisticStructures;

namespace FinancialStructures.Database.Statistics
{
    public static partial class PortfolioStatisticGenerators
    {
        /// <summary>
        /// Returns the earliest date held in the portfolio.
        /// </summary>
        public static DateTime FirstValueDate(this IPortfolio portfolio)
        {
            DateTime output = DateTime.Today;
            foreach (ISecurity sec in portfolio.Funds)
            {
                if (sec.Any())
                {
                    ICurrency currency = portfolio.Currencies.Find(cur => cur.Name == sec.Currency);
                    DateTime securityEarliest = sec.FirstValue(currency).Day;
                    if (securityEarliest < output)
                    {
                        output = securityEarliest;
                    }
                }
            }
            return output;
        }

        public static List<DatabaseStatistics> GenerateDatabaseStatistics(this IPortfolio portfolio)
        {
            List<DatabaseStatistics> names = new List<DatabaseStatistics>();
            foreach (ISecurity sec in portfolio.Funds)
            {
                names.Add(new DatabaseStatistics(sec.Company, sec.Name, sec.FirstValue().Day, sec.LatestValue().Day, sec.Count(), (sec.LatestValue().Day - sec.FirstValue().Day).Days / (365 * (double)sec.Count())));
            }
            foreach (ICashAccount bankAcc in portfolio.BankAccounts)
            {
                names.Add(new DatabaseStatistics(bankAcc.Name, bankAcc.Company, bankAcc.FirstValue().Day, bankAcc.LatestValue().Day, bankAcc.Count(), 365 * (double)bankAcc.Count() / (bankAcc.LatestValue().Day - bankAcc.FirstValue().Day).Days));
            }

            return names;
        }

        public static async Task<List<PortfolioDaySnapshot>> GenerateHistoryStats(this IPortfolio portfolio, int daysGap)
        {
            List<PortfolioDaySnapshot> outputs = new List<PortfolioDaySnapshot>();
            if (!daysGap.Equals(0))
            {
                DateTime calculationDate = portfolio.FirstValueDate();
                await Task.Run(() => BackGroundTask(calculationDate, portfolio, outputs, daysGap));
            }
            return outputs;
        }

        private static void BackGroundTask(DateTime calculationDate, IPortfolio portfolio, List<PortfolioDaySnapshot> outputs, int daysGap)
        {
            while (calculationDate < DateTime.Today)
            {
                PortfolioDaySnapshot calcuationDateStatistics = new PortfolioDaySnapshot(calculationDate, portfolio);
                outputs.Add(calcuationDateStatistics);
                calculationDate = calculationDate.AddDays(daysGap);
            }
            if (calculationDate == DateTime.Today)
            {
                PortfolioDaySnapshot calcuationDateStatistics = new PortfolioDaySnapshot(calculationDate, portfolio);
                outputs.Add(calcuationDateStatistics);
            }
        }
    }
}
