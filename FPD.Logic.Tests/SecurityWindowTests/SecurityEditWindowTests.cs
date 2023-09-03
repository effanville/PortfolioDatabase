using System;

using Common.Structure.DataEdit;
using Common.UI;

using FinancialStructures.Database;
using FinancialStructures.NamingStructures;

using FPD.Logic.Tests.TestHelpers;
using FPD.Logic.Tests.UserInteractions;
using FPD.Logic.ViewModels.Common;
using FPD.Logic.ViewModels.Security;

using NUnit.Framework;

namespace FPD.Logic.Tests.SecurityWindowTests
{
    /// <summary>
    /// Tests for window displaying security data.
    /// </summary>
    [TestFixture]
    public class SecurityEditWindowTests
    {
        private readonly Func<UiGlobals, IPortfolio, NameData, IUpdater<IPortfolio>, ValueListWindowViewModel> _viewModelFactory
            = (globals, portfolio, name, dataUpdater) => new ValueListWindowViewModel(globals, null, portfolio, "Securities", Account.Security, dataUpdater, (dataStore,
                uiStyles,
                fUiGlobals, 
                selectedName, 
                accountType,
                updater) => new SelectedSecurityViewModel(
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
            var context = new ViewModelTestContext<ValueListWindowViewModel>(
                null,
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
            var context = new ViewModelTestContext<ValueListWindowViewModel>(
                null,
                portfolio,
                _viewModelFactory);
            IPortfolio newData = TestSetupHelper.CreateBasicDataBase();
            context.ViewModel.UpdateData(newData);

            Assert.AreEqual("TestFilePath", context.ViewModel.ModelData.Name);
            Assert.AreEqual(1, context.ViewModel.ModelData.FundsThreadSafe.Count);
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
            Assert.AreEqual("TestFilePath", context.ViewModel.ModelData.Name);
            Assert.AreEqual(1, context.ViewModel.ModelData.FundsThreadSafe.Count);
        }

        [Test]
        public void CanAddTab()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            var context = new ViewModelTestContext<ValueListWindowViewModel>(
                null,
                portfolio,
                _viewModelFactory);

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
