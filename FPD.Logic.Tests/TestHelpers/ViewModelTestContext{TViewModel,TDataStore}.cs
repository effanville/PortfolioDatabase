using System;
using System.IO;
using System.IO.Abstractions.TestingHelpers;

using Common.Structure.DataEdit;
using Common.UI;
using Common.UI.Services;
using Common.UI.ViewModelBases;

using FinancialStructures.NamingStructures;

using Moq;

namespace FPD.Logic.Tests.TestHelpers
{
    internal class ViewModelTestContext<TData, TViewModel, TDataStore>
        where TViewModel : ViewModelBase<TData, TDataStore>
        where TDataStore : class
        where TData : class
    {
        private readonly IUpdater<TDataStore> _dataUpdater;

        protected MockFileSystem FileSystem { get; set; }
        public UiGlobals Globals
        {
            get;
            private set;
        }

        public TDataStore DataStore
        {
            get;
            set;
        }
        public NameData Name
        {
            get;
            set;
        }

        public TViewModel ViewModel
        {
            get;
            private set;
        }

        public ViewModelTestContext(
            TData data,
            NameData name,
            TDataStore dataStore,
            Func<TData, UiGlobals, TDataStore, NameData, IUpdater<TDataStore>, TViewModel> vmGenerator)
        {
            FileSystem = new MockFileSystem();
            Mock<IFileInteractionService> fileMock = TestSetupHelper.CreateFileMock("nothing");
            Mock<IBaseDialogCreationService> dialogMock = TestSetupHelper.CreateDialogMock();
            DataStore = dataStore;
            Name = name;

            _dataUpdater = TestSetupHelper.CreateUpdater(DataStore);
            Globals = TestSetupHelper.CreateGlobalsMock(FileSystem, fileMock.Object, dialogMock.Object);
            ViewModel = vmGenerator(data, Globals, DataStore, Name, _dataUpdater);
            ViewModel.UpdateRequest += _dataUpdater.PerformUpdate;
        }

        public void ResetViewModel(TViewModel newViewModel) => ViewModel = newViewModel;

        public void AddFileToFileSystem(string realLocation, string testLocation)
        {
            string file = File.ReadAllText(realLocation);
            FileSystem.AddFile(testLocation, new MockFileData(file));
        }
    }
}
