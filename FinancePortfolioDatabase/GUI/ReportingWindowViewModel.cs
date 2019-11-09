using GUISupport;
using ReportingStructures;
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

        private int fIndexToDelete;

        public int IndexToDelete
        {
            get { return fIndexToDelete; }
            set { fIndexToDelete = value; OnPropertyChanged(); }
        }

        void ExecuteClearReports(Object obj)
        { 
            ErrorReports.Clear();
            Update();
        }

        void ExecuteClearSelectedReport(Object obj)
        {
            ErrorReports.RemoveReport(IndexToDelete);
            Update();
        }

        public ReportingWindowViewModel()
        {
            ClearReportsCommand = new BasicCommand(ExecuteClearReports);
            ClearSingleReportCommand = new BasicCommand(ExecuteClearSelectedReport);
            Update();
        }

        public void Update()
        {
            ReportsToView = null;
            ReportsToView = ErrorReports.GetReports();
        }
    }
}
