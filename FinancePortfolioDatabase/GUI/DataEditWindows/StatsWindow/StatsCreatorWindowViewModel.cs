using FinancialStructures.DataStructures;
using FinancialStructures.GUIFinanceStructures;
using FinancialStructures.ReportingStructures;
using FinancialStructures.Database;
using GlobalHeldData;
using GUIAccessorFunctions;
using GUISupport;
using FinanceWindows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;
using PortfolioStatsCreatorHelper;

namespace FinanceWindowsViewModels
{
    public class StatsCreatorWindowViewModel : PropertyChangedBase
    {
        private bool fDisplayValueFunds = true;
        public bool DisplayValueFunds
        {
            get { return fDisplayValueFunds; }
            set { fDisplayValueFunds = value; OnPropertyChanged(); GenerateStatistics(); }
        }

        private Uri fDisplayStats;

        public Uri DisplayStats
        {
            get { return fDisplayStats; }
            set { fDisplayStats = value; OnPropertyChanged(); }
        }

        private string fStatsFilepath;

        public string StatsFilepath
        {
            get { return fStatsFilepath; }
            set { fStatsFilepath = value; OnPropertyChanged(); DisplayStats = new Uri(fStatsFilepath);  }
        }

        private int fSelectedIndex;

        public int SelectedIndex
        {
            get { return fSelectedIndex; }
            set { fSelectedIndex = value; OnPropertyChanged(); }
        }

        public ICommand CreateCSVStatsCommand { get; }

        public ICommand CreateInvestmentListCommand { get; }
        public ICommand CreateHTMLCommand { get; }

        public ICommand FileSelect { get; }

        private void ExecuteExportToCSVCommand(Object obj)
        {
            var optionWindow = new StatsOptionsWindow();
            Action<string> StatsOptionFeedback = (filePath) => StatsFeedback(optionWindow, filePath);
            var context = new StatsOptionsViewModel(ExportType.CSV, UpdateReports, StatsOptionFeedback);
            optionWindow.DataContext = context;
            optionWindow.ShowDialog();
        }

        private void ExecuteInvestmentListCommand(Object obj)
        {
            var reports = new ErrorReports();
            SaveFileDialog saving = new SaveFileDialog() { DefaultExt = ".csv", FileName = GlobalHeldData.GlobalData.DatabaseName + "-CSVStats.csv", InitialDirectory = GlobalHeldData.GlobalData.fStatsDirectory };
            saving.Filter = "CSV file|*.csv|All files|*.*";
            if (saving.ShowDialog() == DialogResult.OK)
            {
                if (!saving.FileName.EndsWith(".csv"))
                {
                    saving.FileName += ".csv";
                }
                StreamWriter statsWriter = new StreamWriter(saving.FileName);
                // write in column headers
                statsWriter.WriteLine("Securities Investments");
                statsWriter.WriteLine("Date, Company, Name, Investment Amount");
                foreach (var stats in SecuritiesInvestments)
                {
                    string securitiesData = stats.Day.ToShortDateString() + ", " + stats.Company + ", " + stats.Name + ", " + stats.Value.ToString();
                    statsWriter.WriteLine(securitiesData);
                }
                reports.AddReport($"Created Investment list page at {saving.FileName}.");
                statsWriter.Close();
            }
            else
            {
                reports.AddGeneralReport(ReportType.Error, $"Was not able to create Investment list page at {saving.FileName}");
            }
            saving.Dispose();
            if (reports.Any())
            {
                UpdateReports(reports);
            }
        }

        private void ExecuteCreateHTMLCommand(Object obj)
        {
            var optionWindow = new StatsOptionsWindow();
            Action<string> StatsOptionFeedback = (filePath) => StatsFeedback(optionWindow, filePath);
            var context = new StatsOptionsViewModel(ExportType.HTML, UpdateReports, StatsOptionFeedback);
            optionWindow.DataContext = context;
            optionWindow.ShowDialog();
        }
        private void StatsFeedback(StatsOptionsWindow window, string filePath)
        {
            StatsFilepath = filePath;
            window.Close();
            SelectedIndex = 5;
        }

        private void ExecuteFileSelect(Object obj)
        {
            OpenFileDialog fileSelect = new OpenFileDialog();
            fileSelect.Filter = "HTML file|*.html;*.htm|All files|*.*";
            if (fileSelect.ShowDialog() == DialogResult.OK)
            {
                StatsFilepath = fileSelect.FileName;
            }
        }

        private List<SecurityStatsHolder> fSecuritiesStats;

        public List<SecurityStatsHolder> SecuritiesStats
        {
            get { return fSecuritiesStats; }
            set { fSecuritiesStats = value; OnPropertyChanged(); }
        }

        private List<DailyValuation_Named> fSecuritiesInvestments;

        public List<DailyValuation_Named> SecuritiesInvestments
        {
            get { return fSecuritiesInvestments; }
            set { fSecuritiesInvestments = value; OnPropertyChanged(); }
        }

        private List<BankAccountStatsHolder> fBankAccountStats;
        public List<BankAccountStatsHolder> BankAccountStats
        {
            get { return fBankAccountStats; }
            set { fBankAccountStats = value; OnPropertyChanged(); }
        }

        private List<DatabaseStatistics> fDatabaseStats;
        public List<DatabaseStatistics> DatabaseStats
        {
            get { return fDatabaseStats; }
            set { fDatabaseStats = value; OnPropertyChanged(); }
        }

        Action<bool> UpdateMainWindow;
        Action<ErrorReports> UpdateReports;

        public void GenerateStatistics()
        {
            SecuritiesStats = GlobalData.Finances.GenerateSecurityStatistics(DisplayValueFunds);
            SecuritiesInvestments = GlobalData.Finances.AllSecuritiesInvestments();
            BankAccountStats = GlobalData.Finances.GenerateBankAccountStatistics(DisplayValueFunds);
            DatabaseStats = GlobalData.Finances.GenerateDatabaseStatistics();
        }

        public StatsCreatorWindowViewModel(Action<bool> updateWindow, Action<ErrorReports> updateReports)
        {
            GenerateStatistics();
            UpdateMainWindow = updateWindow;
            UpdateReports = updateReports;
            CreateCSVStatsCommand = new BasicCommand(ExecuteExportToCSVCommand);
            CreateInvestmentListCommand = new BasicCommand(ExecuteInvestmentListCommand);
            CreateHTMLCommand = new BasicCommand(ExecuteCreateHTMLCommand);
            FileSelect = new BasicCommand(ExecuteFileSelect);
            SelectedIndex = 0;
        }
    }
}
