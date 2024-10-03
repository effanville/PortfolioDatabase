using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;

using Effanville.FinancialStructures.Database;
using Effanville.FPD.Logic.Configuration;
using Effanville.FPD.Logic.Tests.TestHelpers;
using Effanville.FPD.Logic.ViewModels;
using Effanville.FPD.Logic.ViewModels.Stats;

using NUnit.Framework;

namespace Effanville.FPD.Logic.Tests
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
        public async Task CanLoadWithNames()
        {
            IPortfolio portfolio = TestSetupHelper.CreateBasicDataBase();

            IViewModelFactory viewModelFactory = TestSetupHelper.CreateViewModelFactory(portfolio, new MockFileSystem(), null, null);
            ViewModelTestContext<IPortfolio, StatsViewModel> context = new ViewModelTestContext<IPortfolio, StatsViewModel>(
                null,
                Account.All,
                new StatsDisplayConfiguration(),
                nameof(StatsViewModel),
                portfolio,
                viewModelFactory);
            context.ViewModel.UpdateData(context.Portfolio, false);

            await Task.Delay(3000);
            
            Assert.Multiple(() =>
            {
                Assert.That(context.ViewModel.Stats, Has.Count.EqualTo(ExpectedNumberTabs));
                Assert.That(context.ViewModel.DisplayValueFunds, Is.EqualTo(true));
            });
        }

        /// <summary>
        /// The defaults are loaded correctly.
        /// </summary>
        [TestCase(false)]
        [TestCase(true)]
        public async Task CanStoreConfig(bool valueFunds)
        {
            StatsDisplayConfiguration configuration = new StatsDisplayConfiguration();
            IPortfolio portfolio = TestSetupHelper.CreateBasicDataBase();
            IViewModelFactory viewModelFactory = TestSetupHelper.CreateViewModelFactory(portfolio, new MockFileSystem(), null, null);

            ViewModelTestContext<IPortfolio, StatsViewModel> context = new ViewModelTestContext<IPortfolio, StatsViewModel>(
                null,
                Account.All,
                configuration,
                nameof(StatsViewModel),
                portfolio,
                viewModelFactory);
            context.ViewModel.UpdateData(context.Portfolio, false);

            await Task.Delay(3000);
            Assert.Multiple(() =>
            {
                Assert.That(context.ViewModel.Stats, Has.Count.EqualTo(ExpectedNumberTabs));
                Assert.That(context.ViewModel.DisplayValueFunds, Is.EqualTo(true));
            });
            context.ViewModel.DisplayValueFunds = valueFunds;
            Assert.That(context.ViewModel.DisplayValueFunds, Is.EqualTo(valueFunds));

            context.ResetViewModel(new StatsViewModel(context.Globals, null, configuration, context.Portfolio));

            context.ViewModel.UpdateData(context.Portfolio, false);
            await Task.Delay(3000);
            Assert.Multiple(() =>
            {
                Assert.That(context.ViewModel.DisplayValueFunds, Is.EqualTo(valueFunds));
                Assert.That(context.ViewModel.Stats, Has.Count.EqualTo(ExpectedNumberTabs));
            });
        }
    }
}