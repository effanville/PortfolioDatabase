using System.IO.Abstractions.TestingHelpers;
﻿using FPD.Logic.Tests.TestHelpers;
using NUnit.Framework;

namespace FPD.Logic.Tests
{
    /// <summary>
    /// Tests for the MainWindowViewModel.
    /// </summary>
    public class MainWindowTests : MainWindowViewModelTestHelper
    {
        private const int fExpectedNumberTabs = 10;

        /// <summary>
        /// Checks whether the MainWindowViewModel can be loaded successfully.
        /// </summary>
        [Test]
        public void CanSuccessfullyCreateMainViewModel()
        {
            Assert.AreEqual(fExpectedNumberTabs, ViewModel.Tabs.Count);
        }

        [Test]
        public void CanSuccessfullyOpenNewPortfolio()
        {
            Assert.AreEqual(fExpectedNumberTabs, ViewModel.Tabs.Count);

            ViewModel.OptionsToolbarCommands.NewDatabaseCommand.Execute(1);

            Assert.AreEqual(fExpectedNumberTabs, ViewModel.Tabs.Count);
        }

        [Test]
        public void CanSuccessfullyLoadPortfolio()
        {
            Assert.AreEqual(fExpectedNumberTabs, ViewModel.Tabs.Count);

            ViewModel.OptionsToolbarCommands.LoadDatabaseCommand.Execute(1);

            Assert.AreEqual(fExpectedNumberTabs, ViewModel.Tabs.Count);
        }

        [Test]
        public void CanSuccessfullySavePortfolio()
        {
            Assert.AreEqual(fExpectedNumberTabs, ViewModel.Tabs.Count);

            ViewModel.OptionsToolbarCommands.SaveDatabaseCommand.Execute(1);

            Assert.AreEqual(fExpectedNumberTabs, ViewModel.Tabs.Count);

            MockFileData savedFile = FileSystem.GetFile("c:/temp/newDatabase.xml");
            Assert.IsNotEmpty(savedFile.TextContents);
        }
    }
}
