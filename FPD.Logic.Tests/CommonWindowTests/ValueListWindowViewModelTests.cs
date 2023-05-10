using FPD.Logic.ViewModels.Common;
using FPD.Logic.Tests.TestHelpers;
using FPD.Logic.Tests.ViewModelExtensions;
using FinancialStructures.Database;
using FinancialStructures.NamingStructures;
using NUnit.Framework;

namespace FPD.Logic.Tests.CommonWindowTests
{
    /// <summary>
    /// Tests for window displaying single data stream data.
    /// </summary>
    [TestFixture]
    public class ValueListWindowViewModelTests : ValueListWindowViewModelTestHelper
    {
        [Test]
        public void CanLoadSuccessfully()
        {
            Portfolio = TestSetupHelper.CreateBasicDataBase();
            Assert.AreEqual(1, ViewModel.Tabs.Count);
            DataNamesViewModel nameModel = ViewModel.DataNames();
            Assert.AreEqual(1, nameModel.DataNames.Count);
        }

        [Test]
        public void CanUpdateData()
        {
            IPortfolio newData = TestSetupHelper.CreateBasicDataBase();
            ViewModel.UpdateData(newData);

            Assert.AreEqual("TestFilePath", ViewModel.DataStore.Name);
            Assert.AreEqual(1, ViewModel.DataStore.BankAccountsThreadSafe.Count);
        }

        [Test]
        public void CanUpdateDataAndRemoveOldTab()
        {
            Portfolio = TestSetupHelper.CreateBasicDataBase();
            NameData newNameData = new NameData("Fidelity", "Europe");
            ViewModel.LoadTabFunc(newNameData);

            Assert.AreEqual(2, ViewModel.Tabs.Count);

            IPortfolio newData = TestSetupHelper.CreateBasicDataBase();
            ViewModel.UpdateData(newData);
            Assert.AreEqual(1, ViewModel.Tabs.Count);
            Assert.AreEqual("TestFilePath", ViewModel.DataStore.Name);
            Assert.AreEqual(1, ViewModel.DataStore.BankAccountsThreadSafe.Count);
        }

        [Test]
        public void CanAddTab()
        {
            Portfolio = TestSetupHelper.CreateBasicDataBase();
            NameData newData = new NameData("Fidelity", "China");
            ViewModel.LoadTabFunc(newData);

            Assert.AreEqual(2, ViewModel.Tabs.Count);
        }
    }
}
