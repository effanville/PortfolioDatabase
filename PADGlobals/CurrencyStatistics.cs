using System;
using System.Collections.Generic;
using System.Text;
using FinancialStructures.FinanceStructures;
using GlobalHeldData;

namespace PADGlobals
{
    public static class CurrencyStatistics
    {
        public static double CurrencyValue(string sectorName, DateTime date)
        {
            foreach (var currency in GlobalData.Finances.Currencies)
            {
                if (currency.GetName() == sectorName)
                {
                    return currency.Value(date).Value;
                }
            }

            return 0.0;
        }
    }
}
