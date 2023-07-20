﻿using Common.Structure.DisplayClasses;
using FPD.Logic.ViewModels;
using FPD.Logic.ViewModels.Common;
using FPD.Logic.ViewModels.Security;
using FPD.Logic.Tests.TestHelpers;
using FPD.Logic.Tests.ViewModelExtensions;
using FinancialStructures.Database;
using FinancialStructures.NamingStructures;
using NUnit.Framework;

namespace FPD.Logic.Tests
{
    /// <summary>
    /// Tests to ensure the entire ViewModel system integrates together.
    /// </summary>
    public class IntegrationTests : MainWindowViewModelTestHelper
    {
        /// <summary>
        /// The open database button propagates the new database to all tabs in the main view.
        /// </summary>
        [Test]
        public void OpenDatabaseUpdatesAllTabs()
        {
            ViewModel.OptionsToolbarCommands.LoadDatabaseCommand.Execute(1);

            Assert.AreEqual(1, ViewModel.ProgramPortfolio.BankAccountsThreadSafe.Count);
            Assert.AreEqual(1, ViewModel.ProgramPortfolio.FundsThreadSafe.Count);
            Assert.AreEqual(1, ViewModel.ProgramPortfolio.BenchMarksThreadSafe.Count);

            BasicDataViewModel dataView = ViewModel.Tabs[0] as BasicDataViewModel;
            Assert.AreEqual("saved", dataView.PortfolioNameText);
            Assert.AreEqual("Total Securities: 1", dataView.SecurityTotalText);
            Assert.AreEqual("Total Bank Accounts: 1", dataView.BankAccountTotalText);

            SecurityEditWindowViewModel securityView = ViewModel.SecurityWindow();
            DataNamesViewModel securityNamesView = securityView.DataNames();
            Assert.AreEqual(1, securityNamesView.DataNames.Count);

            ValueListWindowViewModel bankAccView = ViewModel.Window(Account.BankAccount);
            DataNamesViewModel bankAccNamesView = bankAccView.DataNames();
            Assert.AreEqual(1, bankAccNamesView.DataNames.Count);
        }


        [Test]
        public void AddingSecurityUpdatesSuccessfully()
        {
            SecurityEditWindowViewModel securityViewModel = ViewModel.SecurityWindow();
            DataNamesViewModel securityNames = securityViewModel.Tabs[0] as DataNamesViewModel;
            RowData selectedInitialName = new RowData(new NameData(), false, securityNames.TypeOfAccount, securityNames._updater, null);
            securityNames.DataNames.Add(selectedInitialName);
            securityNames.SelectionChangedCommand.Execute(selectedInitialName);

            Selectable<NameData> selectedEditedName = new Selectable<NameData>(new NameData("Forgotton", "New"), false);
            securityNames.CreateCommand.Execute(selectedEditedName);

            Assert.AreEqual(1, securityNames.DataNames.Count);
        }
    }
}
