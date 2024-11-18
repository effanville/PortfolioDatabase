using System;
using System.Collections.Generic;

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
        IPortfolio portfolio = PortfolioFactory.GenerateEmpty();
        string[] names = name.Split('-');
        NameData nameData = new NameData(names[0], names[1]);
        portfolio.TryAdd(account, nameData, _testContext.Globals.ReportLogger);
        portfolio.TryGetAccount(account, nameData, out ISecurity valueList);
        _testContext.ModelData = valueList;
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
        IPortfolio portfolio = PortfolioFactory.GenerateEmpty();
        string[] names = name.Split('-');
        NameData nameData = new NameData(names[0], names[1]);
        portfolio.TryAdd(account, nameData, _testContext.Globals.ReportLogger);
        portfolio.TryGetAccount(account, nameData, out ISecurity security);
        foreach (TableRow row in table.Rows)
        {
            string date = row["Date"];
            DateTime.TryParse(date, out DateTime actualDate);

            string value = row["UnitPrice"];
            decimal.TryParse(value, out decimal decimalValue);

            string type = row["Type"];
            if (type == "UnitPrice")
            {
                security.SetData(actualDate, decimalValue);
            }
            else if (type == "Trade")
            {
                SecurityTrade trade = FromRow(nameData, row);
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
        string date = row["Date"];
        DateTime.TryParse(date, out DateTime actualDate);

        string value = row["UnitPrice"];
        decimal.TryParse(value, out decimal decimalValue);
        string tradeType = row["TradeType"];
        TradeType eff = Enum.Parse<TradeType>(tradeType);
        string numShares = row["NumShares"];
        decimal.TryParse(numShares, out decimal decimalNumShares);
        string costs = row["Costs"];
        decimal.TryParse(costs, out decimal decimalCosts);
        SecurityTrade trade = new SecurityTrade(
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
        => _testContext.ViewModel.UpdateData(_testContext.ModelData, false);

    [Then(@"the SelectedSecurityViewModel has (.*) unitprices displayed")]
    public void ThenTheSelectedSecurityViewModelHasUnitpricesDisplayed(int p0)
        => Assert.That(_testContext.ViewModel.TLVM.Valuations.Count, Is.EqualTo(p0));

    [Then(@"the SelectedSecurityViewModel has (.*) trades displayed")]
    public void ThenTheSelectedSecurityViewModelHasTradesDisplayed(int p0)
        => Assert.That(_testContext.ViewModel.Trades.Count, Is.EqualTo(p0));

    [When(@"I add SelectedSecurityViewModel data with")]
    public void WhenIAddSelectedSingleDataViewModelDataWith(Table table)
    {
        for (int index = 0; index < table.RowCount; index++)
        {
            TableRow row = table.Rows[index];
            string date = row["Date"];
            string value = row["UnitPrice"];

            DateTime.TryParse(date, out DateTime actualDate);
            decimal.TryParse(value, out decimal decimalValue);
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
        List<DailyValuation> valuations = _testContext.ViewModel.TLVM.Valuations;
        Assert.That(valuations.Count, Is.EqualTo(table.RowCount));
        for (int index = 0; index < table.RowCount; index++)
        {
            DailyValuation valuation = valuations[index];
            TableRow row = table.Rows[index];
            string date = row["Date"];
            string value = row["UnitPrice"];

            DateTime.TryParse(date, out DateTime actualDate);
            decimal.TryParse(value, out decimal decimalValue);
            Assert.Multiple(() =>
            {
                Assert.That(valuation.Day, Is.EqualTo(actualDate));
                Assert.That(valuation.Value, Is.EqualTo(decimalValue));
            });
        }
    }

    [When(@"I add SelectedSecurityViewModel trade data for (.*) with")]
    public void WhenIAddSelectedSecurityViewModelTradeDataWith(string name, Table table)
    {
        string[] names = name.Split('-');
        NameData nameData = new NameData(names[0], names[1]);
        foreach (TableRow row in table.Rows)
        {
            SecurityTrade trade = FromRow(nameData, row);
            _testContext.ViewModel.AddNewTrade(trade);
        }
    }

    [Then(@"the SSVM trade values are")]
    public void ThenTheSsvmTradeValuesAre(Table table)
    {
        NameData name = _testContext.ViewModel.ModelData.Names;
        List<SecurityTrade> valuations = _testContext.ViewModel.Trades;
        Assert.That(valuations.Count, Is.EqualTo(table.RowCount));
        for (int index = 0; index < table.RowCount; index++)
        {
            SecurityTrade actualTrade = valuations[index];
            TableRow row = table.Rows[index];
            SecurityTrade expectedTrade = FromRow(name, row);
            Assert.Multiple(() =>
            {
                Assert.That(actualTrade.TradeType, Is.EqualTo(expectedTrade.TradeType));
                Assert.That(actualTrade.Company, Is.EqualTo(expectedTrade.Company));
                Assert.That(actualTrade.Name, Is.EqualTo(expectedTrade.Name));
                
                Assert.That(actualTrade.Day, Is.EqualTo(expectedTrade.Day));
                Assert.That(actualTrade.NumberShares, Is.EqualTo(expectedTrade.NumberShares));
                Assert.That(actualTrade.UnitPrice, Is.EqualTo(expectedTrade.UnitPrice));
                Assert.That(actualTrade.TradeCosts, Is.EqualTo(expectedTrade.TradeCosts));
            });
        }
    }

    [When(@"I edit SelectedSecurityViewModel trade data (.*) to")]
    public void WhenIEditSelectedSecurityViewModelTradeDataForBarclaysCurrentTo(int p0, Table table)
    {
        SecurityTrade oldTrade = _testContext.ViewModel.Trades[p0 - 1];
        SecurityTrade newTrade = FromRow(_testContext.ViewModel.ModelData.Names, table.Rows[0]);
        _testContext.ViewModel.EditTrade(oldTrade, newTrade );
    }

    [When(@"I delete SelectedSecurityViewModel trade data (.*)")]
    public void WhenIDeleteSelectedSecurityViewModelTradeData(int p0) 
        => _testContext.ViewModel.DeleteTrade(_testContext.ViewModel.Trades[p0-1]);
}