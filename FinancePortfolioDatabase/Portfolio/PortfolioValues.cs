using DataStructures;
using FinanceFunctionsList;
using System;
using System.Collections.Generic;

namespace FinanceStructures
{
    public partial class Portfolio
    {
        /// <summary>
        /// Returns the earliest date held in the portfolio.
        /// </summary>
        public DateTime FirstValueDate()
        {
            var output = new DateTime();
            output = DateTime.Today;
            foreach (var sec in fFunds)
            {
                var securityEarliest = sec.FirstValue().Day;
                if (securityEarliest < output)
                {
                    output = securityEarliest;
                }
            }
            return output;
        }
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

        public double TotalProfit()
        {
            double total = 0;
            foreach (var sec in fFunds)
            {
                if (sec.Any())
                {
                    total += Profit(sec.GetName(), sec.GetCompany());
                }
            }

            return total;
        }

        /// <summary>
        /// If possible, returns the IRR of all securities over the time period.
        /// </summary>
        public double IRRPortfolio(DateTime earlierTime, DateTime laterTime)
        {
            if (Funds.Count == 0)
            {
                return double.NaN;
            }
            double earlierValue = 0;
            double laterValue = 0;
            var Investments = new List<DailyValuation>();

            foreach (var security in Funds)
            {
                if (security.Any())
                {
                    earlierValue += security.GetNearestEarlierValuation(earlierTime).Value;
                    laterValue += security.GetNearestEarlierValuation(laterTime).Value;
                    Investments.AddRange(security.GetInvestmentsBetween(earlierTime, laterTime));
                }
            }

            return FinancialFunctions.IRRTime(new DailyValuation(earlierTime, earlierValue), Investments, new DailyValuation(laterTime, laterValue));
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
