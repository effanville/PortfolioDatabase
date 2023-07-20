using System.IO.Abstractions;
using Common.UI;
using Common.UI.Services;
using FPD.Logic.ViewModels.Common;
using FinancialStructures.Database;
using Moq;
using NUnit.Framework;

namespace FPD.Logic.Tests.TestHelpers
{
    public abstract class ValueListWindowViewModelTestHelper
    {
        private IPortfolio fPortfolio;

        protected IPortfolio Portfolio
        {
            get => fPortfolio;
            set
            {
                fPortfolio = value;
                ViewModel?.UpdateData(fPortfolio);
            }
        }

        protected ValueListWindowViewModel ViewModel
        {
            get;
            private set;
        }

        protected Account AccountType
        {
            get;
            set;
        } = Account.Security;

        [SetUp]
        public void Setup()
        {
            Mock<IFileInteractionService> fileMock = TestSetupHelper.CreateFileMock("nothing");
            Mock<IBaseDialogCreationService> dialogMock = TestSetupHelper.CreateDialogMock();
            Portfolio = TestSetupHelper.CreateEmptyDataBase();

            var dataUpdater = TestSetupHelper.CreateUpdater(Portfolio);
            UiGlobals globals = TestSetupHelper.CreateGlobalsMock(new FileSystem(), fileMock.Object, dialogMock.Object, TestSetupHelper.DummyReportLogger);
            ViewModel = new ValueListWindowViewModel(globals, null, Portfolio, "Title", AccountType, dataUpdater);
        }

        [TearDown]
        public void TearDown()
        {
            ViewModel = null;
            Portfolio.Clear();
        }
    }
}
