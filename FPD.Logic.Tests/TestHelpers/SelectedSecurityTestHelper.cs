using System.IO.Abstractions;
using Common.UI;
using Common.UI.Services;
using FPD.Logic.ViewModels.Security;
using FinancialStructures.Database;
using FinancialStructures.NamingStructures;
using Moq;
using NUnit.Framework;
using Common.Structure.DataEdit;

namespace FPD.Logic.Tests.TestHelpers
{
    public abstract class SelectedSecurityTestHelper
    {
        private IUpdater<IPortfolio> _dataUpdater;
        protected IPortfolio Portfolio
        {
            get;
            set;
        }

        protected NameData Name
        {
            get;
            set;
        }

        protected SelectedSecurityViewModel ViewModel
        {
            get;
            private set;
        }

        [SetUp]
        public void Setup()
        {
            Mock<IFileInteractionService> fileMock = TestSetupHelper.CreateFileMock("nothing");
            Mock<IBaseDialogCreationService> dialogMock = TestSetupHelper.CreateDialogMock();
            Portfolio = TestSetupHelper.CreateBasicDataBase();
            Name = new NameData("Fidelity", "China");

            _dataUpdater = TestSetupHelper.CreateUpdater(Portfolio);
            UiGlobals globals = TestSetupHelper.CreateGlobalsMock(new FileSystem(), fileMock.Object, dialogMock.Object);
            ViewModel = new SelectedSecurityViewModel(Portfolio, TestSetupHelper.DummyReportLogger, null, globals, Name, Account.Security);
            ViewModel.UpdateRequest += _dataUpdater.PerformUpdate;
        }

        [TearDown]
        public void TearDown()
        {
            ViewModel = null;
            Portfolio.Clear();
        }
    }
}
