using System;

using Effanville.Common.Structure.DataStructures;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.DataStructures;
using Effanville.FinancialStructures.FinanceStructures;
using Effanville.FinancialStructures.NamingStructures;
using Effanville.FPD.Logic.Tests.Context;
using Effanville.FPD.Logic.Tests.UserInteractions;
using Effanville.FPD.Logic.ViewModels.Security;

using NUnit.Framework;

using TechTalk.SpecFlow;

namespace Effanville.FPD.Logic.Tests.Steps;

[Binding]
public class SelectedSecurityViewModelSteps
{
    private readonly ViewModelTestContext<ISecurity, SelectedSecurityViewModel> _testContext;

    public SelectedSecurityViewModelSteps(ViewModelTestContext<ISecurity, SelectedSecurityViewModel> testContext)
    {
        _testContext = testContext;
    }

    [Given(@"I have a SelectedSecurityViewModel with account (.*) and name (.*) and no data")]
    public void GivenIHaveASelectedSecurityViewModelWithNameBarclaysCurrentAndNoData(Account account, string name)
    {
        var portfolio = PortfolioFactory.GenerateEmpty();
        string[] names = name.Split('-');
        var nameData = new NameData(names[0], names[1]);
        portfolio.TryAdd(account, nameData, _testContext.Globals.ReportLogger);
        portfolio.TryGetAccount(account, nameData, out var valueList);
        _testContext.ModelData = valueList as ISecurity;
        _testContext.Updater.Database = portfolio;
        _testContext.ViewModel = new SelectedSecurityViewModel(
            portfolio,
            _testContext.ModelData,
            _testContext.Styles,
            _testContext.Globals,
            _testContext.ModelData.Names,
            account,
            _testContext.Updater);
    }

    [Given(@"I have a SelectedSecurityViewModel with account (.*) and name (.*) and data")]
    public void GivenIHaveASelectedAssetViewModelWithAccountSecurityAndData(Account account, string name, Table table)
    {
        var portfolio = PortfolioFactory.GenerateEmpty();
        string[] names = name.Split('-');
        var nameData = new NameData(names[0], names[1]);
        portfolio.TryAdd(account, nameData, _testContext.Globals.ReportLogger);
        portfolio.TryGetAccount(account, nameData, out var valueList);
        var security = valueList as ISecurity;
        foreach (var row in table.Rows)
        {
            var date = row["Date"];
            DateTime.TryParse(date, out var actualDate);

            var value = row["UnitPrice"];
            decimal.TryParse(value, out var decimalValue);

            var type = row["Type"];
            if (type == "UnitPrice")
            {
                security.SetData(actualDate, decimalValue);
            }
            else if (type == "Trade")
            {
                var trade = FromRow(nameData, row);
                security.TryAddOrEditTradeData(trade, trade);
            }
        }

        _testContext.Updater.Database = portfolio;
        _testContext.ModelData = security;
        _testContext.ViewModel = new SelectedSecurityViewModel(
            portfolio,
            _testContext.ModelData,
            _testContext.Styles,
            _testContext.Globals,
            _testContext.ModelData.Names,
            Account.Asset,
            _testContext.Updater);
    }

    private SecurityTrade FromRow(NameData name, TableRow row)
    {
        var date = row["Date"];
        DateTime.TryParse(date, out var actualDate);

        var value = row["UnitPrice"];
        decimal.TryParse(value, out var decimalValue);
        var tradeType = row["TradeType"];
        var eff = Enum.Parse<TradeType>(tradeType);
        var numShares = row["NumShares"];
        decimal.TryParse(numShares, out var decimalNumShares);
        var costs = row["Costs"];
        decimal.TryParse(costs, out var decimalCosts);
        var trade = new SecurityTrade(
            eff,
            name.ToTwoName(),
            actualDate,
            decimalNumShares,
            decimalValue,
            decimalCosts);
        return trade;
    }

    [AfterScenario]
    public void Reset() => _testContext.Reset();

    [Given(@"the SelectedSecurityViewModel is brought into focus")]
    public void GivenTheSelectedSecurityViewModelIsBroughtIntoFocus()
        => _testContext.ViewModel.UpdateData(_testContext.ModelData);

    [Then(@"the SelectedSecurityViewModel has (.*) unitprices displayed")]
    public void ThenTheSelectedSecurityViewModelHasUnitpricesDisplayed(int p0)
        => Assert.AreEqual(p0, _testContext.ViewModel.TLVM.Valuations.Count);

    [Then(@"the SelectedSecurityViewModel has (.*) trades displayed")]
    public void ThenTheSelectedSecurityViewModelHasTradesDisplayed(int p0)
        => Assert.AreEqual(p0, _testContext.ViewModel.Trades.Count);

    [When(@"I add SelectedSecurityViewModel data with")]
    public void WhenIAddSelectedSingleDataViewModelDataWith(Table table)
    {
        for (int index = 0; index < table.RowCount; index++)
        {
            var row = table.Rows[index];
            var date = row["Date"];
            var value = row["UnitPrice"];

            DateTime.TryParse(date, out var actualDate);
            decimal.TryParse(value, out var decimalValue);
            _testContext.ViewModel.TLVM.AddValuation(new DailyValuation(actualDate, decimalValue));
        }
    }

    [When(@"I edit the SSVM (.*) entry to date (.*) and value (.*)")]
    public void WhenIEditTheEntryToDateAndValue(int p0, DateTime p1, decimal p2)
        => _testContext.ViewModel.TLVM.EditValuation(
            _testContext.ViewModel.TLVM.Valuations[p0 - 1],
            new DailyValuation(p1, p2));

    [When(@"I remove the SSVM (.*) entry from the list")]
    public void WhenIRemoveTheEntryFromTheList(int p0)
        => _testContext.ViewModel.TLVM.DeleteValuation(_testContext.ViewModel.TLVM.Valuations[p0 - 1]);

    [When(@"I remove the SSVM trade (.*) entry from the list")]
    public void WhenIRemoveTheTradeEntryFromTheList(int p0)
        => _testContext.ViewModel.DeleteTrade(_testContext.ViewModel.Trades[p0 - 1]);

    [Then(@"the SSVM values are")]
    public void ThenTheSSVMValuesAre(Table table)
    {
        var valuations = _testContext.ViewModel.TLVM.Valuations;
        Assert.AreEqual(table.RowCount, valuations.Count);
        for (int index = 0; index < table.RowCount; index++)
        {
            var valuation = valuations[index];
            var row = table.Rows[index];
            var date = row["Date"];
            var value = row["UnitPrice"];

            DateTime.TryParse(date, out var actualDate);
            decimal.TryParse(value, out var decimalValue);
            Assert.Multiple(() =>
            {
                Assert.AreEqual(actualDate, valuation.Day);
                Assert.AreEqual(decimalValue, valuation.Value);
            });
        }
    }

    [When(@"I add SelectedSecurityViewModel trade data for (.*) with")]
    public void WhenIAddSelectedSecurityViewModelTradeDataWith(string name, Table table)
    {
        string[] names = name.Split('-');
        var nameData = new NameData(names[0], names[1]);
        foreach (var row in table.Rows)
        {
            var trade = FromRow(nameData, row);
            _testContext.ViewModel.AddNewTrade(trade);
        }
    }

    [Then(@"the SSVM trade values are")]
    public void ThenTheSsvmTradeValuesAre(Table table)
    {
        var name = _testContext.ViewModel.ModelData.Names;
        var valuations = _testContext.ViewModel.Trades;
        Assert.AreEqual(table.RowCount, valuations.Count);
        for (int index = 0; index < table.RowCount; index++)
        {
            var actualTrade = valuations[index];
            var row = table.Rows[index];
            var expectedTrade = FromRow(name, row);
            Assert.Multiple(() =>
            {
                Assert.AreEqual(expectedTrade.TradeType, actualTrade.TradeType);
                Assert.AreEqual(expectedTrade.Company, actualTrade.Company);
                Assert.AreEqual(expectedTrade.Name, actualTrade.Name);
                
                Assert.AreEqual(expectedTrade.Day, actualTrade.Day);
                Assert.AreEqual(expectedTrade.NumberShares, actualTrade.NumberShares);
                Assert.AreEqual(expectedTrade.UnitPrice, actualTrade.UnitPrice);
                Assert.AreEqual(expectedTrade.TradeCosts, actualTrade.TradeCosts);
            });
        }
    }

    [When(@"I edit SelectedSecurityViewModel trade data (.*) to")]
    public void WhenIEditSelectedSecurityViewModelTradeDataForBarclaysCurrentTo(int p0, Table table)
    {
        var oldTrade = _testContext.ViewModel.Trades[p0 - 1];
        var newTrade = FromRow(_testContext.ViewModel.ModelData.Names, table.Rows[0]);
        _testContext.ViewModel.EditTrade(oldTrade, newTrade );
    }

    [When(@"I delete SelectedSecurityViewModel trade data (.*)")]
    public void WhenIDeleteSelectedSecurityViewModelTradeData(int p0) 
        => _testContext.ViewModel.DeleteTrade(_testContext.ViewModel.Trades[p0-1]);
}