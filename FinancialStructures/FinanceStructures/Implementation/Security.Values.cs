﻿using System;
using System.Collections.Generic;
using FinancialStructures.DataStructures;
using Common.Structure.DataStructures;
using System.Linq;
using FinancialStructures.NamingStructures;
using Common.Structure.NamingStructures;

namespace FinancialStructures.FinanceStructures.Implementation
{
    public partial class Security
    {
        /// <summary>
        /// Returns data for the specific day.
        /// </summary>
        public SecurityDayData DayData(DateTime day)
        {
            _ = UnitPrice.TryGetValue(day, out double unitPrice);
            _ = Shares.TryGetValue(day, out double shares);
            _ = Investments.TryGetValue(day, out double invest);
            var trade = SecurityTrades.Where(t => t.Day.Equals(day)).FirstOrDefault();
            return new SecurityDayData(day, unitPrice, shares, invest, trade);
        }

        /// <inheritdoc/>
        public List<Labelled<TwoName, DailyValuation>> AllInvestmentsNamed(ICurrency currency = null)
        {
            List<DailyValuation> values = Investments.GetValuesBetween(Investments.FirstDate(), Investments.LatestDate());
            List<Labelled<TwoName, DailyValuation>> namedValues = new List<Labelled<TwoName, DailyValuation>>();

            foreach (DailyValuation value in values)
            {
                if (value != null && value.Value != 0)
                {
                    value.Value *= GetCurrencyValue(value.Day, currency);
                    namedValues.Add(new Labelled<TwoName, DailyValuation>(new TwoName(Names.Company, Names.Name), value));
                }
            }

            return namedValues;
        }

        /// <summary>
        /// Retrieves data in a list ordered by date.
        /// </summary>
        public override List<DailyValuation> ListOfValues()
        {
            var output = GetDataForDisplay();
            List<DailyValuation> thing = new List<DailyValuation>();
            foreach (var dateValue in output)
            {
                thing.Add(new DailyValuation(dateValue.Date, dateValue.Value));
            }

            return thing;
        }

        /// <inheritdoc/>
        public IReadOnlyList<SecurityDayData> GetDataForDisplay()
        {
            List<SecurityDayData> output = new List<SecurityDayData>();
            if (UnitPrice.Any())
            {
                foreach (DailyValuation unitPriceValuation in UnitPrice.GetValuesBetween(UnitPrice.FirstDate(), UnitPrice.LatestDate()))
                {
                    double shares = Shares.NearestEarlierValue(unitPriceValuation.Day).Value;
                    _ = Investments.TryGetValue(unitPriceValuation.Day, out double invest);
                    SecurityDayData thisday = new SecurityDayData(unitPriceValuation.Day, unitPriceValuation.Value, shares, invest);
                    output.Add(thisday);
                }
            }
            if (Shares.Any())
            {
                foreach (DailyValuation sharesValuation in Shares.GetValuesBetween(Shares.FirstDate(), Shares.LatestDate()))
                {
                    if (!UnitPrice.TryGetValue(sharesValuation.Day, out double _))
                    {
                        _ = Investments.TryGetValue(sharesValuation.Day, out double invest);
                        SecurityDayData thisday = new SecurityDayData(sharesValuation.Day, double.NaN, sharesValuation.Value, invest);
                        output.Add(thisday);
                    }
                }
            }
            if (Investments.Any())
            {
                foreach (DailyValuation investmentValuation in Investments.GetValuesBetween(Investments.FirstDate(), Investments.LatestDate()))
                {
                    if (!UnitPrice.TryGetValue(investmentValuation.Day, out double _) && !Shares.TryGetValue(investmentValuation.Day, out double _))
                    {
                        SecurityDayData thisday = new SecurityDayData(investmentValuation.Day, double.NaN, double.NaN, investmentValuation.Value);

                        output.Add(thisday);
                    }
                }
            }
            output.Sort();
            return output;
        }

        private static double GetCurrencyValue(DateTime date, ICurrency currency)
        {
            return currency == null ? 1.0 : currency.Value(date)?.Value ?? 1.0;
        }
    }
}
