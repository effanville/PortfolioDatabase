using System;
using System.IO.Abstractions;
using Common.UI;
using Common.UI.Services;
using FPD.Logic.ViewModels.Asset;
using FinancialStructures.Database;
using FinancialStructures.NamingStructures;
using Moq;
using NUnit.Framework;

namespace FPD.Logic.Tests.TestHelpers
{
    public abstract class SelectedAssetTestHelper
    {
        private Action<Action<IPortfolio>> DataUpdater => action => action(Portfolio);

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

            UiGlobals globals = TestSetupHelper.CreateGlobalsMock(new FileSystem(), fileMock.Object, dialogMock.Object);
            ViewModel = new SelectedAssetViewModel(Portfolio, DataUpdater, TestSetupHelper.DummyReportLogger, null, globals, Name);
        }

        [TearDown]
        public void TearDown()
        {
            ViewModel = null;
            Portfolio.Clear();
        }
    }
}
