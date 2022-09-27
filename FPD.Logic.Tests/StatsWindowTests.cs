using FPD.Logic.ViewModels.Stats;
using FPD.Logic.Tests.TestHelpers;
using NUnit.Framework;

namespace FPD.Logic.Tests
{
    /// <summary>
    /// Tests to ensure that the stats window displays what it should do.
    /// </summary>
    [TestFixture]
    public class StatsWindowTests : StatsWindowTestHelper
    {
        /// <summary>
        /// The defaults are loaded correctly.
        /// </summary>
        [Test]
        public void CanLoadWithNames()
        {
            Assert.Multiple(() =>
            {
                Assert.AreEqual(6, ViewModel.Stats.Count);
                Assert.AreEqual(true, ViewModel.DisplayValueFunds);
            });
        }

        /// <summary>
        /// The defaults are loaded correctly.
        /// </summary>
        [TestCase(false)]
        [TestCase(true)]
        public void CanStoreConfig(bool valueFunds)
        {
            Assert.AreEqual(6, ViewModel.Stats.Count);
            Assert.AreEqual(true, ViewModel.DisplayValueFunds);

            ViewModel.DisplayValueFunds = valueFunds;
            Assert.AreEqual(valueFunds, ViewModel.DisplayValueFunds);

            ViewModel = new StatsViewModel(Globals, null, VMConfiguration, Portfolio);

            Assert.AreEqual(valueFunds, ViewModel.DisplayValueFunds);
            Assert.AreEqual(6, ViewModel.Stats.Count);
        }
    }
}
