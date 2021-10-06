﻿using System;
using Common.Structure.Extensions;
using FinancialStructures.Database;
using FinancialStructures.Database.Statistics;
using FinancialStructures.NamingStructures;

namespace FinancialStructures.Statistics
{
    /// <summary>
    /// Statistic for the <see cref="Statistic.FundFraction"/> enum value.
    /// This calculates the fraction held in an account out of the total held
    /// in the portfolio.
    /// </summary>
    internal class StatisticFundFraction : StatisticBase
    {
        /// <summary>
        /// <inheritdoc/> Overrides default by truncating to 4 d.p.
        /// </summary>
        public override string StringValue => Value.Truncate(4).ToString();

        /// <summary>
        /// Default constructor setting Statistic type.
        /// </summary>
        internal StatisticFundFraction()
            : base(Statistic.FundFraction)
        {
        }


        /// <inheritdoc/>
        public override void Calculate(IPortfolio portfolio, Account account, TwoName name)
        {
            Value = portfolio.Fraction(account.ToTotals(), account, name, DateTime.Today);
        }

        /// <inheritdoc/>
        public override void Calculate(IPortfolio portfolio, Totals total, TwoName name)
        {
            Value = portfolio.TotalFraction(total, name, DateTime.Today);
        }
    }
}
