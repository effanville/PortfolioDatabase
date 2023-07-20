using System.Linq;
using System.Threading;
using FPD.Logic.ViewModels.Common;
using FPD.Logic.Tests.TestHelpers;
using FPD.Logic.Tests.ViewModelExtensions;
using FinancialStructures.Database;
using FinancialStructures.NamingStructures;
using NUnit.Framework;

namespace FPD.Logic.Tests.CommonWindowTests
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public partial class DataNamesViewModelTests : DataNamesViewTestHelper
    {
        [Test]
        public void CanOpen()
        {
            Portfolio = TestSetupHelper.CreateBasicDataBase();
            Assert.AreEqual(1, ViewModel.DataNames.Count);
        }

        [Test]
        public void CanUpdateData()
        {
            IPortfolio newData = TestSetupHelper.CreateBasicDataBase();

            ViewModel.UpdateData(newData);

            Assert.AreEqual(1, ViewModel.DataNames.Count);
        }

        [Test]
        public void CanOpenSecurity()
        {
            IPortfolio output = TestSetupHelper.CreateBasicDataBase();
            IPortfolio portfolio = TestSetupHelper.CreateBasicDataBase();
            var dataUpdater = TestSetupHelper.CreateUpdater(portfolio);
            DataNamesViewModel viewModel = new DataNamesViewModel(output, TestSetupHelper.DummyReportLogger, null, dataUpdater, TestSetupHelper.DummyOpenTab, Account.Security);
            viewModel.UpdateRequest += dataUpdater.PerformUpdate;
            Assert.AreEqual(1, viewModel.DataNames.Count);
        }

        [Test]
        public void CanUpdateSecurityData()
        {
            IPortfolio newData = TestSetupHelper.CreateBasicDataBase();
            ViewModel.UpdateData(newData);

            Assert.AreEqual(1, ViewModel.DataNames.Count);
        }

        [Test]
        public void CanCreateNewSecurity()
        {
            Portfolio = TestSetupHelper.CreateBasicDataBase();
            ViewModel.SelectItem(null);
            var newRowItem = ViewModel.AddNewItem();
            var newItem = newRowItem.Instance;
            newItem.Company = "company";
            newItem.Name = "name";
            newItem.Currency = "GBP";
            newItem.Url = "someUrl";
            ViewModel.CompleteCreate(newRowItem);
            Assert.AreEqual(2, ViewModel.DataNames.Count, "Not enough in the view.");
            Assert.AreEqual(2, Portfolio.FundsThreadSafe.Count, "Not enough in portfolio");
        }

        [Test]
        public void CanEditSecurityName()
        {
            Portfolio = TestSetupHelper.CreateBasicDataBase();
            var item = ViewModel.DataNames[0];
            ViewModel.SelectItem(item.Instance);
            ViewModel.BeginRowEdit(item);
            item.Instance.Company = "NewCompany";
            ViewModel.CompleteEdit(item);

            Assert.AreEqual(1, ViewModel.DataNames.Count);
            Assert.AreEqual(1, Portfolio.FundsThreadSafe.Count);

            Assert.AreEqual("NewCompany", Portfolio.FundsThreadSafe.Single().Names.Company);
        }

        [Test]
        public void CanEditSecurityNameAndUrl()
        {
            Portfolio = TestSetupHelper.CreateBasicDataBase();
            var item = ViewModel.DataNames[0];
            ViewModel.SelectItem(item.Instance);
            ViewModel.BeginRowEdit(item);
            item.Instance.Company = "NewCompany";
            item.Instance.Url = "NewUrl";
            ViewModel.CompleteEdit(item);

            Assert.AreEqual(1, ViewModel.DataNames.Count);
            Assert.AreEqual(1, Portfolio.FundsThreadSafe.Count);

            Assert.AreEqual("NewCompany", Portfolio.FundsThreadSafe.Single().Names.Company);
            Assert.AreEqual("NewUrl", Portfolio.FundsThreadSafe.Single().Names.Url);
        }

        [Test]
        [Ignore("IncompeteArchitecture - Downloader does not currently allow for use in test environment.")]
        public void CanDownloadSecurity()
        {
            NameData item = new NameData("Fidelity", "China");
            ViewModel.SelectItem(item);
            ViewModel.DownloadSelected();

            Assert.AreEqual(1, ViewModel.DataNames.Count);
        }


        [Test]
        public void CanDeleteSecurity()
        {
            Portfolio = TestSetupHelper.CreateBasicDataBase();
            Assert.AreEqual(1, ViewModel.DataStore.FundsThreadSafe.Count);
            Assert.AreEqual(1, Portfolio.FundsThreadSafe.Count);

            NameData item = new NameData("Fidelity", "China");
            ViewModel.SelectItem(item);
            ViewModel.DeleteSelected();
            Assert.AreEqual(0, ViewModel.DataStore.FundsThreadSafe.Count);
            Assert.AreEqual(0, Portfolio.FundsThreadSafe.Count);
        }
    }
}
