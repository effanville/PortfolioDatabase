﻿using System.Linq;
using FPD.Logic.ViewModels.Common;
using FPD.Logic.ViewModels.Asset;
using FPD.Logic.Tests.TestHelpers;
using FinancialStructures.Database;
using FinancialStructures.NamingStructures;
using NUnit.Framework;

namespace FPD.Logic.Tests.AssetWindowTests
{
    /// <summary>
    /// Tests for window displaying security data.
    /// </summary>
    [TestFixture]
    public class AssetEditWindowTests : AssetWindowTestHelper
    {
        [Test]
        public void CanLoadSuccessfully()
        {
            Portfolio = TestSetupHelper.CreateBasicDataBase();
            Assert.AreEqual(1, ViewModel.Tabs.Count);
            object tab = ViewModel.Tabs.Single();
            DataNamesViewModel nameModel = tab as DataNamesViewModel;
            Assert.AreEqual(1, nameModel.DataNames.Count);
        }

        [Test]
        public void CanUpdateData()
        {
            IPortfolio newData = TestSetupHelper.CreateBasicDataBase();
            ViewModel.UpdateData(newData);

            Assert.AreEqual("TestFilePath", ViewModel.DataStore.Name);
            Assert.AreEqual(1, ViewModel.DataStore.FundsThreadSafe.Count);
        }

        [Test]
        public void CanUpdateDataAndRemoveOldTab()
        {
            NameData newNameData = new NameData("Fidelity", "Europe");
            ViewModel.LoadTabFunc(newNameData);

            Assert.AreEqual(2, ViewModel.Tabs.Count);

            IPortfolio newData = TestSetupHelper.CreateBasicDataBase();
            ViewModel.UpdateData(newData);
            Assert.AreEqual(1, ViewModel.Tabs.Count);
            Assert.AreEqual("TestFilePath", ViewModel.DataStore.Name);
            Assert.AreEqual(1, ViewModel.DataStore.FundsThreadSafe.Count);
        }

        [Test]
        public void CanAddTab()
        {
            Portfolio = TestSetupHelper.CreateBasicDataBase();

            NameData newData = new NameData("House", "MyHouse");
            ViewModel.LoadTabFunc(newData);

            Assert.AreEqual(2, ViewModel.Tabs.Count);
            DataNamesViewModel dataNames = DataNames;
            Assert.AreEqual(1, dataNames.DataNames.Count);
            SelectedAssetViewModel selected = SelectedViewModel(newData);
            Assert.IsNotNull(selected);
            Assert.AreEqual(1, selected.ValuesTLVM.Valuations.Count);
        }
    }
}
