﻿using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Windows;
using Common.Structure.DataStructures;
using Common.Structure.Reporting;
using Common.UI;
using Common.UI.Services;
using FinancialStructures.Database;
using FinancialStructures.Database.Extensions;
using FinancialStructures.DataStructures;
using FinancialStructures.NamingStructures;
using Moq;

namespace FPD.Logic.Tests.TestHelpers
{
    public static class TestSetupHelper
    {
        internal static IReportLogger DummyReportLogger = new NothingReportLogger();
        public static Mock<IFileInteractionService> CreateFileMock(string filePath)
        {
            Mock<IFileInteractionService> mockfileinteraction = new Mock<IFileInteractionService>();
            _ = mockfileinteraction.Setup(x => x.OpenFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new FileInteractionResult(true, filePath));
            _ = mockfileinteraction.Setup(x => x.SaveFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new FileInteractionResult(true, filePath));
            return mockfileinteraction;
        }

        public static Mock<IFileInteractionService> CreateFileMock(string openFilePath, string saveFilePath)
        {
            Mock<IFileInteractionService> mockfileinteraction = new Mock<IFileInteractionService>();
            _ = mockfileinteraction.Setup(x => x.OpenFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new FileInteractionResult(true, openFilePath));
            _ = mockfileinteraction.Setup(x => x.SaveFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new FileInteractionResult(true, saveFilePath));
            return mockfileinteraction;
        }

        public static Mock<IDialogCreationService> CreateDialogMock(MessageBoxResult result = MessageBoxResult.Yes)
        {
            Mock<IDialogCreationService> mockfileinteraction = new Mock<IDialogCreationService>();
            _ = mockfileinteraction.Setup(x => x.ShowMessageBox(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<MessageBoxButton>(), It.IsAny<MessageBoxImage>())).Returns(result);
            return mockfileinteraction;
        }

        public static Action<Action<IPortfolio>> CreateDataUpdater(IPortfolio portfolio)
        {
            return action => action(portfolio);
        }

        public static UiGlobals CreateGlobalsMock(IFileSystem fileSystem, IFileInteractionService fileService, IDialogCreationService dialogCreationService, IReportLogger logger = null)
        {
            return new UiGlobals(null, DispatcherSetup().Object, fileSystem, fileService, dialogCreationService, logger ?? DummyReportLogger);
        }

        private static Mock<IDispatcher> DispatcherSetup()
        {
            Mock<IDispatcher> dispatcherMock = new Mock<IDispatcher>();
            _ = dispatcherMock.Setup(x => x.Invoke(It.IsAny<Action>())).Callback((Action a) => a());

            _ = dispatcherMock.Setup(x => x.BeginInvoke(It.IsAny<Action>())).Callback((Action a) => a());
            return dispatcherMock;
        }

        internal static Action<object> DummyOpenTab => action => OpenTab();

        private static void OpenTab()
        {
            return;
        }

        public static IPortfolio CreateBasicDataBase()
        {
            IPortfolio portfolio = PortfolioFactory.GenerateEmpty();
            UpdatePortfolio(portfolio);
            return portfolio;
        }

        public static void UpdatePortfolio(IPortfolio portfolio)
        {
            portfolio.BaseCurrency = "GBP";
            portfolio.FilePath = "TestFilePath";
            _ = portfolio.TryAdd(Account.Security, new NameData("Fidelity", "China", "GBP", "https://markets.ft.com/data/funds/tearsheet/summary?s=gb00b5lxgg05:gbx", new HashSet<string>() { "Bonds", "UK" }), DummyReportLogger);
            var secName = new TwoName("Fidelity", "China");
            _ = portfolio.TryAddOrEditDataToSecurity(secName, new DateTime(2000, 1, 1), new DateTime(2000, 1, 1), 1, 1, 1, new SecurityTrade(TradeType.Buy, secName, new DateTime(2000, 1, 1), 1, 1, 2));
            _ = portfolio.TryAdd(Account.BankAccount, new NameData("Barclays", "currentAccount", url: "https://markets.ft.com/data/funds/tearsheet/summary?s=gb00b5lxgg05:gbx"), DummyReportLogger);
            _ = portfolio.TryAddOrEditData(Account.BankAccount, new NameData("Barclays", "currentAccount"), new DailyValuation(new DateTime(2000, 1, 1), 1), new DailyValuation(new DateTime(2000, 1, 1), 1));
            _ = portfolio.TryAdd(Account.Currency, new NameData(string.Empty, "GBP"), DummyReportLogger);

            _ = portfolio.TryAdd(Account.Benchmark, new NameData(string.Empty, "UK", string.Empty, "http://www.hi.com"), DummyReportLogger);

            _ = portfolio.TryAdd(Account.Asset, new NameData("House", "MyHouse"), DummyReportLogger);
            _ = portfolio.TryAddOrEditData(Account.Asset, new NameData("House", "MyHouse"), new DailyValuation(new DateTime(2020, 1, 1), 300000), new DailyValuation(new DateTime(2020, 1, 1), 300000), DummyReportLogger);
            _ = portfolio.TryAddOrEditAssetDebt(Account.Asset, new NameData("House", "MyHouse"), new DailyValuation(new DateTime(2020, 1, 1), 150000), new DailyValuation(new DateTime(2020, 1, 1), 150000), DummyReportLogger);
        }

        public static IPortfolio CreateEmptyDataBase()
        {
            IPortfolio portfolio = PortfolioFactory.GenerateEmpty();
            portfolio.FilePath = "c:/temp/saved.xml";
            return portfolio;
        }
    }
}
