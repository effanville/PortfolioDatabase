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
        { }

        public BasicDayDataView(DateTime date, double unitPrice, double shareNo, double investment)
        {
            Date = date;
            UnitPrice = unitPrice;
            ShareNo = shareNo;
            Value = UnitPrice * ShareNo;
            Investment = investment;
        }

        public DateTime Date { get; set; }
        public double UnitPrice { get; set; }

        public double ShareNo { get; set; }

        public double Value { get; set; }

        public double Investment { get; set; }
    }
}
