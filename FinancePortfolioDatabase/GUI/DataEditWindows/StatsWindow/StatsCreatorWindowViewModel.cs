using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;
using FinancialStructures.DataStructures;
using GUIAccessorFunctions;
using FinancialStructures.GUIFinanceStructures;
using GUISupport;
using PortfolioStatsCreatorHelper;
using FinancialStructures.ReportingStructures;

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
        public ICommand CreateCSVStatsCommand { get; }

        public ICommand CreateInvestmentListCommand { get; }
        public ICommand CloseCommand { get; }
        public ICommand CreateHTMLCommand { get; }

        private void ExecuteExportToCSVCommand(Object obj)
        {
            SaveFileDialog saving = new SaveFileDialog();
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
                    string securitiesData = stats.Company + ", " + stats.Name + ", " + stats.LatestVal.ToString() + ", " + stats.CARTotal.ToString();
                    statsWriter.WriteLine(securitiesData);
                }
                statsWriter.WriteLine("");
                statsWriter.WriteLine("Bank Account Data");
                statsWriter.WriteLine("Company, Name, Latest Value");
                foreach (BankAccountStatsHolder stats in BankAccountStats)
                {
                    string BankAccData = stats.Company + ", " + stats.Name + ", " + stats.LatestVal.ToString();
                    statsWriter.WriteLine(BankAccData);
                }

                ErrorReports.AddReport($"Created csv statistics at ${saving.FileName}");
                statsWriter.Close();

            }
            else 
            {
                ErrorReports.AddGeneralReport(ReportType.Error, $"Was not able to create csv file at {saving.FileName}");
            }
            saving.Dispose();
        }

        private void ExecuteInvestmentListCommand(Object obj)
        {
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
                ErrorReports.AddReport($"Created Investment list page at {saving.FileName}.");
                statsWriter.Close();
            }
            else
            {
                ErrorReports.AddGeneralReport(ReportType.Error, $"Was not able to create Investment list page at {saving.FileName}");
            }
            saving.Dispose();
        }

        private void ExecuteCreateHTMLCommand(Object obj)
        {
            SaveFileDialog saving = new SaveFileDialog();
            if (saving.ShowDialog() == DialogResult.OK)
            {
                PortfolioStatsCreators.CreateHTMLPage(GlobalHeldData.GlobalData.Finances, new List<string>(), saving.FileName);
                ErrorReports.AddGeneralReport(ReportType.Report, "Created statistics page");
            }
            else 
            { 
                ErrorReports.AddGeneralReport(ReportType.Error, "Was not able to create HTML page in place specified."); 
            }
            saving.Dispose();
        }

        private void ExecuteCloseCommand(Object obj)
        {
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

        Action<bool> UpdateMainWindow;
        Action<string> windowToView;

        public void GenerateStatistics()
        {
            SecuritiesStats = DatabaseAccessor.GenerateSecurityStatistics();
            SecuritiesInvestments = DatabaseAccessor.AllSecuritiesInvestments();
            BankAccountStats = DatabaseAccessor.GenerateBankAccountStatistics();
        }


        public StatsCreatorWindowViewModel(Action<bool> updateWindow, Action<string> pageViewChoice)
        {
            SecStatsVisibility = true;
            SecInvestsVisibility = false;
            BankAccStatsVisibility = false;
            GenerateStatistics();
            windowToView = pageViewChoice;
            UpdateMainWindow = updateWindow;
            CreateCSVStatsCommand = new BasicCommand(ExecuteExportToCSVCommand);
            CreateInvestmentListCommand = new BasicCommand(ExecuteInvestmentListCommand);
            CreateHTMLCommand = new BasicCommand(ExecuteCreateHTMLCommand);
            CloseCommand = new BasicCommand(ExecuteCloseCommand);
        }
    }
}
