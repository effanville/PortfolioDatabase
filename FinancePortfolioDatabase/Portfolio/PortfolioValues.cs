using System;

namespace FinanceStructures
{
    public partial class Portfolio
    {
        /// <summary>
        /// The total value of all securities on the date specified.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public double AllSecuritiesValue(DateTime date)
        {
            double total = 0;
            foreach (var sec in fFunds)
            {
                if (sec.Any())
                {
                total += sec.GetNearestEarlierValuation(date).Value;
                }
            }

            return total;
        }

        /// <summary>
        /// The total value of all bank accounts on the date specified
        /// </summary>
        public double AllBankAccountsValue(DateTime date)
        {
            double total = 0;
            foreach (var acc in fBankAccounts)
            {
                if (acc.Any())
                {
                    total += acc.GetNearestEarlierValuation(date).Value;
                }
            }

            return total;
        }

        /// <summary>
        /// The total value of all securities and bank accounts on the date specified
        /// </summary>
        public double Value(DateTime date)
        {
            return AllSecuritiesValue(date) + AllBankAccountsValue(date);
        }
    }
}
