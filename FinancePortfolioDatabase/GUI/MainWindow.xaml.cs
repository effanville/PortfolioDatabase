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
            InitializeComponent();
        }

        private void OpenHelpDocsCommand(object sender, RoutedEventArgs e)
        {
            var helpwindow = new HelpWindow();
            helpwindow.Show();
        }
    }
}
