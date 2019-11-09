using GUIAccessorFunctions;
using ReportingStructures;
using System.Windows;
using GlobalHeldData;

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
    }
}
