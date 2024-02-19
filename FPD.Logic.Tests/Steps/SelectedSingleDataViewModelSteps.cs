using System;

using Effanville.Common.Structure.DataStructures;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.FinanceStructures;
using Effanville.FinancialStructures.NamingStructures;
using Effanville.FPD.Logic.Tests.Context;
using Effanville.FPD.Logic.Tests.UserInteractions;

using FPD.Logic.ViewModels.Common;

using NUnit.Framework;

using TechTalk.SpecFlow;

namespace Effanville.FPD.Logic.Tests.Steps;

[Binding]
public class SelectedSingleDataViewModelSteps
{    
    private readonly ViewModelTestContext<IValueList, SelectedSingleDataViewModel> _testContext;

    public SelectedSingleDataViewModelSteps(ViewModelTestContext<IValueList, SelectedSingleDataViewModel> testContext)
    {
        _testContext = testContext;
    }
    
    [AfterScenario]
    public void Reset() => _testContext.Reset();

    [Given(@"I have a SelectedSingleDataViewModel with account (.*) and name (.*) and data")]
    public void GivenIHaveASelectedSingleDataViewModelWithAccountSecurityAndData(Account account, string name, Table table)
    {
        var portfolio = PortfolioFactory.GenerateEmpty();
        string[] names = name.Split('-');
        var nameData = new NameData(names[0], names[1]);
        portfolio.TryAdd(account, nameData, _testContext.Globals.ReportLogger);
        portfolio.TryGetAccount(account, nameData, out var valueList);

        foreach (var row in table.Rows)
        {
            var date = row["Date"];
            var value = row["Value"];

            DateTime.TryParse(date, out var actualDate);
            decimal.TryParse(value, out var decimalValue);
            valueList.SetData(actualDate, decimalValue);
        }

        _testContext.Updater.Database = portfolio;
        _testContext.ModelData = valueList;
        _testContext.ViewModel = new SelectedSingleDataViewModel(
            portfolio,
            _testContext.ModelData,
            _testContext.Styles,
            _testContext.Globals,
            _testContext.ModelData.Names,
            account,
            _testContext.Updater);
    }

    [Given(@"I have a SelectedSingleDataViewModel with account (.*) and name (.*) and no data")]
    public void GivenIHaveASelectedSingleDataViewModelWithAccountAndNameAndNoData(Account account, string name)
    {
        var portfolio = PortfolioFactory.GenerateEmpty();
        string[] names = name.Split('-');
        var nameData = new NameData(names[0], names[1]);
        portfolio.TryAdd(account, nameData, _testContext.Globals.ReportLogger);
        portfolio.TryGetAccount(account, nameData, out var valueList);
        _testContext.ModelData = valueList;
        _testContext.Updater.Database = portfolio;
        _testContext.ViewModel = new SelectedSingleDataViewModel(
            portfolio,
            _testContext.ModelData,
            _testContext.Styles,
            _testContext.Globals,
            _testContext.ModelData.Names,
            account,
            _testContext.Updater);
    }    
    
    [StepDefinition(@"the SelectedSingleDataViewModel is brought into focus")]
    public void GivenTheSelectedSingleDataViewModelIsBroughtIntoFocus()
        => _testContext.ViewModel.UpdateData(_testContext.ModelData);

    [Then(@"the SelectedSingleDataViewModel has (.*) valuation displayed")]
    public void ThenTheSelectedSingleDataViewModelHasValuationDisplayed(int p0) 
        => Assert.AreEqual(p0, _testContext.ViewModel.TLVM.Valuations.Count);

    [Then(@"the SelectedSingleDataViewModel values are")]
    public void ThenTheSelectedSingleDataViewModelValuesAre(Table table)
    {
        var valuations = _testContext.ViewModel.TLVM.Valuations;
        Assert.AreEqual(table.RowCount, valuations.Count);
        for (int index = 0; index < table.RowCount; index++)
        {
            var valuation = valuations[index];
            var row = table.Rows[index];
            var date = row["Date"];
            var value = row["Value"];

            DateTime.TryParse(date, out var actualDate);
            decimal.TryParse(value, out var decimalValue);
            Assert.Multiple(() =>
            {
                Assert.AreEqual(actualDate, valuation.Day);
                Assert.AreEqual(decimalValue, valuation.Value);
            });
        }
    }

    [When(@"I add SelectedSingleDataViewModel data with")]
    public void WhenIAddSelectedSingleDataViewModelDataWith(Table table)
    {       
        for (int index = 0; index < table.RowCount; index++)
        {
            var row = table.Rows[index];
            var date = row["Date"];
            var value = row["Value"];

            DateTime.TryParse(date, out var actualDate);
            decimal.TryParse(value, out var decimalValue);
            _testContext.ViewModel.TLVM.AddValuation(new DailyValuation(actualDate, decimalValue));
        }
    }   
    
    [When(@"I edit the SSDVM (.*) entry to date (.*) and value (.*)")]
    public void WhenIEditTheEntryToDateAndValue(int p0, DateTime p1, decimal p2) 
        => _testContext.ViewModel.TLVM.EditValuation(
            _testContext.ViewModel.TLVM.Valuations[p0 - 1], 
            new DailyValuation(p1, p2));
    
    [When(@"I remove the SSDVM (.*) entry from the list")]
    public void WhenIRemoveTheEntryFromTheList(int p0) 
        => _testContext.ViewModel.TLVM.DeleteValuation(_testContext.ViewModel.TLVM.Valuations[p0 - 1]);
    
}