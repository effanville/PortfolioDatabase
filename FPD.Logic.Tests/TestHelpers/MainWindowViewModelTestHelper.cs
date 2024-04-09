using System.IO;
using System.IO.Abstractions.TestingHelpers;

using Effanville.Common.UI;
using Effanville.FinancialStructures.Database;
using Effanville.FPD.Logic.Configuration;
using Effanville.FPD.Logic.TemplatesAndStyles;
using Effanville.FPD.Logic.ViewModels;

using NUnit.Framework;

namespace Effanville.FPD.Logic.Tests.TestHelpers
{
    public abstract class MainWindowViewModelTestHelper
    {
        protected MockFileSystem FileSystem { get; set; }

        protected MainWindowViewModel ViewModel
        {
            get;
            private set;
        }

        [SetUp]
        public void Setup()
        {
            FileSystem = new MockFileSystem();
            string file = File.ReadAllText(TestConstants.ExampleDatabaseLocation + "\\BasicTestDatabase.xml");
            string testPath = "c:/temp/saved.xml";
            string saveFilePath = "c:/temp/newDatabase.xml";
            string testConfigPath = "c:/temp/saved/user.config";

            FileSystem.AddFile(testPath, new MockFileData(file));

            UiGlobals globals = TestSetupHelper.CreateGlobalsMock(FileSystem, TestSetupHelper.CreateFileMock(testPath, saveFilePath).Object, TestSetupHelper.CreateDialogMock().Object);
            
            UserConfiguration config = UserConfiguration.LoadFromUserConfigFile(
                testConfigPath,
                globals.CurrentFileSystem,
                globals.ReportLogger);
            var portfolio = PortfolioFactory.GenerateEmpty();
            var updater = new SynchronousUpdater<IPortfolio>(portfolio);
            var styles = new UiStyles(false);
            ViewModel = new MainWindowViewModel(
                portfolio,
                styles, globals,
                new ViewModelFactory(styles, globals, updater),
                updater,
                config);
        }

        [TearDown]
        public void TearDown()
        {
            ViewModel = null;
        }
    }
}
