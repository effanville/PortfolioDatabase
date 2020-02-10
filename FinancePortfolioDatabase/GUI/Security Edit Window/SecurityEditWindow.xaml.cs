﻿using FinanceWindowsViewModels;
using System.Windows;
using System.Windows.Controls;

namespace FinanceWindows
{
    /// <summary>
    /// Interaction logic for SecurityEditWindow.xaml
    /// </summary>
    public partial class SecurityEditWindow : Grid
    {
        public SecurityEditWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var VM = this.DataContext as SecurityEditWindowViewModel;
            if (TabMain.SelectedIndex != 0)
            {
                VM.Tabs.RemoveAt(TabMain.SelectedIndex);
            }
        }
    }
}