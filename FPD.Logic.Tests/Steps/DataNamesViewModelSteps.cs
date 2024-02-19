using System.Linq;

using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.NamingStructures;
using Effanville.FPD.Logic.Tests.Context;
using Effanville.FPD.Logic.Tests.UserInteractions;

using FPD.Logic.ViewModels.Common;

using NUnit.Framework;

using TechTalk.SpecFlow;

namespace Effanville.FPD.Logic.Tests.Steps;

[Binding]
public class DataNamesViewModelSteps
{
    private readonly DataNamesViewModelTestContext _testContext;

    public DataNamesViewModelSteps(
        DataNamesViewModelTestContext testContext)
    {
        _testContext = testContext;
    }

    [AfterScenario]
    public void Reset()
        => _testContext.Reset();

    [Given(@"I have a DataNamesViewModel with type (.*) and no data")]
    public void GivenIHaveADataNamesViewModelWithNoData(Account account)
        => Create(account, null);

    private void LoadSelectedData(object obj)
        => _testContext.LoadDataCalled = true;

    [Given(@"I have a DataNamesViewModel with type (.*) and data")]
    public void GivenIHaveADataNamesViewModelWithTypeSecurityAndData(Account account, Table table)
        => Create(account, table);

    private void Create(Account account, Table table)
    {
        var portfolio = PortfolioGeneratorHelper.CreateFromTable(table);
        _testContext.ModelData = portfolio;
        _testContext.Updater.Database = portfolio;

        _testContext.ViewModel = new DataNamesViewModel(
            portfolio,
            _testContext.Globals,
            _testContext.Styles,
            _testContext.Updater,
            LoadSelectedData,
            account);
        _testContext.ViewModel.UpdateRequest += _testContext.Updater.PerformUpdate;
    }

    [StepDefinition(@"the DataNamesViewModel is brought into focus")]
    public void GivenTheDataNamesViewModelIsBroughtIntoFocus()
        => _testContext.ViewModel.UpdateData(_testContext.ModelData);

    [Then(@"I can see the DataNamesViewModel type is (.*)")]
    public void ThenICanSeeTheTypeIs(Account account)
        => Assert.AreEqual(account, _testContext.ViewModel.DataType);

    [Then(@"I can see the DNVW has header (.*)")]
    public void ThenICanSeeTheDnvwHasHeader(string header)
        => Assert.AreEqual(header, _testContext.ViewModel.Header);

    [Then(@"the user can see the number of names is (.*)")]
    public void ThenTheUserCanSeeTheNumberOfNamesIs(int p0)
        => Assert.AreEqual(p0, _testContext.ViewModel.DataNames.Count);

    [When(@"new names are added to the database")]
    public void WhenNewNamesAreAddedToTheDatabase(Table table)
        => PortfolioGeneratorHelper.UpdateModelData(_testContext.ModelData, table);

    [Then(@"the action to open the tab is called\.")]
    public void ThenTheActionToOpenTheTabIsCalled()
        => Assert.AreEqual(true, _testContext.LoadDataCalled);

    [When(@"I click on the open data button")]
    public void WhenIClickOnTheOpenDataButton()
        => _testContext.ViewModel.ViewData();

    [When(@"I select the names row with data")]
    public void WhenISelectTheNamesRowWithData(Table table)
    {
        var nameData = FromRow(table.Rows[0]);
        _testContext.ViewModel.SelectName(nameData);
    }

    [When(@"I add a name with data")]
    public void WhenIAddANameWithData(Table table)
    {
        var nameData = FromRow(table.Rows[0]);
        _testContext.ViewModel.AddName(nameData);
    }

    [Then(@"the dataName update event is called")]
    public void ThenTheDataNameUpdateEventIsCalled()
        => Assert.AreEqual(true, _testContext.LoadDataCalled);

    [Then(@"the user can see the DataNames are")]
    public void ThenTheUserCanSeeTheDataNamesAre(Table table)
    {
        var dataNames = _testContext.ViewModel.DataNames;
        var rows = table.Rows;
        for (int index = 0; index < rows.Count; index++)
        {
            var name = FromRow(rows[index]);
            AreNameDataEqual(name, dataNames[index].Instance);
        }
    }
    
    [When(@"I edit the (.*) name data to")]
    public void WhenIEditTheNameDataTo(int index, Table table)
    {
        var newName = FromRow(table.Rows[0]);
        var selectedRow = _testContext.ViewModel.DataNames[index - 1];
        _testContext.ViewModel.EditName(selectedRow, newName);
    }
    
    private static NameData FromRow(TableRow row)
        => new NameData(
            row["Company"],
            row["Name"],
            row["Currency"],
            row["Url"],
            row["Sectors"].Split(',').ToHashSet());

    [When(@"I remove the (.*) data name")]
    public void WhenIRemoveTheDataName(int p0)
    {
        var selectedRow = _testContext.ViewModel.DataNames[p0 - 1];
        _testContext.ViewModel.DeleteName(selectedRow.Instance);
    }
    
    [Then(@"the dataNames portfolio has only (.*) of type (.*)")]
    public void ThenThePortfolioHasOnlyOfTypeSecurity(int p0, Account account)
    {
        var numberAccounts = _testContext.ModelData.NameDataForAccount(account);
        Assert.AreEqual(p0, numberAccounts.Count);
    }

    [When(@"I download the data for the selected DataName")]
    public void WhenIDownloadTheDataForTheSelectedDataName() 
        => _testContext.ViewModel.DownloadSelected();

    [Then(@"I can see that the data has been downloaded")]
    public void ThenICanSeeThatTheDataHasBeenDownloaded()
    {
        Assert.Inconclusive("Need to implement check to ensure that the data is downloaded.");
    }

    void AreNameDataEqual(NameData expected, NameData actual)
    {
        if (expected == null)
        {
            Assert.IsNull(actual);
        }

        if (expected != null)
        {
            Assert.IsNotNull(actual);
        }

        Assert.Multiple(() =>
            {
                Assert.AreEqual(expected.Company, actual.Company);
                Assert.AreEqual(expected.Name, actual.Name);
                Assert.AreEqual(expected.Url, actual.Url);
                Assert.AreEqual(expected.Currency, actual.Currency);
                Assert.AreEqual(expected.SectorsFlat, actual.SectorsFlat);
            }
        );
    }
}