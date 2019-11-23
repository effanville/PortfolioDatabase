using System;
using System.Collections.Generic;
using FinanceFunctionsList;
using DataStructures;

namespace FinanceStructures
{
    public partial class Portfolio
    {
        public double Profit(string name, string company)
        {
            if (TryGetSecurity(name, company, out Security desired))
            {
                if (desired.Any())
                {
                    return desired.LatestValue().Value - desired.TotalInvestment();
                }
            }

            return double.NaN;
        }

        public double FundsFraction(string name, string company)
        {
            if (TryGetSecurity(name, company, out Security desired))
            {
                if (desired.Any())
                {
                    return desired.LatestValue().Value / AllSecuritiesValue(DateTime.Today);
                }
            }

            return double.NaN;
        }
        public List<DailyValuation_Named> GetSecurityInvestments(string name, string company)
        {
            if (TryGetSecurity(name, company, out Security desired))
            {
                if (desired.Any())
                {
                    return desired.GetAllInvestmentsNamed();
                }
            }

            return null;
        }

        /// <summary>
        /// If possible, returns the CAR of the security specified.
        /// </summary>
        public double CAR(string name, string company, DateTime earlierTime, DateTime laterTime)
        {
            if (TryGetSecurity(name, company, out Security desired))
            {
                if (desired.Any())
                {
                    return desired.CAR(earlierTime, laterTime);
                }
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
                if (desired.Any())
                {
                    return desired.IRR();
                }
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
                if (desired.Any())
                {
                    return desired.IRRTime(earlierTime, laterTime);
                }
            }

            return double.NaN;
        }
    }
}
