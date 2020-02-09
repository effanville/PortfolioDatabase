﻿using FinanceWindowsViewModels;
using FinancialStructures.ReportingStructures;
using FinancialStructures.Database;
using Microsoft.Win32;
using System.Windows;
using System.Reflection;
using System;

namespace FinanceWindows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static class AssemblyCreationDate
        {
            public static readonly DateTime Value;

            static AssemblyCreationDate()
            {
                Version version = Assembly.GetExecutingAssembly().GetName().Version;
                Value = new DateTime(2000, 1, 1, 20, 24, 30).AddDays(version.Build).AddSeconds(version.MinorRevision * 2);
            }
        }

        private static string versionNumber = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();

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
            var VM = DataContext as MainWindowViewModel;
            if (result == MessageBoxResult.Yes)
            {
                SaveFileDialog saving = new SaveFileDialog() { DefaultExt = "xml", FileName = VM.portfolio.DatabaseName + VM.portfolio.Extension, InitialDirectory = VM.portfolio.Directory };
                saving.Filter = "XML Files|*.xml|All Files|*.*";
                if (saving.ShowDialog() == true)
                {
                    VM.portfolio.SetFilePath(saving.FileName);
                    var vm = DataContext as MainWindowViewModel;
                    vm.portfolio.SavePortfolio(vm.benchMarks, saving.FileName, new ErrorReports());
                }
                
                // saving.Dispose();
            }
            if (result == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
            }
        }
    }
}
