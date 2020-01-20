using FinancialStructures.FinanceStructures;

namespace FinancialStructures.Database
{
    public static partial class PortfolioBankAccount
    {
        public static double BankAccountLatestValue(this Portfolio portfolio, string name, string company)
        {
            if (!portfolio.TryGetBankAccount(name, company, out CashAccount desired))
            {
                return double.NaN;
            }

            return desired.LatestValue().Value;
        }

        public static double BankAccountTotal(this Portfolio portfolio)
        {
            if (portfolio != null)
            {
                double sum = 0;
                foreach (var acc in portfolio.GetBankAccounts())
                {
                    sum += acc.LatestValue().Value;
                }

                return sum;
            }

            return double.NaN;
        }
    }
}
