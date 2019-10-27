using System;
using System.Collections.Generic;
using System.Linq;
using FinanceStructures;

namespace FinanceFunctionsList
{
    public static class FinancialFunctions
    {
        /// <summary>
        /// Returns the compound annual rate from the firstValue to the lastValue in the time last-first
        /// </summary>
        public static double CAR(DateTime first, double firstValue, DateTime last, double lastValue)
        {
            return Math.Pow(lastValue / firstValue, 365.0 / (last - first).Days);
        }

        /// <summary>
        /// Returns the compound annual rate from the firstValue to the lastValue in the time last-first
        /// </summary>
        public static double CAR(DailyValuation firstValue, DailyValuation secondValue)
        {
            return CAR(firstValue.Day, firstValue.Value, secondValue.Day, secondValue.Value);
        }

        private static double Evaluator(List<DailyValuation> investments, DailyValuation latest, double expectedReturnRate)
        {
            double sum = 0;
            for (int i = 0; i < investments.Count(); ++i)
            {
                sum += investments[i].Value * Math.Pow((1 + expectedReturnRate), (latest.Day - investments[i].Day).Days);
            }
            return latest.Value - sum;
        }

        /// <summary>
        /// Returns the internal rate of return of a collection of investments, having achieve a certain value
        /// Is a bisection method.
        /// </summary>
        public static double IRR(List<DailyValuation> investments, DailyValuation latestValue)
        {
            // More than one investment so have to perform bisection method.
            double lowestValue = -1;
            double highestValue = 2;

            // pre iteration checks (if function has same value at each side then return false
            double funcLow = Evaluator(investments, latestValue, lowestValue);
            double funcHigh = Evaluator(investments, latestValue, highestValue);

            if ((funcLow < 0 && funcHigh < 0) || (funcLow > 0 && funcHigh > 0))
            {
                return double.NaN;
            }

            double middleValue = (lowestValue + highestValue) / 2.0;
            double funcMiddle = Evaluator(investments, latestValue, middleValue);

            int iterationNumber = 0;
            // stop after certain number of iterations (this also guarantees a certain error)
            while (iterationNumber < 20)
            {
                funcLow = Evaluator(investments, latestValue, lowestValue);
                funcHigh = Evaluator(investments, latestValue, highestValue);
                funcMiddle = Evaluator(investments, latestValue, middleValue);

                if ((funcLow < 0 && funcMiddle < 0) || (funcLow > 0 && funcMiddle > 0))
                {
                    lowestValue = middleValue;
                    middleValue = (lowestValue + highestValue) / 2.0;
                }

                if ((funcHigh < 0 && funcMiddle < 0) || (funcHigh > 0 && funcMiddle > 0))
                {
                    highestValue = middleValue;
                    middleValue = (lowestValue + highestValue) / 2.0;
                }

                iterationNumber++;
            }

            return middleValue;
        }

        /// <summary>
        /// Calculates Internal rate of return of a collection of investments over the last timelength number of months
        /// </summary>
        public static double IRRTime(List<DailyValuation> investments, DailyValuation latestValue, DailyValuation startValue)
        {
            // reduce number of investments to recent only
            var recentInvestments = new List<DailyValuation>();
            recentInvestments.Add(startValue);

            foreach (DailyValuation value in investments)
            {
                if (value.Day > startValue.Day)
                {
                    recentInvestments.Add(value);
                }
            }

            return IRR(recentInvestments, latestValue);
        }
    }
}
