﻿using System;
using FinancialStructures.FinanceStructures.Implementation;
using FinancialStructures.NamingStructures;

namespace FinancialStructures.Tests.TestDatabaseConstructor
{
    public class CurrencyConstructor
    {
        public Currency item;

        public CurrencyConstructor(string company, string name, string currency = null, string url = null, string sectors = null)
        {
            var names = new NameData(company, name, currency, url);
            names.SectorsFlat = sectors;
            item = new Currency(names);
        }

        public CurrencyConstructor WithData(DateTime date, double price)
        {
            item.Values.SetData(date, price);

            return this;
        }
    }
}
