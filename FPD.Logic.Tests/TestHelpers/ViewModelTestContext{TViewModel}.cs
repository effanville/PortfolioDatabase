using System.IO;
using System.IO.Abstractions.TestingHelpers;

using Effanville.Common.Structure.DataEdit;
using Effanville.Common.UI;
using Effanville.Common.UI.Services;
using Effanville.Common.UI.ViewModelBases;

using FinancialStructures.Database;
using FinancialStructures.NamingStructures;

using FPD.Logic.Configuration;
using FPD.Logic.ViewModels;

using Moq;

namespace FPD.Logic.Tests.TestHelpers
{
    internal sealed class ViewModelTestContext<TData, TViewModel>
        where TViewModel : ViewModelBase<TData, IPortfolio>
        where TData : class
    {        
        private MockFileSystem FileSystem { get; set; }
        public UiGlobals Globals { get; private set; }
        public TData Data { get; private set; }

        public IPortfolio Portfolio { get; set; }
        
        public NameData Name { get; set; }

        public TViewModel ViewModel { get; private set; }

        public ViewModelTestContext(
            TData data,
            NameData name,
            Account account,
            IPortfolio dataStore,
            IViewModelFactory viewModelFactory)
        {
            Data = data;
            FileSystem = new MockFileSystem();
            Mock<IFileInteractionService> fileMock = TestSetupHelper.CreateFileMock("nothing");
            Mock<IBaseDialogCreationService> dialogMock = TestSetupHelper.CreateDialogMock();
            Portfolio = dataStore;
            Name = name;

            IUpdater<IPortfolio> dataUpdater = TestSetupHelper.CreateUpdater(Portfolio);
            Globals = TestSetupHelper.CreateGlobalsMock(FileSystem, fileMock.Object, dialogMock.Object);
            ViewModel = viewModelFactory.GenerateViewModel(data, Name, account, dataStore) as TViewModel;
            if(ViewModel != null)
            {
                ViewModel.UpdateRequest += dataUpdater.PerformUpdate;
            }
        }
        
        public ViewModelTestContext(
            NameData name,
            Account account,
            IConfiguration configuration,
            string viewModelType,
            IPortfolio dataStore,
            IViewModelFactory viewModelFactory)
        {
            Data = null;
            FileSystem = new MockFileSystem();
            Mock<IFileInteractionService> fileMock = TestSetupHelper.CreateFileMock("nothing");
            Mock<IBaseDialogCreationService> dialogMock = TestSetupHelper.CreateDialogMock();
            Portfolio = dataStore;
            Name = name;

            IUpdater<IPortfolio> dataUpdater = TestSetupHelper.CreateUpdater(Portfolio);
            Globals = TestSetupHelper.CreateGlobalsMock(FileSystem, fileMock.Object, dialogMock.Object);
            ViewModel = viewModelFactory.GenerateViewModel(dataStore, Name, account, configuration, viewModelType) as TViewModel;
            if(ViewModel != null)
            {
                ViewModel.UpdateRequest += dataUpdater.PerformUpdate;
            }
        }
        
        public void ResetViewModel(TViewModel newViewModel) => ViewModel = newViewModel;

        public void AddFileToFileSystem(string realLocation, string testLocation)
        {
            string file = File.ReadAllText(realLocation);
            FileSystem.AddFile(testLocation, new MockFileData(file));
        }
    }
}
