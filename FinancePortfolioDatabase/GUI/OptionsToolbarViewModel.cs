using FinanceWindows;
using FinancialStructures.Database;
using FinancialStructures.FinanceStructures;
using FinancialStructures.ReportingStructures;
using GlobalHeldData;
using GUIAccessorFunctions;
using GUISupport;
using PADGlobals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace FinanceWindowsViewModels
{
    public class OptionsToolbarViewModel : PropertyChangedBase
    {
        private Portfolio Portfolio;
        private List<Sector> Sectors;
        Action<bool> UpdateMainWindow;
        Action<ErrorReports> UpdateReports;

        public ICommand OpenHelpCommand { get; }
        public ICommand NewDatabaseCommand { get; }
        public ICommand SaveDatabaseCommand { get; }
        public ICommand LoadDatabaseCommand { get; }
        public ICommand UpdateDataCommand { get; }
        public ICommand RefreshCommand { get; }

        public OptionsToolbarViewModel(Portfolio portfolio, List<Sector> sectors, Action<bool> updateWindow, Action<ErrorReports> updateReports)
        {
            Portfolio = portfolio;
            Sectors = sectors;
            UpdateMainWindow = updateWindow;
            UpdateReports = updateReports;
            OpenHelpCommand = new BasicCommand(OpenHelpDocsCommand);

            NewDatabaseCommand = new BasicCommand(ExecuteNewDatabase);
            SaveDatabaseCommand = new BasicCommand(ExecuteSaveDatabase);
            LoadDatabaseCommand = new BasicCommand(ExecuteLoadDatabase);
            UpdateDataCommand = new BasicCommand(ExecuteUpdateData);
            RefreshCommand = new BasicCommand(RefreshData);
        }

        private void OpenHelpDocsCommand(Object obj)
        {
            var helpwindow = new HelpWindow();
            helpwindow.Show();
        }
        public void ExecuteNewDatabase(Object obj)
        {
            var reports = new ErrorReports();
            DatabaseAccessor.ClearPortfolio();
            DatabaseAccessor.SetFilePath("");
            DatabaseAccessor.LoadPortfolio(reports);
            UpdateMainWindow(true);
            if (reports.Any())
            {
                UpdateReports(reports);
            }
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

        public async void ExecuteLoadDatabase(Object obj)
        {
            var reports = new ErrorReports();
            OpenFileDialog openFile = new OpenFileDialog() { DefaultExt = "xml" };
            openFile.Filter = "XML Files|*.xml|All Files|*.*";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                DatabaseAccessor.SetFilePath(openFile.FileName);
                DatabaseAccessor.ClearPortfolio();
                await Task.Run(() => DatabaseAccessor.LoadPortfolio(reports));
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
            await DataUpdater.Downloader(Portfolio, Sectors, UpdateReports, reports).ConfigureAwait(false);

            if (reports.Any())
            {
                UpdateReports(reports);
            }

            UpdateMainWindow(false);
        }
    }
}
