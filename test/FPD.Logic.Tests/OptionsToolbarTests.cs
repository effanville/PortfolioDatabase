using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;

using Effanville.Common.Structure.DataEdit;
using Effanville.Common.UI.Services;
using Effanville.FinancialStructures.Database;
using Effanville.FPD.Logic.Tests.TestHelpers;
using Effanville.FPD.Logic.ViewModels;

using Microsoft.Extensions.Logging;

using Moq;

using NUnit.Framework;

namespace Effanville.FPD.Logic.Tests
{
    public class OptionsToolbarTests
    {
        [Test]
        public void CanOpenNewDatabase()
        {
            FileSystem fileSystem = new FileSystem();
            Mock<ILogger<OptionsToolbarViewModel>> loggerMock = new Mock<ILogger<OptionsToolbarViewModel>>();
            Mock<IFileInteractionService> fileMock = TestSetupHelper.CreateFileMock("notNeeded");
            Mock<IBaseDialogCreationService> dialogMock = TestSetupHelper.CreateDialogMock(MessageBoxOutcome.Yes);
            IPortfolio portfolio = TestSetupHelper.CreateBasicDataBase();
            IDataStoreUpdater<IPortfolio> dataUpdater = TestSetupHelper.SetupUpdater(portfolio);
            OptionsToolbarViewModel viewModel = new OptionsToolbarViewModel(
                loggerMock.Object,
                TestSetupHelper.SetupGlobalsMock(fileSystem, fileMock.Object, dialogMock.Object),
                null,
                portfolio,
                TestSetupHelper.SetupDownloader());
            viewModel.UpdateRequest += dataUpdater.PerformUpdate;
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
            Mock<ILogger<OptionsToolbarViewModel>> loggerMock = new Mock<ILogger<OptionsToolbarViewModel>>();
            MockFileSystem tempFileSystem = new MockFileSystem();
            string file = File.ReadAllText(TestConstants.ExampleDatabaseLocation + "\\BasicTestDatabase.xml");
            string testPath = "c:/temp/saved.xml";

            string name = tempFileSystem.Path.GetFileNameWithoutExtension(testPath);
            tempFileSystem.AddFile(testPath, new MockFileData(file));

            Mock<IFileInteractionService> fileMock = TestSetupHelper.CreateFileMock(testPath);
            Mock<IBaseDialogCreationService> dialogMock = TestSetupHelper.CreateDialogMock();
            IPortfolio portfolio = TestSetupHelper.CreateEmptyDataBase();
            IDataStoreUpdater<IPortfolio> dataUpdater = TestSetupHelper.SetupUpdater(portfolio);
            OptionsToolbarViewModel viewModel = new OptionsToolbarViewModel(
                loggerMock.Object,
                TestSetupHelper.SetupGlobalsMock(tempFileSystem, fileMock.Object, dialogMock.Object),
                null,
                portfolio,
                TestSetupHelper.SetupDownloader());
            viewModel.UpdateRequest += dataUpdater.PerformUpdate;
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
            Mock<ILogger<OptionsToolbarViewModel>> loggerMock = new Mock<ILogger<OptionsToolbarViewModel>>();
            MockFileSystem tempFileSystem = new MockFileSystem();
            string file = File.ReadAllText(TestConstants.ExampleDatabaseLocation + "\\BasicTestDatabase.xml");
            string testPath = "c:/temp/database.xml";
            string savePath = "c:/temp/saved.xml";

            tempFileSystem.AddFile(testPath, new MockFileData(file));
            string name = tempFileSystem.Path.GetFileNameWithoutExtension(testPath);

            Mock<IFileInteractionService> fileMock = TestSetupHelper.CreateFileMock(testPath, savePath);
            Mock<IBaseDialogCreationService> dialogMock = TestSetupHelper.CreateDialogMock();
            IPortfolio portfolio = TestSetupHelper.CreateEmptyDataBase();
            IDataStoreUpdater<IPortfolio> dataUpdater = TestSetupHelper.SetupUpdater(portfolio);
            OptionsToolbarViewModel viewModel = new OptionsToolbarViewModel(
                loggerMock.Object,
                TestSetupHelper.SetupGlobalsMock(tempFileSystem, fileMock.Object, dialogMock.Object),
                null,
                portfolio,
                TestSetupHelper.SetupDownloader());
            viewModel.UpdateRequest += dataUpdater.PerformUpdate;
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
            Mock<ILogger<OptionsToolbarViewModel>> loggerMock = new Mock<ILogger<OptionsToolbarViewModel>>();
            MockFileSystem tempFileSystem = new MockFileSystem();
            string file = File.ReadAllText(TestConstants.ExampleDatabaseLocation + "\\BasicTestDatabase.xml");
            string testPath = "c:/temp/database.xml";
            string name = tempFileSystem.Path.GetFileNameWithoutExtension(testPath);

            Mock<IFileInteractionService> fileMock = TestSetupHelper.CreateFileMock(testPath);
            Mock<IBaseDialogCreationService> dialogMock = TestSetupHelper.CreateDialogMock();
            IPortfolio portfolio = TestSetupHelper.CreateBasicDataBase();
            IDataStoreUpdater<IPortfolio> dataUpdater = TestSetupHelper.SetupUpdater(portfolio);
            OptionsToolbarViewModel viewModel = new OptionsToolbarViewModel(
                loggerMock.Object,
                TestSetupHelper.SetupGlobalsMock(tempFileSystem, fileMock.Object, dialogMock.Object),
                null,
                portfolio,
                TestSetupHelper.SetupDownloader());
            viewModel.UpdateRequest += dataUpdater.PerformUpdate;
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
            Mock<ILogger<OptionsToolbarViewModel>> loggerMock = new Mock<ILogger<OptionsToolbarViewModel>>();
            FileSystem fileSystem = new FileSystem();
            string testFilePath = TestConstants.ExampleDatabaseLocation + "\\BasicTestDatabase.xml";
            Mock<IFileInteractionService> fileMock = TestSetupHelper.CreateFileMock(testFilePath);
            Mock<IBaseDialogCreationService> dialogMock = TestSetupHelper.CreateDialogMock();
            IPortfolio portfolio = TestSetupHelper.CreateEmptyDataBase();
            IDataStoreUpdater<IPortfolio> dataUpdater = TestSetupHelper.SetupUpdater(portfolio);
            OptionsToolbarViewModel viewModel = new OptionsToolbarViewModel(
                loggerMock.Object,
                TestSetupHelper.SetupGlobalsMock(fileSystem, fileMock.Object, dialogMock.Object),
                null,
                portfolio,
                TestSetupHelper.SetupDownloader());
            viewModel.UpdateRequest += dataUpdater.PerformUpdate;
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
            Mock<ILogger<OptionsToolbarViewModel>> loggerMock = new Mock<ILogger<OptionsToolbarViewModel>>();
            FileSystem fileSystem = new FileSystem();
            Mock<IFileInteractionService> fileMock = TestSetupHelper.CreateFileMock("notNeeded");
            Mock<IBaseDialogCreationService> dialogMock = TestSetupHelper.CreateDialogMock(MessageBoxOutcome.Yes);
            IPortfolio portfolio = TestSetupHelper.CreateBasicDataBase();
            IDataStoreUpdater<IPortfolio> dataUpdater = TestSetupHelper.SetupUpdater(portfolio);
            OptionsToolbarViewModel viewModel = new OptionsToolbarViewModel(
                loggerMock.Object,
                TestSetupHelper.SetupGlobalsMock(fileSystem, fileMock.Object, dialogMock.Object),
                null,
                portfolio,
                TestSetupHelper.SetupDownloader());
            viewModel.UpdateRequest += dataUpdater.PerformUpdate;
            viewModel.RefreshCommand.Execute(1);
            //Check that data held is an empty database

            Assert.That(portfolio.Name, Is.EqualTo("TestFilePath"));
            Assert.That(portfolio.Funds.Count, Is.EqualTo(1));
            Assert.That(portfolio.BankAccounts.Count, Is.EqualTo(1));
            Assert.That(portfolio.BenchMarks.Count, Is.EqualTo(1));
        }
    }
}
