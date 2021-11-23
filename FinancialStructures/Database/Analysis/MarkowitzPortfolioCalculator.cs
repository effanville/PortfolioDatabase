using System.Collections.Generic;
using FinancialStructures.FinanceStructures;
using FinancialStructures.NamingStructures;

namespace FinancialStructures.Database.Analysis
{
    /// <summary>
    /// Calculates analysis for a portfolio.
    /// </summary>
    public static class PortfolioAnalysisCalculator
    {
        /// <summary>
        /// Calculates the Markowitz portfolio from the data in the input portfolio
        /// </summary>
        public static IPortfolio CalculateMarkowitzPortfolio(IPortfolio portfolio, double desiredReturn = 0.05)
        {
            var meanYearlyReturns = MeanYearlyIRR(portfolio);
            var covariances = SecurityCovariances(portfolio, meanYearlyReturns);

            // now required to perform minimisation 
            return null;
        }

        private static Dictionary<TwoName, double> MeanYearlyIRR(IPortfolio portfolio)
        {
            var result = new Dictionary<TwoName, double>();
            foreach (var security in portfolio.FundsThreadSafe)
            {
                result.Add(security.Names.ToTwoName(), MeanYearlyIRR(security));
            }

            return result;
        }

        private static double MeanYearlyIRR(ISecurity security)
        {
            var unitPrices = security.UnitPrice;
            var latestDate = unitPrices.LatestValuation().Day;
            var dateMinusYear = latestDate.AddDays(-365);
            var earliestDate = unitPrices.FirstValuation().Day;
            double sumReturns = 0;
            int numYears = 0;
            while (dateMinusYear > earliestDate)
            {
                sumReturns += unitPrices.CAR(dateMinusYear, latestDate);
                dateMinusYear = dateMinusYear.AddDays(-365);
                latestDate = latestDate.AddDays(-365);
                numYears++;
            }

            return sumReturns / (double)numYears;
        }

        private static Dictionary<(TwoName, TwoName), double> SecurityCovariances(IPortfolio portfolio, Dictionary<TwoName, double> meanYearlyReturns)
        {
            var result = new Dictionary<(TwoName, TwoName), double>();
            foreach (var security in portfolio.FundsThreadSafe)
            {
                foreach (var otherSecurity in portfolio.FundsThreadSafe)
                {
                    double secYearlyReturn = meanYearlyReturns[security.Names.ToTwoName()];
                    double otherSecYearlyReturn = meanYearlyReturns[otherSecurity.Names.ToTwoName()];
                    result.Add((security.Names.ToTwoName(), otherSecurity.Names.ToTwoName()), Covariance(security, secYearlyReturn, otherSecurity, otherSecYearlyReturn));
                }
            }

            return result;
        }

        private static double Covariance(ISecurity firstSecurity, double firstYearlyReturn, ISecurity secondSecurity, double secondYearlyReturn)
        {
            var unitPrices = firstSecurity.UnitPrice;
            var otherUnitPrices = secondSecurity.UnitPrice;
            var latestDate = unitPrices.LatestValuation().Day;
            var dateMinusYear = latestDate.AddDays(-365);
            var earliestDate = unitPrices.FirstValuation().Day;
            double sumReturns = 0;
            int numYears = 0;
            while (dateMinusYear > earliestDate)
            {
                sumReturns += (unitPrices.CAR(dateMinusYear, latestDate) - firstYearlyReturn) * (otherUnitPrices.CAR(dateMinusYear, latestDate) - secondYearlyReturn);
                dateMinusYear = dateMinusYear.AddDays(-365);
                latestDate = latestDate.AddDays(-365);
                numYears++;
            }

            if (numYears == 0)
            {
                return 0;
            }

            return sumReturns / ((double)numYears - 1);
        }
    }
}
