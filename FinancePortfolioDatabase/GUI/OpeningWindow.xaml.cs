using System;
using System.Windows;
using FinanceWindowsViewModels;

namespace FinancePortfolioDatabase
{
    /// <summary>
    /// Interaction logic for OpeningWindow.xaml
    /// </summary>
    public partial class OpeningWindow : Window
    {
        public OpeningWindow()
        {
            Action closeThis = new Action(this.Close);
            var viewModel = new OpeningWindowViewModel(closeThis);
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
