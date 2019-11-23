using GUIFinanceStructures;
using GUISupport;
using ReportingStructures;
using SecurityHelperFunctions;
using System;
using System.Windows.Input;

namespace FinanceWindowsViewModels.SecurityEdit
{
    public class UserButtonsViewModel :PropertyChangedBase
    {
        private NameComp fSelectedName;


        private BasicDayDataView fSelectedValues;

        public ICommand AddSecurityCommand { get; }

        
        private void ExecuteAddSecurity(Object obj)
        {
            SubWindowView("Name", false);
        }

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

        public ICommand AddValuationCommand { get; }

        private void ExecuteAddValuationCommand(Object obj)
        {
            SubWindowView("Data", false);
        }

        public ICommand ShowEditSecurityCommand { get; }

        private void ExecuteShowEditSecurity(Object obj)
        {
            SubWindowView("Data", true);
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
        Action<string, bool> SubWindowView;
        public UserButtonsViewModel(Action<bool> updateWindow, Action<string> pageViewChoice, Action<string, bool> subWindowToView, NameComp newName, BasicDayDataView newData)
        {
            UpdateMainWindow = updateWindow;
            windowToView = pageViewChoice;
            SubWindowView = subWindowToView;
            AddSecurityCommand = new BasicCommand(ExecuteAddSecurity);
            DeleteSecurityCommand = new BasicCommand(ExecuteDeleteSecurity);
            AddValuationCommand = new BasicCommand(ExecuteAddValuationCommand);
            ShowEditSecurityCommand = new BasicCommand(ExecuteShowEditSecurity);
            DeleteValuationCommand = new BasicCommand(ExecuteDeleteValuation);
            CloseCommand = new BasicCommand(ExecuteCloseCommand);
            fSelectedName = newName;
            fSelectedValues = newData;
        }
    }
}
