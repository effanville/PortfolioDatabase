using GUIAccessorFunctions;
using System;
using System.Windows.Forms;
using System.Windows.Input;
using GUISupport;
using FinancialStructures.ReportingStructures;
using PADGlobals;

namespace FinanceWindowsViewModels
{
    public class OptionsPanelViewModel : PropertyChangedBase
    {
        public ICommand OpenSecurityEditWindowCommand { get; }

        public ICommand OpenBankAccountEditWindowCommand { get; }

        public ICommand OpenSectorEditWindowCommand { get; }

        public ICommand OpenStatsCreatorWindowCommand { get; }

        public ICommand SaveDatabaseCommand { get; }

        public ICommand LoadDatabaseCommand { get; }
        public ICommand UpdateDataCommand { get; }

        public void ExecuteSecurityEditWindow(Object obj)
        {
            UpdateMainWindow(true);
            windowToView("SecurityEditWindow");
        }

        public void ExecuteBankAccEditWindow(Object obj)
        {
            UpdateMainWindow(true);
            windowToView("BankAccEditWindow");
        }
        public void ExecuteSectorEditWindow(Object obj)
        {
            UpdateMainWindow(true);
            windowToView("SectorEditWindow");
        }
        public void ExecuteStatsCreatorWindow(Object obj)
        {
            UpdateMainWindow(true);
            windowToView("StatsCreatorWindow");
        }

        public void ExecuteSaveDatabase(Object obj)
        {
            var reports = new ErrorReports();
            SaveFileDialog saving = new SaveFileDialog();
            if (saving.ShowDialog() == DialogResult.OK)
            {
                //if (!File.Exists(saving.FileName))
                {
                    DatabaseAccessor.SetFilePath(saving.FileName);
                }
            }

            DatabaseAccessor.SavePortfolio(reports);
            saving.Dispose();
            UpdateMainWindow(true);
            if (reports.Any())
            {
                UpdateReports(reports);
            }
        }

        public void ExecuteLoadDatabase(Object obj)
        {
            var reports = new ErrorReports();
            OpenFileDialog openFile = new OpenFileDialog();
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                DatabaseAccessor.SetFilePath(openFile.FileName);
                DatabaseAccessor.ClearPortfolio();
                DatabaseAccessor.LoadPortfolio(reports);
                reports.AddGeneralReport(ReportType.Report, $"Loaded new database from {openFile.FileName}");
            }
            openFile.Dispose();

            if (reports.Any())
            {
                UpdateReports(reports);
            }
            UpdateMainWindow(false);
            UpdateSubWindows(false);
        }

        public async void ExecuteUpdateData(Object obj)
        {
            var reports = new ErrorReports();
            await DataUpdater.Downloader(reports).ConfigureAwait(false);

            if (reports.Any())
            {
                UpdateReports(reports);
            }
        }

        Action<bool> UpdateMainWindow;
        Action<bool> UpdateSubWindows;
        Action<string> windowToView;
        Action<ErrorReports> UpdateReports;

        public OptionsPanelViewModel(Action<bool> updateWindow, Action<bool> updateSubWindow, Action<string> pageViewChoice, Action<ErrorReports> updateReports)
        {
            UpdateMainWindow = updateWindow;
            UpdateSubWindows = updateSubWindow;
            windowToView = pageViewChoice;
            UpdateReports = updateReports;
            OpenSecurityEditWindowCommand = new BasicCommand(ExecuteSecurityEditWindow);
            OpenBankAccountEditWindowCommand = new BasicCommand(ExecuteBankAccEditWindow);
            OpenSectorEditWindowCommand = new BasicCommand(ExecuteSectorEditWindow);
            OpenStatsCreatorWindowCommand = new BasicCommand(ExecuteStatsCreatorWindow);
            SaveDatabaseCommand = new BasicCommand(ExecuteSaveDatabase);
            LoadDatabaseCommand = new BasicCommand(ExecuteLoadDatabase);
            UpdateDataCommand = new BasicCommand(ExecuteUpdateData);
        }
    }
}
