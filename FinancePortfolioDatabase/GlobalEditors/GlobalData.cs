using System;
using System.Collections.Generic;
using FinanceStructures;

namespace GlobalHeldData
{
    public static class GlobalData
    {
        public static string fDatabaseFilePath;

        public static DateTime fToday = DateTime.Today;

        private static Portfolio fPersonalFinances;

        public static Portfolio Finances
        {
            get { return fPersonalFinances; }
            private set { fPersonalFinances = value; }
        }

        private static List<Sector> fBenchmarks;

        public static List<Sector> BenchMark
        {
            get { return fBenchmarks; }
        }

        private static List<Sector> fSectors;

        public static List<Sector> Sectors
        {
            get { return fSectors; }
        }

        /// <summary>
        ///  loads in data 
        ///  This should only every be called once in the loading routine.
        /// </summary>
        public static void LoadDatabase(Portfolio database)
        {
            if (database == null)
            {
                Finances = new Portfolio();
            }
            else
            {
                Finances = database;
            }
        }
    }
}
