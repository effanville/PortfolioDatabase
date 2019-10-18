using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancePortfolioDatabase
{
    public partial class Portfolio
    {
        /// <summary>
        /// If possible, returns the CAR of the security specified.
        /// </summary>
        public double CAR(string name, string company, DateTime earlierTime, DateTime laterTime)
        {
            if (TryGetSecurity(name, company, out Security desired))
            {
                return desired.CAR(earlierTime, laterTime);
            }

            return double.NaN;
        }

        /// <summary>
        /// If possible, returns the IRR of the security specified.
        /// </summary>
        public double IRR(string name, string company)
        {
            if (TryGetSecurity(name, company, out Security desired))
            {
                return desired.IRR();
            }

            return double.NaN;
        }

        /// <summary>
        /// If possible, returns the IRR of the security specified over the time period.
        /// </summary>
        public double IRRTime(string name, string company, DateTime earlierTime, DateTime laterTime)
        {
            if (TryGetSecurity(name, company, out Security desired))
            {
                return desired.IRRTime(earlierTime, laterTime);
            }

            return double.NaN;
        }

        /// <summary>
        /// If possible, returns the IRR of all securities in the company specified over the time period.
        /// </summary>
        public double IRRCompany(string company, DateTime earlierTime, DateTime laterTime)
        {
            var securities = CompanySecurities(company);
            if (securities.Count == 0)
            {
                return double.NaN;
            }
            double earlierValue = 0;
            double laterValue = 0;
            var Investments = new List<DailyValuation>();

            foreach (var security in securities)
            {
                earlierValue += security.GetNearestEarlierValuation(earlierTime).Value;
                laterValue += security.GetNearestEarlierValuation(laterTime).Value;
                Investments.AddRange(security.GetInvestmentsBetween(earlierTime, laterTime));
            }

            return FinancialFunctions.IRRTime(Investments,new DailyValuation( laterTime, laterValue), new DailyValuation( earlierTime, earlierValue));
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
                earlierValue += security.GetNearestEarlierValuation(earlierTime).Value;
                laterValue += security.GetNearestEarlierValuation(laterTime).Value;
                Investments.AddRange(security.GetInvestmentsBetween(earlierTime, laterTime));
            }

            return FinancialFunctions.IRRTime(Investments, new DailyValuation(laterTime, laterValue), new DailyValuation(earlierTime, earlierValue));
        }
    }
}
