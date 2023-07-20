using System.IO.Abstractions;
using Common.UI;
using Common.UI.Services;
using FPD.Logic.ViewModels.Asset;
using FinancialStructures.Database;
using FinancialStructures.NamingStructures;
using Moq;
using NUnit.Framework;
using Common.Structure.DataEdit;

namespace FPD.Logic.Tests.TestHelpers
{
    public abstract class SelectedAssetTestHelper
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

        protected SelectedAssetViewModel ViewModel
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
            Name = new NameData("House", "MyHouse");

            _dataUpdater = TestSetupHelper.CreateUpdater(Portfolio);
            UiGlobals globals = TestSetupHelper.CreateGlobalsMock(new FileSystem(), fileMock.Object, dialogMock.Object);
            ViewModel = new SelectedAssetViewModel(Portfolio, TestSetupHelper.DummyReportLogger, null, globals, Name);
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
