﻿﻿﻿using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;

using Effanville.Common.Structure.DataEdit;
using Effanville.Common.UI.Services;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.Persistence;
using Effanville.FPD.Logic.Tests.TestHelpers;
using Effanville.FPD.Logic.ViewModels;

using NUnit.Framework;

namespace Effanville.FPD.Logic.Tests
{
    public class OptionsToolbarTests
    {
        [Test]
        public void CanOpenNewDatabase()
        {
            FileSystem fileSystem = new FileSystem();

            var fileMock = TestSetupHelper.CreateFileMock("notNeeded");
            var dialogMock = TestSetupHelper.CreateDialogMock(MessageBoxOutcome.Yes);
            IPortfolio portfolio = TestSetupHelper.CreateBasicDataBase();
            IUpdater updater = TestSetupHelper.SetupUpdater();
            var mockGlobals = TestSetupHelper.SetupGlobalsMock(fileSystem, fileMock, dialogMock);
            OptionsToolbarViewModel viewModel = new OptionsToolbarViewModel(
                mockGlobals,
                null,
                portfolio,
                TestSetupHelper.SetupDownloader(),
                updater,
                new PortfolioPersistence(mockGlobals.ReportLogger));
            viewModel.NewDatabaseCommand.Execute(1);
            //Check that data held is an empty database

            Assert.That(portfolio.Name, Is.EqualTo(null));
            Assert.That(portfolio.Funds.Count, Is.EqualTo(0));
            Assert.That(portfolio.BankAccounts.Count, Is.EqualTo(0));
            Assert.That(portfolio.BenchMarks.Count, Is.EqualTo(0));
        }

        [Test]
        public void CanOpenDatabase()
        {
            MockFileSystem tempFileSystem = new MockFileSystem();
            string file = File.ReadAllText(TestConstants.ExampleDatabaseLocation + "\\BasicTestDatabase.xml");
            string testPath = "c:/temp/saved.xml";

            string name = tempFileSystem.Path.GetFileNameWithoutExtension(testPath);
            tempFileSystem.AddFile(testPath, new MockFileData(file));

            IFileInteractionService fileMock = TestSetupHelper.CreateFileMock(testPath);
            IBaseDialogCreationService dialogMock = TestSetupHelper.CreateDialogMock();
            IPortfolio portfolio = TestSetupHelper.CreateEmptyDataBase();
            IUpdater updater = TestSetupHelper.SetupUpdater();
            var mockGlobals = TestSetupHelper.SetupGlobalsMock(tempFileSystem, fileMock, dialogMock);
            OptionsToolbarViewModel viewModel = new OptionsToolbarViewModel(
                mockGlobals,
                null,
                portfolio,
                TestSetupHelper.SetupDownloader(),
                updater,
                new PortfolioPersistence(mockGlobals.ReportLogger));
            viewModel.LoadDatabaseCommand.Execute(1);
            //Input prespecified example database

            Assert.That(portfolio.Name, Is.EqualTo(name));
            Assert.That(portfolio.Funds.Count, Is.EqualTo(1));
            Assert.That(portfolio.BankAccounts.Count, Is.EqualTo(1));
            Assert.That(portfolio.BenchMarks.Count, Is.EqualTo(1));
        }

        [Test]
        public void CanOpenAndSaveDatabase()
        {
            MockFileSystem tempFileSystem = new MockFileSystem();
            string file = File.ReadAllText(TestConstants.ExampleDatabaseLocation + "\\BasicTestDatabase.xml");
            string testPath = "c:/temp/database.xml";
            string savePath = "c:/temp/saved.xml";

            tempFileSystem.AddFile(testPath, new MockFileData(file));
            string name = tempFileSystem.Path.GetFileNameWithoutExtension(testPath);

            IFileInteractionService fileMock = TestSetupHelper.CreateFileMock(testPath, savePath);
            IBaseDialogCreationService dialogMock = TestSetupHelper.CreateDialogMock();
            IPortfolio portfolio = TestSetupHelper.CreateEmptyDataBase();
            IUpdater updater = TestSetupHelper.SetupUpdater();
            var mockGlobals = TestSetupHelper.SetupGlobalsMock(tempFileSystem, fileMock, dialogMock);
            OptionsToolbarViewModel viewModel = new OptionsToolbarViewModel(
                mockGlobals,
                null,
                portfolio,
                TestSetupHelper.SetupDownloader(),
                updater,
                new PortfolioPersistence(mockGlobals.ReportLogger));
            viewModel.LoadDatabaseCommand.Execute(1);
            //Input prespecified example database

            Assert.That(portfolio.Name, Is.EqualTo(name));
            Assert.That(portfolio.Funds.Count, Is.EqualTo(1));
            Assert.That(portfolio.BankAccounts.Count, Is.EqualTo(1));
            Assert.That(portfolio.BenchMarks.Count, Is.EqualTo(1));

            viewModel.SaveDatabaseCommand.Execute(1);

            MockFileData savedFile = tempFileSystem.GetFile(savePath);
            Assert.That(savedFile.TextContents, Is.EqualTo(file));
        }

        [Test]
        public void CanSaveDatabase()
        {
            MockFileSystem tempFileSystem = new MockFileSystem();
            string file = File.ReadAllText(TestConstants.ExampleDatabaseLocation + "\\BasicTestDatabase.xml");
            string testPath = "c:/temp/database.xml";
            string name = tempFileSystem.Path.GetFileNameWithoutExtension(testPath);

            IFileInteractionService fileMock = TestSetupHelper.CreateFileMock(testPath);
            IBaseDialogCreationService dialogMock = TestSetupHelper.CreateDialogMock();
            IPortfolio portfolio = TestSetupHelper.CreateBasicDataBase();
            IUpdater updater = TestSetupHelper.SetupUpdater();
            var mockGlobals = TestSetupHelper.SetupGlobalsMock(tempFileSystem, fileMock, dialogMock);
            OptionsToolbarViewModel viewModel = new OptionsToolbarViewModel(
                mockGlobals,
                null,
                portfolio,
                TestSetupHelper.SetupDownloader(),
                updater,
                new PortfolioPersistence(mockGlobals.ReportLogger));
            viewModel.SaveDatabaseCommand.Execute(1);
            //Input prespecified example database

            Assert.That(portfolio.Name, Is.EqualTo(name));
            Assert.That(portfolio.Funds.Count, Is.EqualTo(1));
            Assert.That(portfolio.BankAccounts.Count, Is.EqualTo(1));
            Assert.That(portfolio.BenchMarks.Count, Is.EqualTo(1));
            Assert.That(portfolio.Assets.Count, Is.EqualTo(1));
        }

        [Test]
        [Ignore("IncompeteArchitecture - Downloader does not currently allow for use in test environment.")]
        public void CanUpdateDatabase()
        {
            FileSystem fileSystem = new FileSystem();
            string testFilePath = TestConstants.ExampleDatabaseLocation + "\\BasicTestDatabase.xml";
            IFileInteractionService fileMock = TestSetupHelper.CreateFileMock(testFilePath);
            IBaseDialogCreationService dialogMock = TestSetupHelper.CreateDialogMock();
            IPortfolio portfolio = TestSetupHelper.CreateEmptyDataBase();
            IUpdater updater = TestSetupHelper.SetupUpdater();
            var mockGlobals = TestSetupHelper.SetupGlobalsMock(fileSystem, fileMock, dialogMock);
            OptionsToolbarViewModel viewModel = new OptionsToolbarViewModel(
                mockGlobals,
                null,
                portfolio,
                TestSetupHelper.SetupDownloader(),
                updater,
                new PortfolioPersistence(mockGlobals.ReportLogger));
            viewModel.UpdateDataCommand.Execute(1);
            //Input prespecified example database

            Assert.That(portfolio.Name, Is.EqualTo(testFilePath));
            Assert.That(portfolio.Funds.Count, Is.EqualTo(1));
            Assert.That(portfolio.BankAccounts.Count, Is.EqualTo(1));
            Assert.That(portfolio.BenchMarks.Count, Is.EqualTo(1));
        }

        [Test]
        public void CanRefreshDatabase()
        {
            FileSystem fileSystem = new FileSystem();
            IFileInteractionService fileMock = TestSetupHelper.CreateFileMock("notNeeded");
            IBaseDialogCreationService dialogMock = TestSetupHelper.CreateDialogMock(MessageBoxOutcome.Yes);
            IPortfolio portfolio = TestSetupHelper.CreateBasicDataBase();
            IUpdater updater = TestSetupHelper.SetupUpdater();
            var mockGlobals = TestSetupHelper.SetupGlobalsMock(fileSystem, fileMock, dialogMock);
            OptionsToolbarViewModel viewModel = new OptionsToolbarViewModel(
                mockGlobals,
                null,
                portfolio,
                TestSetupHelper.SetupDownloader(),
                updater,
                new PortfolioPersistence(mockGlobals.ReportLogger));
            viewModel.RefreshCommand.Execute(1);
            //Check that data held is an empty database

            Assert.That(portfolio.Name, Is.EqualTo("TestFilePath"));
            Assert.That(portfolio.Funds.Count, Is.EqualTo(1));
            Assert.That(portfolio.BankAccounts.Count, Is.EqualTo(1));
            Assert.That(portfolio.BenchMarks.Count, Is.EqualTo(1));
        }
    }
}
