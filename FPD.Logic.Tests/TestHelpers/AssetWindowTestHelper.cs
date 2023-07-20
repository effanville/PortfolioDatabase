using System.IO.Abstractions;
using System.Linq;
using Common.UI;
using Common.UI.Services;
using FPD.Logic.ViewModels.Common;
using FPD.Logic.ViewModels.Asset;
using FinancialStructures.Database;
using FinancialStructures.NamingStructures;
using Moq;
using NUnit.Framework;

namespace FPD.Logic.Tests.TestHelpers
{
    public abstract class AssetWindowTestHelper
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

        protected AssetEditWindowViewModel ViewModel
        {
            get;
            private set;
        }

        private DataNamesViewModel fDataNames;

        public DataNamesViewModel DataNames
        {
            get
            {
                fDataNames ??= (DataNamesViewModel)ViewModel.Tabs.First(tab => tab is DataNamesViewModel);
                return fDataNames;
            }
        }

        public SelectedAssetViewModel SelectedViewModel(NameData name)
        {
            return (SelectedAssetViewModel)ViewModel.Tabs.First(tab => tab is SelectedAssetViewModel vm && vm.Header == name.ToString());
        }


        [SetUp]
        public void Setup()
        {
            Mock<IFileInteractionService> fileMock = TestSetupHelper.CreateFileMock("nothing");
            Mock<IBaseDialogCreationService> dialogMock = TestSetupHelper.CreateDialogMock();
            Portfolio = TestSetupHelper.CreateEmptyDataBase();

            UiGlobals globals = TestSetupHelper.CreateGlobalsMock(new FileSystem(), fileMock.Object, dialogMock.Object);

            var dataUpdater = TestSetupHelper.CreateUpdater(Portfolio);
            ViewModel = new AssetEditWindowViewModel(globals, null, Portfolio, dataUpdater);
        }

        [TearDown]
        public void TearDown()
        {
            ViewModel = null;
            fDataNames = null;
            Portfolio.Clear();
        }
    }
}
