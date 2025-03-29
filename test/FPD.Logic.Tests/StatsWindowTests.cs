using System.IO.Abstractions.TestingHelpers;
using System.Threading;
using System.Threading.Tasks;

using Effanville.Common.Structure.DataEdit;
using Effanville.Common.UI;
using Effanville.Common.UI.Services;
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
        [RequiresThread(ApartmentState.STA)]
        public async Task CanLoadWithNames()
        {
            IPortfolio portfolio = TestSetupHelper.CreateBasicDataBase();
            var fileSystem = new MockFileSystem();
            IFileInteractionService fileMock = TestSetupHelper.CreateFileMock("nothing");
            IBaseDialogCreationService dialogMock = TestSetupHelper.CreateDialogMock();

            IUpdater updater = TestSetupHelper.SetupUpdater();
            UiGlobals globals = TestSetupHelper.SetupGlobalsMock(fileSystem, fileMock, dialogMock);
            IViewModelFactory viewModelFactory = TestSetupHelper.SetupViewModelFactory(
                null,
                globals,
                updater,
                null,
                new UserConfiguration(),
                new StatisticsProvider(portfolio));

            StatsViewModel viewModel = viewModelFactory.GenerateViewModel(portfolio, "", Account.All, nameof(StatsViewModel)) as StatsViewModel;
            viewModel.UpdateData(portfolio, false);

            await Task.Delay(3000);

            Assert.Multiple(() =>
            {
                Assert.That(viewModel.Stats, Has.Count.EqualTo(ExpectedNumberTabs));
                Assert.That(viewModel.DisplayValueFunds, Is.EqualTo(true));
            });
        }

        /// <summary>
        /// The defaults are loaded correctly.
        /// </summary>
        [TestCase(false)]
        [TestCase(true)]
        public async Task CanStoreConfig(bool valueFunds)
        {
            UserConfiguration configuration = new UserConfiguration();
            IPortfolio portfolio = TestSetupHelper.CreateBasicDataBase();

            var fileSystem = new MockFileSystem();
            IFileInteractionService fileMock = TestSetupHelper.CreateFileMock("nothing");
            IBaseDialogCreationService dialogMock = TestSetupHelper.CreateDialogMock();

            IUpdater updater = TestSetupHelper.SetupUpdater();
            UiGlobals globals = TestSetupHelper.SetupGlobalsMock(fileSystem, fileMock, dialogMock);
            IViewModelFactory viewModelFactory = TestSetupHelper.SetupViewModelFactory(
                null,
                globals,
                updater,
                null,
                configuration,
                new StatisticsProvider(portfolio));

            StatsViewModel viewModel = viewModelFactory.GenerateViewModel(portfolio, "", Account.All, nameof(StatsViewModel)) as StatsViewModel;
            viewModel.UpdateData(portfolio, false);

            await Task.Delay(3000);
            Assert.Multiple(() =>
            {
                Assert.That(viewModel.Stats, Has.Count.EqualTo(ExpectedNumberTabs));
                Assert.That(viewModel.DisplayValueFunds, Is.EqualTo(true));
            });
            viewModel.DisplayValueFunds = valueFunds;
            Assert.That(viewModel.DisplayValueFunds, Is.EqualTo(valueFunds));

            viewModel = viewModelFactory.GenerateViewModel(portfolio, "", Account.All, nameof(StatsViewModel)) as StatsViewModel;

            viewModel.UpdateData(portfolio, false);
            await Task.Delay(3000);
            Assert.Multiple(() =>
            {
                Assert.That(viewModel.DisplayValueFunds, Is.EqualTo(valueFunds));
                Assert.That(viewModel.Stats, Has.Count.EqualTo(ExpectedNumberTabs));
            });
        }
    }
}