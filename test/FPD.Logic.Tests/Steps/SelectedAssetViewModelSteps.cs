using System;
using System.Collections.Generic;

using Effanville.Common.Structure.DataStructures;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.FinanceStructures;
using Effanville.FinancialStructures.NamingStructures;
using Effanville.FPD.Logic.Tests.Context;
using Effanville.FPD.Logic.Tests.UserInteractions;
using Effanville.FPD.Logic.ViewModels;
using Effanville.FPD.Logic.ViewModels.Asset;

using NUnit.Framework;

using TechTalk.SpecFlow;

namespace Effanville.FPD.Logic.Tests.Steps;

[Binding]
public class SelectedAssetViewModelSteps
{
    private readonly ViewModelTestContext<IAmortisableAsset, SelectedAssetViewModel> _testContext;

    public SelectedAssetViewModelSteps(ViewModelTestContext<IAmortisableAsset, SelectedAssetViewModel> testContext)
    {
        _testContext = testContext;
    }

    [Given(@"I have a SelectedAssetViewModel with name Barclays-Current and no data")]
    public void GivenIHaveASelectedAssetViewModelWithNameAndNoData(string name)
    {
        IPortfolio portfolio = PortfolioFactory.GenerateEmpty();
        string[] names = name.Split('-');
        NameData nameData = new NameData(names[0], names[1]);
        portfolio.TryAdd(Account.Asset, nameData, _testContext.Globals.ReportLogger);
        portfolio.TryGetAccount(Account.Asset, nameData, out IAmortisableAsset valueList);
        _testContext.ModelData = valueList;
        _testContext.Updater.Database = portfolio;
        _testContext.ViewModel = new SelectedAssetViewModel(
            new StatisticsProvider(portfolio),
            _testContext.ModelData,
            _testContext.Styles,
            _testContext.Globals,
            _testContext.ModelData.Names,
            Account.Asset,
            _testContext.Updater,
            _testContext.PortfolioDataDownloader);
    }

    [AfterScenario]
    public void Reset() => _testContext.Reset();

    [Given(@"I have a SelectedAssetViewModel with name (.*) and data")]
    public void GivenIHaveASelectedAssetViewModelWithAccountSecurityAndData(string name, Table table)
    {
        IPortfolio portfolio = PortfolioFactory.GenerateEmpty();
        string[] names = name.Split('-');
        NameData nameData = new NameData(names[0], names[1]);
        portfolio.TryAdd(Account.Asset, nameData, _testContext.Globals.ReportLogger);
        portfolio.TryGetAccount(Account.Asset, nameData, out IAmortisableAsset asset);
        foreach (TableRow row in table.Rows)
        {
            string date = row["Date"];
            string value = row["Value"];

            DateTime.TryParse(date, out DateTime actualDate);
            decimal.TryParse(value, out decimal decimalValue);

            string type = row["Type"];
            if (type == "Value")
            {
                asset.SetData(actualDate, decimalValue);
            }
            else if (type == "Debt")
            {
                asset.SetDebt(actualDate, decimalValue);
            }
            else if (type == "Payment")
            {
                asset.SetPayment(actualDate, decimalValue);
            }
        }

        _testContext.Updater.Database = portfolio;
        _testContext.ModelData = asset;
        _testContext.ViewModel = new SelectedAssetViewModel(
            new StatisticsProvider(portfolio),
            _testContext.ModelData,
            _testContext.Styles,
            _testContext.Globals,
            _testContext.ModelData.Names,
            Account.Asset,
            _testContext.Updater,
            _testContext.PortfolioDataDownloader);
    }

    [Given(@"I have a SelectedAssetViewModel with name (.*) and no data")]
    public void GivenIHaveASelectedSingleDataViewModelWithAccountAndNameAndNoData(string name)
    {
        IPortfolio portfolio = PortfolioFactory.GenerateEmpty();
        string[] names = name.Split('-');
        NameData nameData = new NameData(names[0], names[1]);
        portfolio.TryAdd(Account.Asset, nameData, _testContext.Globals.ReportLogger);
        portfolio.TryGetAccount(Account.Asset, nameData, out IAmortisableAsset valueList);

        _testContext.Updater.Database = portfolio;
        _testContext.ModelData = valueList;
        _testContext.ViewModel = new SelectedAssetViewModel(
            new StatisticsProvider(portfolio),
            _testContext.ModelData,
            _testContext.Styles,
            _testContext.Globals,
            _testContext.ModelData.Names,
            Account.Asset,
            _testContext.Updater,
            _testContext.PortfolioDataDownloader);
    }

    [Given(@"the SelectedAssetViewModel is brought into focus")]
    public void GivenTheSelectedAssetViewModelIsBroughtIntoFocus()
        => _testContext.ViewModel.UpdateData(_testContext.ModelData, false);


    [Then(@"the SelectedAssetViewModel has (.*) values displayed")]
    public void ThenTheSelectedAssetViewModelHasValuationDisplayed(int p0)
        => Assert.That(_testContext.ViewModel.ValuesTLVM.Valuations.Count, Is.EqualTo(p0));

    [Then(@"the SelectedAssetViewModel has (.*) debt values displayed")]
    public void ThenTheSelectedAssetViewModelHasDebtValuationDisplayed(int p0)
        => Assert.That(_testContext.ViewModel.DebtTLVM.Valuations.Count, Is.EqualTo(p0));

    [Then(@"the SelectedAssetViewModel has (.*) payment values displayed")]
    public void ThenTheSelectedAssetViewModelHasPaymentValuationDisplayed(int p0)
        => Assert.That(_testContext.ViewModel.PaymentsTLVM.Valuations.Count, Is.EqualTo(p0));

    [When(@"I add SAVM data with")]
    public void WhenIAddSelectedSingleDataViewModelDataWith(Table table)
    {
        for (int index = 0; index < table.RowCount; index++)
        {
            TableRow row = table.Rows[index];
            string date = row["Date"];
            string value = row["Value"];

            DateTime.TryParse(date, out DateTime actualDate);
            decimal.TryParse(value, out decimal decimalValue);
            _testContext.ViewModel.ValuesTLVM.AddValuation(new DailyValuation(actualDate, decimalValue));
        }
    }

    [When(@"I edit the SAVM (.*) entry to date (.*) and value (.*)")]
    public void WhenIEditTheEntryToDateAndValue(int p0, DateTime p1, decimal p2)
        => _testContext.ViewModel.ValuesTLVM.EditValuation(
            _testContext.ViewModel.ValuesTLVM.Valuations[p0 - 1],
            new DailyValuation(p1, p2));

    [When(@"I remove the SAVM (.*) entry from the list")]
    public void WhenIRemoveTheEntryFromTheList(int p0)
        => _testContext.ViewModel.ValuesTLVM.DeleteValuation(_testContext.ViewModel.ValuesTLVM.Valuations[p0 - 1]);


    [When(@"I add SAVM debt data with")]
    public void WhenIAddDebtDataWith(Table table)
    {
        for (int index = 0; index < table.RowCount; index++)
        {
            TableRow row = table.Rows[index];
            string date = row["Date"];
            string value = row["Value"];

            DateTime.TryParse(date, out DateTime actualDate);
            decimal.TryParse(value, out decimal decimalValue);
            _testContext.ViewModel.DebtTLVM.AddValuation(new DailyValuation(actualDate, decimalValue));
        }
    }

    [When(@"I edit the SAVM debt (.*) entry to date (.*) and value (.*)")]
    public void WhenIEditTheDebtEntryToDateAndValue(int p0, DateTime p1, decimal p2)
        => _testContext.ViewModel.DebtTLVM.EditValuation(
            _testContext.ViewModel.DebtTLVM.Valuations[p0 - 1],
            new DailyValuation(p1, p2));

    [When(@"I remove the SAVM debt (.*) entry from the list")]
    public void WhenIRemoveTheDebtEntryFromTheList(int p0)
        => _testContext.ViewModel.DebtTLVM.DeleteValuation(_testContext.ViewModel.DebtTLVM.Valuations[p0 - 1]);

    [When(@"I add SAVM payment data with")]
    public void WhenIAddPaymentDataWith(Table table)
    {
        for (int index = 0; index < table.RowCount; index++)
        {
            TableRow row = table.Rows[index];
            string date = row["Date"];
            string value = row["Value"];

            DateTime.TryParse(date, out DateTime actualDate);
            decimal.TryParse(value, out decimal decimalValue);
            _testContext.ViewModel.PaymentsTLVM.AddValuation(new DailyValuation(actualDate, decimalValue));
        }
    }

    [When(@"I edit the SAVM payment (.*) entry to date (.*) and value (.*)")]
    public void WhenIEditThePaymentEntryToDateAndValue(int p0, DateTime p1, decimal p2)
        => _testContext.ViewModel.PaymentsTLVM.EditValuation(
            _testContext.ViewModel.PaymentsTLVM.Valuations[p0 - 1],
            new DailyValuation(p1, p2));

    [When(@"I remove the SAVM payment (.*) entry from the list")]
    public void WhenIRemoveThePaymentEntryFromTheList(int p0)
        => _testContext.ViewModel.PaymentsTLVM.DeleteValuation(_testContext.ViewModel.PaymentsTLVM.Valuations[p0 - 1]);

    [Then(@"the SAVM values are")]
    public void ThenTheSavmValuesAre(Table table)
    {
        List<DailyValuation> valuations = _testContext.ViewModel.ValuesTLVM.Valuations;
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

    [Then(@"the SAVM payment values are")]
    public void ThenTheSavmPaymentsValuesAre(Table table)
    {
        List<DailyValuation> valuations = _testContext.ViewModel.PaymentsTLVM.Valuations;
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

    [Then(@"the SAVM debt values are")]
    public void ThenTheSavmDebtValuesAre(Table table)
    {
        List<DailyValuation> valuations = _testContext.ViewModel.DebtTLVM.Valuations;
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
}