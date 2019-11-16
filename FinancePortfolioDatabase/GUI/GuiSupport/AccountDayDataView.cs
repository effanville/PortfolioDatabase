using System;

namespace GUIFinanceStructures
{
    public class AccountDayDataView
    {
        public AccountDayDataView()
        { }

        public AccountDayDataView(DateTime date, double unitPrice)
        {
            Date = date;
            Amount = unitPrice;
        }

        public DateTime Date { get; set; }
        public double Amount { get; set; }
    }
}
