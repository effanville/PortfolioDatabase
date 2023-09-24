using FPD.Logic.ViewModels.Common;
using FPD.Logic.Tests.TestHelpers;
using FinancialStructures.Database;
using FinancialStructures.NamingStructures;
using NUnit.Framework;
using System.IO.Abstractions.TestingHelpers;

using FPD.Logic.Tests.UserInteractions;

namespace FPD.Logic.Tests.CommonWindowTests
{
    /// <summary>
    /// Tests for window displaying single data stream data.
    /// </summary>
    [TestFixture]
    public class ValueListWindowViewModelTests
    {
        [Test]
        public void CanLoadSuccessfully()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            var viewModelFactory = TestSetupHelper.CreateViewModelFactory(portfolio, new MockFileSystem(), null, null);
            var context = new ViewModelTestContext<IPortfolio, ValueListWindowViewModel>(
                portfolio,
                new NameData("Fidelity", "China"),
                Account.BankAccount,
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
                new NameData("Fidelity", "China"),
                Account.BankAccount,
                portfolio,
                viewModelFactory);
            IPortfolio newData = TestSetupHelper.CreateBasicDataBase();
            context.ViewModel.UpdateData(newData);

            Assert.AreEqual("TestFilePath", context.ViewModel.ModelData.Name);
            Assert.AreEqual(1, context.ViewModel.ModelData.BankAccountsThreadSafe.Count);
        }

        [Test]
        public void CanUpdateDataAndRemoveOldTab()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            var viewModelFactory = TestSetupHelper.CreateViewModelFactory(portfolio, new MockFileSystem(), null, null);

            var context = new ViewModelTestContext<IPortfolio, ValueListWindowViewModel>(
                portfolio,
                new NameData("Fidelity", "China"),
                Account.BankAccount,
                portfolio,
                viewModelFactory);
            NameData newNameData = new NameData("Fidelity", "Europe");
            context.ViewModel.LoadTabFunc(newNameData);

            Assert.AreEqual(2, context.ViewModel.Tabs.Count);

            IPortfolio newData = TestSetupHelper.CreateBasicDataBase();
            context.ViewModel.UpdateData(newData);
            Assert.AreEqual(1, context.ViewModel.Tabs.Count);
            Assert.AreEqual("TestFilePath", context.ViewModel.ModelData.Name);
            Assert.AreEqual(1, context.ViewModel.ModelData.BankAccountsThreadSafe.Count);
        }

        [Test]
        public void CanAddTab()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            var viewModelFactory = TestSetupHelper.CreateViewModelFactory(portfolio, new MockFileSystem(), null, null);

            var context = new ViewModelTestContext<IPortfolio, ValueListWindowViewModel>(
                portfolio,
                new NameData("Fidelity", "China"),
                Account.BankAccount,
                portfolio,
                viewModelFactory);
            NameData newData = new NameData("Fidelity", "China");
            context.ViewModel.LoadTabFunc(newData);

            Assert.AreEqual(2, context.ViewModel.Tabs.Count);
        }
    }
}
