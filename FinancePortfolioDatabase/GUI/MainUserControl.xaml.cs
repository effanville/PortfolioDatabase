using System.Windows.Controls;
using FinanceWindowsViewModels;

namespace FinanceWindows
{
    /// <summary>
    /// Interaction logic for MainUserControl.xaml
    /// </summary>
    public partial class MainUserControl : UserControl
    {
        public MainUserControl()
        {
            var viewModel = new MainWindowViewModel();
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
