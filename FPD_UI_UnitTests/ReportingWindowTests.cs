using System.Linq;
using FinanceWindowsViewModels;
using NUnit.Framework;
using StructureCommon.Reporting;

namespace FPD_UI_UnitTests
{
    /// <summary>
    /// Tests for the report display panel.
    /// </summary>
    public class ReportingWindowTests
    {
        /// <summary>
        /// Ensures the two report structures display the same data.
        /// </summary>
        [Test]
        public void ReportsSync()
        {
            ReportingWindowViewModel viewModel = new ReportingWindowViewModel
            {
                ReportingSeverity = ReportSeverity.Detailed
            };
            viewModel.Reports.AddErrorReport(ReportSeverity.Critical, ReportType.Error, ReportLocation.Unknown, "Is this added?");
            viewModel.SyncReports();

            Assert.AreEqual(1, viewModel.ReportsToView.Count, "Viewable reports should have one report added.");
            Assert.AreEqual(ReportType.Error, viewModel.ReportsToView.Single().ErrorType);
            Assert.AreEqual(ReportLocation.Unknown, viewModel.ReportsToView.Single().ErrorLocation);
            Assert.AreEqual("Is this added?", viewModel.ReportsToView.Single().Message);
        }

        /// <summary>
        /// Ensures the window list updates when one adds a report.
        /// </summary>
        [Test]
        public void CanAddReport()
        {
            ReportingWindowViewModel viewModel = new ReportingWindowViewModel
            {
                ReportingSeverity = ReportSeverity.Detailed
            };
            viewModel.UpdateReport(ReportSeverity.Useful, ReportType.Error, ReportLocation.Unknown, "Is this added?");

            Assert.AreEqual(1, viewModel.Reports.GetReports().Count, "Reports should have a report added.");
            Assert.AreEqual(1, viewModel.ReportsToView.Count, "Viewable reports should have one report added.");
            Assert.AreEqual(ReportType.Error, viewModel.ReportsToView.Single().ErrorType);
            Assert.AreEqual(ReportLocation.Unknown, viewModel.ReportsToView.Single().ErrorLocation);
            Assert.AreEqual("Is this added?", viewModel.ReportsToView.Single().Message);
        }

        /// <summary>
        /// Ensures reports clear when clear reports button is pressed.
        /// </summary>
        [Test]
        public void CanClearReports()
        {
            ReportingWindowViewModel viewModel = new ReportingWindowViewModel
            {
                ReportingSeverity = ReportSeverity.Detailed
            };
            viewModel.UpdateReport(ReportSeverity.Useful, ReportType.Error, ReportLocation.Unknown, "Is this added?");
            viewModel.UpdateReport(ReportSeverity.Useful, ReportType.Error, ReportLocation.Unknown, "Is this also added?");

            Assert.AreEqual(2, viewModel.Reports.GetReports().Count, "Reports should have a report added.");
            Assert.AreEqual(2, viewModel.ReportsToView.Count, "Viewable reports should have one report added.");

            viewModel.ClearReportsCommand.Execute(3);
            Assert.AreEqual(0, viewModel.Reports.GetReports().Count, "Reports should have been cleared.");
            Assert.AreEqual(0, viewModel.ReportsToView.Count, "Viewable reports should have been cleared.");
        }

        /// <summary>
        /// Ensure the correct selected report is removed when clear single report is pressed.
        /// </summary>
        [Test]
        public void CanClearSingleReport()
        {
            ReportingWindowViewModel viewModel = new ReportingWindowViewModel
            {
                ReportingSeverity = ReportSeverity.Detailed
            };
            viewModel.UpdateReport(ReportSeverity.Useful, ReportType.Error, ReportLocation.Unknown, "Is this added?");
            viewModel.UpdateReport(ReportSeverity.Useful, ReportType.Error, ReportLocation.Unknown, "Is this also added?");

            Assert.AreEqual(2, viewModel.Reports.GetReports().Count, "Reports should have a report added.");
            Assert.AreEqual(2, viewModel.ReportsToView.Count, "Viewable reports should have one report added.");

            viewModel.IndexToDelete = 1;
            viewModel.ClearSingleReportCommand.Execute(3);
            Assert.AreEqual(1, viewModel.Reports.GetReports().Count, "Reports should have been cleared.");
            Assert.AreEqual(1, viewModel.ReportsToView.Count, "Viewable reports should have been cleared.");
            Assert.AreEqual(ReportType.Error, viewModel.ReportsToView.Single().ErrorType);
            Assert.AreEqual(ReportLocation.Unknown, viewModel.ReportsToView.Single().ErrorLocation);
            Assert.AreEqual("Is this added?", viewModel.ReportsToView.Single().Message);
        }
    }
}