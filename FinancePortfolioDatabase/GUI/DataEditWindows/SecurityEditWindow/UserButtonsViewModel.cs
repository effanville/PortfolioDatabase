using FinancialStructures.GUIFinanceStructures;
using GUISupport;
using FinancialStructures.ReportingStructures;
using SecurityHelperFunctions;
using System;
using System.Windows.Input;

namespace FinanceWindowsViewModels.SecurityEdit
{
    public class UserButtonsViewModel :PropertyChangedBase
    {
        private NameComp fSelectedName;


        private BasicDayDataView fSelectedValues;

        public ICommand DeleteSecurityCommand { get; }

        private void ExecuteDeleteSecurity(Object obj)
        {
            if (fSelectedName != null)
            {
                SecurityEditor.TryDeleteSecurity(fSelectedName.Name, fSelectedName.Company);
            }
            else
            {
                ErrorReports.AddError("Something went wrong when trying to delete security.");
            }

            UpdateMainWindow(true);
        }

        public void UpdateButtonViewData(NameComp newName, BasicDayDataView newData)
        {
            fSelectedName = newName;
            fSelectedValues = newData;
        }

        public ICommand DeleteValuationCommand { get; }

        private void ExecuteDeleteValuation(Object obj)
        {
            if (fSelectedName != null && fSelectedValues != null)
            {
                SecurityEditor.TryDeleteSecurityData(fSelectedName.Name, fSelectedName.Company, fSelectedValues.Date, fSelectedValues.ShareNo, fSelectedValues.UnitPrice, fSelectedValues.Investment);
            }

            UpdateMainWindow(true);
        }

        public ICommand CloseCommand { get; }

        private void ExecuteCloseCommand(Object obj)
        {
            UpdateMainWindow(false);
            windowToView("dataview");
        }

        Action<bool> UpdateMainWindow;
        Action<string> windowToView;
        public UserButtonsViewModel(Action<bool> updateWindow, Action<string> pageViewChoice, NameComp newName, BasicDayDataView newData)
        {
            UpdateMainWindow = updateWindow;
            windowToView = pageViewChoice;
            DeleteSecurityCommand = new BasicCommand(ExecuteDeleteSecurity);
            DeleteValuationCommand = new BasicCommand(ExecuteDeleteValuation);
            CloseCommand = new BasicCommand(ExecuteCloseCommand);
            fSelectedName = newName;
            fSelectedValues = newData;
        }
    }
}
