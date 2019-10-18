using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinancePortfolioDatabase;
using GlobalHeldData;

namespace SecurityStatisticsFunctions
{
    /// <summary>
    /// Helper class to get statistics for 
    /// </summary>
    public static class SecurityStatisticsHelper
    {
        public static double SecurityLatestValue(string name, string company)
        {
            if (!GlobalData.Finances.TryGetSecurity(name, company, out Security desired))
            {
                return double.NaN;
            }
            return desired.LatestValue().Value;
        }

        public static double SecurityCAR(string name, string company, DateTime earlierTime, DateTime laterTime)
        {
            return GlobalData.Finances.CAR( name, company, earlierTime, laterTime);
        }

        public static double SecurityIRR(string name, string company)
        {
            return GlobalData.Finances.IRR( name, company);
        }

        public static double SecurityIRRTime(string name, string company, DateTime earlierTime, DateTime laterTime)
        {
            return GlobalData.Finances.IRRTime( name, company, earlierTime, laterTime);
        }
    }
}
