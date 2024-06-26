using System;

using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.FinanceStructures;
using Effanville.FPD.Logic.Tests.Context;
using Effanville.FPD.Logic.Tests.UserInteractions;
using Effanville.FPD.Logic.ViewModels.Common;

using NUnit.Framework;

using TechTalk.SpecFlow;

namespace Effanville.FPD.Logic.Tests.Steps;

[Binding]
public class ValueListWindowSteps
{
    private readonly ViewModelTestContext<IPortfolio, ValueListWindowViewModel> _testContext;

    public ValueListWindowSteps(ViewModelTestContext<IPortfolio, ValueListWindowViewModel> testContext)
    {
        _testContext = testContext;
    }

    [AfterScenario]
    public void Reset() => _testContext.Reset();

    [Given(@"I have a ValueListWindowViewModel with type (.*) and no data")]
    public void GivenIHaveAValueListWindowViewModelWithTypeAndNoData(Account account)
        => Create(account, null);

    [Given(@"I have a ValueListWindowViewModel with type (.*) and data")]
    public void GivenIHaveAValueListWindowViewModelWithTypeAndNoData(Account account, Table table)
        => Create(account, table);
    
    [StepDefinition(@"the ValueListWindowViewModel is brought into focus")]
    public void GivenTheDataNamesViewModelIsBroughtIntoFocus()
        => _testContext.ViewModel.UpdateData(_testContext.ModelData);
    private void Create(Account account, Table table)
    {
        var portfolio = PortfolioGeneratorHelper.CreateFromTable(table);
        _testContext.ModelData = portfolio;
        _testContext.Updater.Database = portfolio;

        _testContext.ViewModel = new ValueListWindowViewModel(
            _testContext.Globals,
            _testContext.Styles,
            portfolio,
            account.ToString(),
            account,
            _testContext.Updater,
            _testContext.ViewModelFactory);
        _testContext.ViewModel.UpdateRequest += _testContext.Updater.PerformUpdate;
    }

    [Then(@"I can see the ValueListWindowViewModel type is (.*)")]
    public void ThenICanSeeTheValueListWindowViewModelTypeIs(Account account)
        => Assert.AreEqual(account, _testContext.ViewModel.DataType);

    [Then(@"the user can see the number of ValueListWindowViewModel tabs is (.*)")]
    public void ThenTheUserCanSeeTheNumberOfValueListWindowViewModelTabsIs(int p0)
        => Assert.AreEqual(p0, _testContext.ViewModel.Tabs.Count);

    [Then(@"the user can see the number of VLWVM data name entries is (.*)")]
    public void ThenTheUserCanSeeTheNumberOfDataNameEntriesIs(int p0)
    {
        DataNamesViewModel nameModel = _testContext.ViewModel.GetDataNamesViewModel();
        Assert.AreEqual(p0, nameModel.DataNames.Count);
    }    
    
    [When(@"VLWVM new names are added to the database")]
    public void WhenNewNamesAreAddedToTheDatabase(Table table)
        => PortfolioGeneratorHelper.UpdateModelData(_testContext.ModelData, table);
   
    [When(@"VLWVM names are removed from the database")]
    public void WhenNamesAreRemovedFromTheDatabase(Table table)
        => PortfolioGeneratorHelper.RemoveModelData(_testContext.ModelData, table);

    [When(@"the user loads a VLWVM tab from name")]
    public void WhenTheUserLoadsAvlwvmTabFromName(Table table)
    {
        var name = PortfolioGeneratorHelper.NameDataFromRow(table.Rows[0]);
        _testContext.ViewModel.LoadTabFunc(name);
    }

    [Then(@"the user selects the VLWVM tab index (.*) with name")]
    public void ThenTheUserSelectsTheVlwvmTabIndex(int p0, Table table)
    {
        var desiredTab = _testContext.ViewModel.Tabs[p0 - 1];
        var expectedName = PortfolioGeneratorHelper.NameDataFromRow(table.Rows[0]);
        Assert.IsNotNull(desiredTab);
        switch (_testContext.ViewModel.DataType)
        {
            case Account.Security:
            case Account.Pension:
            {
                var tab = desiredTab as StyledClosableViewModelBase<ISecurity, IPortfolio>;
                var actualName = tab.ModelData.Names;
                Assert.AreEqual(expectedName.Company, actualName.Company);
                Assert.AreEqual(expectedName.Name, actualName.Name);
                break;
            }
            case Account.Benchmark:
            case Account.BankAccount:
            case Account.Currency:
            {
                var tab = desiredTab as StyledClosableViewModelBase<IValueList, IPortfolio>;
                var actualName = tab.ModelData.Names;
                Assert.AreEqual(expectedName.Company, actualName.Company);
                Assert.AreEqual(expectedName.Name, actualName.Name);
                break;
            }
            case Account.Asset:
            {
                var tab = desiredTab as StyledClosableViewModelBase<IAmortisableAsset, IPortfolio>;
                var actualName = tab.ModelData.Names;
                Assert.AreEqual(expectedName.Company, actualName.Company);
                Assert.AreEqual(expectedName.Name, actualName.Name);
                break;
            }
            case Account.Unknown:
            case Account.All:
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}