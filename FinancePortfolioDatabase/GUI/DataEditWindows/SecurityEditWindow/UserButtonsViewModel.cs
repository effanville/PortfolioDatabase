using FinancialStructures.GUIFinanceStructures;
using GUISupport;
using FinancialStructures.ReportingStructures;
using SecurityHelperFunctions;
using System;
using System.Windows.Input;
using PADGlobals;

namespace FinanceWindowsViewModels.SecurityEdit
{
    public class UserButtonsViewModel :PropertyChangedBase
    {
        private NameComp fSelectedName;


        private BasicDayDataView fSelectedValues;
        public ICommand DownloadCommand { get; }

        private async void ExecuteDownloadCommand(Object obj)
        {
            var reports = new ErrorReports();
            if (fSelectedName != null)
            {
                await DataUpdater.DownloadSecurity(fSelectedName.Company, fSelectedName.Name, reports).ConfigureAwait(false);
            }
            UpdateMainWindow(true);
            if (reports.Any())
            {
                UpdateReports(reports);
            }
        }
        public ICommand DeleteSecurityCommand { get; }

        private void ExecuteDeleteSecurity(Object obj)
        {
            var reports = new ErrorReports();
            if (fSelectedName != null)
            {
                SecurityEditor.TryDeleteSecurity(fSelectedName.Name, fSelectedName.Company, reports);
            }
            else
            {
                reports.AddError("Something went wrong when trying to delete security.");
            }

            if (reports.Any())
            {
                UpdateReports(reports);
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
            var reports = new ErrorReports();
            if (fSelectedName != null && fSelectedValues != null)
            {
                SecurityEditor.TryDeleteSecurityData(reports, fSelectedName.Name, fSelectedName.Company, fSelectedValues.Date, fSelectedValues.ShareNo, fSelectedValues.UnitPrice, fSelectedValues.Investment);
            }

            if (reports.Any())
            {
                UpdateReports(reports);
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
        Action<ErrorReports> UpdateReports;
        public UserButtonsViewModel(Action<bool> updateWindow, Action<string> pageViewChoice, Action<ErrorReports> updateReports, NameComp newName, BasicDayDataView newData)
        {
            UpdateMainWindow = updateWindow;
            windowToView = pageViewChoice;
            UpdateReports = updateReports;
            DownloadCommand = new BasicCommand(ExecuteDownloadCommand);
            DeleteSecurityCommand = new BasicCommand(ExecuteDeleteSecurity);
            DeleteValuationCommand = new BasicCommand(ExecuteDeleteValuation);
            CloseCommand = new BasicCommand(ExecuteCloseCommand);
            fSelectedName = newName;
            fSelectedValues = newData;
        }
    }
}
