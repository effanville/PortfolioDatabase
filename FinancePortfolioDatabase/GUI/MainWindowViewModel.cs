using FinancialStructures.ReportingStructures;
using GUISupport;
using System;

namespace FinanceWindowsViewModels
{
    public class MainWindowViewModel : PropertyChangedBase
    {
        private bool fDataWindowVisibility;
        public bool DataWindowVisibility
        {
            get { return fDataWindowVisibility; }
            set { fDataWindowVisibility = value; OnPropertyChanged(); }
        }

        private bool fSecurityEditWindowVisibility;
        public bool SecurityEditWindowVisibility
        {
            get { return fSecurityEditWindowVisibility; }
            set { fSecurityEditWindowVisibility = value; OnPropertyChanged(); }
        }

        private bool fBankAccEditWindowVisibility;
        public bool BankAccEditWindowVisibility
        {
            get { return fBankAccEditWindowVisibility; }
            set { fBankAccEditWindowVisibility = value; OnPropertyChanged(); }
        }

        private bool fSectorEditWindowVisibility;
        public bool SectorEditWindowVisibility
        {
            get { return fSectorEditWindowVisibility; }
            set { fSectorEditWindowVisibility = value; OnPropertyChanged(); }
        }
        private bool fStatsEditWindowVisibility;
        public bool StatsEditWindowVisibility
        {
            get { return fStatsEditWindowVisibility; }
            set { fStatsEditWindowVisibility = value; OnPropertyChanged(); }
        }

        public MainWindowViewModel()
        {
            OptionsPanelCommands = new OptionsPanelViewModel(UpdateWindow, UpdateSubWindow, displayWindowChoice, UpdateReports);
            DataView = new BasicDataViewModel(UpdateWindow);
            ReportsViewModel = new ReportingWindowViewModel();
            DataWindowVisibility = true;
            SecurityEditWindowVisibility = false;
        }
        Action<ErrorReports> UpdateReports => (val) => AddReports(val);

        public void AddReports(ErrorReports reports)
        {
            ReportsViewModel.UpdateReports(reports);
        }

        Action<bool> UpdateWindow => (val) => UpdateData(val);

        public void UpdateData(object obj)
        {
            if (obj is bool updateReportsOnly)
            {
                if (!updateReportsOnly)
                {
                    DataView.DataUpdate();
                }

                UpdateSubWindowData(false);
            }
        }

        Action<bool> UpdateSubWindow => (val) => UpdateSubWindowData(val);

        public void UpdateSubWindowData(object obj)
        {
            if (SecurityEditViewModel != null)
            {
                SecurityEditViewModel.UpdateFundListBox();
            }
            if (BankAccEditViewModel != null)
            {
                BankAccEditViewModel.UpdateAccountListBox();
            }
            if (SectorEditViewModel != null)
            {
                SectorEditViewModel.UpdateSectorListBox();
            }
            if (StatsEditViewModel != null)
            {
                StatsEditViewModel.GenerateStatistics();
            }
        }

        Action<string> displayWindowChoice => (name) => WindowDisplayChoice(name);

        private void WindowDisplayChoice(string windowName)
        {
            switch (windowName)
            {
                case "SecurityEditWindow":
                    OpenSecurityEditWindow();
                    break;
                case "BankAccEditWindow":
                    OpenBankAccEditWindow();
                    break;
                case "SectorEditWindow":
                    OpenSectorEditWindow();
                    break;
                case "StatsCreatorWindow":
                    OpenStatsCreatorWindow();
                    break;
                default:
                    ShowDataView();
                    break;
            }
        }

        private void ShowDataView()
        {
            UpdateData(true);
            SecurityEditViewModel = null;
            SectorEditViewModel = null;
            BankAccEditViewModel = null;
            StatsEditViewModel = null;
            DataWindowVisibility = true;
            SecurityEditWindowVisibility = false;
            SectorEditWindowVisibility = false;
            BankAccEditWindowVisibility = false;
            StatsEditWindowVisibility = false;
        }

        private void OpenSecurityEditWindow()
        {
            SecurityEditViewModel = new SecurityEditWindowViewModel(UpdateWindow, displayWindowChoice, UpdateReports);
            DataWindowVisibility = false;
            SectorEditWindowVisibility = false;
            BankAccEditWindowVisibility = false;
            StatsEditWindowVisibility = false;
            SecurityEditWindowVisibility = true;
        }

        private void OpenBankAccEditWindow()
        {
            BankAccEditViewModel = new BankAccEditWindowViewModel(UpdateWindow, displayWindowChoice, UpdateReports);
            DataWindowVisibility = false;
            SectorEditWindowVisibility = false;
            SecurityEditWindowVisibility = false;
            StatsEditWindowVisibility = false;
            BankAccEditWindowVisibility = true;
        }
        private void OpenSectorEditWindow()
        {
            SectorEditViewModel = new SectorEditWindowViewModel(UpdateWindow, displayWindowChoice, UpdateReports);
            DataWindowVisibility = false;
            SecurityEditWindowVisibility = false;
            BankAccEditWindowVisibility = false;
            StatsEditWindowVisibility = false;
            SectorEditWindowVisibility = true;
        }

        private void OpenStatsCreatorWindow()
        {
            StatsEditViewModel = new StatsCreatorWindowViewModel(UpdateWindow, displayWindowChoice, UpdateReports);
            DataWindowVisibility = false;
            SecurityEditWindowVisibility = false;
            BankAccEditWindowVisibility = false;
            SectorEditWindowVisibility = false;
            StatsEditWindowVisibility = true;
        }

        private OptionsPanelViewModel fOptionsPanelCommands;

        public OptionsPanelViewModel OptionsPanelCommands
        {
            get { return fOptionsPanelCommands; }
            set { fOptionsPanelCommands = value; OnPropertyChanged(); }
        }
        private BasicDataViewModel fDataView;

        public BasicDataViewModel DataView
        {
            get { return fDataView; }
            set { fDataView = value; OnPropertyChanged(); }
        }

        private SecurityEditWindowViewModel fSecurityEditViewModel;
        public SecurityEditWindowViewModel SecurityEditViewModel
        {
            get { return fSecurityEditViewModel; }
            set { fSecurityEditViewModel = value; OnPropertyChanged(); }
        }

        private SectorEditWindowViewModel fSectorEditViewModel;

        public SectorEditWindowViewModel SectorEditViewModel
        {
            get { return fSectorEditViewModel; }
            set { fSectorEditViewModel = value; OnPropertyChanged(); }
        }

        private BankAccEditWindowViewModel fBankAccEditViewModel;

        public BankAccEditWindowViewModel BankAccEditViewModel
        {
            get { return fBankAccEditViewModel; }
            set { fBankAccEditViewModel = value; OnPropertyChanged(); }
        }

        private StatsCreatorWindowViewModel fStatsViewModel;

        public StatsCreatorWindowViewModel StatsEditViewModel
        {
            get { return fStatsViewModel; }
            set { fStatsViewModel = value; OnPropertyChanged(); }
        }

        private ReportingWindowViewModel fReports;
        public ReportingWindowViewModel ReportsViewModel
        {
            get { return fReports; }
            set { fReports = value; OnPropertyChanged(); }
        }
    }
}
