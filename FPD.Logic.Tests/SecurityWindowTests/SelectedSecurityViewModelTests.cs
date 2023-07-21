using System;
using System.Linq;
using System.Threading;
using Common.Structure.DataStructures;
using FPD.Logic.Tests.TestHelpers;
using FPD.Logic.Tests.ViewModelExtensions;
using FinancialStructures.Database;
using FinancialStructures.DataStructures;
using FinancialStructures.FinanceStructures;
using NUnit.Framework;
using FinancialStructures.NamingStructures;
using FPD.Logic.ViewModels.Security;
using Common.UI;
using Common.Structure.DataEdit;

namespace FPD.Logic.Tests.SecurityWindowTests
{
    [TestFixture]
    public sealed class SelectedSecurityViewModelTests
    {
        Func<UiGlobals, IPortfolio, NameData, IUpdater<IPortfolio>, SelectedSecurityViewModel> _viewModelFactory
            = (globals, portfolio, name, dataUpdater) => new SelectedSecurityViewModel(
                portfolio,
                TestSetupHelper.DummyReportLogger,
                null,
                globals,
                name,
                Account.Security);

        [Test]
        public void CanOpenWindow()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            var context = new ViewModelTestContext<SelectedSecurityViewModel>(
                new NameData("Fidelity", "China"),
                portfolio,
                _viewModelFactory);
            Assert.AreEqual(1, context.ViewModel.TLVM.Valuations.Count);
        }

        [Test]
        public void CanAddTradeValue()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            var context = new ViewModelTestContext<SelectedSecurityViewModel>(
                new NameData("Fidelity", "China"),
                portfolio,
                _viewModelFactory);
            Assert.AreEqual(1, context.ViewModel.Trades.Count);
            context.ViewModel.SelectTrade(null);
            SecurityTrade newItem = context.ViewModel.AddNewTrade();
            context.ViewModel.BeginEditTrade();
            newItem.Day = new DateTime(2002, 1, 1);
            newItem.NumberShares = 5;
            context.ViewModel.CompleteEditTrade(context.Portfolio);

            Assert.AreEqual(2, context.ViewModel.Trades.Count);
            Assert.AreEqual(1, context.Portfolio.FundsThreadSafe.Single().Count());
        }

        [Test]
        public void CanEditTradeValue()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            var context = new ViewModelTestContext<SelectedSecurityViewModel>(
                new NameData("Fidelity", "China"),
                portfolio,
                _viewModelFactory);
            Assert.AreEqual(1, context.ViewModel.Trades.Count);
            SecurityTrade item = context.ViewModel.Trades[0];
            context.ViewModel.SelectTrade(item);
            context.ViewModel.BeginEditTrade();
            item.Day = new DateTime(2000, 1, 1);
            item.NumberShares = 1;
            context.ViewModel.CompleteEditTrade(context.Portfolio);

            Assert.AreEqual(1, context.ViewModel.Trades.Count);
            Assert.AreEqual(1, context.Portfolio.FundsThreadSafe.Single().Count());
            Assert.AreEqual(new DateTime(2000, 1, 1), context.Portfolio.FundsThreadSafe.Single().FirstValue().Day);
        }

        [Test]
        [RequiresThread(ApartmentState.STA)]
        public void CanDeleteTrade()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            var context = new ViewModelTestContext<SelectedSecurityViewModel>(
                new NameData("Fidelity", "China"),
                portfolio,
                _viewModelFactory);
            Assert.AreEqual(1, context.ViewModel.Trades.Count);
            context.ViewModel.SelectTrade(context.ViewModel.Trades[0]);
            context.ViewModel.DeleteSelectedTrade(context.Portfolio);
            _ = context.Portfolio.TryGetAccount(Account.Security, context.Name, out IValueList valueList);
            ISecurity security = valueList as ISecurity;
            Assert.AreEqual(0, security.Values.Count());
            Assert.AreEqual(1, security.UnitPrice.Count());
            Assert.AreEqual(0, security.Trades.Count);
        }

        [Test]
        public void CanAddValue()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            var context = new ViewModelTestContext<SelectedSecurityViewModel>(
                new NameData("Fidelity", "China"),
                portfolio,
                _viewModelFactory);
            Assert.AreEqual(1, context.ViewModel.TLVM.Valuations.Count);
            context.ViewModel.SelectUnitPrice(null);
            DailyValuation newItem = context.ViewModel.AddNewUnitPrice();
            context.ViewModel.BeginEdit();
            newItem.Day = new DateTime(2002, 1, 1);
            newItem.Value = 1;
            context.ViewModel.CompleteEdit(context.Portfolio);

            Assert.AreEqual(2, context.ViewModel.TLVM.Valuations.Count);
            Assert.AreEqual(2, context.Portfolio.FundsThreadSafe.Single().Count());
        }

        [Test]
        public void CanEditValue()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            var context = new ViewModelTestContext<SelectedSecurityViewModel>(
                new NameData("Fidelity", "China"),
                portfolio,
                _viewModelFactory);
            Assert.AreEqual(1, context.ViewModel.TLVM.Valuations.Count);
            DailyValuation item = context.ViewModel.TLVM.Valuations[0];
            context.ViewModel.SelectUnitPrice(item);
            context.ViewModel.BeginEdit();
            item.Day = new DateTime(2000, 1, 1);
            item.Value = 1;
            context.ViewModel.CompleteEdit(context.Portfolio);

            Assert.AreEqual(1, context.ViewModel.TLVM.Valuations.Count);
            Assert.AreEqual(1, context.Portfolio.FundsThreadSafe.Single().Count());
            Assert.AreEqual(new DateTime(2000, 1, 1), context.Portfolio.FundsThreadSafe.Single().FirstValue().Day);
        }

        [Test]
        [Ignore("IncompeteArchitecture - FileInteraction does not currently allow for use in test environment.")]
        public void CanAddFromCSV()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            var context = new ViewModelTestContext<SelectedSecurityViewModel>(
                new NameData("Fidelity", "China"),
                portfolio,
                _viewModelFactory);
            Assert.AreEqual(1, context.ViewModel.TLVM.Valuations.Count);
        }

        [Test]
        [Ignore("IncompeteArchitecture - FileInteraction does not currently allow for use in test environment.")]
        public void CanWriteToCSV()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            var context = new ViewModelTestContext<SelectedSecurityViewModel>(
                new NameData("Fidelity", "China"),
                portfolio,
                _viewModelFactory);
            Assert.AreEqual(1, context.ViewModel.TLVM.Valuations.Count);
        }

        [Test]
        [RequiresThread(ApartmentState.STA)]
        public void CanDeleteValue()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            var context = new ViewModelTestContext<SelectedSecurityViewModel>(
                new NameData("Fidelity", "China"),
                portfolio,
                _viewModelFactory);
            Assert.AreEqual(1, context.ViewModel.TLVM.Valuations.Count);
            context.ViewModel.SelectUnitPrice(context.ViewModel.TLVM.Valuations[0]);
            context.ViewModel.DeleteSelected(context.Portfolio);
            _ = context.Portfolio.TryGetAccount(Account.Security, context.Name, out IValueList valueList);
            ISecurity security = valueList as ISecurity;
            Assert.AreEqual(0, security.Values.Count());
            Assert.AreEqual(0, security.UnitPrice.Count());
            Assert.AreEqual(1, security.Trades.Count);
        }
    }
}
