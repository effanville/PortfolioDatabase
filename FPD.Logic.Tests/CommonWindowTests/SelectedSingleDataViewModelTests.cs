using System;
using System.Linq;

using Common.Structure.DataEdit;
using Common.Structure.DataStructures;
using Common.UI;

using FinancialStructures.Database;
using FinancialStructures.FinanceStructures;
using FinancialStructures.NamingStructures;

using FPD.Logic.Tests.TestHelpers;
using FPD.Logic.Tests.ViewModelExtensions;
using FPD.Logic.ViewModels.Common;

using NUnit.Framework;

namespace FPD.Logic.Tests.CommonWindowTests
{
    [TestFixture]
    public class SelectedSingleDataViewModelTests
    {
        private readonly Func<IValueList, UiGlobals, IPortfolio, NameData, IUpdater<IPortfolio>, SelectedSingleDataViewModel> _viewModelFactory
            = (data, globals, portfolio, name, dataUpdater) => new SelectedSingleDataViewModel(
                portfolio,
                data,
                null,
                globals,
                new NameData("Barclays", "currentAccount"),
                Account.BankAccount,
                dataUpdater);

        [Test]
        public void CanOpenWindow()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            var context = new ViewModelTestContext<IValueList, SelectedSingleDataViewModel>(
                null,
                null,
                portfolio,
                _viewModelFactory);
            Assert.AreEqual(1, context.ViewModel.TLVM.Valuations.Count);
        }

        [Test]
        public void CanAddValue()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            var context = new ViewModelTestContext<IValueList, SelectedSingleDataViewModel>(
                null,
                null,
                portfolio,
                _viewModelFactory);
            Assert.AreEqual(1, context.ViewModel.TLVM.Valuations.Count);
            context.ViewModel.SelectItem(null);
            DailyValuation newItem = context.ViewModel.AddNewItem();

            context.ViewModel.BeginEdit();
            newItem.Day = new DateTime(2002, 1, 1);
            newItem.Value = 1;
            context.ViewModel.CompleteEdit(context.Portfolio);

            Assert.AreEqual(2, context.ViewModel.TLVM.Valuations.Count);
            Assert.AreEqual(2, context.Portfolio.BankAccountsThreadSafe.Single().Count());
        }

        [Test]
        public void CanEditValue()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            var context = new ViewModelTestContext<IValueList, SelectedSingleDataViewModel>(
                null,
                null,
                portfolio,
                _viewModelFactory);
            Assert.AreEqual(1, context.ViewModel.TLVM.Valuations.Count);
            DailyValuation item = context.ViewModel.TLVM.Valuations[0];
            context.ViewModel.SelectItem(item);
            context.ViewModel.BeginEdit();
            item.Day = new DateTime(2000, 1, 1);
            item.Value = 1;
            context.ViewModel.CompleteEdit(context.Portfolio);

            Assert.AreEqual(1, context.ViewModel.TLVM.Valuations.Count);
            Assert.AreEqual(1, context.Portfolio.BankAccountsThreadSafe.Single().Count());
            Assert.AreEqual(new DateTime(2000, 1, 1), context.Portfolio.BankAccountsThreadSafe.Single().FirstValue().Day);
        }


        [Test]
        [Ignore("IncompeteArchitecture - FileInteraction does not currently allow for use in test environment.")]
        public void CanAddFromCSV()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            var context = new ViewModelTestContext<IValueList, SelectedSingleDataViewModel>(
                null,
                new NameData("Barclays", "currentAccount"),
                portfolio,
                _viewModelFactory);
            Assert.AreEqual(1, context.ViewModel.TLVM.Valuations.Count);
        }

        [Test]
        [Ignore("IncompeteArchitecture - FileInteraction does not currently allow for use in test environment.")]
        public void CanWriteToCSV()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            var context = new ViewModelTestContext<IValueList, SelectedSingleDataViewModel>(
                null,
                null,
                portfolio,
                _viewModelFactory);
            Assert.AreEqual(1, context.ViewModel.TLVM.Valuations.Count);
        }

        [Test]
        public void CanDeleteValue()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            var context = new ViewModelTestContext<IValueList, SelectedSingleDataViewModel>(
                null,
                null,
                portfolio,
                _viewModelFactory);
            Assert.AreEqual(1, context.ViewModel.TLVM.Valuations.Count);
            var name = new NameData("Barclays", "currentAccount");
            context.ViewModel.SelectItem(context.ViewModel.TLVM.Valuations[0]);
            context.ViewModel.DeleteSelected(context.Portfolio);
            _ = context.Portfolio.TryGetAccount(Account.BankAccount, name, out IValueList bankAccount);
            Assert.AreEqual(0, bankAccount.Values.Count());
            Assert.AreEqual(0, context.ViewModel.TLVM.Valuations.Count);
        }
    }
}
