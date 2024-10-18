using System;
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
using Effanville.FinancialStructures.NamingStructures;
using Effanville.FPD.Logic.Configuration;
using Effanville.FPD.Logic.Tests.Support;
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
            _ = mockfileinteraction.Setup(x => x.OpenFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new FileInteractionResult(true, filePath));
            _ = mockfileinteraction.Setup(x => x.SaveFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new FileInteractionResult(true, filePath));
            return mockfileinteraction;
        }

        internal static IViewModelFactory CreateViewModelFactory(
            IPortfolio portfolio,
            IFileSystem fileSystem,
            IFileInteractionService fileService,
            IBaseDialogCreationService dialogCreationService,
            IConfiguration config,
            IReportLogger logger = null) 
            => new ViewModelFactory(
                null, 
                CreateGlobalsMock(fileSystem, fileService, dialogCreationService, logger), 
                CreateUpdater(portfolio),
                config);

        public static Mock<IFileInteractionService> CreateFileMock(string openFilePath, string saveFilePath)
        {
            Mock<IFileInteractionService> mockfileinteraction = new Mock<IFileInteractionService>();
            _ = mockfileinteraction.Setup(x => x.OpenFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new FileInteractionResult(true, openFilePath));
            _ = mockfileinteraction.Setup(x => x.SaveFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new FileInteractionResult(true, saveFilePath));
            return mockfileinteraction;
        }

        public static Mock<IBaseDialogCreationService> CreateDialogMock(MessageBoxOutcome result = MessageBoxOutcome.Yes)
        {
            Mock<IBaseDialogCreationService> mockfileinteraction = new Mock<IBaseDialogCreationService>();
            _ = mockfileinteraction.Setup(x => x.ShowMessageBox(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<BoxButton>(), It.IsAny<BoxImage>())).Returns(result);
            return mockfileinteraction;
        }

        public static IUpdater<TDataStore> CreateUpdater<TDataStore>(TDataStore portfolio) where TDataStore : class
            => new SynchronousUpdater<TDataStore>() { Database = portfolio };

        public static UiGlobals CreateGlobalsMock(IFileSystem fileSystem, IFileInteractionService fileService, IBaseDialogCreationService dialogCreationService, IReportLogger logger = null)
        {
            return new UiGlobals(null, TestDependencies.SetupDispatcher(), fileSystem, fileService, dialogCreationService, logger ?? DummyReportLogger);
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
            _ = portfolio.TryAdd(Account.Security, new NameData("Fidelity", "China", "GBP", "https://markets.ft.com/data/funds/tearsheet/summary?s=gb00b5lxgg05:gbx", new HashSet<string>() { "Bonds", "UK" }), DummyReportLogger);
            TwoName secName = new TwoName("Fidelity", "China");
            _ = portfolio.TryAddOrEditTradeData(Account.Security, secName, new SecurityTrade(TradeType.Buy, secName, new DateTime(2000, 1, 1), 1, 1, 1), new SecurityTrade(TradeType.Buy, secName, new DateTime(2000, 1, 1), 1, 1, 2));
            _ = portfolio.TryAddOrEditData(Account.Security, secName, new DailyValuation(new DateTime(2000, 1, 1), 1), new DailyValuation(new DateTime(2000, 1, 1), 1));
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
            portfolio.Name = "saved";
            return portfolio;
        }
    }
}
