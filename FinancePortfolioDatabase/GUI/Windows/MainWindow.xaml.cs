﻿using System;
using System.ComponentModel;
using System.IO;
using System.IO.Abstractions;
using System.Reflection;
using System.Windows;
using Common.Structure.Reporting;
using Common.UI;
using Common.UI.Services;
using FinancePortfolioDatabase.GUI.ViewModels;
using FinancePortfolioDatabase.GUI.ViewModels.Common;
using FinancialStructures.Database.Extensions;

namespace FinancePortfolioDatabase.GUI.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly UiGlobals fUiGlobals;

        /// <summary>
        /// Construct an instance of the main window.
        /// </summary>
        public MainWindow()
        {
            FileInteractionService FileInteractionService = new FileInteractionService(this);
            DialogCreationService DialogCreationService = new DialogCreationService(this);
            fUiGlobals = new UiGlobals(null, new DispatcherInstance(), new FileSystem(), FileInteractionService, DialogCreationService, null);
            MainWindowViewModel viewModel = new MainWindowViewModel(fUiGlobals);
            InitializeComponent();

            Assembly assembly = Assembly.GetExecutingAssembly();
            string informationVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            Title = "Financial Database v" + informationVersion;

            DataContext = viewModel;
        }

        /// <summary>
        /// Prints all error reports from the report logger instance.
        /// </summary>
        /// <param name="exception"></param>
        public void PrintErrorLog(Exception exception)
        {
            FileInteractionResult result = fUiGlobals.FileInteractionService.SaveFile("log", string.Empty, fUiGlobals.CurrentWorkingDirectory, filter: "log Files|*.log|All Files|*.*");
            if (result.Success)
            {
                using (Stream stream = fUiGlobals.CurrentFileSystem.FileStream.Create(result.FilePath, FileMode.Create))
                using (TextWriter writer = new StreamWriter(stream))
                {
                    foreach (ErrorReport report in fUiGlobals.ReportLogger.Reports.GetReports())
                    {
                        writer.WriteLine(report.ToString());
                    }

                    writer.WriteLine(exception.Message);
                    writer.WriteLine(exception.StackTrace);
                }
            }
        }

        /// <summary>
        /// Event fires when one closes the window. Checks if the user wishes to save or not.
        /// </summary>
        /// <remarks>
        /// This should really check if the data has changed or not, but this
        /// is not currently possible.
        /// </remarks>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            MainWindowViewModel VM = DataContext as MainWindowViewModel;
            VM.SaveConfig();
            MessageBoxResult result = VM.ProgramPortfolio.IsAlteredSinceSave
                ? fUiGlobals.DialogCreationService.ShowMessageBox("Data has changed since last saved. Would you like to save changes before closing?", $"Closing {Title}.", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning)
                : fUiGlobals.DialogCreationService.ShowMessageBox("There is a small chance that the data has changed since last save (due to neglect on my part). Would you like to save before closing?", $"Closing {Title}.", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                FileInteractionResult savingResult = fUiGlobals.FileInteractionService.SaveFile("xml", fUiGlobals.CurrentFileSystem.Path.GetFileName(VM.ProgramPortfolio.FilePath), VM.ProgramPortfolio.Directory(fUiGlobals.CurrentFileSystem), "XML Files|*.xml|All Files|*.*");
                if (savingResult.Success)
                {
                    VM.ProgramPortfolio.FilePath = savingResult.FilePath;
                    MainWindowViewModel vm = DataContext as MainWindowViewModel;
                    vm.ProgramPortfolio.SavePortfolio(savingResult.FilePath, fUiGlobals.CurrentFileSystem, vm.ReportLogger);
                }
            }
            if (result == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
            }
        }

        private void CloseTabCommand(object sender, RoutedEventArgs e)
        {
            MainWindowViewModel VM = DataContext as MainWindowViewModel;
            if (MainTabControl.SelectedIndex != 0)
            {
                if (VM.Tabs[MainTabControl.SelectedIndex] is DataDisplayViewModelBase vmBase && vmBase.Closable)
                {
                    VM.Tabs.RemoveAt(MainTabControl.SelectedIndex);
                }
            }
        }
    }
}
