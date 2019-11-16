using DataStructures;
using FinanceFunctionsList;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FinanceStructures
{
    public partial class Portfolio
    {
        public bool DoesCompanyExist(string company)
        {
            foreach (Security sec in Funds)
            {
                if (sec.GetCompany() == company)
                {
                    return true;
                }
            }

            return false;
        }

        public List<Security> CompanySecurities(string company)
        {
            var securities = new List<Security>();
            foreach (var sec in Funds)
            {
                if (sec.GetCompany() == company)
                {
                    securities.Add(sec);
                }
            }
            securities.Sort();
            return securities;
        }

        public List<DailyValuation_Named> GetCompanyInvestments(string company)
        {
            var output = new List<DailyValuation_Named>();
            foreach (var sec in CompanySecurities(company))
            {
                output.AddRange(sec.GetAllInvestmentsNamed());
            }

            return output;
        }

        public double CompanyValue(string company, DateTime date)
        {
            var securities = CompanySecurities(company);
            if (securities.Count() == 0)
            {
                return double.NaN;
            }
            double value = 0;
            foreach (var security in securities)
            {
                if (security.Any())
                {
                    value += security.GetNearestEarlierValuation(date).Value;
                }
            }

            return value;
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
                if (security.Any())
                {
                    earlierValue += security.GetNearestEarlierValuation(earlierTime).Value;
                    laterValue += security.GetNearestEarlierValuation(laterTime).Value;
                    Investments.AddRange(security.GetInvestmentsBetween(earlierTime, laterTime));
                }
            }

            return FinancialFunctions.IRRTime(new DailyValuation(earlierTime, earlierValue), Investments, new DailyValuation(laterTime, laterValue));
        }
    }
}
