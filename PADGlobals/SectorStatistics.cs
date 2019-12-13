using System;
using System.Collections.Generic;
using System.Text;
using FinancialStructures.FinanceStructures;
using GlobalHeldData;

namespace PADGlobals
{
    public static class SectorStatistics
    {
        public static double SectorTotal(string sectorName, DateTime date)
        {
            return GlobalData.Finances.SectorValue(sectorName, date);
        }

        public static List<Security> SectorSecurities(string sectorName)
        {
            return GlobalData.Finances.SectorSecurities(sectorName);
        }

        public static double SectorProfit(string sectorName)
        {
            return GlobalData.Finances.SectorProfit(sectorName);
        }

        public static double IRRSector(string sectorName, DateTime earlierTime, DateTime laterTime)
        {
            return GlobalData.Finances.IRRSector(sectorName, earlierTime, laterTime);
        }
    }
}
