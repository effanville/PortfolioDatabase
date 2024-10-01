using System;
using System.Collections.Generic;
using System.Linq;

using Effanville.Common.Structure.Reporting;
using Effanville.FPD.Logic.Tests.Context;
using Effanville.FPD.Logic.ViewModels;

using NUnit.Framework;

using TechTalk.SpecFlow;

namespace Effanville.FPD.Logic.Tests.Steps;

[Binding]
public class ReportingViewModelSteps
{
    private readonly ViewModelTestContext<ErrorReports, ReportingWindowViewModel> _testContext;

    public ReportingViewModelSteps(ViewModelTestContext<ErrorReports, ReportingWindowViewModel> testContext)
    {
        _testContext = testContext;
    }

    [AfterScenario]
    public void Reset() => _testContext.Reset();

    [Given(@"I have a ReportingViewModel with default level (.*) and no reports")]
    public void GivenIHaveAReportingViewModelNoReports(ReportSeverity reportSeverity)
        => Instantiate(reportSeverity, null);

    [Given(@"I have a ReportingViewModel with default level (.*) and reports")]
    public void GivenIHaveAReportingViewModelWithDefaultLevelUsefulAndReports(ReportSeverity reportSeverity,
        Table table)
        => Instantiate(reportSeverity, table);

    private void Instantiate(ReportSeverity reportSeverity, Table table)
    {
        _testContext.ViewModel = new ReportingWindowViewModel(
            _testContext.Globals,
            _testContext.Styles) { ReportingSeverity = reportSeverity };
        WhenAReportIsAddedToTheRvmWithData(table);
        _testContext.ModelData = _testContext.ViewModel.ModelData;
    }

    [Given(@"the ReportingViewModel is brought into focus")]
    public void GivenTheReportingViewModelIsBroughtIntoFocus()
        => _testContext.ViewModel.UpdateData(_testContext.ModelData);

    [Then(@"I can see that the RVM has (.*) reports")]
    public void ThenICanSeeThatTheRvmHasReports(int p0) 
        => Assert.AreEqual(p0, _testContext.ViewModel.ModelData.Count());

    [Then(@"I can see that the RVM display has (.*) reports")]
    public void ThenICanSeeThatTheRvmDisplayHasReports(int p0) 
        => Assert.AreEqual(p0, _testContext.ViewModel.ReportsToView.Count);

    [When(@"reports are added to the RVM with data")]
    public void WhenAReportIsAddedToTheRvmWithData(Table table)
    {
        if (table == null)
        {
            return;
        }

        foreach (TableRow row in table.Rows)
        {
            ErrorReport report = FromRow(row);
            _testContext.ViewModel.UpdateReport(report.ErrorSeverity, report.ErrorType, report.ErrorLocation,
                report.Message);
        }
    }

    [Then(@"the reports in the RVM display have data")]
    public void ThenTheReportsInTheRvmDisplayHaveData(Table table)
    {
        List<ErrorReport> actualViewReports = _testContext.ViewModel.ReportsToView;
        Assert.AreEqual(table.RowCount, actualViewReports.Count());
        for (int index = 0; index < table.RowCount; index++)
        {
            TableRow row = table.Rows[index];
            ErrorReport expectedReport = FromRow(row);
            ErrorReport actualReport = actualViewReports[index];
            Assert.AreEqual(expectedReport.ErrorSeverity, actualReport.ErrorSeverity);
            Assert.AreEqual(expectedReport.ErrorType, actualReport.ErrorType);
            Assert.AreEqual(expectedReport.ErrorLocation, actualReport.ErrorLocation);
            Assert.AreEqual(expectedReport.Message, actualReport.Message);
        }
    }

    [Then(@"the reports in the RVM have data")]
    public void ThenTheReportsInTheRvmHaveData(Table table)
    {
        ErrorReports actualReports = _testContext.ModelData;
        Assert.AreEqual(table.RowCount, actualReports.Count());
        for (int index = 0; index < table.RowCount; index++)
        {
            TableRow row = table.Rows[index];
            ErrorReport expectedReport = FromRow(row);
            ErrorReport actualReport = actualReports[index];
            Assert.AreEqual(expectedReport.ErrorSeverity, actualReport.ErrorSeverity);
            Assert.AreEqual(expectedReport.ErrorType, actualReport.ErrorType);
            Assert.AreEqual(expectedReport.ErrorLocation, actualReport.ErrorLocation);
            Assert.AreEqual(expectedReport.Message, actualReport.Message);
        }
    }

    private static ErrorReport FromRow(TableRow row)
    {
        ReportSeverity severity = Enum.Parse<ReportSeverity>(row["Severity"]);
        ReportType reportType = Enum.Parse<ReportType>(row["Type"]);
        string location = row["Location"];
        string message = row["Message"];
        return new ErrorReport(severity, reportType, location, message);
    }

    [When(@"reports are cleared from the RVM")]
    public void WhenReportsAreClearedFromTheRvm()
        => _testContext.ViewModel.ClearReportsCommand.Execute(null);

    [When(@"the (.*) report is cleared from the RVM")]
    public void WhenTheReportIsClearedFromTheRvm(int p0)
    {
        ErrorReport report = _testContext.ViewModel.ReportsToView[p0 - 1];
        _testContext.ViewModel.DeleteReport(report);
    }
}