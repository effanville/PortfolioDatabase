using FinancialStructures.Database;
using FinancialStructures.ReportingStructures;
using GUIAccessorFunctions;
using GUISupport;
using System;

namespace FinanceWindowsViewModels
{
    public class MainWindowViewModel : PropertyChangedBase
    {
        public MainWindowViewModel()
        {
            DatabaseAccessor.LoadPortfolio(new ErrorReports()); 
            OptionsToolbarCommands = new OptionsToolbarViewModel(UpdateWindow, UpdateSubWindow, UpdateReports);
            DataView = new BasicDataViewModel(UpdateWindow);
            SecurityEditViewModel = new SecurityEditWindowViewModel(UpdateWindow, UpdateReports);
            BankAccEditViewModel = new BankAccEditWindowViewModel(UpdateWindow, UpdateReports);
            SectorEditViewModel = new SectorEditWindowViewModel(UpdateWindow, UpdateReports);
            CurrencyEditViewModel = new CurrencyEditWindowViewModel(UpdateWindow, UpdateReports);
            StatsEditViewModel = new StatsCreatorWindowViewModel(UpdateWindow, UpdateReports);
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
                    DataView.DataUpdate();
                }

                UpdateSubWindowData(false);
            }
        }

        Action<bool> UpdateSubWindow => (val) => UpdateSubWindowData(val);

        public void UpdateSubWindowData(object obj)
        {
               SecurityEditViewModel.UpdateFundListBox();
               BankAccEditViewModel.UpdateAccountListBox();
                SectorEditViewModel.UpdateSectorListBox();
                StatsEditViewModel.GenerateStatistics();
            CurrencyEditViewModel.UpdateSectorListBox();
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
