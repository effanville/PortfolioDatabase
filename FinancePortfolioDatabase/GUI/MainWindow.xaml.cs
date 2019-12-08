using GUIAccessorFunctions;
using GlobalHeldData;
using System.Windows;
using System.Windows.Forms;

namespace FinanceWindows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DatabaseAccessor.LoadPortfolio();
            InitializeComponent();

            Title = "Financial Database v" + AssemblyCreationDate.Value.ToString("yyyy.MM.dd.HHmmss");
        }

        private void OpenHelpDocsCommand(object sender, RoutedEventArgs e)
        {
            var helpwindow = new HelpWindow();
            helpwindow.Show();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string msg = "Data may not be saved. Would you like to save before closing?";
            MessageBoxResult result =
                  System.Windows.MessageBox.Show(
                    msg,
                    $"Closing {Title}.",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                SaveFileDialog saving = new SaveFileDialog();
                if (saving.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //if (!File.Exists(saving.FileName))
                    {
                        DatabaseAccessor.SetFilePath(saving.FileName);
                    }
                }

                DatabaseAccessor.SavePortfolio();
                saving.Dispose();
            }
            if (result == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
            }
        }
    }
}
