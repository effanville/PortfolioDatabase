﻿using System;
using System.Globalization;
using FinancialStructures.Database;
using FinancialStructures.NamingStructures;

namespace FinancialStructures.Statistics
{
    internal class StatisticLatestValue : StatisticBase
    {
        internal StatisticLatestValue()
            : base(Statistic.LatestValue)
        {
        }

        /// <inheritdoc/>
        public override void Calculate(IPortfolio portfolio, Account account, TwoName name)
        {
            Value = portfolio.Value(account, name, DateTime.Today);
            fCurrency = portfolio.BaseCurrency;
        }

        /// <inheritdoc/>
        public override void Calculate(IPortfolio portfolio, Totals total, TwoName name)
        {
            Value = portfolio.TotalValue(total, DateTime.Today, name);
            fCurrency = portfolio.BaseCurrency;
        }
    }
}
