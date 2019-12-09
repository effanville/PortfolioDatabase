using GUISupport;
using FinancialStructures.ReportingStructures;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace FinanceWindowsViewModels
{
    public class ReportingWindowViewModel : PropertyChangedBase
    {
        public ICommand ClearReportsCommand { get; }

        public ICommand ClearSingleReportCommand { get; }

        private List<ErrorReport> fReportsToView;

        public List<ErrorReport> ReportsToView
        {
            get { return fReportsToView; }
            set { fReportsToView = value; OnPropertyChanged(); }
        }

        private ErrorReports fReports;
        
        public ErrorReports Reports
        { 
            get { return fReports; }
            set { fReports = value; OnPropertyChanged(); }
        }

        private int fIndexToDelete;

        public int IndexToDelete
        {
            get { return fIndexToDelete; }
            set { fIndexToDelete = value; OnPropertyChanged(); }
        }

        private void SyncReports()
        {
            ReportsToView = Reports.GetReports();
        }

        void ExecuteClearReports(Object obj)
        {
            Reports = new ErrorReports();
            SyncReports();
        }

        void ExecuteClearSelectedReport(Object obj)
        {
            Reports.RemoveReport(IndexToDelete);
            SyncReports();
        }

        public void UpdateReports(ErrorReports reports)
        {
            Reports.AddReports(reports);
            SyncReports();
        }

        public ReportingWindowViewModel()
        {
            Reports = new ErrorReports();
            ReportsToView = new List<ErrorReport>();
            ClearReportsCommand = new BasicCommand(ExecuteClearReports);
            ClearSingleReportCommand = new BasicCommand(ExecuteClearSelectedReport);
            SyncReports();
        }

    }
}
