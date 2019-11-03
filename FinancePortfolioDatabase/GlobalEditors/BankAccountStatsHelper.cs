using System;
using FinanceStructures;
using GlobalHeldData;

namespace BankAccountStatisticsFunctions
{
    /// <summary>
    /// Helper class to get statistics for 
    /// </summary>
    public static class BankAccountStatistics
    {
        public static double BankAccountLatestValue(string name, string company)
        {
            if (!GlobalData.Finances.TryGetBankAccount(name, company, out CashAccount desired))
            {
                return double.NaN;
            }
            return desired.LatestValue().Value;
        }
    }
}
