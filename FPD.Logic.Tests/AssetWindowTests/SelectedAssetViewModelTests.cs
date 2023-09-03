using System;
using System.Linq;
using System.Threading;

using Common.Structure.DataEdit;
using Common.Structure.DataStructures;
using Common.UI;

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
        private readonly Func<UiGlobals, IPortfolio, NameData, IUpdater<IPortfolio>, SelectedAssetViewModel> _viewModelFactory
            = (globals, portfolio, name, dataUpdater) => new SelectedAssetViewModel(
                portfolio,
                null,
                globals,
                name,
                Account.Asset,
                dataUpdater);

        [Test]
        public void CanOpenWindow()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            var context = new ViewModelTestContext<SelectedAssetViewModel>(
                new NameData("House", "MyHouse"),
                portfolio,
                _viewModelFactory);
            Assert.AreEqual(1, context.ViewModel.ValuesTLVM.Valuations.Count);
            Assert.AreEqual(1, context.ViewModel.DebtTLVM.Valuations.Count);
        }

        [Test]
        public void CanAddDebtValue()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            var context = new ViewModelTestContext<SelectedAssetViewModel>(
                new NameData("House", "MyHouse"),
                portfolio,
                _viewModelFactory);
            Assert.AreEqual(1, context.ViewModel.DebtTLVM.Valuations.Count);
            context.ViewModel.SelectDebt(null);
            DailyValuation newItem = context.ViewModel.AddNewDebt();
            context.ViewModel.BeginEditDebt();
            newItem.Day = new DateTime(2002, 1, 1);
            newItem.Value = 5;
            context.ViewModel.CompleteEditTrade(context.Portfolio);

            Assert.AreEqual(2, context.ViewModel.DebtTLVM.Valuations.Count);
            Assert.AreEqual(1, context.Portfolio.FundsThreadSafe.Single().Count());
        }

        [Test]
        public void CanEditDebtValue()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            var context = new ViewModelTestContext<SelectedAssetViewModel>(
                new NameData("House", "MyHouse"),
                portfolio,
                _viewModelFactory);
            Assert.AreEqual(1, context.ViewModel.DebtTLVM.Valuations.Count);
            DailyValuation item = context.ViewModel.DebtTLVM.Valuations[0];
            context.ViewModel.SelectDebt(item);
            context.ViewModel.BeginEditDebt();
            item.Day = new DateTime(2000, 1, 1);
            item.Value = 1;
            context.ViewModel.CompleteEditTrade(context.Portfolio);

            Assert.AreEqual(1, context.ViewModel.DebtTLVM.Valuations.Count);
            Assert.AreEqual(1, context.Portfolio.FundsThreadSafe.Single().Count());
            Assert.AreEqual(new DateTime(2000, 1, 1), context.Portfolio.FundsThreadSafe.Single().FirstValue().Day);
        }

        [Test]
        [RequiresThread(ApartmentState.STA)]
        public void CanDeleteDebt()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            var context = new ViewModelTestContext<SelectedAssetViewModel>(
                new NameData("House", "MyHouse"),
                portfolio,
                _viewModelFactory);
            Assert.AreEqual(1, context.ViewModel.DebtTLVM.Valuations.Count);
            context.ViewModel.SelectDebt(context.ViewModel.DebtTLVM.Valuations[0]);
            context.ViewModel.DeleteSelectedDebt(context.Portfolio);
            _ = context.Portfolio.TryGetAccount(Account.Asset, context.Name, out IValueList valueList);
            IAmortisableAsset security = valueList as IAmortisableAsset;
            Assert.AreEqual(1, security.Values.Count());
            Assert.AreEqual(0, security.Debt.Count());
        }

        [Test]
        public void CanAddValue()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            var context = new ViewModelTestContext<SelectedAssetViewModel>(
                new NameData("House", "MyHouse"),
                portfolio,
                _viewModelFactory);
            Assert.AreEqual(1, context.ViewModel.ValuesTLVM.Valuations.Count);
            context.ViewModel.SelectValue(null);
            DailyValuation newItem = context.ViewModel.AddNewUnitPrice();
            context.ViewModel.BeginEdit();
            newItem.Day = new DateTime(2002, 1, 1);
            newItem.Value = 1;
            context.ViewModel.CompleteEdit(context.Portfolio);

            Assert.AreEqual(2, context.ViewModel.ValuesTLVM.Valuations.Count);
            Assert.AreEqual(2, context.Portfolio.Assets.Single().Count());
        }

        [Test]
        public void CanEditValue()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            var context = new ViewModelTestContext<SelectedAssetViewModel>(
                new NameData("House", "MyHouse"),
                portfolio,
                _viewModelFactory);
            Assert.AreEqual(1, context.ViewModel.ValuesTLVM.Valuations.Count);
            DailyValuation item = context.ViewModel.ValuesTLVM.Valuations[0];
            context.ViewModel.SelectValue(item);
            context.ViewModel.BeginEdit();
            item.Day = new DateTime(2000, 1, 1);
            item.Value = 1;
            context.ViewModel.CompleteEdit(context.Portfolio);

            Assert.AreEqual(1, context.ViewModel.ValuesTLVM.Valuations.Count);
            Assert.AreEqual(1, context.Portfolio.FundsThreadSafe.Single().Count());
            Assert.AreEqual(new DateTime(2000, 1, 1), context.Portfolio.FundsThreadSafe.Single().FirstValue().Day);
        }

        [Test]
        [Ignore("IncompeteArchitecture - FileInteraction does not currently allow for use in test environment.")]
        public void CanAddFromCSV()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            var context = new ViewModelTestContext<SelectedAssetViewModel>(
                new NameData("House", "MyHouse"),
                portfolio,
                _viewModelFactory);
            Assert.AreEqual(1, context.ViewModel.ValuesTLVM.Valuations.Count);
        }

        [Test]
        [Ignore("IncompeteArchitecture - FileInteraction does not currently allow for use in test environment.")]
        public void CanWriteToCSV()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            var context = new ViewModelTestContext<SelectedAssetViewModel>(
                new NameData("House", "MyHouse"),
                portfolio,
                _viewModelFactory);
            Assert.AreEqual(1, context.ViewModel.ValuesTLVM.Valuations.Count);
        }

        [Test]
        [RequiresThread(ApartmentState.STA)]
        public void CanDeleteValue()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            var context = new ViewModelTestContext<SelectedAssetViewModel>(
                new NameData("House", "MyHouse"),
                portfolio,
                _viewModelFactory);
            Assert.AreEqual(1, context.ViewModel.ValuesTLVM.Valuations.Count);
            context.ViewModel.SelectValue(context.ViewModel.ValuesTLVM.Valuations[0]);
            context.ViewModel.DeleteSelected(context.Portfolio);
            _ = context.Portfolio.TryGetAccount(Account.Asset, context.Name, out IValueList valueList);
            IAmortisableAsset security = valueList as IAmortisableAsset;
            Assert.AreEqual(0, security.Values.Count());
            Assert.AreEqual(1, security.Debt.Count());
        }
    }
}
