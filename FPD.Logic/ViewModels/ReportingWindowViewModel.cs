using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using Effanville.Common.Structure.Reporting;
using Effanville.Common.UI;
using Effanville.Common.UI.Commands;
using Effanville.Common.UI.Services;
using Effanville.Common.UI.ViewModelBases;
using Effanville.FPD.Logic.TemplatesAndStyles;

namespace Effanville.FPD.Logic.ViewModels
{
    /// <summary>
    /// View model for the user control that displays reports.
    /// </summary>
    public class ReportingWindowViewModel : ViewModelBase<ErrorReports>
    {
        private List<ErrorReport> _reportsToView = new List<ErrorReport>();

        private UiStyles _styles;

        /// <summary>
        /// The style object containing the style for the ui.
        /// </summary>
        public UiStyles Styles
        {
            get => _styles;
            set => SetAndNotify(ref _styles, value);
        }

        /// <summary>
        /// The reports to display in the control. This is a sublist of <see cref="ReportingWindowViewModel.ModelData"/> filtered by <see cref="ReportingSeverity"/>.
        /// </summary>
        public List<ErrorReport> ReportsToView
        {
            get => _reportsToView;
            set => SetAndNotify(ref _reportsToView, value);
        }

        private bool _isExpanded;

        /// <summary>
        /// Whether the view is expanded or not.
        /// </summary>
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetAndNotify(ref _isExpanded, value);
        }

        private ReportSeverity _reportingSeverity;

        /// <summary>
        /// The selected reporting severity.
        /// </summary>
        public ReportSeverity ReportingSeverity
        {
            get => _reportingSeverity;
            set
            {
                SetAndNotify(ref _reportingSeverity, value);
                SyncReports();
            }
        }

        /// <summary>
        /// List of all types of ReportSeverity.
        /// </summary>
        public static List<ReportSeverity> ReportSeverityValues =>
            Enum.GetValues(typeof(ReportSeverity)).Cast<ReportSeverity>().ToList();

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ReportingWindowViewModel(UiGlobals uiGlobals, UiStyles styles)
            : base("Reports", new ErrorReports(), uiGlobals)
        {
            Styles = styles;
            IsExpanded = false;
            ClearReportsCommand = new RelayCommand(ExecuteClearReports);
            ExportReportsCommand = new RelayCommand(ExecuteExportReportsCommand);
        }

        internal void SyncReports()
        {
            ReportsToView = null;
            ReportsToView = ModelData.GetReports(ReportingSeverity).ToList();
            if (ReportsToView != null && (ReportsToView?.Any() ?? false))
            {
                IsExpanded = true;
            }
        }

        /// <summary>
        /// Command to clear all reports in the control.
        /// </summary>
        public ICommand ClearReportsCommand { get; }

        private void ExecuteClearReports()
        {
            ModelData.Clear();
            SyncReports();
        }

        /// <summary>
        /// Command to export reports to a csv.
        /// </summary>
        public ICommand ExportReportsCommand { get; }

        private void ExecuteExportReportsCommand() => _ = Task.Factory.StartNew(ExecuteExportReports);

        private void ExecuteExportReports()
        {
            try
            {
                FileInteractionResult result =
                    DisplayGlobals.FileInteractionService.SaveFile(".csv", "errorReports.csv");
                if (!result.Success)
                {
                    return;
                }

                ModelData.Save(result.FilePath, DisplayGlobals.CurrentFileSystem);
            }
            catch (IOException ex)
            {
                DisplayGlobals.CurrentDispatcher.Invoke(() => ModelData.AddErrorReport(ReportSeverity.Critical,
                    ReportType.Error, ReportLocation.Saving, $"Error when saving reports: {ex.Message}"));
            }
        }
        
        /// <summary>
        /// Command to delete a selected report.
        /// </summary>
        public void DeleteReports(IList<ErrorReport> reports)
        {
            int numberReports = reports.Count;
            if (ModelData.RemoveReports(reports) != numberReports)
            {
                ReportLogger.Log(ReportType.Error,"Reports","Could not find error report to remove.");
            }
            SyncReports();
        }
        
        /// <summary>
        /// Command to delete a selected report.
        /// </summary>
        public void DeleteReport(ErrorReport report)
        {
            if (!ModelData.RemoveReport(report))
            {
                ReportLogger.Log(ReportType.Error,"Reports","Could not find error report to remove.");
            }
            SyncReports();
        }

        /// <summary>
        /// Updates the reports in the window.
        /// </summary>
        public void UpdateReport(ReportSeverity severity, ReportType type, string location, string message)
        {
            ModelData.AddErrorReport(severity, type, location, message);
            SyncReports();
        }
    }
}