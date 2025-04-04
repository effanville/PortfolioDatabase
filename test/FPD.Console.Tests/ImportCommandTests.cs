using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;

using Effanville.Common.Console;
using Effanville.Common.Structure.DataStructures;
using Effanville.Common.Structure.Reporting;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.Persistence;

using Microsoft.Extensions.Configuration;

using NSubstitute;

using NUnit.Framework;

namespace Effanville.FPD.Console.Tests;

[TestFixture]
public sealed class ImportCommandTests
{
    private static IEnumerable<TestCaseData> ValidationSource()
    {
        yield return new TestCaseData(null, false);
        yield return new TestCaseData(Array.Empty<string>(), false);
        yield return new TestCaseData(new[] { "--hats", "no" }, false);
        yield return new TestCaseData(new[] { "--filepath", "no" }, false);
        yield return new TestCaseData(new[] { "--filepath", @"c:\\temp\\file.xml" }, false);
        yield return new TestCaseData(new[] { "--filepath", @"c:\\temp\\file.xml", "--importfilepath", @"c:\\temp\\other-file.xml" }, true);
        yield return new TestCaseData(new[] { "--filepath", @"c:\\temp\\file.xml", "--importfilepath", "fish" }, false);
        yield return new TestCaseData(new[] { "--filepath", @"c:\\temp\\file.xml", "--importfilepath", @"c:\\temp\\other-file.xml", "nothoing", "fish" }, true);
    }

    [TestCaseSource(nameof(ValidationSource))]
    public void CanValidateTest(string[] args, bool expectedValidation)
    {
        IConfiguration config = new ConfigurationBuilder()
            .AddCommandLine(new ConsoleCommandArgs(args).GetEffectiveArgs())
            .AddEnvironmentVariables()
            .Build();
        var mockFileSystem = new MockFileSystem();
        mockFileSystem.AddFile(@"c:\\temp\\file.xml", new MockFileData("some contents"));
        mockFileSystem.AddFile(@"c:\\temp\\other-file.xml", new MockFileData("some other contents"));
        var reportLogger = new LogReporter(null, new SingleTaskQueue(), saveInternally: true);
        var persistence = Substitute.For<IPersistence<IPortfolio>>();
        var importCommand = new ImportCommand(mockFileSystem, null, reportLogger, config, persistence);

        bool isValidated = importCommand.Validate();
        Assert.That(isValidated, Is.EqualTo(expectedValidation));
    }
}