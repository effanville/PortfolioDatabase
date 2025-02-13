using System;
using System.Collections.Generic;

using Effanville.Common.Structure.DataStructures;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.FinanceStructures;
using Effanville.FinancialStructures.NamingStructures;
using Effanville.FPD.Logic.Tests.Context;
using Effanville.FPD.Logic.Tests.UserInteractions;
using Effanville.FPD.Logic.ViewModels;
using Effanville.FPD.Logic.ViewModels.Common;

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
        IPortfolio portfolio = PortfolioFactory.GenerateEmpty();
        string[] names = name.Split('-');
        NameData nameData = new NameData(names[0], names[1]);
        portfolio.TryAdd(account, nameData);
        portfolio.TryGetAccount(account, nameData, out IValueList valueList);

        foreach (TableRow row in table.Rows)
        {
            string date = row["Date"];
            string value = row["Value"];

            DateTime.TryParse(date, out DateTime actualDate);
            decimal.TryParse(value, out decimal decimalValue);
            valueList.SetData(actualDate, decimalValue);
        }

        _testContext.Updater.Database = portfolio;
        _testContext.ModelData = valueList;
        _testContext.ViewModel = new SelectedSingleDataViewModel(
            new StatisticsProvider(portfolio),
            _testContext.ModelData,
            _testContext.Styles,
            _testContext.Globals,
            _testContext.ModelData.Names,
            account,
            _testContext.DataUpdater);
    }

    [Given(@"I have a SelectedSingleDataViewModel with account (.*) and name (.*) and no data")]
    public void GivenIHaveASelectedSingleDataViewModelWithAccountAndNameAndNoData(Account account, string name)
    {
        IPortfolio portfolio = PortfolioFactory.GenerateEmpty();
        string[] names = name.Split('-');
        NameData nameData = new NameData(names[0], names[1]);
        portfolio.TryAdd(account, nameData);
        portfolio.TryGetAccount(account, nameData, out IValueList valueList);
        _testContext.ModelData = valueList;
        _testContext.Updater.Database = portfolio;
        _testContext.ViewModel = new SelectedSingleDataViewModel(
            new StatisticsProvider(portfolio),
            _testContext.ModelData,
            _testContext.Styles,
            _testContext.Globals,
            _testContext.ModelData.Names,
            account,
            _testContext.DataUpdater);
    }

    [StepDefinition(@"the SelectedSingleDataViewModel is brought into focus")]
    public void GivenTheSelectedSingleDataViewModelIsBroughtIntoFocus()
        => _testContext.ViewModel.UpdateData(_testContext.ModelData, false);

    [Then(@"the SelectedSingleDataViewModel has (.*) valuation displayed")]
    public void ThenTheSelectedSingleDataViewModelHasValuationDisplayed(int p0)
        => Assert.That(_testContext.ViewModel.TLVM.Valuations.Count, Is.EqualTo(p0));

    [Then(@"the SelectedSingleDataViewModel values are")]
    public void ThenTheSelectedSingleDataViewModelValuesAre(Table table)
    {
        List<DailyValuation> valuations = _testContext.ViewModel.TLVM.Valuations;
        Assert.That(valuations.Count, Is.EqualTo(table.RowCount));
        for (int index = 0; index < table.RowCount; index++)
        {
            DailyValuation valuation = valuations[index];
            TableRow row = table.Rows[index];
            string date = row["Date"];
            string value = row["Value"];

            DateTime.TryParse(date, out DateTime actualDate);
            decimal.TryParse(value, out decimal decimalValue);
            Assert.Multiple(() =>
            {
                Assert.That(valuation.Day, Is.EqualTo(actualDate));
                Assert.That(valuation.Value, Is.EqualTo(decimalValue));
            });
        }
    }

    [When(@"I add SelectedSingleDataViewModel data with")]
    public void WhenIAddSelectedSingleDataViewModelDataWith(Table table)
    {
        for (int index = 0; index < table.RowCount; index++)
        {
            TableRow row = table.Rows[index];
            string date = row["Date"];
            string value = row["Value"];

            DateTime.TryParse(date, out DateTime actualDate);
            decimal.TryParse(value, out decimal decimalValue);
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