﻿using System;
using System.Collections.Generic;
using System.IO.Abstractions;

using Effanville.Common.Structure.DataEdit;
using Effanville.Common.Structure.DataStructures;
using Effanville.Common.Structure.Reporting;
using Effanville.Common.UI;
using Effanville.Common.UI.Services;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.Database.Extensions.DataEdit;
using Effanville.FinancialStructures.DataStructures;
using Effanville.FinancialStructures.Download;
using Effanville.FinancialStructures.NamingStructures;
using Effanville.FPD.Logic.Configuration;
using Effanville.FPD.Logic.TemplatesAndStyles;
using Effanville.FPD.Logic.ViewModels;

using Moq;

namespace Effanville.FPD.Logic.Tests.TestHelpers
{
    public static class TestSetupHelper
    {
        internal static IReportLogger DummyReportLogger => new NothingReportLogger();

        public static Mock<IFileInteractionService> CreateFileMock(string filePath)
        {
            Mock<IFileInteractionService> mockfileinteraction = new Mock<IFileInteractionService>();
            _ = mockfileinteraction.Setup(x => x.OpenFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new FileInteractionResult(true, filePath));
            _ = mockfileinteraction.Setup(x => x.SaveFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new FileInteractionResult(true, filePath));
            return mockfileinteraction;
        }

        public static IUiStyles SetupDefaultStyles()
        {
            Mock<IUiStyles> styles = new Mock<IUiStyles>();
            styles.SetupGet(x => x.IsLightTheme).Returns(true);
            return styles.Object;
        }

        internal static IViewModelFactory SetupViewModelFactory(
            IUiStyles styles,
            UiGlobals globals,
            IUpdater updater,
            IPortfolioDataDownloader downloader,
            IConfiguration config,
            IAccountStatisticsProvider statisticsProvider)
            => new ViewModelFactory(
                styles,
                globals,
                updater,
                downloader,
                config,
                statisticsProvider);

        public static Mock<IFileInteractionService> CreateFileMock(string openFilePath, string saveFilePath)
        {
            Mock<IFileInteractionService> mockfileinteraction = new Mock<IFileInteractionService>();
            _ = mockfileinteraction.Setup(x => x.OpenFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new FileInteractionResult(true, openFilePath));
            _ = mockfileinteraction.Setup(x => x.SaveFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new FileInteractionResult(true, saveFilePath));
            return mockfileinteraction;
        }

        public static Mock<IBaseDialogCreationService> CreateDialogMock(MessageBoxOutcome result = MessageBoxOutcome.Yes)
        {
            Mock<IBaseDialogCreationService> mockfileinteraction = new Mock<IBaseDialogCreationService>();
            _ = mockfileinteraction.Setup(x => x.ShowMessageBox(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<BoxButton>(), It.IsAny<BoxImage>())).Returns(result);
            return mockfileinteraction;
        }

        public static IUpdater SetupUpdater() => new SynchronousUpdater();

        public static IAccountStatisticsProvider SetupProvider()
        {
            Mock<IAccountStatisticsProvider> mockProvider = new Mock<IAccountStatisticsProvider>();
            return mockProvider.Object;
        }

        public static UiGlobals SetupGlobalsMock(
            IFileSystem fileSystem,
            IFileInteractionService fileService,
            IBaseDialogCreationService dialogCreationService,
            IReportLogger logger = null)
            => new UiGlobals(
                null,
                SetupDispatcher(),
                fileSystem,
                fileService,
                dialogCreationService,
                logger ?? DummyReportLogger);

        public static IDispatcher SetupDispatcher()
        {
            Mock<IDispatcher> dispatcherMock = new Mock<IDispatcher>();
            _ = dispatcherMock.Setup(x => x.Invoke(It.IsAny<Action>())).Callback((Action a) => a());

            _ = dispatcherMock.Setup(x => x.BeginInvoke(It.IsAny<Action>())).Callback((Action a) => a());
            return dispatcherMock.Object;
        }
        public static IPortfolioDataDownloader SetupDownloader()
        {
            Mock<IPortfolioDataDownloader> downloaderMock = new Mock<IPortfolioDataDownloader>();

            return downloaderMock.Object;
        }

        public static IReportLogger SetupReportLogger()
        {
            LogReporter reportLogger = new LogReporter(LogAction, saveInternally: true);
            return reportLogger;

            void LogAction(ReportSeverity sev, ReportType error, string loc, string msg)
            {
            }
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
            portfolio.Name = "TestFilePath";
            _ = portfolio.TryAdd(Account.Security, new NameData("Fidelity", "China", "GBP", "https://markets.ft.com/data/funds/tearsheet/summary?s=gb00b5lxgg05:gbx", new HashSet<string>() { "Bonds", "UK" }));
            TwoName secName = new TwoName("Fidelity", "China");
            _ = portfolio.TryAddOrEditTradeData(Account.Security, secName, new SecurityTrade(TradeType.Buy, secName, new DateTime(2000, 1, 1), 1, 1, 1), new SecurityTrade(TradeType.Buy, secName, new DateTime(2000, 1, 1), 1, 1, 2));
            _ = portfolio.TryAddOrEditData(Account.Security, secName, new DailyValuation(new DateTime(2000, 1, 1), 1), new DailyValuation(new DateTime(2000, 1, 1), 1));
            _ = portfolio.TryAdd(Account.BankAccount, new NameData("Barclays", "currentAccount", url: "https://markets.ft.com/data/funds/tearsheet/summary?s=gb00b5lxgg05:gbx"));
            _ = portfolio.TryAddOrEditData(Account.BankAccount, new NameData("Barclays", "currentAccount"), new DailyValuation(new DateTime(2000, 1, 1), 1), new DailyValuation(new DateTime(2000, 1, 1), 1));
            _ = portfolio.TryAdd(Account.Currency, new NameData(string.Empty, "GBP"));

            _ = portfolio.TryAdd(Account.Benchmark, new NameData(string.Empty, "UK", string.Empty, "http://www.hi.com"));

            _ = portfolio.TryAdd(Account.Asset, new NameData("House", "MyHouse"));
            _ = portfolio.TryAddOrEditData(Account.Asset, new NameData("House", "MyHouse"), new DailyValuation(new DateTime(2020, 1, 1), 300000), new DailyValuation(new DateTime(2020, 1, 1), 300000));
            _ = portfolio.TryAddOrEditAssetDebt(Account.Asset, new NameData("House", "MyHouse"), new DailyValuation(new DateTime(2020, 1, 1), 150000), new DailyValuation(new DateTime(2020, 1, 1), 150000));
        }

        public static IPortfolio CreateEmptyDataBase()
        {
            IPortfolio portfolio = PortfolioFactory.GenerateEmpty();
            portfolio.Name = "saved";
            return portfolio;
        }
    }
}
