using System;
using System.Collections.Generic;

namespace FinancialStructures.GUIFinanceStructures
{
    public class NameCompDate : NameComp
    {
        private DateTime fDateToRecord;
        public DateTime DateToRecord
        {
            get { return fDateToRecord; }
            set { fDateToRecord = value; }
        }
        public NameCompDate() : base()
        {
        }

        public NameCompDate(string name, string company, string url, List<string> sectors, DateTime date, bool newValue) : base(name, company, url, sectors, newValue)
        {
            fDateToRecord = date;
        }

        public NameCompDate(string name, string company, string url, bool newValue) : base(name, company, url, newValue)
        {
            fDateToRecord = DateTime.MinValue;
        }
    }
}
