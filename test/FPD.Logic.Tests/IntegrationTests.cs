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

            ValueListWindowViewModel securityView = ViewModel.SecurityWindow();
            securityView.UpdateData(ViewModel.ProgramPortfolio, false);
            DataNamesViewModel securityNamesView = securityView.GetDataNamesViewModel();
            securityNamesView.UpdateData(ViewModel.ProgramPortfolio, false);
            Assert.That(securityNamesView.DataNames.Count, Is.EqualTo(1));

            ValueListWindowViewModel bankAccView = ViewModel.Window(Account.BankAccount);
            bankAccView.UpdateData(ViewModel.ProgramPortfolio, false);
            DataNamesViewModel bankAccNamesView = bankAccView.GetDataNamesViewModel();
            bankAccView.UpdateData(ViewModel.ProgramPortfolio, false);
            Assert.That(bankAccNamesView.DataNames.Count, Is.EqualTo(1));
        }


        [Test]
        public void AddingSecurityUpdatesSuccessfully()
        {
            ValueListWindowViewModel securityViewModel = ViewModel.SecurityWindow();
            DataNamesViewModel securityNames = securityViewModel.Tabs[0] as DataNamesViewModel;
            Assert.That(securityNames != null, nameof(securityNames) + " != null");

            NameDataViewModel selectedInitialName = new NameDataViewModel("", new NameData(), false, securityNames.UpdateNameData, null, null);
            securityNames.DataNames.Add(selectedInitialName);
            securityNames.SelectionChangedCommand.Execute(selectedInitialName);
            Selectable<NameData> selectedEditedName = new Selectable<NameData>(new NameData("Forgotten", "New"), false);
            securityNames.CreateCommand.Execute(selectedEditedName);
            Assert.That(securityNames.DataNames.Count, Is.EqualTo(1));
        }
    }
}
