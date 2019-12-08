using System;

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

        public NameCompDate(string name, string company, string url, DateTime date, bool newValue) : base(name, company,url, newValue)
        {
            fDateToRecord = date;
        }

        public NameCompDate(string name, string company, string url, bool newValue) : base(name, company, url, newValue)
        {
            fDateToRecord = DateTime.MinValue;
        }
    }
}
