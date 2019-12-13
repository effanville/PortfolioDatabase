using FinancialStructures.DataStructures;
using FinancialStructures.FinanceFunctionsList;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinancialStructures.FinanceStructures
{
    public partial class Portfolio
    {
        public double SectorValue(string sectorName, DateTime date)
        {
            double sum = 0;
            if (fFunds != null)
            {
                foreach (var fund in fFunds)
                {
                    if (fund.IsSectorLinked(sectorName))
                    {
                        sum += fund.GetNearestEarlierValuation(date).Value;
                    }
                }
            }

            return sum;
        }

        public List<Security> SectorSecurities(string sectorName)
        {
            var securities = new List<Security>();
            foreach (var sec in Funds)
            {
                if (sec.IsSectorLinked(sectorName))
                {
                    securities.Add(sec);
                }
            }
            securities.Sort();
            return securities;
        }

        public List<DailyValuation_Named> GetSectorInvestments(string company)
        {
            var output = new List<DailyValuation_Named>();
            foreach (var sec in SectorSecurities(company))
            {
                output.AddRange(sec.GetAllInvestmentsNamed());
            }

            return output;
        }

        public double SectorProfit(string sectorName)
        {
            var securities = SectorSecurities(sectorName);
            double value = 0;
            foreach (var security in securities)
            {
                if (security.Any())
                {
                    value += security.LatestValue().Value - security.TotalInvestment();
                }
            }

            return value;
        }

        public double SectorFraction(string sectorName, DateTime date)
        {
            return SectorValue(sectorName, date) / Value(date);
        }

        /// <summary>
        /// If possible, returns the IRR of all securities in the sector specified over the time period.
        /// </summary>
        public double IRRSector(string sectorName, DateTime earlierTime, DateTime laterTime)
        {
            var securities = SectorSecurities(sectorName);
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
