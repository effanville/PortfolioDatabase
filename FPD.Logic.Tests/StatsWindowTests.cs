using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Net;

using FinancialStructures.Database;

using FPD.Logic.Configuration;
using FPD.Logic.Tests.TestHelpers;
using FPD.Logic.ViewModels.Stats;

using NUnit.Framework;

namespace FPD.Logic.Tests
{
    /// <summary>
    /// Tests to ensure that the stats window displays what it should do.
    /// </summary>
    [TestFixture]
    public class StatsWindowTests
    {
        private const int ExpectedNumberTabs = 7;

        /// <summary>
        /// The defaults are loaded correctly.
        /// </summary>
        [Test]
        public void CanLoadWithNames()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();

            var viewModelFactory = TestSetupHelper.CreateViewModelFactory(portfolio, new MockFileSystem(), null, null);
            var context = new ViewModelTestContext<IPortfolio, StatsViewModel>(
                null,
                Account.All,
                new StatsDisplayConfiguration(),
                nameof(StatsViewModel),
                portfolio,
                viewModelFactory);
            context.ViewModel.UpdateData(context.Portfolio);
            Assert.Multiple(() =>
            {
                Assert.AreEqual(ExpectedNumberTabs, context.ViewModel.Stats.Count);
                Assert.AreEqual(true, context.ViewModel.DisplayValueFunds);
            });
        }

        /// <summary>
        /// The defaults are loaded correctly.
        /// </summary>
        [TestCase(false)]
        [TestCase(true)]
        public void CanStoreConfig(bool valueFunds)
        {
            var configuration = new StatsDisplayConfiguration();
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            var viewModelFactory = TestSetupHelper.CreateViewModelFactory(portfolio, new MockFileSystem(), null, null);

            var context = new ViewModelTestContext<IPortfolio, StatsViewModel>(
                null,
                Account.All,
                configuration,
                nameof(StatsViewModel),
                portfolio,
                viewModelFactory);
            context.ViewModel.UpdateData(context.Portfolio);

            Assert.AreEqual(ExpectedNumberTabs, context.ViewModel.Stats.Count);
            Assert.AreEqual(true, context.ViewModel.DisplayValueFunds);

            context.ViewModel.DisplayValueFunds = valueFunds;
            Assert.AreEqual(valueFunds, context.ViewModel.DisplayValueFunds);

            context.ResetViewModel(new StatsViewModel(context.Globals, null, configuration, context.Portfolio));

            context.ViewModel.UpdateData(context.Portfolio);
            Assert.AreEqual(valueFunds, context.ViewModel.DisplayValueFunds);
            Assert.AreEqual(ExpectedNumberTabs, context.ViewModel.Stats.Count);
        }
    }
}