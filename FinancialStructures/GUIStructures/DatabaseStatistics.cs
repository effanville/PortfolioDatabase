﻿using System;

namespace FinancialStructures.GUIFinanceStructures
{
    public class DatabaseStatistics : NameComp
    {
        private DateTime fFirstDate;
        public DateTime FirstDate
        {
            get { return fFirstDate; }
            set { fFirstDate = value; }
        }

        private DateTime fDate;
        public DateTime Date
        {
            get { return fDate; }
            set { fDate = value; }
        }

        private int fNumEntries;

        public int NumEntries
        {
            get { return fNumEntries; }
            set { fNumEntries = value; }
        }

        private double fEntryYearDensity;

        public double EntryYearDensity
        {
            get { return fEntryYearDensity; }
            set { fEntryYearDensity = value; }
        }


        public DatabaseStatistics(string company, string name,DateTime firstDate, DateTime latestDate, int numEntries, double density) : base(name, company)
        {
            FirstDate = firstDate;
            Date = latestDate;
            NumEntries = numEntries;
            EntryYearDensity = density;
        }

        public DatabaseStatistics(string company, string name) : base(name, company)
        {
        }
    }
}
