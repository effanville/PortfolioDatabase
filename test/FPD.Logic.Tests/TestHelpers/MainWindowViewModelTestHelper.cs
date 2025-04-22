using System.IO;
using System.IO.Abstractions.TestingHelpers;

using Effanville.Common.UI;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.Persistence;
using Effanville.FPD.Logic.Configuration;
using Effanville.FPD.Logic.TemplatesAndStyles;
using Effanville.FPD.Logic.ViewModels;
using Effanville.FPD.Logic.ViewModels.Stats;

using Microsoft.Extensions.Logging;

using NSubstitute;

using NUnit.Framework;

namespace Effanville.FPD.Logic.Tests.TestHelpers
{
    public abstract class MainWindowViewModelTestHelper
    {
        protected MockFileSystem FileSystem { get; set; }

        protected MainWindowViewModel ViewModel { get; private set; }

        [SetUp]
        public void Setup()
        {
            FileSystem = new MockFileSystem();
            string file = File.ReadAllText(TestConstants.ExampleDatabaseLocation + "\\BasicTestDatabase.xml");
            string testPath = "c:/temp/saved.xml";
            string saveFilePath = "c:/temp/newDatabase.xml";
            string testConfigPath = "c:/temp/saved/user.config";

            FileSystem.AddFile(testPath, new MockFileData(file));

            ILogger<OptionsToolbarViewModel> loggerMock = Substitute.For<ILogger<OptionsToolbarViewModel>>();
            ILogger<ReportingWindowViewModel> loggerReportMock = Substitute.For<ILogger<ReportingWindowViewModel>>();
            UiGlobals globals = TestSetupHelper.SetupGlobalsMock(
                FileSystem,
                TestSetupHelper.CreateFileMock(testPath, saveFilePath),
                TestSetupHelper.CreateDialogMock());

            UserConfiguration config = UserConfiguration.LoadFromUserConfigFile(
                testConfigPath,
                globals.CurrentFileSystem,
                globals.ReportLogger);
            IPortfolio portfolio = PortfolioFactory.GenerateEmpty();
            Common.Structure.DataEdit.SynchronousUpdater updater = new Common.Structure.DataEdit.SynchronousUpdater();
            IUiStyles styles = TestSetupHelper.SetupDefaultStyles();
            var downloader = TestSetupHelper.SetupDownloader();
            ViewModel = new MainWindowViewModel(globals,
                styles,
                portfolio,
                new ViewModelFactory(styles, globals, updater, downloader, config, new StatisticsProvider(portfolio)),
                config,
                new ReportingWindowViewModel(loggerReportMock, globals, styles),
                new OptionsToolbarViewModel(globals, styles, portfolio, downloader, updater, new PortfolioPersistence(globals.ReportLogger)),
                new BasicDataViewModel(globals, styles, portfolio, updater),
                new StatisticsChartsViewModel(globals, portfolio, styles, updater));
        }

        [TearDown]
        public void TearDown()
        {
            ViewModel = null;
        }
    }
}
