using Effanville.Common.Structure.DisplayClasses;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.NamingStructures;
using Effanville.FPD.Logic.Tests.TestHelpers;
using Effanville.FPD.Logic.Tests.UserInteractions;
using Effanville.FPD.Logic.Tests.ViewModelExtensions;

using FPD.Logic.ViewModels;
using FPD.Logic.ViewModels.Common;

using NUnit.Framework;

namespace Effanville.FPD.Logic.Tests
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

            Assert.AreEqual(1, ViewModel.ProgramPortfolio.BankAccounts.Count);
            Assert.AreEqual(1, ViewModel.ProgramPortfolio.Funds.Count);
            Assert.AreEqual(1, ViewModel.ProgramPortfolio.BenchMarks.Count);

            BasicDataViewModel dataView = ViewModel.Tabs[0] as BasicDataViewModel;
            dataView.UpdateData(ViewModel.ProgramPortfolio);
            Assert.That(dataView != null, nameof(dataView) + " != null");
            Assert.AreEqual("saved", dataView.PortfolioNameText);
            Assert.AreEqual("Total Securities: 1", dataView.SecurityTotalText);
            Assert.AreEqual("Total Bank Accounts: 1", dataView.BankAccountTotalText);

            ValueListWindowViewModel securityView = ViewModel.SecurityWindow();
            securityView.UpdateData(ViewModel.ProgramPortfolio);
            DataNamesViewModel securityNamesView = securityView.GetDataNamesViewModel();
            securityNamesView.UpdateData(ViewModel.ProgramPortfolio);
            Assert.AreEqual(1, securityNamesView.DataNames.Count);

            ValueListWindowViewModel bankAccView = ViewModel.Window(Account.BankAccount);
            bankAccView.UpdateData(ViewModel.ProgramPortfolio);
            DataNamesViewModel bankAccNamesView = bankAccView.GetDataNamesViewModel();
            bankAccView.UpdateData(ViewModel.ProgramPortfolio);
            Assert.AreEqual(1, bankAccNamesView.DataNames.Count);
        }


        [Test]
        public void AddingSecurityUpdatesSuccessfully()
        {
            ValueListWindowViewModel securityViewModel = ViewModel.SecurityWindow();
            DataNamesViewModel securityNames = securityViewModel.Tabs[0] as DataNamesViewModel;
            Assert.That(securityNames != null, nameof(securityNames) + " != null");
            
            RowData selectedInitialName = new RowData(new NameData(), false, securityNames.DataType, securityNames._updater, null);
            securityNames.DataNames.Add(selectedInitialName);
            securityNames.SelectionChangedCommand.Execute(selectedInitialName);
            Selectable<NameData> selectedEditedName = new Selectable<NameData>(new NameData("Forgotten", "New"), false);
            securityNames.CreateCommand.Execute(selectedEditedName);
            Assert.AreEqual(1, securityNames.DataNames.Count);
        }
    }
}
