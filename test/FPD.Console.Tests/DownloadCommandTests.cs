using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;

using Effanville.Common.Console;
using Effanville.Common.Structure.DataStructures;
using Effanville.Common.Structure.Reporting;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.Persistence;
using Effanville.FPD.Console.Utilities.Mail;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using NSubstitute;

using NUnit.Framework;

namespace Effanville.FPD.Console.Tests;

[TestFixture]
public sealed class DownloadCommandTests
{
    private static IEnumerable<TestCaseData> ValidationSource()
    {
        yield return new TestCaseData(null, false);
        yield return new TestCaseData(Array.Empty<string>(), false);
        yield return new TestCaseData(new[] { "--hats", "no" }, false);
        yield return new TestCaseData(new[] { "--filepath", "no" }, false);
        yield return new TestCaseData(new[] { "--filepath", @"c:\\temp\\file.xml" }, true);
        yield return new TestCaseData(new[] { "--filepath", @"c:\\temp\\file.xml", "--updateStats", @"c:\\temp\\other-file.xml" }, false);
        yield return new TestCaseData(new[] { "--filepath", @"c:\\temp\\file.xml", "--updateStats", "true" }, true);
        yield return new TestCaseData(new[] { "--filepath", @"c:\\temp\\file.xml", "--mailTo", "true" }, true);
    }

    [TestCaseSource(nameof(ValidationSource))]
    public void CanValidateTest(string[] args, bool expectedValidation)
    {
        var mockFileSystem = new MockFileSystem();
        mockFileSystem.AddFile(@"c:\\temp\\file.xml", new MockFileData("some contents"));
        var reportLogger = new LogReporter(null, new SingleTaskQueue(), saveInternally: true);
        var mailSender = Substitute.For<IMailSender>();
        var persistence = Substitute.For<IPersistence<IPortfolio>>();
        ILogger<DownloadCommand> logger = Substitute.For<ILogger<DownloadCommand>>();
        IConfiguration config = new ConfigurationBuilder()
            .AddCommandLine(new ConsoleCommandArgs(args).GetEffectiveArgs())
            .AddEnvironmentVariables()
            .Build();
        var downloadCommand = new DownloadCommand(mockFileSystem, logger, reportLogger, config, mailSender, persistence);
        bool isValidated = downloadCommand.Validate();
        Assert.That(isValidated, Is.EqualTo(expectedValidation));
    }
}