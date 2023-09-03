using FPD.Logic.ViewModels.Common;
using FPD.Logic.Tests.TestHelpers;
using FinancialStructures.Database;
using FinancialStructures.NamingStructures;
using NUnit.Framework;
using Common.Structure.DataEdit;
using Common.UI;
using System;
using FPD.Logic.Tests.UserInteractions;

namespace FPD.Logic.Tests.CommonWindowTests
{
    /// <summary>
    /// Tests for window displaying single data stream data.
    /// </summary>
    [TestFixture]
    public class ValueListWindowViewModelTests
    {
        private readonly Func<UiGlobals, IPortfolio, NameData, IUpdater<IPortfolio>, ValueListWindowViewModel> _viewModelFactory
            = (globals, portfolio, name, dataUpdater) => new ValueListWindowViewModel(
                globals,
                null,
                portfolio,
                "Title",
                Account.Security,
                dataUpdater,
                (dataStore,
                    uiStyles,
                    fUiGlobals, 
                    selectedName, 
                    accountType,
                    updater) => new SelectedSingleDataViewModel(
                    dataStore,
                    uiStyles,
                    fUiGlobals, 
                    selectedName, 
                    accountType,
                    updater));

        [Test]
        public void CanLoadSuccessfully()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            var context = new ViewModelTestContext<ViewModels.Common.ValueListWindowViewModel>(
                new NameData("Fidelity", "China"),
                portfolio,
                _viewModelFactory);
            Assert.AreEqual(1, context.ViewModel.Tabs.Count);
            DataNamesViewModel nameModel = context.ViewModel.GetDataNamesViewModel();
            Assert.AreEqual(1, nameModel.DataNames.Count);
        }

        [Test]
        public void CanUpdateData()
        {
            var portfolio = TestSetupHelper.CreateEmptyDataBase();
            var context = new ViewModelTestContext<ViewModels.Common.ValueListWindowViewModel>(
                new NameData("Fidelity", "China"),
                portfolio,
                _viewModelFactory);
            IPortfolio newData = TestSetupHelper.CreateBasicDataBase();
            context.ViewModel.UpdateData(newData);

            Assert.AreEqual("TestFilePath", context.ViewModel.ModelData.Name);
            Assert.AreEqual(1, context.ViewModel.ModelData.BankAccountsThreadSafe.Count);
        }

        [Test]
        public void CanUpdateDataAndRemoveOldTab()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            var context = new ViewModelTestContext<ViewModels.Common.ValueListWindowViewModel>(
                new NameData("Fidelity", "China"),
                portfolio,
                _viewModelFactory);
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
            var context = new ViewModelTestContext<ViewModels.Common.ValueListWindowViewModel>(
                new NameData("Fidelity", "China"),
                portfolio,
                _viewModelFactory);
            NameData newData = new NameData("Fidelity", "China");
            context.ViewModel.LoadTabFunc(newData);

            Assert.AreEqual(2, context.ViewModel.Tabs.Count);
        }
    }
}
