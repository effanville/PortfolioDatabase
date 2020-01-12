﻿using FinancialStructures.DataStructures;
using FinancialStructures.FinanceStructures;
using GlobalHeldData;
using System;
using System.Collections.Generic;

namespace SecurityStatisticsFunctions
{
    /// <summary>
    /// Helper class to get statistics for a security.
    /// </summary>
    public static class SecurityStatistics
    {
        public static double SecurityLatestValue(string name, string company)
        {
            if (!GlobalData.Finances.TryGetSecurity(name, company, out Security desired) || !desired.Any())
            {
                return double.NaN;
            }
            var currencyName = desired.GetCurrency();
            var currency = GlobalData.Finances.Currencies.Find(cur => cur.Name == currencyName);
            return desired.LatestValue(currency).Value;
        }

        public static List<DailyValuation_Named> GetSecurityInvestments(string name, string company)
        {
            if (!GlobalData.Finances.TryGetSecurity(name, company, out Security desired))
            {
                return new List<DailyValuation_Named>();
            }
            var currencyName = desired.GetCurrency();
            var currency = GlobalData.Finances.Currencies.Find(cur => cur.Name == currencyName);
            return desired.GetAllInvestmentsNamed(currency);
        }

        public static double SecurityCAR(string name, string company, DateTime earlierTime, DateTime laterTime)
        {
            return GlobalData.Finances.CAR(name, company, earlierTime, laterTime);
        }

        public static double SecurityIRR(string name, string company)
        {
            return GlobalData.Finances.IRR(name, company);
        }

        public static double SecurityIRRTime(string name, string company, DateTime earlierTime, DateTime laterTime)
        {
            return GlobalData.Finances.IRR(name, company, earlierTime, laterTime);
        }

        public static double RecentChange(string name, string company)
        {
            return GlobalData.Finances.RecentChange(name,company);
        }

        public static double Profit(string name, string company)
        {
            return GlobalData.Finances.Profit(name, company);
        }

        public static double FundsFraction(string name, string company)
        {
            return GlobalData.Finances.FundsFraction(name, company);
        }

        public static double TotalValue(DateTime date)
        {
            return GlobalData.Finances.Value(date);
        }
    }
}
