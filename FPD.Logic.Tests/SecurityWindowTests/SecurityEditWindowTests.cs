using System.IO.Abstractions.TestingHelpers;

using FinancialStructures.Database;
using FinancialStructures.NamingStructures;

using FPD.Logic.Tests.TestHelpers;
using FPD.Logic.Tests.UserInteractions;
using FPD.Logic.ViewModels.Common;

using NUnit.Framework;

namespace FPD.Logic.Tests.SecurityWindowTests
{
    /// <summary>
    /// Tests for window displaying security data.
    /// </summary>
    [TestFixture]
    public class SecurityEditWindowTests
    {
        [Test]
        public void CanLoadSuccessfully()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            var viewModelFactory = TestSetupHelper.CreateViewModelFactory(portfolio, new MockFileSystem(), null, null);
            var context = new ViewModelTestContext<IPortfolio, ValueListWindowViewModel>(
                portfolio,
                null,
                Account.All,
                portfolio,
                viewModelFactory);
            Assert.AreEqual(1, context.ViewModel.Tabs.Count);
            DataNamesViewModel nameModel = context.ViewModel.GetDataNamesViewModel();
            Assert.AreEqual(1, nameModel.DataNames.Count);
        }

        [Test]
        public void CanUpdateData()
        {
            var portfolio = TestSetupHelper.CreateEmptyDataBase();
            var viewModelFactory = TestSetupHelper.CreateViewModelFactory(portfolio, new MockFileSystem(), null, null);
            var context = new ViewModelTestContext<IPortfolio, ValueListWindowViewModel>(
                portfolio,
                null,
                Account.All,
                portfolio,
                viewModelFactory);
            IPortfolio newData = TestSetupHelper.CreateBasicDataBase();
            context.ViewModel.UpdateData(newData);

            Assert.AreEqual("TestFilePath", context.ViewModel.ModelData.Name);
            Assert.AreEqual(1, context.ViewModel.ModelData.FundsThreadSafe.Count);
        }

        [Test]
        public void CanUpdateDataAndRemoveOldTab()
        {
            var portfolio = TestSetupHelper.CreateEmptyDataBase();
            var viewModelFactory = TestSetupHelper.CreateViewModelFactory(portfolio, new MockFileSystem(), null, null);
            var context = new ViewModelTestContext<IPortfolio, ValueListWindowViewModel>(
                portfolio,
                null,
                Account.All,
                portfolio,
                viewModelFactory);
            NameData newNameData = new NameData("Fidelity", "Europe");
            context.ViewModel.LoadTabFunc(newNameData);

            Assert.AreEqual(2, context.ViewModel.Tabs.Count);

            IPortfolio newData = TestSetupHelper.CreateBasicDataBase();
            context.ViewModel.UpdateData(newData);
            Assert.AreEqual(1, context.ViewModel.Tabs.Count);
            Assert.AreEqual("TestFilePath", context.ViewModel.ModelData.Name);
            Assert.AreEqual(1, context.ViewModel.ModelData.FundsThreadSafe.Count);
        }

        [Test]
        public void CanAddTab()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            var viewModelFactory = TestSetupHelper.CreateViewModelFactory(portfolio, new MockFileSystem(), null, null);
            var context = new ViewModelTestContext<IPortfolio, ValueListWindowViewModel>(
                portfolio,
                null,
                Account.All,
                portfolio,
                viewModelFactory);

            NameData newData = new NameData("Fidelity", "China");
            context.ViewModel.LoadTabFunc(newData);

            Assert.AreEqual(2, context.ViewModel.Tabs.Count);
            DataNamesViewModel dataNames = context.ViewModel.GetDataNamesViewModel();
            Assert.AreEqual(1, dataNames.DataNames.Count);
            var selected = context.ViewModel.SelectedSecurityTab(newData);
            Assert.IsNotNull(selected);
            Assert.AreEqual(1, selected.TLVM.Valuations.Count);
        }
    }
}
