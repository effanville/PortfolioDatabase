using System;

using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.FinanceStructures;
using Effanville.FinancialStructures.NamingStructures;
using Effanville.FinancialStructures.Persistence;
using Effanville.FPD.Logic.Tests.Context;
using Effanville.FPD.Logic.Tests.TestHelpers;
using Effanville.FPD.Logic.Tests.UserInteractions;
using Effanville.FPD.Logic.ViewModels;
using Effanville.FPD.Logic.ViewModels.Common;
using Effanville.FPD.Logic.ViewModels.Stats;

using Microsoft.Extensions.Logging;

using NSubstitute;

using NUnit.Framework;

using Reqnroll;

namespace Effanville.FPD.Logic.Tests.Steps;

[Binding]
public class MainWindowSteps
{
    private readonly MainViewModelTestContext _testContext;

    public MainWindowSteps(MainViewModelTestContext testContext)
        => _testContext = testContext;

    [AfterScenario]
    public void Reset() => _testContext.Reset();

    [Given(@"I have a MainWindowViewModel with no data")]
    public void GivenIHaveAMainWindowViewModelWithNoData()
        => Create(null);

    [Given(@"I have a MainWindowViewModel with data")]
    public void GivenIHaveAMainWindowViewModelWithTypeAndNoData(Table table)
        => Create(table);
    private void Create(Table table)
    {
        IPortfolio portfolio = TestSetupHelper.CreateEmptyDataBase();
        ILogger<ReportingWindowViewModel> loggerReportMock = Substitute.For<ILogger<ReportingWindowViewModel>>();
        _testContext.ModelData = portfolio;

        _testContext.ViewModel = new MainWindowViewModel(
            _testContext.Globals,
            portfolio,
            _testContext.ViewModelFactory,
            null, //config,
            new ReportingWindowViewModel(loggerReportMock, _testContext.Globals),
            new OptionsToolbarViewModel(_testContext.Globals, portfolio, _testContext.PortfolioDataDownloader, _testContext.Updater, new PortfolioPersistence(_testContext.Globals.ReportLogger)),
            new BasicDataViewModel(_testContext.Globals, portfolio, _testContext.Updater),
            new StatisticsChartsViewModel(_testContext.Globals, portfolio, _testContext.Updater));

        PortfolioGeneratorHelper.UpdateModelData(_testContext.ModelData, table);
        _testContext.ViewModel.OnModelUpdated(null, new PortfolioEventArgs(Account.All));
    }

    [Then(@"the user can see the number of MainWindowViewModel tabs is (.*)")]
    public void ThenTheUserCanSeeTheNumberOfMainWindowViewModelTabsIs(int p0)
        => Assert.That(_testContext.ViewModel.Tabs.Count, Is.EqualTo(p0));

    [Then(@"the user can see a tab with type (.*)")]
    public void ThenTheUserCanSeeATabWithType(Account accountType)
    {
        DataNamesViewModel accVM = _testContext.ViewModel.OpenAccountTab(accountType);
        Assert.That(accVM.DataType, Is.EqualTo(accountType));
    }

    [Then(@"the user can navigate to the tab with type (.*)")]
    public void ThenTheUserCanNavigateToTheTabWithType(Account accountType)
    {
        DataNamesViewModel accVM = _testContext.ViewModel.OpenAccountTab(accountType);
        _testContext.FocusedTab = accVM;
        Assert.That(accVM.DataType, Is.EqualTo(accountType));
    }

    [Then(@"the user can see the number of data name entries is (.*)")]
    public void ThenTheUserCanSeeTheNumberOfDataNameEntriesIs(int numberEntries)
        => Assert.That((_testContext.FocusedTab as DataNamesViewModel).DataNames.Count, Is.EqualTo(numberEntries));

    [When(@"MVM new names are added to the database")]
    public void WhenNewNamesAreAddedToTheDatabase(Table table)
    {
        string accountAsString = table.Rows[0]["Account"];
        Account account = Enum.Parse<Account>(accountAsString);
        NameData nameData = TableParsers.NameFromRow(table.Rows[0]);
        DataNamesViewModel accVM = _testContext.ViewModel.OpenAccountTab(account);
        _testContext.FocusedTab = accVM;
        var dialogService = _testContext.Globals.DialogCreationService as TestDialogService;
        accVM.AddName(dialogService, nameData);
        PortfolioGeneratorHelper.UpdateModelData(_testContext.ModelData, table);
    }

    [When(@"MVM names are removed from the database")]
    public void WhenNamesAreRemovedFromTheDatabase(Table table)
    {
        string accountAsString = table.Rows[0]["Account"];
        Account account = Enum.Parse<Account>(accountAsString);
        NameData nameData = TableParsers.NameFromRow(table.Rows[0]);

        DataNamesViewModel accVM = _testContext.ViewModel.OpenAccountTab(account);
        _testContext.FocusedTab = accVM;
        accVM.DeleteName(nameData);
        PortfolioGeneratorHelper.RemoveModelData(_testContext.ModelData, table);
    }

    [When(@"the user loads a tab from name")]
    public void WhenTheUserLoadsTabFromName(Table table)
    {
        NameData nameData = TableParsers.NameFromRow(table.Rows[0]);

        DataNamesViewModel accVM = _testContext.FocusedTab as DataNamesViewModel;
        NameData name = TableParsers.NameFromRow(table.Rows[0]);
        accVM.SelectName(name);
        accVM.OpenTab();
    }

    [Then(@"the user can navigate to the tab with name")]
    public void ThenTheUserSelectsTheMvmTabIndex(Table table)
    {
        string accountAsString = table.Rows[0]["Account"];
        Account account = Enum.Parse<Account>(accountAsString);
        NameData nameData = TableParsers.NameFromRow(table.Rows[0]);

        switch (account)
        {
            case Account.Unknown:
            case Account.All:
            {
                Assert.Fail($"Should never hit account of type {account}");
                return;
            }
            case Account.Security:
            case Account.Pension:
            {
                bool accountExists = _testContext.ModelData.TryGetAccount<ISecurity>(account, nameData, out var data);
                Assert.That(accountExists, Is.True);
                var tab = _testContext.ViewModel.OpenTab(data);
                Assert.That(tab, Is.Not.Null);
                break;
            }
            case Account.BankAccount:
            case Account.Benchmark:
            case Account.Currency:
            {
                bool accountExists = _testContext.ModelData.TryGetAccount<IValueList>(account, nameData, out var data);
                Assert.That(accountExists, Is.True);
                var tab = _testContext.ViewModel.OpenTab(data);
                Assert.That(tab, Is.Not.Null);
                break;
            }
            case Account.Asset:
            {
                bool accountExists = _testContext.ModelData.TryGetAccount<IAmortisableAsset>(account, nameData, out var data);
                Assert.That(accountExists, Is.True);
                var tab = _testContext.ViewModel.OpenTab(data);
                Assert.That(tab, Is.Not.Null);
                break;
            }
        }

    }
}