using System;
using System.Collections.Generic;
using System.Reflection;
using FinanceStructures;

namespace GlobalHeldData
{
    internal static class AssemblyCreationDate
    {
        public static readonly DateTime Value;

        static AssemblyCreationDate()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            Value = new DateTime(2000, 1, 2,12,24,30).AddDays(version.Build).AddSeconds(version.MinorRevision * 2);
        }
    }
    public static class GlobalData
    {
        public static string versionNumber = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
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

        public static void ClearDatabase()
        {
            Finances = new Portfolio();
            BenchMarks = new List<Sector>();
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
