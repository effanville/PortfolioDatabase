using System;

namespace FinancialStructures.GUIFinanceStructures
{
    public class DatabaseStatistics : NameComp
    {
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

        public DatabaseStatistics(string company, string name, DateTime latestDate, int numEntries) : base(name, company)
        {
            Date = latestDate;
            NumEntries = numEntries;
        }

        public DatabaseStatistics(string company, string name) : base(name, company)
        {
        }
    }
}
