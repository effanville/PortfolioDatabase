using System.Linq;
using System.Threading;
using FPD.Logic.ViewModels.Common;
using FPD.Logic.Tests.TestHelpers;
using FPD.Logic.Tests.ViewModelExtensions;
using FinancialStructures.Database;
using FinancialStructures.NamingStructures;
using NUnit.Framework;
using System.IO.Abstractions.TestingHelpers;

namespace FPD.Logic.Tests.CommonWindowTests
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public sealed class DataNamesViewModelTests
    {
        [Test]
        public void CanOpen()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            var viewModelFactory = TestSetupHelper.CreateViewModelFactory(portfolio, new MockFileSystem(), null, null);
            var context = new ViewModelTestContext<IPortfolio, DataNamesViewModel>(
                null,
                null,
                Account.Security,
                portfolio,
                viewModelFactory);
            Assert.AreEqual(1, context.ViewModel.DataNames.Count);
        }

        [Test]
        public void CanUpdateData()
        {
            var portfolio = TestSetupHelper.CreateEmptyDataBase();
            var viewModelFactory = TestSetupHelper.CreateViewModelFactory(portfolio, new MockFileSystem(), null, null);
            var context = new ViewModelTestContext<IPortfolio, DataNamesViewModel>(
                null,
                null,
                Account.Security,
                portfolio,
                viewModelFactory);
            IPortfolio newData = TestSetupHelper.CreateBasicDataBase();

            context.ViewModel.UpdateData(newData);

            Assert.AreEqual(1, context.ViewModel.DataNames.Count);
        }

        [Test]
        public void CanOpenSecurity()
        {
            var portfolio = TestSetupHelper.CreateEmptyDataBase();
            var viewModelFactory = TestSetupHelper.CreateViewModelFactory(portfolio, new MockFileSystem(), null, null);
            var context = new ViewModelTestContext<IPortfolio, DataNamesViewModel>(
                null,
                null,
                Account.Security,
                portfolio,
                viewModelFactory);
            IPortfolio output = TestSetupHelper.CreateBasicDataBase();
            var dataUpdater = TestSetupHelper.CreateUpdater(portfolio);
            DataNamesViewModel viewModel = new DataNamesViewModel(output, TestSetupHelper.CreateGlobalsMock(null,null, null,TestSetupHelper.DummyReportLogger ), null, dataUpdater, TestSetupHelper.DummyOpenTab, Account.Security);
            viewModel.UpdateRequest += dataUpdater.PerformUpdate;
            Assert.AreEqual(1, viewModel.DataNames.Count);
        }

        [Test]
        public void CanUpdateSecurityData()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            var viewModelFactory = TestSetupHelper.CreateViewModelFactory(portfolio, new MockFileSystem(), null, null);
            var context = new ViewModelTestContext<IPortfolio, DataNamesViewModel>(
                null,
                null,
                Account.Security,
                portfolio,
                viewModelFactory);
            IPortfolio newData = TestSetupHelper.CreateBasicDataBase();
            context.ViewModel.UpdateData(newData);

            Assert.AreEqual(1, context.ViewModel.DataNames.Count);
        }

        [Test]
        public void CanCreateNewSecurity()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            var viewModelFactory = TestSetupHelper.CreateViewModelFactory(portfolio, new MockFileSystem(), null, null);
            var context = new ViewModelTestContext<IPortfolio, DataNamesViewModel>(
                null,
                null,
                Account.Security,
                portfolio,
                viewModelFactory);
            context.ViewModel.SelectItem(null);
            var newRowItem = context.ViewModel.AddNewItem();
            var newItem = newRowItem.Instance;
            newItem.Company = "company";
            newItem.Name = "name";
            newItem.Currency = "GBP";
            newItem.Url = "someUrl";
            context.ViewModel.CompleteCreate(newRowItem);
            Assert.AreEqual(2, context.ViewModel.DataNames.Count, "Not enough in the view.");
            Assert.AreEqual(2, context.Portfolio.FundsThreadSafe.Count, "Not enough in portfolio");
        }

        [Test]
        public void CanEditSecurityName()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            var viewModelFactory = TestSetupHelper.CreateViewModelFactory(portfolio, new MockFileSystem(), null, null);
            var context = new ViewModelTestContext<IPortfolio, DataNamesViewModel>(
                null,
                null,
                Account.Security,
                portfolio,
                viewModelFactory);
            var item = context.ViewModel.DataNames[0];
            context.ViewModel.SelectItem(item.Instance);
            context.ViewModel.BeginRowEdit(item);
            item.Instance.Company = "NewCompany";
            context.ViewModel.CompleteEdit(item);

            Assert.AreEqual(1, context.ViewModel.DataNames.Count);
            Assert.AreEqual(1, context.Portfolio.FundsThreadSafe.Count);

            Assert.AreEqual("NewCompany", context.Portfolio.FundsThreadSafe.Single().Names.Company);
        }

        [Test]
        public void CanEditSecurityNameAndUrl()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            var viewModelFactory = TestSetupHelper.CreateViewModelFactory(portfolio, new MockFileSystem(), null, null);
            var context = new ViewModelTestContext<IPortfolio, DataNamesViewModel>(
                null,
                null,
                Account.Security,
                portfolio,
                viewModelFactory);
            var item = context.ViewModel.DataNames[0];
            context.ViewModel.SelectItem(item.Instance);
            context.ViewModel.BeginRowEdit(item);
            item.Instance.Company = "NewCompany";
            item.Instance.Url = "NewUrl";
            context.ViewModel.CompleteEdit(item);

            Assert.AreEqual(1, context.ViewModel.DataNames.Count);
            Assert.AreEqual(1, context.Portfolio.FundsThreadSafe.Count);

            Assert.AreEqual("NewCompany", context.Portfolio.FundsThreadSafe.Single().Names.Company);
            Assert.AreEqual("NewUrl", context.Portfolio.FundsThreadSafe.Single().Names.Url);
        }

        [Test]
        [Ignore("IncompeteArchitecture - Downloader does not currently allow for use in test environment.")]
        public void CanDownloadSecurity()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            var viewModelFactory = TestSetupHelper.CreateViewModelFactory(portfolio, new MockFileSystem(), null, null);
            var context = new ViewModelTestContext<IPortfolio, DataNamesViewModel>(
                portfolio,
                new NameData("Fidelity", "China"),
                Account.Security,
                portfolio,
                viewModelFactory);
            NameData item = new NameData("Fidelity", "China");
            context.ViewModel.SelectItem(item);
            context.ViewModel.DownloadSelected();

            Assert.AreEqual(1, context.ViewModel.DataNames.Count);
        }


        [Test]
        public void CanDeleteSecurity()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            var viewModelFactory = TestSetupHelper.CreateViewModelFactory(portfolio, new MockFileSystem(), null, null);
            var context = new ViewModelTestContext<IPortfolio, DataNamesViewModel>(
                portfolio,
                new NameData("Fidelity", "China"),
                Account.Security,
                portfolio,
                viewModelFactory);
            Assert.AreEqual(1, context.ViewModel.ModelData.FundsThreadSafe.Count);
            Assert.AreEqual(1, context.Portfolio.FundsThreadSafe.Count);

            NameData item = new NameData("Fidelity", "China");
            context.ViewModel.SelectItem(item);
            context.ViewModel.DeleteSelected();
            Assert.AreEqual(0, context.ViewModel.ModelData.FundsThreadSafe.Count);
            Assert.AreEqual(0, context.Portfolio.FundsThreadSafe.Count);
        }
    }
}
