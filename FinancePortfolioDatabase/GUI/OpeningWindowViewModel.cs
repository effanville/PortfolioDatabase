using System.Windows.Input;
using System;
using System.Windows;
using System.Windows.Forms;
using GUIAccessorFunctions;
using FinanceWindows;
using GUISupport;

namespace FinanceWindowsViewModels
{
    class OpeningWindowViewModel : PropertyChangedBase
    {
        private Action CloseWindowAction;

        public ICommand BrowseForFileCommand { get; }

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
            DatabaseAccessor.SetFilePath(FilePath);
            DatabaseAccessor.LoadPortfolio();
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
            openFile.Dispose();
        }


        public OpeningWindowViewModel(Action CloseWindow)
        {
            CloseWindowAction = CloseWindow;
            OpenProgramCommand = new RelayCommand<Window>(this.ExecuteOpenMainWindow);
            BrowseForFileCommand = new BasicCommand(ExecuteBrowseForFileCommand);
        }
    }
}
