using System;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Threading;

using Common.Structure.DataStructures;

using FinancialStructures.Database;
using FinancialStructures.FinanceStructures;
using FinancialStructures.NamingStructures;

using FPD.Logic.Tests.TestHelpers;
using FPD.Logic.Tests.ViewModelExtensions;
using FPD.Logic.ViewModels.Asset;

using NUnit.Framework;

namespace FPD.Logic.Tests.AssetWindowTests
{
    [TestFixture]
    public class SelectedAssetViewModelTests
    {
        [Test]
        public void CanOpenWindow()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            portfolio.TryGetAccount(Account.Asset, new NameData("House", "MyHouse"), out var desired);
            var asset = desired as IAmortisableAsset;
            var viewModelFactory = TestSetupHelper.CreateViewModelFactory(portfolio, new MockFileSystem(), null, null);
            var context = new ViewModelTestContext<IAmortisableAsset, SelectedAssetViewModel>(
                asset,
                new NameData("House", "MyHouse"),
                Account.Asset,
                portfolio,
                viewModelFactory);
            Assert.AreEqual(1, context.ViewModel.ValuesTLVM.Valuations.Count);
            Assert.AreEqual(1, context.ViewModel.DebtTLVM.Valuations.Count);
        }

        [Test]
        public void CanAddDebtValue()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();            
            portfolio.TryGetAccount(Account.Asset, new NameData("House", "MyHouse"), out var desired);
            var asset = desired as IAmortisableAsset;
            var viewModelFactory = TestSetupHelper.CreateViewModelFactory(portfolio, new MockFileSystem(), null, null);
            var context = new ViewModelTestContext<IAmortisableAsset, SelectedAssetViewModel>(
                asset,
                new NameData("House", "MyHouse"),
                Account.Asset,
                portfolio,
                viewModelFactory);
            Assert.AreEqual(1, context.ViewModel.DebtTLVM.Valuations.Count);
            context.ViewModel.SelectDebt(null);
            DailyValuation newItem = context.ViewModel.AddNewDebt();
            context.ViewModel.BeginEditDebt();
            newItem.Day = new DateTime(2002, 1, 1);
            newItem.Value = 5;
            context.ViewModel.CompleteEditTrade(context.Data);

            Assert.AreEqual(2, context.ViewModel.DebtTLVM.Valuations.Count);
            Assert.AreEqual(1, context.Portfolio.FundsThreadSafe.Single().Count());
        }

        [Test]
        public void CanEditDebtValue()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            portfolio.TryGetAccount(Account.Asset, new NameData("House", "MyHouse"), out var desired);
            var asset = desired as IAmortisableAsset;
            var viewModelFactory = TestSetupHelper.CreateViewModelFactory(portfolio, new MockFileSystem(), null, null);

            var context = new ViewModelTestContext<IAmortisableAsset, SelectedAssetViewModel>(
                asset,
                new NameData("House", "MyHouse"),
                Account.Asset,
                portfolio,
                viewModelFactory);
            Assert.AreEqual(1, context.ViewModel.DebtTLVM.Valuations.Count);
            DailyValuation item = context.ViewModel.DebtTLVM.Valuations[0];
            context.ViewModel.SelectDebt(item);
            context.ViewModel.BeginEditDebt();
            item.Day = new DateTime(2000, 1, 1);
            item.Value = 1;
            context.ViewModel.CompleteEditTrade(context.Data);

            Assert.AreEqual(1, context.ViewModel.DebtTLVM.Valuations.Count);
            Assert.AreEqual(1, context.Portfolio.FundsThreadSafe.Single().Count());
            Assert.AreEqual(new DateTime(2000, 1, 1), context.Portfolio.FundsThreadSafe.Single().FirstValue().Day);
        }

        [Test]
        [RequiresThread(ApartmentState.STA)]
        public void CanDeleteDebt()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            portfolio.TryGetAccount(Account.Asset, new NameData("House", "MyHouse"), out var desired);
            var asset = desired as IAmortisableAsset;
            var viewModelFactory = TestSetupHelper.CreateViewModelFactory(portfolio, new MockFileSystem(), null, null);

            var context = new ViewModelTestContext<IAmortisableAsset, SelectedAssetViewModel>(
                asset,
                new NameData("House", "MyHouse"), 
                Account.Asset,
                portfolio,
                viewModelFactory);
            Assert.AreEqual(1, context.ViewModel.DebtTLVM.Valuations.Count);
            context.ViewModel.SelectDebt(context.ViewModel.DebtTLVM.Valuations[0]);
            context.ViewModel.DeleteSelectedDebt(context.Data);
            Assert.AreEqual(1, context.Data.Values.Count());
            Assert.AreEqual(0, context.Data.Debt.Count());
        }

        [Test]
        public void CanAddValue()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            portfolio.TryGetAccount(Account.Asset, new NameData("House", "MyHouse"), out var desired);
            var asset = desired as IAmortisableAsset;
            var viewModelFactory = TestSetupHelper.CreateViewModelFactory(portfolio, new MockFileSystem(), null, null);
            var context = new ViewModelTestContext<IAmortisableAsset, SelectedAssetViewModel>(
                asset,
                new NameData("House", "MyHouse"),
                Account.Asset,
                portfolio,
                viewModelFactory);
            Assert.AreEqual(1, context.ViewModel.ValuesTLVM.Valuations.Count);
            context.ViewModel.SelectValue(null);
            DailyValuation newItem = context.ViewModel.AddNewUnitPrice();
            context.ViewModel.BeginEdit();
            newItem.Day = new DateTime(2002, 1, 1);
            newItem.Value = 1;
            context.ViewModel.CompleteEdit(context.Data);

            Assert.AreEqual(2, context.ViewModel.ValuesTLVM.Valuations.Count);
            Assert.AreEqual(2, context.Portfolio.Assets.Single().Count());
        }

        [Test]
        public void CanEditValue()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            portfolio.TryGetAccount(Account.Asset, new NameData("House", "MyHouse"), out var desired);
            var asset = desired as IAmortisableAsset;
            var viewModelFactory = TestSetupHelper.CreateViewModelFactory(portfolio, new MockFileSystem(), null, null);

            var context = new ViewModelTestContext<IAmortisableAsset, SelectedAssetViewModel>(
                asset,
                new NameData("House", "MyHouse"),
                Account.Asset,
                portfolio,
                viewModelFactory);
            Assert.AreEqual(1, context.ViewModel.ValuesTLVM.Valuations.Count);
            DailyValuation item = context.ViewModel.ValuesTLVM.Valuations[0];
            context.ViewModel.SelectValue(item);
            context.ViewModel.BeginEdit();
            item.Day = new DateTime(2000, 1, 1);
            item.Value = 1;
            context.ViewModel.CompleteEdit(context.Data);

            Assert.AreEqual(1, context.ViewModel.ValuesTLVM.Valuations.Count);
            Assert.AreEqual(1, context.Portfolio.FundsThreadSafe.Single().Count());
            Assert.AreEqual(new DateTime(2000, 1, 1), context.Portfolio.FundsThreadSafe.Single().FirstValue().Day);
        }

        [Test]
        [Ignore("IncompeteArchitecture - FileInteraction does not currently allow for use in test environment.")]
        public void CanAddFromCSV()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            portfolio.TryGetAccount(Account.Asset, new NameData("House", "MyHouse"), out var desired);
            var asset = desired as IAmortisableAsset;
            var viewModelFactory = TestSetupHelper.CreateViewModelFactory(portfolio, new MockFileSystem(), null, null);
            var context = new ViewModelTestContext<IAmortisableAsset, SelectedAssetViewModel>(
                asset,
                new NameData("House", "MyHouse"),
                Account.Asset,
                portfolio,
                viewModelFactory);
            Assert.AreEqual(1, context.ViewModel.ValuesTLVM.Valuations.Count);
        }

        [Test]
        [Ignore("IncompeteArchitecture - FileInteraction does not currently allow for use in test environment.")]
        public void CanWriteToCSV()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            portfolio.TryGetAccount(Account.Asset, new NameData("House", "MyHouse"), out var desired);
            var asset = desired as IAmortisableAsset;
            var viewModelFactory = TestSetupHelper.CreateViewModelFactory(portfolio, new MockFileSystem(), null, null);
            var context = new ViewModelTestContext<IAmortisableAsset,SelectedAssetViewModel>(
                asset,
                new NameData("House", "MyHouse"),
                Account.Asset,
                portfolio,
                viewModelFactory);
            Assert.AreEqual(1, context.ViewModel.ValuesTLVM.Valuations.Count);
        }

        [Test]
        [RequiresThread(ApartmentState.STA)]
        public void CanDeleteValue()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            portfolio.TryGetAccount(Account.Asset, new NameData("House", "MyHouse"), out var desired);
            var asset = desired as IAmortisableAsset;
            var viewModelFactory = TestSetupHelper.CreateViewModelFactory(portfolio, new MockFileSystem(), null, null);

            var context = new ViewModelTestContext<IAmortisableAsset, SelectedAssetViewModel>(
                asset,
                new NameData("House", "MyHouse"),
                Account.Asset,
                portfolio,
                viewModelFactory);
            Assert.AreEqual(1, context.ViewModel.ValuesTLVM.Valuations.Count);
            context.ViewModel.SelectValue(context.ViewModel.ValuesTLVM.Valuations[0]);
            context.ViewModel.DeleteSelected(asset);
            Assert.AreEqual(0, asset.Values.Count());
            Assert.AreEqual(1, asset.Debt.Count());
        }
    }
}
