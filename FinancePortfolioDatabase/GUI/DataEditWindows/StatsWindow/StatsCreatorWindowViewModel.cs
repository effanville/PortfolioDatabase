using FinancialStructures.DataStructures;
using FinancialStructures.GUIFinanceStructures;
using FinancialStructures.ReportingStructures;
using GUIAccessorFunctions;
using GUISupport;
using PortfolioStatsCreatorHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;

namespace FinanceWindowsViewModels
{
    public class StatsCreatorWindowViewModel : PropertyChangedBase
    {
        private bool fSecStatsVisibility;

        public bool SecStatsVisibility
        {
            get { return fSecStatsVisibility; }
            set { fSecStatsVisibility = value; OnPropertyChanged(); }
        }

        private bool fSecInvestsVisibility;

        public bool SecInvestsVisibility
        {
            get { return fSecInvestsVisibility; }
            set { fSecInvestsVisibility = value; OnPropertyChanged(); }
        }

        private bool fBankAccStatsVisibility;

        public bool BankAccStatsVisibility
        {
            get { return fBankAccStatsVisibility; }
            set { fBankAccStatsVisibility = value; OnPropertyChanged(); }
        }

        private bool fDatabaseStatsVisibility;
        public bool DatabaseStatsVisibility
        {
            get { return fDatabaseStatsVisibility; }
            set { fDatabaseStatsVisibility = value; OnPropertyChanged(); }
        }

        private bool fDisplayValueFunds = true;
        public bool DisplayValueFunds
        {
            get { return fDisplayValueFunds; }
            set { fDisplayValueFunds = value; OnPropertyChanged(); GenerateStatistics(); }
        }
        public ICommand CreateCSVStatsCommand { get; }

        public ICommand CreateInvestmentListCommand { get; }
        public ICommand CloseCommand { get; }
        public ICommand CreateHTMLCommand { get; }

        private void ExecuteExportToCSVCommand(Object obj)
        {
            var reports = new ErrorReports();
            SaveFileDialog saving = new SaveFileDialog();
            saving.DefaultExt = ".csv";
            if (saving.ShowDialog() == DialogResult.OK)
            {
                if (!saving.FileName.EndsWith(".csv"))
                {
                    saving.FileName += ".csv";
                }
                StreamWriter statsWriter = new StreamWriter(saving.FileName);
                // write in column headers
                statsWriter.WriteLine("Securities Data");
                statsWriter.WriteLine("Company, Name, Latest Value, CAR total");
                foreach (SecurityStatsHolder stats in SecuritiesStats)
                {
                    if (stats.LatestVal > 0)
                    {
                        string securitiesData = stats.Company + ", " + stats.Name + ", " + stats.LatestVal.ToString() + ", " + stats.CARTotal.ToString();
                        statsWriter.WriteLine(securitiesData);
                    }
                }
                statsWriter.WriteLine("");
                statsWriter.WriteLine("Bank Account Data");
                statsWriter.WriteLine("Company, Name, Latest Value");
                foreach (BankAccountStatsHolder stats in BankAccountStats)
                {
                    string BankAccData = stats.Company + ", " + stats.Name + ", " + stats.LatestVal.ToString();
                    statsWriter.WriteLine(BankAccData);
                }

                reports.AddReport($"Created csv statistics at ${saving.FileName}");
                statsWriter.Close();

            }
            else
            {
                reports.AddGeneralReport(ReportType.Error, $"Was not able to create csv file at {saving.FileName}");
            }
            saving.Dispose();
            if (reports.Any())
            {
                UpdateReports(reports);
            }
        }

        private void ExecuteInvestmentListCommand(Object obj)
        {
            var reports = new ErrorReports();
            SaveFileDialog saving = new SaveFileDialog();
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
            var reports = new ErrorReports();
            SaveFileDialog saving = new SaveFileDialog();
            if (saving.ShowDialog() == DialogResult.OK)
            {
                PortfolioStatsCreators.CreateHTMLPage(GlobalHeldData.GlobalData.Finances, new List<string>(), saving.FileName, DisplayValueFunds);
                reports.AddGeneralReport(ReportType.Report, "Created statistics page");
            }
            else
            {
                reports.AddGeneralReport(ReportType.Error, "Was not able to create HTML page in place specified.");
            }
            saving.Dispose();
            if (reports.Any())
            {
                UpdateReports(reports);
            }
        }

        private void ExecuteCloseCommand(Object obj)
        {
            UpdateMainWindow(false);
            windowToView("dataview");
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
        Action<string> windowToView;
        Action<ErrorReports> UpdateReports;

        public void GenerateStatistics()
        {
            SecuritiesStats = DatabaseAccessor.GenerateSecurityStatistics(DisplayValueFunds);
            SecuritiesInvestments = DatabaseAccessor.AllSecuritiesInvestments();
            BankAccountStats = DatabaseAccessor.GenerateBankAccountStatistics(DisplayValueFunds);
            DatabaseStats = DatabaseAccessor.GenerateDatabaseStatistics();
        }


        public StatsCreatorWindowViewModel(Action<bool> updateWindow, Action<string> pageViewChoice, Action<ErrorReports> updateReports)
        {
            SecStatsVisibility = true;
            SecInvestsVisibility = false;
            BankAccStatsVisibility = false;
            DatabaseStatsVisibility = false;
            GenerateStatistics();
            windowToView = pageViewChoice;
            UpdateMainWindow = updateWindow;
            UpdateReports = updateReports;
            CreateCSVStatsCommand = new BasicCommand(ExecuteExportToCSVCommand);
            CreateInvestmentListCommand = new BasicCommand(ExecuteInvestmentListCommand);
            CreateHTMLCommand = new BasicCommand(ExecuteCreateHTMLCommand);
            CloseCommand = new BasicCommand(ExecuteCloseCommand);
        }
    }
}
