using System.Collections.Generic;
using System.Linq;

using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.NamingStructures;
using Effanville.FPD.Logic.Tests.Context;
using Effanville.FPD.Logic.Tests.UserInteractions;
using Effanville.FPD.Logic.ViewModels.Common;

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
        IPortfolio portfolio = PortfolioGeneratorHelper.CreateFromTable(table);
        _testContext.ModelData = portfolio;

        _testContext.ViewModel = new DataNamesViewModel(
            portfolio,
            _testContext.Globals,
            _testContext.Styles,
            _testContext.Updater,
            _testContext.PortfolioDataDownloader,
            LoadSelectedData,
            account);
    }

    [StepDefinition(@"the DataNamesViewModel is brought into focus")]
    public void GivenTheDataNamesViewModelIsBroughtIntoFocus()
        => _testContext.ViewModel.UpdateData(_testContext.ModelData, false);

    [Then(@"I can see the DataNamesViewModel type is (.*)")]
    public void ThenICanSeeTheTypeIs(Account account)
        => Assert.That(_testContext.ViewModel.DataType, Is.EqualTo(account));

    [Then(@"I can see the DNVW has header (.*)")]
    public void ThenICanSeeTheDnvwHasHeader(string header)
        => Assert.That(_testContext.ViewModel.Header, Is.EqualTo(header));

    [Then(@"the user can see the number of names is (.*)")]
    public void ThenTheUserCanSeeTheNumberOfNamesIs(int p0)
        => Assert.That(_testContext.ViewModel.DataNames.Count, Is.EqualTo(p0));

    [When(@"new names are added to the database")]
    public void WhenNewNamesAreAddedToTheDatabase(Table table)
        => PortfolioGeneratorHelper.UpdateModelData(_testContext.ModelData, table);

    [Then(@"the action to open the tab is called\.")]
    public void ThenTheActionToOpenTheTabIsCalled()
        => Assert.That(_testContext.LoadDataCalled, Is.EqualTo(true));

    [When(@"I click on the open data button")]
    public void WhenIClickOnTheOpenDataButton()
        => _testContext.ViewModel.ViewData();

    [When(@"I select the names row with data")]
    public void WhenISelectTheNamesRowWithData(Table table)
    {
        NameData nameData = FromRow(table.Rows[0]);
        _testContext.ViewModel.SelectName(nameData);
    }

    [When(@"I add a name with data")]
    public void WhenIAddANameWithData(Table table)
    {
        NameData nameData = FromRow(table.Rows[0]);
        _testContext.ViewModel.AddName(nameData);
    }

    [Then(@"the dataName update event is called")]
    public void ThenTheDataNameUpdateEventIsCalled()
        => Assert.That(_testContext.LoadDataCalled, Is.EqualTo(true));

    [Then(@"the user can see the DataNames are")]
    public void ThenTheUserCanSeeTheDataNamesAre(Table table)
    {
        List<NameDataViewModel> dataNames = _testContext.ViewModel.DataNames.ToList();
        TableRows rows = table.Rows;
        for (int index = 0; index < rows.Count; index++)
        {
            NameData name = FromRow(rows[index]);
            AreNameDataEqual(name, dataNames[index].ModelData);
        }
    }

    [When(@"I edit the (.*) name data to")]
    public void WhenIEditTheNameDataTo(int index, Table table)
    {
        NameData newName = FromRow(table.Rows[0]);
        NameDataViewModel selectedRow = _testContext.ViewModel.DataNames[index - 1];
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
        NameDataViewModel selectedRow = _testContext.ViewModel.DataNames[p0 - 1];
        _testContext.ViewModel.DeleteName(selectedRow.ModelData);
    }

    [Then(@"the dataNames portfolio has only (.*) of type (.*)")]
    public void ThenThePortfolioHasOnlyOfTypeSecurity(int p0, Account account)
    {
        IReadOnlyList<NameData> numberAccounts = _testContext.ModelData.NameDataForAccount(account);
        Assert.That(numberAccounts.Count, Is.EqualTo(p0));
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
            Assert.That(actual, Is.Null);
        }

        if (expected != null)
        {
            Assert.That(actual, Is.Not.Null);
        }

        Assert.Multiple(() =>
            {
                Assert.That(actual.Company, Is.EqualTo(expected.Company));
                Assert.That(actual.Name, Is.EqualTo(expected.Name));
                Assert.That(actual.Url, Is.EqualTo(expected.Url));
                Assert.That(actual.Currency, Is.EqualTo(expected.Currency));
                Assert.That(actual.SectorsFlat, Is.EqualTo(expected.SectorsFlat));
            }
        );
    }
}