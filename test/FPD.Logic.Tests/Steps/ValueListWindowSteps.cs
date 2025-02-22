using System;

using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.FinanceStructures;
using Effanville.FinancialStructures.NamingStructures;
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
        => _testContext.ViewModel.UpdateData(_testContext.ModelData, false);
    private void Create(Account account, Table table)
    {
        IPortfolio portfolio = PortfolioGeneratorHelper.CreateFromTable(table);
        _testContext.ModelData = portfolio;

        _testContext.ViewModel = new ValueListWindowViewModel(
            _testContext.Globals,
            _testContext.Styles,
            portfolio,
            account.ToString(),
            account,
            _testContext.Updater,
            _testContext.ViewModelFactory);
    }

    [Then(@"I can see the ValueListWindowViewModel type is (.*)")]
    public void ThenICanSeeTheValueListWindowViewModelTypeIs(Account account)
        => Assert.That(_testContext.ViewModel.DataType, Is.EqualTo(account));

    [Then(@"the user can see the number of ValueListWindowViewModel tabs is (.*)")]
    public void ThenTheUserCanSeeTheNumberOfValueListWindowViewModelTabsIs(int p0)
        => Assert.That(_testContext.ViewModel.Tabs.Count, Is.EqualTo(p0));

    [Then(@"the user can see the number of VLWVM data name entries is (.*)")]
    public void ThenTheUserCanSeeTheNumberOfDataNameEntriesIs(int p0)
    {
        DataNamesViewModel nameModel = _testContext.ViewModel.GetDataNamesViewModel();
        Assert.That(nameModel.DataNames.Count, Is.EqualTo(p0));
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
        NameData name = PortfolioGeneratorHelper.NameDataFromRow(table.Rows[0]);
        _testContext.ViewModel.LoadTabFunc(name);
    }

    [Then(@"the user selects the VLWVM tab index (.*) with name")]
    public void ThenTheUserSelectsTheVlwvmTabIndex(int p0, Table table)
    {
        object desiredTab = _testContext.ViewModel.Tabs[p0 - 1];
        NameData expectedName = PortfolioGeneratorHelper.NameDataFromRow(table.Rows[0]);
        Assert.That(desiredTab, Is.Not.Null);
        switch (_testContext.ViewModel.DataType)
        {
            case Account.Security:
            case Account.Pension:
            {
                StyledClosableViewModelBase<ISecurity> tab = desiredTab as StyledClosableViewModelBase<ISecurity>;
                NameData actualName = tab.ModelData.Names;
                Assert.That(actualName.Company, Is.EqualTo(expectedName.Company));
                Assert.That(actualName.Name, Is.EqualTo(expectedName.Name));
                break;
            }
            case Account.Benchmark:
            case Account.BankAccount:
            case Account.Currency:
            {
                StyledClosableViewModelBase<IValueList> tab = desiredTab as StyledClosableViewModelBase<IValueList>;
                NameData actualName = tab.ModelData.Names;
                Assert.That(actualName.Company, Is.EqualTo(expectedName.Company));
                Assert.That(actualName.Name, Is.EqualTo(expectedName.Name));
                break;
            }
            case Account.Asset:
            {
                StyledClosableViewModelBase<IAmortisableAsset> tab = desiredTab as StyledClosableViewModelBase<IAmortisableAsset>;
                NameData actualName = tab.ModelData.Names;
                Assert.That(actualName.Company, Is.EqualTo(expectedName.Company));
                Assert.That(actualName.Name, Is.EqualTo(expectedName.Name));
                break;
            }
            case Account.Unknown:
            case Account.All:
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}