using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;

using Common.Structure.DataStructures;
using Common.Structure.Reporting;

using Effanville.Common.Console;

using NUnit.Framework;

namespace Effanville.FPD.Console.Tests;

[TestFixture]
public sealed class StatisticsCommandTests
{
    private static IEnumerable<TestCaseData> ValidationSource()
    {
        yield return new TestCaseData(null, false);
        yield return new TestCaseData(Array.Empty<string>(), false);
        yield return new TestCaseData(new [] {"--hats", "no"}, false);
        yield return new TestCaseData(new [] {"--filepath", "no"}, false);
        yield return new TestCaseData(new [] {"--filepath", @"c:\\temp\\file.xml"}, true);
        yield return new TestCaseData(new [] {"--filepath", @"c:\\temp\\file.xml", "--outputPath", "true"}, true);
        yield return new TestCaseData(new [] {"--filepath", @"c:\\temp\\file.xml", "docType", "fish"}, true);
        yield return new TestCaseData(new [] {"--filepath", @"c:\\temp\\file.xml", "--mailTo", "true"}, true);
    }

    [TestCaseSource(nameof(ValidationSource))]
    public void CanValidateTest(string[] args, bool expectedValidation)
    {
        string error;
        var mockFileSystem = new MockFileSystem();
        mockFileSystem.AddFile(@"c:\\temp\\file.xml", new MockFileData("some contents"));
        var consoleInstance = new ConsoleInstance(WriteError, WriteReport);
        var logReporter = new LogReporter(ReportAction, new SingleTaskQueue(), saveInternally: true);

        var statisticsCommand = new StatisticsCommand(mockFileSystem);
        bool isValidated = statisticsCommand.Validate(consoleInstance, logReporter, args);
        Assert.That(isValidated, Is.EqualTo(expectedValidation));
        
        return;
        void WriteError(string err)
        {
            error = err;
        }

        void WriteReport(string rep)
        {
        }

        void ReportAction(ReportSeverity severity, ReportType reportType, string location, string text)
        {
            string message = $"({reportType}) - [{location}] - {text}";
            if (reportType == ReportType.Error)
            {
                WriteError(message);
            }
            else
            {
                WriteReport(message);
            }
        }
    }
    
    
}