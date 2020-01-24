using System;

namespace FinancialStructures.GUIFinanceStructures
{
    public class AccountDayDataView
    {
        public AccountDayDataView()
        {
            Date = DateTime.Today;
            NewValue = true;
        }

        public AccountDayDataView(DateTime date, double unitPrice, bool newValue = true)
        {
            fDate = date;
            fAmount = unitPrice;
            NewValue = newValue;
        }

        public AccountDayDataView Copy()
        {
            return new AccountDayDataView(Date, Amount, NewValue);
        }


        public bool NewValue { get; set; }
        private DateTime fDate;
        public DateTime Date
        {
            get { return fDate; }
            set { fDate = value; NewValue = true; }
        }

        private double fAmount;

        public double Amount
        {
            get { return fAmount; }
            set { fAmount = value; NewValue = true; }
        }
    }
}
