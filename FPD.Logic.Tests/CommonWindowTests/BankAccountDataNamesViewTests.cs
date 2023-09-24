using System;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;

using Common.Structure.DisplayClasses;

using FinancialStructures.Database;
using FinancialStructures.NamingStructures;

using FPD.Logic.Tests.TestHelpers;
using FPD.Logic.Tests.ViewModelExtensions;
using FPD.Logic.ViewModels.Common;

using NUnit.Framework;

namespace FPD.Logic.Tests.CommonWindowTests
{
    [TestFixture]
    public class BankAccountDataNamesViewTests
    {
        [Test]
        public void CanCreateNew()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            var viewModelFactory = TestSetupHelper.CreateViewModelFactory(portfolio, new MockFileSystem(), null, null);
            var context = new ViewModelTestContext<IPortfolio, DataNamesViewModel>(
                portfolio,
                new NameData("Barclays", "currentAccount"),
                Account.BankAccount,
                portfolio,
                viewModelFactory);
            context.ViewModel.SelectItem(null);

            var newRowItem = context.ViewModel.AddNewItem();
            var newItem = newRowItem.Instance;
            newItem.Company = "Company";
            newItem.Name = "name";
            newItem.Currency = "GBP";
            newItem.Url = "someUrl";
            context.ViewModel.CompleteCreate(newRowItem);
            Assert.AreEqual(2, context.ViewModel.DataNames.Count);
            Assert.AreEqual(2, context.Portfolio.BankAccountsThreadSafe.Count);
        }

        [Test]
        [STAThread]
        public void CanEditName()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            var viewModelFactory = TestSetupHelper.CreateViewModelFactory(portfolio, new MockFileSystem(), null, null);
            var context = new ViewModelTestContext<IPortfolio, DataNamesViewModel>(
                portfolio,
                new NameData("Barclays", "currentAccount"),
                Account.BankAccount,
                portfolio,
                viewModelFactory);
            context.ViewModel.SelectItem(null);
            var row = context.ViewModel.DataNames[0];
            NameData item = row.Instance;
            context.ViewModel.SelectItem(item);
            context.ViewModel.BeginRowEdit(row);
            item.Company = "NewCompany";
            context.ViewModel.CompleteEdit(row);

            Assert.AreEqual(1, context.ViewModel.DataNames.Count);
            Assert.AreEqual(1, context.Portfolio.BankAccountsThreadSafe.Count);

            Assert.AreEqual("NewCompany", context.Portfolio.BankAccountsThreadSafe.Single().Names.Company);
        }

        [Test]
        [Ignore("IncompleteArchitecture - Downloader does not currently allow for use in test environment.")]
        public void CanDownload()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            var viewModelFactory = TestSetupHelper.CreateViewModelFactory(portfolio, new MockFileSystem(), null, null);
            var context = new ViewModelTestContext<IPortfolio, DataNamesViewModel>(
                portfolio,
                new NameData("Barclays", "currentAccount"),
                Account.BankAccount,
                portfolio,
                viewModelFactory);
            context.ViewModel.SelectItem(null);
            SelectableEquatable<NameData> item = context.ViewModel.DataNames.First();
            context.ViewModel.SelectItem(item.Instance);
            context.ViewModel.DownloadSelected();

            Assert.AreEqual(1, context.ViewModel.DataNames.Count);
            bool account = context.Portfolio.TryGetAccount(Account.BankAccount, new TwoName("Barclays", "currentAccount"), out FinancialStructures.FinanceStructures.IValueList sec);
            Assert.IsTrue(account);
            Assert.AreEqual(2, sec.Values.Count());
        }

        [Test]
        public void CanDelete()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            var viewModelFactory = TestSetupHelper.CreateViewModelFactory(portfolio, new MockFileSystem(), null, null);
            var context = new ViewModelTestContext<IPortfolio, DataNamesViewModel>(
                portfolio,
                new NameData("Barclays", "currentAccount"),
                Account.BankAccount,
                portfolio,
                viewModelFactory);
            context.ViewModel.SelectItem(null);
            Assert.AreEqual(1, context.ViewModel.ModelData.FundsThreadSafe.Count);
            Assert.AreEqual(1, context.Portfolio.BankAccountsThreadSafe.Count);
            NameData item = new NameData("Barclays", "currentAccount");
            context.ViewModel.SelectItem(item);
            context.ViewModel.DeleteSelected();
            Assert.AreEqual(0, context.ViewModel.ModelData.BankAccountsThreadSafe.Count);
            Assert.AreEqual(0, context.Portfolio.BankAccountsThreadSafe.Count);
        }
    }
}
