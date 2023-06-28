using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using Common.Structure.Reporting;
using Common.UI;
using Common.UI.Commands;
using Common.UI.Services;
using Common.UI.ViewModelBases;

using FPD.Logic.TemplatesAndStyles;

namespace FPD.Logic.ViewModels
{
    /// <summary>
    /// View model for the user control that displays reports.
    /// </summary>
    public class ReportingWindowViewModel : PropertyChangedBase
    {
        private List<ErrorReport> fReportsToView = new List<ErrorReport>();
        private readonly UiGlobals fUiGlobals;

        private UiStyles fStyles;

        /// <summary>
        /// The style object containing the style for the ui.
        /// </summary>
        public UiStyles Styles
        {
            get => fStyles;
            set => SetAndNotify(ref fStyles, value, nameof(Styles));
        }

        /// <summary>
        /// The reports to display in the control. This is a sublist of <see cref="Reports"/> filtered by <see cref="ReportingSeverity"/>.
        /// </summary>
        public List<ErrorReport> ReportsToView
        {
            get => fReportsToView;
            set => SetAndNotify(ref fReportsToView, value, nameof(ReportsToView));
        }

        private bool fIsExpanded;
        /// <summary>
        /// Whether the view is expanded or not.
        /// </summary>
        public bool IsExpanded
        {
            get => fIsExpanded;
            set => SetAndNotify(ref fIsExpanded, value, nameof(IsExpanded));
        }

        /// <summary>
        /// Complete list of all reports stored.
        /// </summary>
        public ErrorReports Reports { get; set; }

        /// <summary>
        /// Selected index of the reports.
        /// </summary>
        public int IndexToDelete { get; set; }

        private ReportSeverity fReportingSeverity;

        /// <summary>
        /// The selected reporting severity.
        /// </summary>
        public ReportSeverity ReportingSeverity
        {
            get => fReportingSeverity;
            set
            {
                SetAndNotify(ref fReportingSeverity, value, nameof(ReportingSeverity));
                SyncReports();
            }
        }

        /// <summary>
        /// List of all types of ReportSeverity.
        /// </summary>
        public static List<ReportSeverity> ReportSeverityValues => Enum.GetValues(typeof(ReportSeverity)).Cast<ReportSeverity>().ToList();

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ReportingWindowViewModel(UiGlobals uiGlobals, UiStyles styles)
        {
            Styles = styles;
            IsExpanded = false;
            fUiGlobals = uiGlobals;
            Reports = new ErrorReports();
            ClearReportsCommand = new RelayCommand(ExecuteClearReports);
            ExportReportsCommand = new RelayCommand(ExecuteExportReportsCommand);
            SyncReports();
        }

        internal void SyncReports()
        {
            ReportsToView = null;
            ReportsToView = Reports.GetReports(ReportingSeverity).ToList();
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
            Reports.Clear();
            SyncReports();
        }

        /// <summary>
        /// Command to export reports to a csv.
        /// </summary>
        public ICommand ExportReportsCommand { get; }
        private void ExecuteExportReportsCommand() => _ = Task.Factory.StartNew(() => ExecuteExportReports());

        private void ExecuteExportReports()
        {
            try
            {
                FileInteractionResult result = fUiGlobals.FileInteractionService.SaveFile(".csv", "errorReports.csv");
                if (result.Success)
                {
                    using (StreamWriter writer = new StreamWriter(result.FilePath))
                    {
                        writer.WriteLine("Severity,ErrorType,Location,Message");
                        foreach (ErrorReport report in Reports.GetReports())
                        {
                            writer.WriteLine($"{report.ErrorSeverity},{report.ErrorType},{report.ErrorLocation},{report.Message}");
                        }
                    }
                }
            }
            catch (IOException ex)
            {

                fUiGlobals.CurrentDispatcher.Invoke(() => Reports.AddErrorReport(ReportSeverity.Critical, ReportType.Error, ReportLocation.Saving, $"Error when saving reports: {ex.Message}"));
            }
        }

        /// <summary>
        /// Command to delete a selected report.
        /// </summary>
        public void DeleteReport()
        {
            if (IndexToDelete >= 0)
            {
                Reports.RemoveReport(IndexToDelete);
                SyncReports();
            }
        }

        /// <summary>
        /// Updates the reports in the window.
        /// </summary>
        public void UpdateReport(ReportSeverity severity, ReportType type, string location, string message)
        {
            Reports.AddErrorReport(severity, type, location, message);
            SyncReports();
        }
    }
}
