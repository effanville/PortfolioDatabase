﻿using System;
using System.Collections.Generic;
using Common.Structure.DataStructures;

namespace FinancialStructures.FinanceStructures.Implementation.Asset
{
    /// <summary>
    /// An implementation of an asset that can have a debt against it.
    /// </summary>
    public sealed partial class AmortisableAsset : ValueList, IAmortisableAsset
    {
        /// <inheritdoc/>
        public override DailyValuation Value(DateTime date)
        {
            return Value(date, null);
        }

        /// <inheritdoc/>
        public DailyValuation Value(DateTime date, ICurrency currency = null)
        {
            if (!Values.Any())
            {
                return null;
            }

            decimal totalValue = Values.ValueZeroBefore(date)?.Value ?? 0.0m;
            decimal currentDebt = Debt.ValueZeroBefore(date)?.Value ?? 0.0m;
            decimal value = (totalValue - currentDebt) * GetCurrencyValue(date, currency);
            return new DailyValuation(date, value);
        }

        /// <inheritdoc/>
        public override DailyValuation LatestValue()
        {
            return LatestValue(null);
        }

        /// <inheritdoc/>
        public DailyValuation LatestValue(ICurrency currency)
        {
            DailyValuation latestDate = Values.LatestValuation();
            DailyValuation latestDebt = Debt.LatestValuation();
            if (latestDate == null)
            {
                return null;
            }

            decimal latestDebtValue = latestDebt?.Value ?? 0.0m;

            decimal latestValue = (latestDate.Value - latestDebtValue) * GetCurrencyValue(latestDate.Day, currency);

            return new DailyValuation(latestDate.Day, latestValue);
        }

        /// <inheritdoc/>
        public override DailyValuation ValueBefore(DateTime date)
        {
            return ValueBefore(date, null);
        }

        /// <inheritdoc/>
        public DailyValuation ValueBefore(DateTime date, ICurrency currency)
        {
            if (!Values.Any())
            {
                return null;
            }
            DailyValuation val = Values.ValueBefore(date);
            decimal currentDebt = Debt.ValueBefore(date)?.Value ?? 0.0m;

            if (val == null)
            {
                return null;
            }

            val.Value -= currentDebt;
            val.Value *= GetCurrencyValue(val.Day, currency);
            return val;
        }

        /// <inheritdoc/>
        public override DailyValuation FirstValue()
        {
            return FirstValue(null);
        }

        /// <inheritdoc/>
        public DailyValuation FirstValue(ICurrency currency)
        {
            DailyValuation firstDate = Values.FirstValuation();
            DailyValuation firstDebt = Debt.FirstValuation();
            if (firstDate == null)
            {
                return null;
            }

            decimal firstDebtValue = firstDebt?.Value ?? 0.0m;

            decimal firstValue = (firstDate.Value - firstDebtValue) * GetCurrencyValue(firstDate.Day, currency);

            return new DailyValuation(firstDate.Day, firstValue);
        }

        /// <inheritdoc/>
        public override DailyValuation ValueOnOrBefore(DateTime date)
        {
            return ValueOnOrBefore(date, null);
        }

        /// <inheritdoc/>
        public DailyValuation ValueOnOrBefore(DateTime date, ICurrency currency = null)
        {
            DailyValuation value = Values.ValueOnOrBefore(date);
            decimal currentDebt = Debt.ValueOnOrBefore(date)?.Value ?? 0.0m;
            if (value == null)
            {
                return null;
            }

            value.Value -= currentDebt;
            value.Value *= GetCurrencyValue(value.Day, currency);
            return value;
        }


        /// <summary>
        /// Retrieves data in a list ordered by date.
        /// </summary>
        public override List<DailyValuation> ListOfValues()
        {
            var values = Values.Values();

            List<DailyValuation> thing = new List<DailyValuation>();
            foreach (DailyValuation dateValue in values)
            {
                decimal debt = Debt.Value(dateValue.Day)?.Value ?? 0.0m;
                thing.Add(new DailyValuation(dateValue.Day, dateValue.Value - debt));
            }

            return thing;
        }

        private static decimal GetCurrencyValue(DateTime date, ICurrency currency)
        {
            return currency == null ? 1.0m : currency.Value(date)?.Value ?? 1.0m;
        }

        /// <inheritdoc/>
        public decimal TotalCost()
        {
            return Payments.Sum();
        }

        /// <inheritdoc/>
        public decimal TotalCost(DateTime date)
        {
            return Payments.Sum(val => val.Day <= date);
        }

        /// <inheritdoc/>
        public decimal TotalCost(DateTime earlierDate, DateTime laterDate)
        {
            return Payments.Sum(val => val.Day >= earlierDate && val.Day <= laterDate);
        }
    }
}