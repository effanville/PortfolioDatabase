﻿using FinancialStructures.Database;
using FinancialStructures.Database.Statistics;
using FinancialStructures.NamingStructures;
using StructureCommon.Extensions;

namespace FinancialStructures.Statistics
{
    internal class StatisticLatestDate : IStatistic
    {
        internal StatisticLatestDate()
        {
            StatType = Statistic.LatestDate;
        }

        /// <inheritdoc/>
        public Statistic StatType
        {
            get;
        }

        /// <inheritdoc/>
        public double Value
        {
            get;
            private set;
        }

        /// <inheritdoc/>
        public string StringValue
        {
            get;
            private set;
        }

        /// <inheritdoc/>
        public bool IsNumeric
        {
            get
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public object ValueAsObject
        {
            get
            {
                return IsNumeric ? (object)Value : (object)StringValue;
            }
        }

        /// <inheritdoc/>
        public void Calculate(IPortfolio portfolio, Account account, TwoName name)
        {
            switch (account)
            {
                case Account.Security:
                {
                    if (!portfolio.TryGetSecurity(name, out var desired))
                    {
                        return;
                    }

                    StringValue = desired.LatestValue().Day.ToUkDateString();
                    return;
                }
                case Account.Benchmark:
                case Account.BankAccount:
                case Account.Currency:
                {
                    if (!portfolio.TryGetAccount(account, name, out var desired))
                    {
                        return;
                    }

                    StringValue = desired.LatestValue().Day.ToUkDateString();
                    return;
                }
                default:
                    return;
            }
        }

        /// <inheritdoc/>
        public void Calculate(IPortfolio portfolio, Totals total, TwoName name)
        {
            StringValue = portfolio.LatestDate(total, name.Company).ToUkDateString();
        }

        /// <inheritdoc/>
        public int CompareTo(IStatistic other)
        {
            return Value.CompareTo(other.Value);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return StringValue;
        }
    }
}
