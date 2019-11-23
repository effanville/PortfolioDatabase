using DataStructures;
using FinanceStructures;
using GUIFinanceStructures;
using GUISupport;
using ReportingStructures;
using SecurityHelperFunctions;
using System;
using System.Windows.Input;

namespace FinanceWindowsViewModels.SecurityEdit
{
    public class DataEditViewModel : PropertyChangedBase
    {
        private NameComp fSelectedName;
        private Security fSelectedSecurity;

        private string fDateEdit;

        public string DateEdit
        {
            get { return fDateEdit; }
            set { fDateEdit = value; OnPropertyChanged(); }
        }

        private string fSharesEdit;

        public string SharesEdit
        {
            get { return fSharesEdit; }
            set { fSharesEdit = value; OnPropertyChanged(); }
        }

        private string fUnitPriceEdit;

        public string UnitPriceEdit
        {
            get { return fUnitPriceEdit; }
            set { fUnitPriceEdit = value; OnPropertyChanged(); }
        }

        private bool fNewInvestment;
        public bool NewInvestment
        {
            get { return fNewInvestment; }
            set { fNewInvestment = value; OnPropertyChanged(); }
        }

        public ICommand AddDataCommand { get; }

        /// <summary>
        /// Adds data to selected security.
        /// </summary>
        /// <remarks>
        /// This method not sufficient
        /// Must rethink how I deal with new investments.</remarks>
        private void ExecuteAddDataCommand(Object obj)
        {
            if (fSelectedName != null)
            {
                if (DateTime.TryParse(DateEdit, out DateTime date))
                {
                    if (Double.TryParse(SharesEdit, out double shares) && Double.TryParse(UnitPriceEdit, out double unitPrice))
                    {
                        double investmentValue = 0;
                        if (NewInvestment)
                        {
                            //user specified added new money to fund, so difference between last number of shares and numer shares now is considered new money.  
                            //need to calculate this. Either have previous investment, so get numbershares
                            // or is new fund and all money added is new.
                            if (fSelectedSecurity.TryGetEarlierData(date, out DailyValuation earlierPrice, out DailyValuation earlierShareNo, out DailyValuation earlierInvestment))
                            {
                                investmentValue = (shares - earlierShareNo.Value) * unitPrice;
                            }
                            else
                            {
                                investmentValue = shares * unitPrice;
                            }
                        }

                        //Current 
                        SecurityEditor.TryAddDataToSecurity(fSelectedName.Name, fSelectedName.Company, date, shares, unitPrice, investmentValue);
                    }
                    else
                    {
                        ErrorReports.AddError($"Number of Shares of {SharesEdit} or unit price {UnitPriceEdit} was not parsable as a number.");
                    }
                }
                else
                {
                    ErrorReports.AddError($"Date of {DateEdit} was not parsable as a Date.");
                }
            }

            SubWindowView("nowt", false);
            UpdateMainWindow(true);
        }

        public ICommand EditSecurityCommand { get; }

        private void ExecuteEditSecurity(Object obj)
        {
            if (fSelectedName != null)
            {
                if (DateTime.TryParse(DateEdit, out DateTime date))
                {
                    if (Double.TryParse(SharesEdit, out double shares) && Double.TryParse(UnitPriceEdit, out double unitPrice))
                    {
                        double investmentValue = 0;
                        if (NewInvestment)
                        {
                            investmentValue = shares * unitPrice;
                        }
                        SecurityEditor.TryEditSecurity(fSelectedName.Name, fSelectedName.Company, date, shares, unitPrice, investmentValue);
                    }
                }
                else
                {
                    ErrorReports.AddError($"Date format of {DateEdit} was not suitable.");
                }
            }
            else
            {
                ErrorReports.AddReport("No security or data selected to edit");
            }

            SubWindowView("nowt", false);
            UpdateMainWindow(true);
        }

        Action<bool> UpdateMainWindow;
        Action<string> windowToView;
        Action<string, bool> SubWindowView;

        public DataEditViewModel(Action<bool> updateWindow, Action<string> pageViewChoice, Action<string, bool> subWindowToView)
        {
            UpdateMainWindow = updateWindow;
            windowToView = pageViewChoice;
            SubWindowView = subWindowToView;

            AddDataCommand = new BasicCommand(ExecuteAddDataCommand);
            EditSecurityCommand = new BasicCommand(ExecuteEditSecurity);
        }
    }
}
