using FinancialStructures.DataStructures;
using FinancialStructures.GUIFinanceStructures;
using FinancialStructures.ReportingStructures;
using GUISupport;
using PortfolioStatsCreatorHelper;
using GlobalHeldData;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Input;

namespace FinanceWindowsViewModels
{
    public class StatsOptionsViewModel : PropertyChangedBase
    {
        private bool fDisplayValueFunds = true;
        public bool displayValueFunds
        {
            get { return fDisplayValueFunds; }
            set { fDisplayValueFunds = value; OnPropertyChanged(); }
        }

        private bool fSpacing = true;
        public bool spacing
        {
            get { return fSpacing; }
            set { fSpacing = value; OnPropertyChanged(); }
        }

        private bool fColours = true;
        public bool colours
        {
            get { return fColours; }
            set { fColours = value; OnPropertyChanged(); }
        }

        private List<VisibleName> fColumnNames;

        public List<VisibleName> ColumnNames
        {
            get { return fColumnNames; }
            set { fColumnNames = value; OnPropertyChanged(); }
        }

        private List<VisibleName> fBankColumnNames;

        public List<VisibleName> BankColumnNames
        {
            get { return fBankColumnNames; }
            set { fBankColumnNames = value; OnPropertyChanged(); }
        }

        public ICommand ExportCommand { get; }

        private void ExecuteExportHTMLCommand(Object obj)
        {
            var reports = new ErrorReports();
            SaveFileDialog saving = new SaveFileDialog() { DefaultExt = ".html", FileName = GlobalHeldData.GlobalData.DatabaseName + "-HTMLStats.html", InitialDirectory = GlobalHeldData.GlobalData.fStatsDirectory };
            saving.Filter = "Html file|*.html|All files|*.*";
            string path = null;
            if (saving.ShowDialog() == DialogResult.OK)
            {
                path = saving.FileName;
                var selected = new List<string>();
                foreach (var column in ColumnNames)
                {
                    if (column.Visible || column.Name == "Name" || column.Name == "Company")
                    {
                        selected.Add(column.Name);
                    }
                }
                var BankSelected = new List<string>();
                foreach (var column in BankColumnNames)
                {
                    if (column.Visible || column.Name == "Name" || column.Name == "Company")
                    {
                        BankSelected.Add(column.Name);
                    }
                }
                var options = new UserOptions() { DisplayValueFunds = displayValueFunds, Spacing = spacing, Colours = colours, SecurityDataToExport = selected, BankAccDataToExport = BankSelected};
                PortfolioStatsCreators.CreateHTMLPageCustom(GlobalData.Finances, GlobalData.BenchMarks, saving.FileName, options);
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

            CloseWindowAction(path);
        }

        private void ExecuteExportCSVCommand(Object obj)
        {
            var reports = new ErrorReports();
            SaveFileDialog saving = new SaveFileDialog() { DefaultExt = ".csv", FileName = GlobalHeldData.GlobalData.DatabaseName + "-CSVStats.csv", InitialDirectory = GlobalHeldData.GlobalData.fStatsDirectory };
            saving.Filter = "Csv file|*.csv|All files|*.*";
            string path = null;
            if (saving.ShowDialog() == DialogResult.OK)
            {
                path = saving.FileName;
                var selected = new List<string>();
                foreach (var column in ColumnNames)
                {
                    if (column.Visible || column.Name == "Name" || column.Name == "Company")
                    {
                        selected.Add(column.Name);
                    }
                }
                var BankSelected = new List<string>();
                foreach (var column in BankColumnNames)
                {
                    if (column.Visible || column.Name == "Name" || column.Name == "Company")
                    {
                        BankSelected.Add(column.Name);
                    }
                }
                var options = new UserOptions() { DisplayValueFunds = displayValueFunds, Spacing = spacing, Colours = colours, SecurityDataToExport = selected, BankAccDataToExport = BankSelected };
                CSVStatsCreator.CreateCSVPageCustom(GlobalData.Finances, GlobalData.BenchMarks, saving.FileName, options);
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

            CloseWindowAction(path);
        }


        Action<ErrorReports> UpdateReports;
        private Action<string> CloseWindowAction;

        public StatsOptionsViewModel(ExportType exportType, Action<ErrorReports> updateReports, Action<string> CloseWindow)
        {
            UpdateReports = updateReports;
            CloseWindowAction = CloseWindow;
            if (exportType == ExportType.HTML)
            {
                ExportCommand = new BasicCommand(ExecuteExportHTMLCommand);
            }
            if (exportType == ExportType.CSV)
            {
                ExportCommand = new BasicCommand(ExecuteExportCSVCommand);
            }
            
            var totals = new SecurityStatsHolder();
            var properties = totals.GetType().GetProperties();
            ColumnNames = new List<VisibleName>();
            foreach (var info in properties)
            {
                ColumnNames.Add(new VisibleName(info.Name, true));
            }

            var BankNames = new DailyValuation_Named();
            var props = BankNames.GetType().GetProperties();
            BankColumnNames = new List<VisibleName>();
            foreach (var info in props)
            {
                if (info.Name == "Day")
                { 
                    BankColumnNames.Add(new VisibleName(info.Name, false)); 
                }
                else
                {
                    BankColumnNames.Add(new VisibleName(info.Name, true));
                }
            }
        }
    }

    public class VisibleName
    {
        public string Name { get; set; }
        public bool Visible { get; set; }
        public VisibleName()
        { }

        public VisibleName(string name, bool vis)
        {
            Visible = vis;
            Name = name;
        }
    }
}
