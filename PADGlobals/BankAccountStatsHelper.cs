using FinancialStructures.FinanceStructures;
using GlobalHeldData;

namespace BankAccountStatisticsFunctions
{
    /// <summary>
    /// Helper class to get statistics for bank accounts
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

        public static double BankAccountTotal()
        {
            if (GlobalData.Finances != null)
            {
                double sum = 0;
                foreach (var acc in GlobalData.Finances.GetBankAccounts())
                {
                    sum += acc.LatestValue().Value;
                }

                return sum;
            }

            return double.NaN;
        }
    }
}
