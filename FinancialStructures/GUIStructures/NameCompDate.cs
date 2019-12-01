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

        public NameCompDate(string name, string company, DateTime date, bool newValue) : base(name, company, newValue)
        {
            fDateToRecord = date;
        }

        public NameCompDate(string name, string company, bool newValue) : base(name, company, newValue)
        {
            fDateToRecord = DateTime.MinValue;
        }
    }
}
