using System;
using System.Linq;
using System.Threading;
using Common.Structure.DataStructures;
using FPD.Logic.Tests.TestHelpers;
using FPD.Logic.Tests.ViewModelExtensions;
using FinancialStructures.Database;
using FinancialStructures.FinanceStructures;
using NUnit.Framework;

namespace FPD.Logic.Tests.AssetWindowTests
{
    [TestFixture]
    public class SelectedAssetViewModelTests : SelectedAssetTestHelper
    {
        [Test]
        public void CanOpenWindow()
        {
            Assert.AreEqual(1, ViewModel.ValuesTLVM.Valuations.Count);
            Assert.AreEqual(1, ViewModel.DebtTLVM.Valuations.Count);
        }

        [Test]
        public void CanAddDebtValue()
        {
            Assert.AreEqual(1, ViewModel.DebtTLVM.Valuations.Count);
            ViewModel.SelectDebt(null);
            DailyValuation newItem = ViewModel.AddNewDebt();
            ViewModel.BeginEditDebt();
            newItem.Day = new DateTime(2002, 1, 1);
            newItem.Value = 5;
            ViewModel.CompleteEditTrade(Portfolio);

            Assert.AreEqual(2, ViewModel.DebtTLVM.Valuations.Count);
            Assert.AreEqual(1, Portfolio.FundsThreadSafe.Single().Count());
        }

        [Test]
        public void CanEditDebtValue()
        {
            Assert.AreEqual(1, ViewModel.DebtTLVM.Valuations.Count);
            DailyValuation item = ViewModel.DebtTLVM.Valuations[0];
            ViewModel.SelectDebt(item);
            ViewModel.BeginEditDebt();
            item.Day = new DateTime(2000, 1, 1);
            item.Value = 1;
            ViewModel.CompleteEditTrade(Portfolio);

            Assert.AreEqual(1, ViewModel.DebtTLVM.Valuations.Count);
            Assert.AreEqual(1, Portfolio.FundsThreadSafe.Single().Count());
            Assert.AreEqual(new DateTime(2000, 1, 1), Portfolio.FundsThreadSafe.Single().FirstValue().Day);
        }

        [Test]
        [RequiresThread(ApartmentState.STA)]
        public void CanDeleteDebt()
        {
            Assert.AreEqual(1, ViewModel.DebtTLVM.Valuations.Count);
            ViewModel.SelectDebt(ViewModel.DebtTLVM.Valuations[0]);
            ViewModel.DeleteSelectedDebt(Portfolio);
            _ = Portfolio.TryGetAccount(Account.Asset, Name, out IValueList valueList);
            IAmortisableAsset security = valueList as IAmortisableAsset;
            Assert.AreEqual(1, security.Values.Count());
            Assert.AreEqual(0, security.Debt.Count());
        }

        [Test]
        public void CanAddValue()
        {
            Assert.AreEqual(1, ViewModel.ValuesTLVM.Valuations.Count);
            ViewModel.SelectValue(null);
            DailyValuation newItem = ViewModel.AddNewUnitPrice();
            ViewModel.BeginEdit();
            newItem.Day = new DateTime(2002, 1, 1);
            newItem.Value = 1;
            ViewModel.CompleteEdit(Portfolio);

            Assert.AreEqual(2, ViewModel.ValuesTLVM.Valuations.Count);
            Assert.AreEqual(2, Portfolio.Assets.Single().Count());
        }

        [Test]
        public void CanEditValue()
        {
            Assert.AreEqual(1, ViewModel.ValuesTLVM.Valuations.Count);
            DailyValuation item = ViewModel.ValuesTLVM.Valuations[0];
            ViewModel.SelectValue(item);
            ViewModel.BeginEdit();
            item.Day = new DateTime(2000, 1, 1);
            item.Value = 1;
            ViewModel.CompleteEdit(Portfolio);

            Assert.AreEqual(1, ViewModel.ValuesTLVM.Valuations.Count);
            Assert.AreEqual(1, Portfolio.FundsThreadSafe.Single().Count());
            Assert.AreEqual(new DateTime(2000, 1, 1), Portfolio.FundsThreadSafe.Single().FirstValue().Day);
        }

        [Test]
        [Ignore("IncompeteArchitecture - FileInteraction does not currently allow for use in test environment.")]
        public void CanAddFromCSV()
        {
            Assert.AreEqual(1, ViewModel.ValuesTLVM.Valuations.Count);
        }

        [Test]
        [Ignore("IncompeteArchitecture - FileInteraction does not currently allow for use in test environment.")]
        public void CanWriteToCSV()
        {
            Assert.AreEqual(1, ViewModel.ValuesTLVM.Valuations.Count);
        }

        [Test]
        [RequiresThread(ApartmentState.STA)]
        public void CanDeleteValue()
        {
            Assert.AreEqual(1, ViewModel.ValuesTLVM.Valuations.Count);
            ViewModel.SelectValue(ViewModel.ValuesTLVM.Valuations[0]);
            ViewModel.DeleteSelected(Portfolio);
            _ = Portfolio.TryGetAccount(Account.Asset, Name, out IValueList valueList);
            IAmortisableAsset security = valueList as IAmortisableAsset;
            Assert.AreEqual(0, security.Values.Count());
            Assert.AreEqual(1, security.Debt.Count());
        }
    }
}
