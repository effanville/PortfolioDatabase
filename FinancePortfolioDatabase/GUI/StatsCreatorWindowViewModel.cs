using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;
using GUIAccessorFunctions;
using GUIFinanceStructures;
using GUISupport;
using PortfolioStatsCreatorHelper;

namespace FinanceWindowsViewModels
{
    public class StatsCreatorWindowViewModel : PropertyChangedBase
    {
        public ICommand CreateCSVStatsCommand { get; }
        public ICommand CloseCommand { get; }
        public ICommand CreateHTMLCommand { get; }

        private void ExecuteExportToCSVCommand(Object obj)
        {
            StreamWriter statsWriter = new StreamWriter("statistics.csv");
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

            statsWriter.Close();
        }

        private void ExecuteCreateHTMLCommand(Object obj)
        {
            PortfolioStatsCreators.TryCreateHTMLPage(GlobalHeldData.GlobalData.Finances, new List<string>());
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
        private List<BankAccountStatsHolder> fBankAccountStats;
        public List<BankAccountStatsHolder> BankAccountStats
        {
            get { return fBankAccountStats; }
            set { fBankAccountStats = value; OnPropertyChanged(); }
        }

        Action<bool> UpdateMainWindow;
        Action<string> windowToView;

        public StatsCreatorWindowViewModel(Action<bool> updateWindow, Action<string> pageViewChoice)
        {
            SecuritiesStats = DatabaseAccessor.GenerateSecurityStatistics();
            BankAccountStats = DatabaseAccessor.GenerateBankAccountStatistics();
            windowToView = pageViewChoice;
            UpdateMainWindow = updateWindow;
            CreateCSVStatsCommand = new BasicCommand(ExecuteExportToCSVCommand);
            CreateHTMLCommand = new BasicCommand(ExecuteCreateHTMLCommand);
            CloseCommand = new BasicCommand(ExecuteCloseCommand);
        }
    }
}
