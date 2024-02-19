using System.IO;
using System.IO.Abstractions.TestingHelpers;

using Effanville.Common.UI;
using Effanville.FinancialStructures.Database;
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

            FileSystem.AddFile(testPath, new MockFileData(file));

            UiGlobals globals = TestSetupHelper.CreateGlobalsMock(FileSystem, TestSetupHelper.CreateFileMock(testPath, saveFilePath).Object, TestSetupHelper.CreateDialogMock().Object);
            ViewModel = new MainWindowViewModel(globals, new SynchronousUpdater<IPortfolio>());
        }

        [TearDown]
        public void TearDown()
        {
            ViewModel = null;
        }
    }
}
