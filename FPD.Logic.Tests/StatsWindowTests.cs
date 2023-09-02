using System;

using Common.Structure.DataEdit;
using Common.UI;

using FinancialStructures.Database;
using FinancialStructures.NamingStructures;

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
        private readonly Func<UiGlobals, IPortfolio, NameData, IUpdater<IPortfolio>, IConfiguration, StatsViewModel> _viewModelFactory
            = (globals, portfolio, name, dataUpdater, config) => new StatsViewModel(
                globals,
                null,
                config,
                portfolio,
                Account.All);

        private const int fExpectedNumberTabs = 7;

        /// <summary>
        /// The defaults are loaded correctly.
        /// </summary>
        [Test]
        public void CanLoadWithNames()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            var context = new ViewModelTestContext<StatsViewModel>(
            null,
            portfolio,
            (globals, portfolio, name, dataUpdater) => _viewModelFactory(globals, portfolio, name, dataUpdater, new StatsDisplayConfiguration()));
            Assert.Multiple(() =>
            {
                Assert.AreEqual(fExpectedNumberTabs, context.ViewModel.Stats.Count);
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
            var context = new ViewModelTestContext<StatsViewModel>(
            null,
            portfolio,
            (globals, portfolio, name, dataUpdater) => _viewModelFactory(globals, portfolio, name, dataUpdater, configuration));

            Assert.AreEqual(fExpectedNumberTabs, context.ViewModel.Stats.Count);
            Assert.AreEqual(true, context.ViewModel.DisplayValueFunds);

            context.ViewModel.DisplayValueFunds = valueFunds;
            Assert.AreEqual(valueFunds, context.ViewModel.DisplayValueFunds);

            context.ResetViewModel(new StatsViewModel(context.Globals, null, configuration, context.Portfolio));

            Assert.AreEqual(valueFunds, context.ViewModel.DisplayValueFunds);
            Assert.AreEqual(fExpectedNumberTabs, context.ViewModel.Stats.Count);
        }
    }
}
