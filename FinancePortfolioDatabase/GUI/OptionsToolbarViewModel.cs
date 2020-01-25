using FinanceWindows;
using System;
using GUISupport;
using System.Windows.Input;
using GUIAccessorFunctions;
using System.Windows.Forms;
using FinancialStructures.ReportingStructures;
using GlobalHeldData;
using System.IO;
using PADGlobals;
using System.Collections.Generic;

namespace FinanceWindowsViewModels
{
    public class OptionsToolbarViewModel :PropertyChangedBase
    {
        public OptionsToolbarViewModel(Action<bool> updateWindow, Action<bool> updateSubWindow, Action<ErrorReports> updateReports)
        {
            UpdateMainWindow = updateWindow;
            UpdateSubWindows = updateSubWindow;
            UpdateReports = updateReports;
            OpenHelpCommand = new BasicCommand(OpenHelpDocsCommand);
            SaveDatabaseCommand = new BasicCommand(ExecuteSaveDatabase);
            LoadDatabaseCommand = new BasicCommand(ExecuteLoadDatabase);
            UpdateDataCommand = new BasicCommand(ExecuteUpdateData);
            RefreshCommand = new BasicCommand(RefreshData);
        }

        Action<bool> UpdateMainWindow;
        Action<bool> UpdateSubWindows;
        Action<ErrorReports> UpdateReports;

        public ICommand OpenHelpCommand { get; }

        public ICommand SaveDatabaseCommand { get; }

        public ICommand LoadDatabaseCommand { get; }
        public ICommand UpdateDataCommand { get; }
        public ICommand RefreshCommand { get; }

        private void OpenHelpDocsCommand(Object obj)
        {
            var helpwindow = new HelpWindow();
            helpwindow.Show();
        }

        public void ExecuteSaveDatabase(Object obj)
        {
            var reports = new ErrorReports();
            SaveFileDialog saving = new SaveFileDialog() { DefaultExt = "xml", FileName = GlobalData.DatabaseName + Path.GetExtension(GlobalData.fDatabaseFilePath), InitialDirectory = Path.GetDirectoryName(GlobalData.fDatabaseFilePath) };
            saving.Filter = "XML Files|*.xml|All Files|*.*";
            if (saving.ShowDialog() == DialogResult.OK)
            {
                DatabaseAccessor.SetFilePath(saving.FileName);
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
            OpenFileDialog openFile = new OpenFileDialog() { DefaultExt = "xml" };
            openFile.Filter = "XML Files|*.xml|All Files|*.*";
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
        }

        private void RefreshData(object obj)
        {
            UpdateMainWindow(false);
        }

        private async void ExecuteUpdateData(Object obj)
        {
            var reports = new ErrorReports();
            await DataUpdater.Downloader(reports).ConfigureAwait(false);

            if (reports.Any())
            {
                UpdateReports(reports);
            }

            UpdateMainWindow(false);
        }
    }
}
