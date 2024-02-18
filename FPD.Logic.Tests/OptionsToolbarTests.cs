using System;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;

using Effanville.Common.UI.Services;

using FPD.Logic.ViewModels;
using FPD.Logic.Tests.TestHelpers;
using FinancialStructures.Database;
using Moq;
using NUnit.Framework;

namespace FPD.Logic.Tests
{
    public class OptionsToolbarTests
    {
        [Test]
        public void CanOpenNewDatabase()
        {
            FileSystem fileSystem = new FileSystem();
            Mock<IFileInteractionService> fileMock = TestSetupHelper.CreateFileMock("notNeeded");
            Mock<IBaseDialogCreationService> dialogMock = TestSetupHelper.CreateDialogMock(MessageBoxOutcome.Yes);
            IPortfolio portfolio = TestSetupHelper.CreateBasicDataBase();
            var dataUpdater = TestSetupHelper.CreateUpdater(portfolio);
            OptionsToolbarViewModel viewModel = new OptionsToolbarViewModel(TestSetupHelper.CreateGlobalsMock(fileSystem, fileMock.Object, dialogMock.Object), null, portfolio);
            viewModel.UpdateRequest += dataUpdater.PerformUpdate;
            viewModel.NewDatabaseCommand.Execute(1);
            //Check that data held is an empty database

            Assert.AreEqual(null, portfolio.Name);
            Assert.AreEqual(0, portfolio.Funds.Count);
            Assert.AreEqual(0, portfolio.BankAccounts.Count);
            Assert.AreEqual(0, portfolio.BenchMarks.Count);
        }

        [Test]
        public void CanOpenDatabase()
        {
            MockFileSystem tempFileSystem = new MockFileSystem();
            string file = File.ReadAllText(TestConstants.ExampleDatabaseLocation + "\\BasicTestDatabase.xml");
            string testPath = "c:/temp/saved.xml";

            string name = tempFileSystem.Path.GetFileNameWithoutExtension(testPath);
            tempFileSystem.AddFile(testPath, new MockFileData(file));

            Mock<IFileInteractionService> fileMock = TestSetupHelper.CreateFileMock(testPath);
            Mock<IBaseDialogCreationService> dialogMock = TestSetupHelper.CreateDialogMock();
            IPortfolio portfolio = TestSetupHelper.CreateEmptyDataBase();
            var dataUpdater = TestSetupHelper.CreateUpdater(portfolio);
            OptionsToolbarViewModel viewModel = new OptionsToolbarViewModel(TestSetupHelper.CreateGlobalsMock(tempFileSystem, fileMock.Object, dialogMock.Object), null, portfolio);
            viewModel.UpdateRequest += dataUpdater.PerformUpdate;
            viewModel.LoadDatabaseCommand.Execute(1);
            //Input prespecified example database

            Assert.AreEqual(name, portfolio.Name);
            Assert.AreEqual(1, portfolio.Funds.Count);
            Assert.AreEqual(1, portfolio.BankAccounts.Count);
            Assert.AreEqual(1, portfolio.BenchMarks.Count);
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

            Mock<IFileInteractionService> fileMock = TestSetupHelper.CreateFileMock(testPath, savePath);
            Mock<IBaseDialogCreationService> dialogMock = TestSetupHelper.CreateDialogMock();
            IPortfolio portfolio = TestSetupHelper.CreateEmptyDataBase();
            var dataUpdater = TestSetupHelper.CreateUpdater(portfolio);
            OptionsToolbarViewModel viewModel = new OptionsToolbarViewModel(TestSetupHelper.CreateGlobalsMock(tempFileSystem, fileMock.Object, dialogMock.Object), null, portfolio);
            viewModel.UpdateRequest += dataUpdater.PerformUpdate;
            viewModel.LoadDatabaseCommand.Execute(1);
            //Input prespecified example database

            Assert.AreEqual(name, portfolio.Name);
            Assert.AreEqual(1, portfolio.Funds.Count);
            Assert.AreEqual(1, portfolio.BankAccounts.Count);
            Assert.AreEqual(1, portfolio.BenchMarks.Count);

            viewModel.SaveDatabaseCommand.Execute(1);

            MockFileData savedFile = tempFileSystem.GetFile(savePath);
            Assert.AreEqual(file, savedFile.TextContents);
        }

        [Test]
        public void CanSaveDatabase()
        {
            MockFileSystem tempFileSystem = new MockFileSystem();
            string file = File.ReadAllText(TestConstants.ExampleDatabaseLocation + "\\BasicTestDatabase.xml");
            string testPath = "c:/temp/database.xml";
            string name = tempFileSystem.Path.GetFileNameWithoutExtension(testPath);

            Mock<IFileInteractionService> fileMock = TestSetupHelper.CreateFileMock(testPath);
            Mock<IBaseDialogCreationService> dialogMock = TestSetupHelper.CreateDialogMock();
            IPortfolio portfolio = TestSetupHelper.CreateBasicDataBase();
            var dataUpdater = TestSetupHelper.CreateUpdater(portfolio);
            OptionsToolbarViewModel viewModel = new OptionsToolbarViewModel(TestSetupHelper.CreateGlobalsMock(tempFileSystem, fileMock.Object, dialogMock.Object), null, portfolio);
            viewModel.UpdateRequest += dataUpdater.PerformUpdate;
            viewModel.SaveDatabaseCommand.Execute(1);
            //Input prespecified example database

            Assert.AreEqual(name, portfolio.Name);
            Assert.AreEqual(1, portfolio.Funds.Count);
            Assert.AreEqual(1, portfolio.BankAccounts.Count);
            Assert.AreEqual(1, portfolio.BenchMarks.Count);
            Assert.AreEqual(1, portfolio.Assets.Count);
        }

        [Test]
        [Ignore("IncompeteArchitecture - Downloader does not currently allow for use in test environment.")]
        public void CanUpdateDatabase()
        {
            FileSystem fileSystem = new FileSystem();
            string testFilePath = TestConstants.ExampleDatabaseLocation + "\\BasicTestDatabase.xml";
            Mock<IFileInteractionService> fileMock = TestSetupHelper.CreateFileMock(testFilePath);
            Mock<IBaseDialogCreationService> dialogMock = TestSetupHelper.CreateDialogMock();
            IPortfolio portfolio = TestSetupHelper.CreateEmptyDataBase();
            var dataUpdater = TestSetupHelper.CreateUpdater(portfolio);
            OptionsToolbarViewModel viewModel = new OptionsToolbarViewModel(TestSetupHelper.CreateGlobalsMock(fileSystem, fileMock.Object, dialogMock.Object), null, portfolio);
            viewModel.UpdateRequest += dataUpdater.PerformUpdate;
            viewModel.UpdateDataCommand.Execute(1);
            //Input prespecified example database

            Assert.AreEqual(testFilePath, portfolio.Name);
            Assert.AreEqual(1, portfolio.Funds.Count);
            Assert.AreEqual(1, portfolio.BankAccounts.Count);
            Assert.AreEqual(1, portfolio.BenchMarks.Count);
        }

        [Test]
        public void CanRefreshDatabase()
        {
            FileSystem fileSystem = new FileSystem();
            Mock<IFileInteractionService> fileMock = TestSetupHelper.CreateFileMock("notNeeded");
            Mock<IBaseDialogCreationService> dialogMock = TestSetupHelper.CreateDialogMock(MessageBoxOutcome.Yes);
            IPortfolio portfolio = TestSetupHelper.CreateBasicDataBase();
            var dataUpdater = TestSetupHelper.CreateUpdater(portfolio);
            OptionsToolbarViewModel viewModel = new OptionsToolbarViewModel(TestSetupHelper.CreateGlobalsMock(fileSystem, fileMock.Object, dialogMock.Object), null, portfolio);
            viewModel.UpdateRequest += dataUpdater.PerformUpdate;
            viewModel.RefreshCommand.Execute(1);
            //Check that data held is an empty database

            Assert.AreEqual("TestFilePath", portfolio.Name);
            Assert.AreEqual(1, portfolio.Funds.Count);
            Assert.AreEqual(1, portfolio.BankAccounts.Count);
            Assert.AreEqual(1, portfolio.BenchMarks.Count);
        }
    }
}
