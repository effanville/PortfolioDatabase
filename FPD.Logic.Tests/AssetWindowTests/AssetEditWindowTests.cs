using System;
using System.Linq;

using Common.Structure.DataEdit;
using Common.UI;

using FinancialStructures.Database;
using FinancialStructures.NamingStructures;

using FPD.Logic.Tests.TestHelpers;
using FPD.Logic.Tests.UserInteractions;
using FPD.Logic.ViewModels.Asset;
using FPD.Logic.ViewModels.Common;

using NUnit.Framework;

namespace FPD.Logic.Tests.AssetWindowTests
{
    /// <summary>
    /// Tests for window displaying security data.
    /// </summary>
    [TestFixture]
    public class AssetEditWindowTests
    {
        private readonly Func<UiGlobals, IPortfolio, NameData, IUpdater<IPortfolio>, ValueListWindowViewModel> _viewModelFactory
            = (globals, portfolio, name, dataUpdater) => new ValueListWindowViewModel(
                globals,
                null,
                portfolio,
                "Asset",
                Account.Asset,
                dataUpdater,
                (dataStore, uiStyles, uiGlobals, selectedName, accountType, updater) => new SelectedAssetViewModel(dataStore, uiStyles, uiGlobals, selectedName, accountType, updater));
        [Test]
        public void CanLoadSuccessfully()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            var context = new ViewModelTestContext<ValueListWindowViewModel>(
                null,
                portfolio,
                _viewModelFactory);
            Assert.AreEqual(1, context.ViewModel.Tabs.Count);
            object tab = context.ViewModel.Tabs.Single();
            DataNamesViewModel nameModel = tab as DataNamesViewModel;
            Assert.AreEqual(1, nameModel.DataNames.Count);
        }

        [Test]
        public void CanUpdateData()
        {
            var portfolio = TestSetupHelper.CreateEmptyDataBase();
            var context = new ViewModelTestContext<ValueListWindowViewModel>(
                null,
                portfolio,
                _viewModelFactory);
            IPortfolio newData = TestSetupHelper.CreateBasicDataBase();
            context.ViewModel.UpdateData(newData);

            Assert.AreEqual("TestFilePath", context.ViewModel.DataStore.Name);
            Assert.AreEqual(1, context.ViewModel.DataStore.FundsThreadSafe.Count);
        }

        [Test]
        public void CanUpdateDataAndRemoveOldTab()
        {
            var portfolio = TestSetupHelper.CreateEmptyDataBase();
            var context = new ViewModelTestContext<ValueListWindowViewModel>(
                null,
                portfolio,
                _viewModelFactory);
            NameData newNameData = new NameData("Fidelity", "Europe");
            context.ViewModel.LoadTabFunc(newNameData);

            Assert.AreEqual(2, context.ViewModel.Tabs.Count);

            IPortfolio newData = TestSetupHelper.CreateBasicDataBase();
            context.ViewModel.UpdateData(newData);
            Assert.AreEqual(1, context.ViewModel.Tabs.Count);
            Assert.AreEqual("TestFilePath", context.ViewModel.DataStore.Name);
            Assert.AreEqual(1, context.ViewModel.DataStore.FundsThreadSafe.Count);
        }

        [Test]
        public void CanAddTab()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            var context = new ViewModelTestContext<ValueListWindowViewModel>(
                null,
                portfolio,
                _viewModelFactory);

            NameData newData = new NameData("House", "MyHouse");
            context.ViewModel.LoadTabFunc(newData);

            Assert.AreEqual(2, context.ViewModel.Tabs.Count);
            DataNamesViewModel dataNames = context.ViewModel.GetDataNamesViewModel();
            Assert.AreEqual(1, dataNames.DataNames.Count);
            SelectedAssetViewModel selected = context.ViewModel.SelectedAssetTab(newData);
            Assert.IsNotNull(selected);
            Assert.AreEqual(1, selected.ValuesTLVM.Valuations.Count);
        }
    }
}
