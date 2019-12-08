using System;

namespace GUIFinanceStructures
{
    public class BasicDayDataView : IComparable
    {
        public int CompareTo(Object obj)
        {
            if (obj is BasicDayDataView dailyView)
            {
                return DateTime.Compare(Date, dailyView.Date);
            }
            return 0;
        }

        public BasicDayDataView()
        { 
            NewValue = true;
            Date = DateTime.Today;
        }

        public BasicDayDataView(DateTime date, double unitPrice, double shareNo, double investment, bool newValue = true)
        {
            Date = date;
            UnitPrice = unitPrice;
            ShareNo = shareNo;
            Value = UnitPrice * ShareNo;
            Investment = investment;
            NewValue = newValue;
        }

        public bool NewValue { get; set; }

        public DateTime Date { get; set; }
        public double UnitPrice { get; set; }

        public double ShareNo { get; set; }

        public double Value { get; set; }

        public double Investment { get; set; }
    }
}
