using FinancialStructures.Database;
using FinancialStructures.FinanceStructures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace GlobalHeldData
{
    public static class AssemblyCreationDate
    {
        public static readonly DateTime Value;

        static AssemblyCreationDate()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            Value = new DateTime(2020, 1, 26, 20, 24, 30).AddDays(version.Build).AddSeconds(version.MinorRevision * 2);
        }
    }
    public static class GlobalData
    {
        public static string versionNumber = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
        public static string fDatabaseFilePath;

        /// <summary>
        /// filepath where statistics are saved. is the same as the database path unless saved elsewhere.
        /// </summary>
        public static string fStatsDirectory = Path.GetDirectoryName(fDatabaseFilePath);

        public static string DatabaseName { get { return Path.GetFileNameWithoutExtension(fDatabaseFilePath); } }

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
        ///  This should only ever be called once in the loading routine.
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
