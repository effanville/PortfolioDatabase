using System;
using System.Collections.Generic;
using DataStructures;
using FinanceStructures;
using GlobalHeldData;

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
                
            return desired.LatestValue().Value;
        }

        public static List<DailyValuation_Named> GetSecurityInvestments(string name, string company)
        {
            if (!GlobalData.Finances.TryGetSecurity(name, company, out Security desired))
            {
                return new List<DailyValuation_Named>();
            }
            return desired.GetAllInvestmentsNamed();
        }

        public static double SecurityCAR(string name, string company, DateTime earlierTime, DateTime laterTime)
        {
            return GlobalData.Finances.CAR( name, company, earlierTime, laterTime);
        }

        public static double SecurityIRR(string name, string company)
        {
            return GlobalData.Finances.IRR( name, company);
        }

        public static double SecurityIRRTime(string name, string company, DateTime earlierTime, DateTime laterTime)
        {
            return GlobalData.Finances.IRRTime( name, company, earlierTime, laterTime);
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
