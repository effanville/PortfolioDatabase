using System.Windows.Input;
using GlobalHeldData;
using System;
using System.Windows;
using System.IO;
using System.Windows.Forms;
using GUIAccessorFunctions;
using FinanceWindows;
using GuiSupport;

namespace FinanceWindowsViewModels
{
    class OpeningWindowViewModel : PropertyChangedBase
    {
        private Action CloseWindowAction;
        public ICommand BrowseForFileCommand { get; }
        public ICommand BrowseForFolderCommand { get; }
        public RelayCommand<Window> OpenNewFileCommand { get; }
        public RelayCommand<Window> OpenProgramCommand { get; }

        private string fFilePath;
        public string FilePath
        {
            get 
            { 
                return fFilePath; 
            }
            set 
            { 
                fFilePath = value;
                OnPropertyChanged();
            }
        }

        public void ExecuteOpenMainWindow(Window window)
        {
            if (File.Exists(fFilePath))
            {
                GlobalData.fDatabaseFilePath = FilePath;
                DatabaseAccessorHelper.LoadPortfolio();
                var fMainWindow = new MainWindow();
                fMainWindow.Show();
                CloseWindowAction();
            }
            else { FilePath = "Specified File Didnt Exist"; }
        }

        public void ExecuteOpenNewFileWindow(Window window)
        {
                GlobalData.fDatabaseFilePath = FilePath;
            DatabaseAccessorHelper.LoadPortfolio();
                var fMainWindow = new MainWindow();
                fMainWindow.Show();

                CloseWindowAction();
        }

        public void ExecuteBrowseForFileCommand(object obj)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                FilePath = openFile.FileName;
            }
        }

        public void ExecuteBrowseForFolderCommand(object obj)
        {
            FolderBrowserDialog openFile = new FolderBrowserDialog();
            if (openFile.ShowDialog()==DialogResult.OK)
            {
                FilePath = openFile.SelectedPath;
            }
        }

        public OpeningWindowViewModel(Action CloseWindow)
        {
            CloseWindowAction = CloseWindow;
            OpenProgramCommand = new RelayCommand<Window>(this.ExecuteOpenMainWindow);
            OpenNewFileCommand = new RelayCommand<Window>(this.ExecuteOpenNewFileWindow);
            BrowseForFileCommand = new BasicCommand(ExecuteBrowseForFileCommand);
            BrowseForFolderCommand = new BasicCommand(ExecuteBrowseForFolderCommand);
        }
    }
}
