using System;
using System.Collections.Generic;
using FinanceStructures;

namespace GlobalHeldData
{
    public static class GlobalData
    {
        public static double versionNumber = 0.1;
        public static string fDatabaseFilePath;

        public static DateTime fToday = DateTime.Today;

        private static Portfolio fPersonalFinances;

        public static Portfolio Finances
        {
            get { return fPersonalFinances; }
            private set { fPersonalFinances = value; }
        }

        private static List<Sector> fBenchmarks;

        public static List<Sector> BenchMarks
        {
            get { return fBenchmarks; }

            private set { fBenchmarks = value; }
        }

        /// <summary>
        ///  loads in data 
        ///  This should only every be called once in the loading routine.
        /// </summary>
        public static void LoadDatabase(Portfolio database, List<Sector> myBenchMarks)
        {
            if (database == null)
            {
                Finances = new Portfolio();
            }
            else
            {
                Finances = database;
            }

            if (myBenchMarks == null)
            {
                BenchMarks = new List<Sector>();
            }
            else
            {
                BenchMarks = myBenchMarks;
            }
        }
    }
}
