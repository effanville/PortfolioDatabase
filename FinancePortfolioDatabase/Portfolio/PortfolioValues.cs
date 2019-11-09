using DataStructures;
using System;
using System.Collections.Generic;

namespace FinanceStructures
{
    public partial class Portfolio
    {
        /// <summary>
        /// Returns a list of all investments in the portfolio securities.
        /// </summary>
        /// <returns></returns>
        public List<DailyValuation_Named> AllSecuritiesInvestments()
        {
            var output = new List<DailyValuation_Named>();
            foreach (var comp in GetSecuritiesCompanyNames())
            {
                output.AddRange( GetCompanyInvestments(comp));
            }
            output.Sort();
            return output;
        }

        /// <summary>
        /// The total value of all securities on the date specified.
        /// </summary>
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
