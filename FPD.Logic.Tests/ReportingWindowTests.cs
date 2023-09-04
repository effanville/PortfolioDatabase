using System.Linq;
using Common.Structure.Reporting;
using Common.UI.Services;
using FPD.Logic.ViewModels;
using FPD.Logic.Tests.TestHelpers;
using Moq;
using NUnit.Framework;
using System.IO.Abstractions.TestingHelpers;

namespace FPD.Logic.Tests
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
            ReportingWindowViewModel viewModel = CreateViewModel("nothing");
            viewModel.ModelData.AddErrorReport(ReportSeverity.Critical, ReportType.Error, "Unknown", "Is this added?");
            viewModel.SyncReports();

            Assert.AreEqual(1, viewModel.ReportsToView.Count(), "Viewable reports should have one report added.");
            Assert.AreEqual(ReportType.Error, viewModel.ReportsToView.Single().ErrorType);
            Assert.AreEqual("Unknown", viewModel.ReportsToView.Single().ErrorLocation);
            Assert.AreEqual("Is this added?", viewModel.ReportsToView.Single().Message);
        }

        /// <summary>
        /// Ensures the window list updates when one adds a report.
        /// </summary>
        [Test]
        public void CanAddReport()
        {
            ReportingWindowViewModel viewModel = CreateViewModel("nothing");
            viewModel.UpdateReport(ReportSeverity.Useful, ReportType.Error, "Unknown", "Is this added?");

            Assert.AreEqual(1, viewModel.ModelData.GetReports().Count, "Reports should have a report added.");
            Assert.AreEqual(1, viewModel.ReportsToView.Count(), "Viewable reports should have one report added.");
            Assert.AreEqual(ReportType.Error, viewModel.ReportsToView.Single().ErrorType);
            Assert.AreEqual("Unknown", viewModel.ReportsToView.Single().ErrorLocation);
            Assert.AreEqual("Is this added?", viewModel.ReportsToView.Single().Message);
        }

        /// <summary>
        /// Ensures reports clear when clear reports button is pressed.
        /// </summary>
        [Test]
        public void CanClearReports()
        {
            ReportingWindowViewModel viewModel = CreateViewModel("nothing");
            viewModel.UpdateReport(ReportSeverity.Useful, ReportType.Error, "Unknown", "Is this added?");
            viewModel.UpdateReport(ReportSeverity.Useful, ReportType.Error, "Unknown", "Is this also added?");

            Assert.AreEqual(2, viewModel.ModelData.GetReports().Count, "Reports should have a report added.");
            Assert.AreEqual(2, viewModel.ReportsToView.Count(), "Viewable reports should have one report added.");

            viewModel.ClearReportsCommand.Execute(3);
            Assert.AreEqual(0, viewModel.ModelData.GetReports().Count, "Reports should have been cleared.");
            Assert.AreEqual(0, viewModel.ReportsToView.Count(), "Viewable reports should have been cleared.");
        }

        /// <summary>
        /// Ensure the correct selected report is removed when clear single report is pressed.
        /// </summary>
        [Test]
        public void CanClearSingleReport()
        {
            ReportingWindowViewModel viewModel = CreateViewModel("nothing");

            viewModel.UpdateReport(ReportSeverity.Useful, ReportType.Error, "Unknown", "Is this added?");
            viewModel.UpdateReport(ReportSeverity.Useful, ReportType.Error, "Unknown", "Is this also added?");

            Assert.AreEqual(2, viewModel.ModelData.GetReports().Count, "Reports should have a report added.");
            Assert.AreEqual(2, viewModel.ReportsToView.Count(), "Viewable reports should have one report added.");

            viewModel.IndexToDelete = 1;
            viewModel.DeleteReport();
            Assert.AreEqual(1, viewModel.ModelData.GetReports().Count, "Reports should have been cleared.");
            Assert.AreEqual(1, viewModel.ReportsToView.Count(), "Viewable reports should have been cleared.");
            Assert.AreEqual(ReportType.Error, viewModel.ReportsToView.Single().ErrorType);
            Assert.AreEqual("Unknown", viewModel.ReportsToView.Single().ErrorLocation);
            Assert.AreEqual("Is this added?", viewModel.ReportsToView.Single().Message);
        }

        private static ReportingWindowViewModel CreateViewModel(string filepath, ReportSeverity reportingSeverity = ReportSeverity.Detailed)
        {
            Mock<IFileInteractionService> mockFileService = TestSetupHelper.CreateFileMock(filepath);
            var mockGlobals = TestSetupHelper.CreateGlobalsMock(new MockFileSystem(), mockFileService.Object, null, null);
            ReportingWindowViewModel viewModel = new ReportingWindowViewModel(mockGlobals, null)
            {
                ReportingSeverity = reportingSeverity
            };
            return viewModel;
        }
    }
}