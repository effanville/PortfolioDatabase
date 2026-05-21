using Effanville.Common.Structure.DisplayClasses;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.NamingStructures;
using Effanville.FPD.Logic.Tests.TestHelpers;
using Effanville.FPD.Logic.Tests.UserInteractions;
using Effanville.FPD.Logic.Tests.ViewModelExtensions;
using Effanville.FPD.Logic.ViewModels;
using Effanville.FPD.Logic.ViewModels.Common;

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

            Assert.That(ViewModel.ProgramPortfolio.BankAccounts.Count, Is.EqualTo(1));
            Assert.That(ViewModel.ProgramPortfolio.Funds.Count, Is.EqualTo(1));
            Assert.That(ViewModel.ProgramPortfolio.BenchMarks.Count, Is.EqualTo(1));

            BasicDataViewModel dataView = ViewModel.Tabs[0] as BasicDataViewModel;
            Assert.That(dataView != null, nameof(dataView) + " != null");
            dataView.UpdateData(ViewModel.ProgramPortfolio, false);
            Assert.That(dataView.PortfolioNameText, Is.EqualTo("saved"));
            Assert.That(dataView.SecurityTotalText, Is.EqualTo("Total Securities: 1"));
            Assert.That(dataView.BankAccountTotalText, Is.EqualTo("Total Bank Accounts: 1"));

            DataNamesViewModel securityView = ViewModel.OpenAccountTab(Account.Security);
            securityView.UpdateData(ViewModel.ProgramPortfolio, false);
            Assert.That(securityView.DataNames.Count, Is.EqualTo(1));

            DataNamesViewModel bankAccView = ViewModel.OpenAccountTab(Account.BankAccount);
            bankAccView.UpdateData(ViewModel.ProgramPortfolio, false);
            Assert.That(bankAccView.DataNames.Count, Is.EqualTo(1));
        }


        [Test]
        public void AddingSecurityUpdatesSuccessfully()
        {
            DataNamesViewModel securityNames = ViewModel.OpenAccountTab(Account.Security);
            Assert.That(securityNames != null, nameof(securityNames) + " != null");

            NameDataViewModel selectedInitialName = new NameDataViewModel("", new NameData(), false, null);
            securityNames.DataNames.Add(selectedInitialName);
            securityNames.SelectionChangedCommand.Execute(selectedInitialName);
            Selectable<NameData> selectedEditedName = new Selectable<NameData>(new NameData("Forgotten", "New"), false);
            securityNames.AddCommand.Execute(selectedEditedName);
            Assert.That(securityNames.DataNames.Count, Is.EqualTo(1));
        }
    }
}
