﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Common.Structure.Reporting;
using Common.UI.Commands;
using Common.UI.Services;
using Common.UI.ViewModelBases;
using FinancePortfolioDatabase.GUI.TemplatesAndStyles;

namespace FinancePortfolioDatabase.GUI.ViewModels
{
    public class ReportingWindowViewModel : PropertyChangedBase
    {
        public UiStyles Styles
        {
            get; set;
        }
        private ErrorReports fReportsToView;
        private readonly IFileInteractionService fFileInteractionService;
        public ErrorReports ReportsToView
        {
            get => fReportsToView;
            set => SetAndNotify(ref fReportsToView, value, nameof(ReportsToView));
        }

        private bool fIsExpanded;
        public bool IsExpanded
        {
            get => fIsExpanded;
            set => SetAndNotify(ref fIsExpanded, value, nameof(IsExpanded));
        }

        public ErrorReports Reports
        {
            get;
            set;
        }

        public int IndexToDelete
        {
            get;
            set;
        }

        private ReportSeverity fReportingSeverity;

        public ReportSeverity ReportingSeverity
        {
            get => fReportingSeverity;
            set
            {
                fReportingSeverity = value;
                OnPropertyChanged(nameof(ReportingSeverity));
                SyncReports();
            }
        }

        public List<ReportSeverity> EnumValues => Enum.GetValues(typeof(ReportSeverity)).Cast<ReportSeverity>().ToList();

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ReportingWindowViewModel(IFileInteractionService fileInteractionService, UiStyles styles)
        {
            Styles = styles;
            IsExpanded = false;
            fFileInteractionService = fileInteractionService;
            Reports = new ErrorReports();
            ReportsToView = new ErrorReports();
            ClearReportsCommand = new RelayCommand(ExecuteClearReports);
            DeleteCommand = new RelayCommand<KeyEventArgs>(ExecuteDeleteReport);
            ExportReportsCommand = new RelayCommand(ExecuteExportReportsCommand);
            SyncReports();
        }

        internal void SyncReports()
        {
            ReportsToView = null;
            ReportsToView = new ErrorReports(Reports.GetReports(ReportingSeverity));
            if (ReportsToView != null && (ReportsToView?.Any() ?? false))
            {
                IsExpanded = true;
            }
        }

        public ICommand ClearReportsCommand
        {
            get;
        }

        private void ExecuteClearReports()
        {
            Reports.Clear();
            SyncReports();
        }

        public ICommand ExportReportsCommand
        {
            get;
        }

        private void ExecuteExportReportsCommand()
        {
            try
            {
                var result = fFileInteractionService.SaveFile(".csv", "errorReports.csv");
                if (result.Success != null && (bool)result.Success)
                {
                    StreamWriter writer = new StreamWriter(result.FilePath);
                    writer.WriteLine("Severity,ErrorType,Location,Message");
                    foreach (var report in Reports.GetReports())
                    {
                        writer.WriteLine(report.ErrorSeverity.ToString() + "," + report.ErrorType + "," + report.ErrorLocation.ToString() + "," + report.Message);
                    }

                    writer.Close();
                }
            }
            catch (IOException)
            {
            }
        }

        public ICommand DeleteCommand
        {
            get;
            set;
        }

        private void ExecuteDeleteReport(KeyEventArgs e)
        {
            if (e.Key == Key.Delete || e.Key == Key.Back)
            {
                if (IndexToDelete >= 0)
                {
                    Reports.RemoveReport(IndexToDelete);
                    SyncReports();
                }
            }
        }

        public void UpdateReport(ReportSeverity severity, ReportType type, ReportLocation location, string message)
        {
            Reports.AddErrorReport(severity, type, location, message);
            SyncReports();
        }
    }
}
