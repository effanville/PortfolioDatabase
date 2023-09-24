using System.IO.Abstractions.TestingHelpers;

using FinancialStructures.Database;

using FPD.Logic.Tests.TestHelpers;
using FPD.Logic.ViewModels;

using NUnit.Framework;

namespace FPD.Logic.Tests
{
    /// <summary>
    /// Tests for the default data window.
    /// </summary>
    [TestFixture]
    internal class BasicDataViewWindowTests
    {
        /// <summary>
        /// Ensures the window displays data if the underlying database is modified.
        /// </summary>
        [Test]
        public void EmptyPortfolioHasEmptyData()
        {
            var portfolio = PortfolioFactory.GenerateEmpty();
            var viewModelFactory = TestSetupHelper.CreateViewModelFactory(portfolio, new MockFileSystem(), null, null);
            var context = new ViewModelTestContext<IPortfolio, BasicDataViewModel>(
                portfolio,
                null,
                Account.All,
                portfolio,
                viewModelFactory);

            Assert.Multiple(() =>
            {
                Assert.IsFalse(context.ViewModel.HasValues);
                Assert.AreEqual("Unsaved database", context.ViewModel.PortfolioNameText);
                Assert.AreEqual(0, context.ViewModel.Notes.Count);
            });
        }

        /// <summary>
        /// Ensures that the window displays data on loading.
        /// </summary>
        [Test]
        public void CanViewData()
        {
            var portfolio = TestSetupHelper.CreateBasicDataBase();
            var viewModelFactory = TestSetupHelper.CreateViewModelFactory(portfolio, new MockFileSystem(), null, null);
            var context = new ViewModelTestContext<IPortfolio, BasicDataViewModel>(
                portfolio,
                null,
                Account.All,
                portfolio,
                viewModelFactory);
            Assert.Multiple(() =>
            {
                Assert.IsTrue(context.ViewModel.HasValues);
                Assert.AreEqual("TestFilePath", context.ViewModel.PortfolioNameText);
                Assert.AreEqual("Total Securities: 1", context.ViewModel.SecurityTotalText);
                Assert.AreEqual("Total Value: £1.00", context.ViewModel.SecurityAmountText);

                Assert.AreEqual("Total Bank Accounts: 1", context.ViewModel.BankAccountTotalText);
                Assert.AreEqual("Total Value: £1.00", context.ViewModel.BankAccountAmountText);
                Assert.AreEqual(0, context.ViewModel.Notes.Count);
            });
        }

        /// <summary>
        /// Ensures the window displays data if the underlying database is modified.
        /// </summary>
        [Test]
        public void CanUpdateData()
        {
            var portfolio = TestSetupHelper.CreateEmptyDataBase();
            var viewModelFactory = TestSetupHelper.CreateViewModelFactory(portfolio, new MockFileSystem(), null, null);
            var context = new ViewModelTestContext<IPortfolio, BasicDataViewModel>(
                portfolio,
                null,
                Account.All,
                portfolio,
                viewModelFactory);
            Assert.IsFalse(context.ViewModel.HasValues);

            // Now update that data.
            TestSetupHelper.UpdatePortfolio(context.Portfolio);
            context.ViewModel.UpdateData(context.Portfolio);

            // Ensure new data has been displayed correctly.
            Assert.Multiple(() =>
            {

                Assert.IsTrue(context.ViewModel.HasValues);
                Assert.AreEqual("Total Securities: 1", context.ViewModel.SecurityTotalText);
                Assert.AreEqual("Total Value: £1.00", context.ViewModel.SecurityAmountText);

                Assert.AreEqual("Total Bank Accounts: 1", context.ViewModel.BankAccountTotalText);
                Assert.AreEqual("Total Value: £1.00", context.ViewModel.BankAccountAmountText);
            });
        }
    }
}
