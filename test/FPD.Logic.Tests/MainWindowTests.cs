using System.IO.Abstractions.TestingHelpers;

using Effanville.FPD.Logic.Tests.TestHelpers;

using NUnit.Framework;

namespace Effanville.FPD.Logic.Tests
{
    /// <summary>
    /// Tests for the MainWindowViewModel.
    /// </summary>
    public class MainWindowTests : MainWindowViewModelTestHelper
    {
        private const int ExpectedNumberTabs = 10;

        /// <summary>
        /// Checks whether the MainWindowViewModel can be loaded successfully.
        /// </summary>
        [Test]
        public void CanSuccessfullyCreateMainViewModel()
        {
            Assert.That(ViewModel.Tabs.Count, Is.EqualTo(ExpectedNumberTabs));
        }

        [Test]
        public void CanSuccessfullyOpenNewPortfolio()
        {
            Assert.That(ViewModel.Tabs.Count, Is.EqualTo(ExpectedNumberTabs));

            ViewModel.OptionsToolbarCommands.NewDatabaseCommand.Execute(1);

            Assert.That(ViewModel.Tabs.Count, Is.EqualTo(ExpectedNumberTabs));
        }

        [Test]
        public void CanSuccessfullyLoadPortfolio()
        {
            Assert.That(ViewModel.Tabs.Count, Is.EqualTo(ExpectedNumberTabs));

            ViewModel.OptionsToolbarCommands.LoadDatabaseCommand.Execute(1);

            Assert.That(ViewModel.Tabs.Count, Is.EqualTo(ExpectedNumberTabs));
        }

        [Test]
        public void CanSuccessfullySavePortfolio()
        {
            Assert.That(ViewModel.Tabs.Count, Is.EqualTo(ExpectedNumberTabs));

            ViewModel.OptionsToolbarCommands.SaveDatabaseCommand.Execute(1);

            Assert.That(ViewModel.Tabs.Count, Is.EqualTo(ExpectedNumberTabs));

            MockFileData savedFile = FileSystem.GetFile("c:/temp/newDatabase.xml");
            Assert.That(savedFile.TextContents, Is.Not.Empty);
        }
    }
}
