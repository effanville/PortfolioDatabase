using System.IO.Abstractions;
using Common.UI;
using Common.UI.Services;
using FPD.Logic.ViewModels.Common;
using FinancialStructures.Database;
using Moq;
using NUnit.Framework;
using Common.Structure.DataEdit;

namespace FPD.Logic.Tests.TestHelpers
{
    public abstract class DataNamesViewTestHelper
    {
        private IUpdater<IPortfolio> _dataUpdater;
        private IPortfolio fPortfolio;

        protected IPortfolio Portfolio
        {
            get => fPortfolio;
            set
            {
                fPortfolio = value;
                ViewModel?.UpdateData(fPortfolio);
                if (_dataUpdater != null)
                {
                    _dataUpdater.Database = fPortfolio;
                }
            }
        }

        protected DataNamesViewModel ViewModel
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

            _dataUpdater = TestSetupHelper.CreateUpdater(Portfolio);
            UiGlobals globals = TestSetupHelper.CreateGlobalsMock(new FileSystem(), fileMock.Object, dialogMock.Object);
            ViewModel = new DataNamesViewModel(Portfolio, TestSetupHelper.DummyReportLogger, null, _dataUpdater, obj => { }, AccountType);
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
