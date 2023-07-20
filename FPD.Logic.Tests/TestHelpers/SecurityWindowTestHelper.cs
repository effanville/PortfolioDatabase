using System.IO.Abstractions;
using System.Linq;
using Common.UI;
using Common.UI.Services;
using FPD.Logic.ViewModels.Common;
using FPD.Logic.ViewModels.Security;
using FinancialStructures.Database;
using FinancialStructures.NamingStructures;
using Moq;
using NUnit.Framework;

namespace FPD.Logic.Tests.TestHelpers
{
    public abstract class SecurityWindowTestHelper
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

        protected SecurityEditWindowViewModel ViewModel
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

        public SelectedSecurityViewModel SelectedViewModel(NameData name)
        {
            return (SelectedSecurityViewModel)ViewModel.Tabs.First(tab => tab is SelectedSecurityViewModel vm && vm.Header == name.ToString());
        }


        [SetUp]
        public void Setup()
        {
            Mock<IFileInteractionService> fileMock = TestSetupHelper.CreateFileMock("nothing");
            Mock<IBaseDialogCreationService> dialogMock = TestSetupHelper.CreateDialogMock();
            Portfolio = TestSetupHelper.CreateEmptyDataBase();

            var dataUpdater = TestSetupHelper.CreateUpdater(Portfolio);
            UiGlobals globals = TestSetupHelper.CreateGlobalsMock(new FileSystem(), fileMock.Object, dialogMock.Object);
            ViewModel = new SecurityEditWindowViewModel(globals, null, Portfolio, "Securities", Account.Security, dataUpdater);
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
