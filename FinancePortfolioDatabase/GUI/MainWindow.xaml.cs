using FinanceWindowsViewModels;
using FinancialStructures.ReportingStructures;
using GlobalHeldData;
using GUIAccessorFunctions;
using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace FinanceWindows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            var viewModel = new MainWindowViewModel();
            InitializeComponent();

            Title = "Financial Database v" + AssemblyCreationDate.Value.ToString("yyyy.MM.dd.HHmmss");

            DataContext = viewModel;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string msg = "Data may not be saved. Would you like to save before closing?";
            MessageBoxResult result =
                  MessageBox.Show(
                    msg,
                    $"Closing {Title}.",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                SaveFileDialog saving = new SaveFileDialog() { DefaultExt = "xml", FileName = GlobalData.DatabaseName + Path.GetExtension(GlobalData.fDatabaseFilePath), InitialDirectory = Path.GetDirectoryName(GlobalData.fDatabaseFilePath) };
                saving.Filter = "XML Files|*.xml|All Files|*.*";
                if (saving.ShowDialog() == true)
                {
                    //if (!File.Exists(saving.FileName))
                    {
                        DatabaseAccessor.SetFilePath(saving.FileName);
                    }
                }

                DatabaseAccessor.SavePortfolio(new ErrorReports());
                // saving.Dispose();
            }
            if (result == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
            }
        }
    }
}
