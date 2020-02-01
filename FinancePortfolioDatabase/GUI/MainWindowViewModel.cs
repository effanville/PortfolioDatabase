using FinancialStructures.ReportingStructures;
using GlobalHeldData;
using DatabaseAccess;
using GUISupport;
using System;

namespace FinanceWindowsViewModels
{
    public class MainWindowViewModel : PropertyChangedBase
    {
        public MainWindowViewModel()
        {
            DatabaseEdit.LoadPortfolio(new ErrorReports());
            OptionsToolbarCommands = new OptionsToolbarViewModel(GlobalData.Finances, GlobalData.BenchMarks, UpdateWindow, UpdateReports);
            DataView = new BasicDataViewModel(GlobalData.Finances, GlobalData.BenchMarks);
            SecurityEditViewModel = new SecurityEditWindowViewModel(GlobalData.Finances, GlobalData.BenchMarks, UpdateWindow, UpdateReports);
            BankAccEditViewModel = new BankAccEditWindowViewModel(GlobalData.Finances, GlobalData.BenchMarks, UpdateWindow, UpdateReports);
            SectorEditViewModel = new SectorEditWindowViewModel(GlobalData.Finances, GlobalData.BenchMarks, UpdateWindow, UpdateReports);
            CurrencyEditViewModel = new CurrencyEditWindowViewModel(GlobalData.Finances, GlobalData.BenchMarks, UpdateWindow, UpdateReports);
            StatsEditViewModel = new StatsCreatorWindowViewModel(GlobalData.Finances, GlobalData.BenchMarks, UpdateWindow, UpdateReports);
            ReportsViewModel = new ReportingWindowViewModel();
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
                    DataView.DataUpdate(GlobalData.Finances, GlobalData.BenchMarks);
                }

                UpdateSubWindowData(false);
            }
        }

        public void UpdateSubWindowData(object obj)
        {
            SecurityEditViewModel.UpdateFundListBox(GlobalData.Finances, GlobalData.BenchMarks);
            BankAccEditViewModel.UpdateAccountListBox(GlobalData.Finances, GlobalData.BenchMarks);
            SectorEditViewModel.UpdateSectorListBox(GlobalData.Finances, GlobalData.BenchMarks);
            StatsEditViewModel.GenerateStatistics(GlobalData.Finances, GlobalData.BenchMarks);
            CurrencyEditViewModel.UpdateSectorListBox(GlobalData.Finances, GlobalData.BenchMarks);
        }

        private OptionsToolbarViewModel fOptionsToolbarCommands;

        public OptionsToolbarViewModel OptionsToolbarCommands
        {
            get { return fOptionsToolbarCommands; }
            set { fOptionsToolbarCommands = value; OnPropertyChanged(); }
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

        private CurrencyEditWindowViewModel fCurrencyEditViewModel;
        public CurrencyEditWindowViewModel CurrencyEditViewModel
        {
            get { return fCurrencyEditViewModel; }
            set { fCurrencyEditViewModel = value; OnPropertyChanged(); }
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
