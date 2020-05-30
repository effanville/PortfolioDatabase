﻿using System;
using System.Windows;
using FinanceWindowsViewModels;
using FPD_UI_UnitTests.TestConstruction;
using NUnit.Framework;

namespace FPD_UI_UnitTests
{
    public class OptionsToolbarTests
    {
        [Test]
        public void CanOpenNewDatabase()
        {
            Moq.Mock<UICommon.Services.IFileInteractionService> fileMock = TestingGUICode.CreateFileMock("notNeeded");
            Moq.Mock<UICommon.Services.IDialogCreationService> dialogMock = TestingGUICode.CreateDialogMock(MessageBoxResult.Yes);
            FinancialStructures.Database.Portfolio portfolio = TestingGUICode.CreateBasicDataBase();
            Action<Action<FinancialStructures.FinanceInterfaces.IPortfolio>> dataUpdater = TestingGUICode.CreateDataUpdater(portfolio);
            OptionsToolbarViewModel viewModel = new OptionsToolbarViewModel(portfolio, dataUpdater, TestingGUICode.DummyReportLogger, fileMock.Object, dialogMock.Object);
            viewModel.NewDatabaseCommand.Execute(1);
            //Check that data held is an empty database

            Assert.AreEqual("", portfolio.FilePath);
            Assert.AreEqual(0, portfolio.Funds.Count);
            Assert.AreEqual(0, portfolio.BankAccounts.Count);
            Assert.AreEqual(0, portfolio.BenchMarks.Count);
        }

        [Test]
        public void CanOpenDatabase()
        {
            string databaseToLoad = TestingGUICode.ExampleDatabaseFolder + "\\BasicTestDatabase.xml";
            string testFilePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + databaseToLoad;
            Moq.Mock<UICommon.Services.IFileInteractionService> fileMock = TestingGUICode.CreateFileMock(testFilePath);
            Moq.Mock<UICommon.Services.IDialogCreationService> dialogMock = TestingGUICode.CreateDialogMock();
            FinancialStructures.Database.Portfolio portfolio = TestingGUICode.CreateEmptyDataBase();
            Action<Action<FinancialStructures.FinanceInterfaces.IPortfolio>> dataUpdater = TestingGUICode.CreateDataUpdater(portfolio);
            OptionsToolbarViewModel viewModel = new OptionsToolbarViewModel(portfolio, dataUpdater, TestingGUICode.DummyReportLogger, fileMock.Object, dialogMock.Object);
            viewModel.LoadDatabaseCommand.Execute(1);
            //Input prespecified example database

            Assert.AreEqual(testFilePath, portfolio.FilePath);
            Assert.AreEqual(1, portfolio.Funds.Count);
            Assert.AreEqual(1, portfolio.BankAccounts.Count);
            Assert.AreEqual(1, portfolio.BenchMarks.Count);
        }

        [Test]
        [Ignore("IncompeteArchitecture - Requires improvements in dialog creation to be able to run.")]
        public void CanOpenHelpDocsPage()
        {
            string databaseToLoad = TestingGUICode.ExampleDatabaseFolder + "\\BasicTestDatabase.xml";
            string testFilePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + databaseToLoad;
            Moq.Mock<UICommon.Services.IFileInteractionService> fileMock = TestingGUICode.CreateFileMock(testFilePath);
            Moq.Mock<UICommon.Services.IDialogCreationService> dialogMock = TestingGUICode.CreateDialogMock();
            FinancialStructures.Database.Portfolio portfolio = TestingGUICode.CreateEmptyDataBase();
            Action<Action<FinancialStructures.FinanceInterfaces.IPortfolio>> dataUpdater = TestingGUICode.CreateDataUpdater(portfolio);
            OptionsToolbarViewModel viewModel = new OptionsToolbarViewModel(portfolio, dataUpdater, TestingGUICode.DummyReportLogger, fileMock.Object, dialogMock.Object);
            viewModel.OpenHelpCommand.Execute(1);
            //Input prespecified example database

            Assert.AreEqual(testFilePath, portfolio.FilePath);
            Assert.AreEqual(1, portfolio.Funds.Count);
            Assert.AreEqual(1, portfolio.BankAccounts.Count);
            Assert.AreEqual(1, portfolio.BenchMarks.Count);
        }

        [Test]
        [Ignore("IncompeteArchitecture - Test needs to have a dummy file structure to do this in.")]
        public void CanSaveDatabase()
        {
            string databaseToLoad = TestingGUICode.ExampleDatabaseFolder + "\\BasicTestDatabase.xml";
            string testFilePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + databaseToLoad;
            Moq.Mock<UICommon.Services.IFileInteractionService> fileMock = TestingGUICode.CreateFileMock(testFilePath);
            Moq.Mock<UICommon.Services.IDialogCreationService> dialogMock = TestingGUICode.CreateDialogMock();
            FinancialStructures.Database.Portfolio portfolio = TestingGUICode.CreateEmptyDataBase();
            Action<Action<FinancialStructures.FinanceInterfaces.IPortfolio>> dataUpdater = TestingGUICode.CreateDataUpdater(portfolio);
            OptionsToolbarViewModel viewModel = new OptionsToolbarViewModel(portfolio, dataUpdater, TestingGUICode.DummyReportLogger, fileMock.Object, dialogMock.Object);
            viewModel.SaveDatabaseCommand.Execute(1);
            //Input prespecified example database

            Assert.AreEqual(testFilePath, portfolio.FilePath);
            Assert.AreEqual(1, portfolio.Funds.Count);
            Assert.AreEqual(1, portfolio.BankAccounts.Count);
            Assert.AreEqual(1, portfolio.BenchMarks.Count);
        }

        [Test]
        [Ignore("IncompeteArchitecture - Downloader does not currently allow for use in test environment.")]
        public void CanUpdateDatabase()
        {
            string databaseToLoad = TestingGUICode.ExampleDatabaseFolder + "\\BasicTestDatabase.xml";
            string testFilePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + databaseToLoad;
            Moq.Mock<UICommon.Services.IFileInteractionService> fileMock = TestingGUICode.CreateFileMock(testFilePath);
            Moq.Mock<UICommon.Services.IDialogCreationService> dialogMock = TestingGUICode.CreateDialogMock();
            FinancialStructures.Database.Portfolio portfolio = TestingGUICode.CreateEmptyDataBase();
            Action<Action<FinancialStructures.FinanceInterfaces.IPortfolio>> dataUpdater = TestingGUICode.CreateDataUpdater(portfolio);
            OptionsToolbarViewModel viewModel = new OptionsToolbarViewModel(portfolio, dataUpdater, TestingGUICode.DummyReportLogger, fileMock.Object, dialogMock.Object);
            viewModel.UpdateDataCommand.Execute(1);
            //Input prespecified example database

            Assert.AreEqual(testFilePath, portfolio.FilePath);
            Assert.AreEqual(1, portfolio.Funds.Count);
            Assert.AreEqual(1, portfolio.BankAccounts.Count);
            Assert.AreEqual(1, portfolio.BenchMarks.Count);
        }

        [Test]
        public void CanRefreshDatabase()
        {
            Moq.Mock<UICommon.Services.IFileInteractionService> fileMock = TestingGUICode.CreateFileMock("notNeeded");
            Moq.Mock<UICommon.Services.IDialogCreationService> dialogMock = TestingGUICode.CreateDialogMock(MessageBoxResult.Yes);
            FinancialStructures.Database.Portfolio portfolio = TestingGUICode.CreateBasicDataBase();
            Action<Action<FinancialStructures.FinanceInterfaces.IPortfolio>> dataUpdater = TestingGUICode.CreateDataUpdater(portfolio);
            OptionsToolbarViewModel viewModel = new OptionsToolbarViewModel(portfolio, dataUpdater, TestingGUICode.DummyReportLogger, fileMock.Object, dialogMock.Object);
            viewModel.RefreshCommand.Execute(1);
            //Check that data held is an empty database

            Assert.AreEqual("TestFilePath", portfolio.FilePath);
            Assert.AreEqual(1, portfolio.Funds.Count);
            Assert.AreEqual(1, portfolio.BankAccounts.Count);
            Assert.AreEqual(1, portfolio.BenchMarks.Count);
        }
    }
}
