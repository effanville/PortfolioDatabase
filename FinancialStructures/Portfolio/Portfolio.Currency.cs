using System;
using System.Collections.Generic;
using System.Text;

namespace FinancialStructures.FinanceStructures
{
    public partial class Portfolio
    {
        public double CurrencyValue(string currencyName, DateTime date)
        {
            foreach (var currency in Currencies)
            {
                if (currency.GetName() == currencyName)
                {
                    return currency.Value(date).Value;
                }
            }

            return 1.0;
        }
    }
}
