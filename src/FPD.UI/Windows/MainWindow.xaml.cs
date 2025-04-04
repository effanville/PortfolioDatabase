﻿using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

using Effanville.Common.Structure.Reporting;
using Effanville.Common.UI.Services;
using Effanville.FinancialStructures.Persistence;
using Effanville.FPD.Logic.ViewModels;

namespace Effanville.FPD.UI.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _resizeInProcess;

        /// <summary>
        /// Construct an instance of the main window.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            Assembly assembly = Assembly.GetExecutingAssembly();
            string informationVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion;
            Title = "Financial Database v" + informationVersion;
        }

        /// <summary>
        /// Prints all error reports from the report logger instance.
        /// </summary>
        /// <param name="exception"></param>
        public async Task PrintErrorLog(Exception exception)
        {
            if (DataContext is not MainWindowViewModel viewModel)
            {
                return;
            }

            FileInteractionResult result = await viewModel.Globals.FileInteractionService.SaveFile("log", string.Empty,
                viewModel.Globals.CurrentWorkingDirectory, filter: "log Files|*.log|All Files|*.*");
            if (!result.Success)
            {
                return;
            }

            using (Stream stream = viewModel.Globals.CurrentFileSystem.FileStream.New(result.FilePath, FileMode.Create))
            using (TextWriter writer = new StreamWriter(stream))
            {
                foreach (ErrorReport report in viewModel.Globals.ReportLogger.Reports.GetReports())
                {
                    writer.WriteLine(report.ToString());
                }

                writer.WriteLine(exception.Message);
                writer.WriteLine(exception.StackTrace);
            }
        }

        /// <summary>
        /// Event fires when one closes the window. Checks if the user wishes to save or not.
        /// </summary>
        /// <remarks>
        /// This should really check if the data has changed or not, but this
        /// is not currently possible.
        /// </remarks>
        private async void Window_Closing(object sender, CancelEventArgs e)
        {
            if (DataContext is not MainWindowViewModel viewModel)
            {
                return;
            }

            viewModel.SaveConfig();
            MessageBoxOutcome result = viewModel.ProgramPortfolio.IsAlteredSinceSave
                ? viewModel.Globals.DialogCreationService.ShowMessageBox(
                    "Data has changed since last saved. Would you like to save changes before closing?",
                    $"Closing {Title}.", BoxButton.YesNoCancel, BoxImage.Warning)
                : viewModel.Globals.DialogCreationService.ShowMessageBox(
                    "There is a small chance that the data has changed since last save (due to neglect on my part). Would you like to save before closing?",
                    $"Closing {Title}.", BoxButton.YesNoCancel, BoxImage.Warning);

            if (result == MessageBoxOutcome.Yes)
            {
                FileInteractionResult savingResult = await viewModel.Globals.FileInteractionService.SaveFile("xml",
                    viewModel.ProgramPortfolio.Name, filter: "XML Files|*.xml|All Files|*.*");
                if (savingResult.Success)
                {
                    viewModel.ProgramPortfolio.Name =
                        viewModel.Globals.CurrentFileSystem.Path.GetFileNameWithoutExtension(savingResult.FilePath);
                    var xmlPersistence = new XmlPortfolioPersistence(viewModel.Globals.ReportLogger);
                    var options = new XmlFilePersistenceOptions(savingResult.FilePath, viewModel.Globals.CurrentFileSystem);
                    xmlPersistence.Save(viewModel.ProgramPortfolio, options);
                }
            }

            if (result == MessageBoxOutcome.Cancel)
            {
                e.Cancel = true;
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current != null
                && Application.Current.MainWindow != null)
            {
                Application.Current.MainWindow.WindowState = WindowState.Minimized;
            }
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current == null
                || Application.Current.MainWindow == null)
            {
                return;
            }

            if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
            {
                Application.Current.MainWindow.ResizeMode = ResizeMode.CanResize;
                Application.Current.MainWindow.WindowState = WindowState.Normal;
            }
            else if (Application.Current.MainWindow.WindowState == WindowState.Normal)
            {
                Application.Current.MainWindow.ResizeMode = ResizeMode.NoResize;
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
                Application.Current.MainWindow.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight - 13;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) => Application.Current.MainWindow?.Close();

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Application.Current.MainWindow?.DragMove();
            }
        }

        private void Resize_Init(object sender, MouseButtonEventArgs e)
        {
            if (sender is Rectangle senderRect)
            {
                _resizeInProcess = true;
                senderRect.CaptureMouse();
            }
        }

        private void Resize_End(object sender, MouseButtonEventArgs e)
        {
            if (sender is Rectangle senderRect)
            {
                _resizeInProcess = false;
                senderRect.ReleaseMouseCapture();
            }
        }

        private void Resizeing_Form(object sender, MouseEventArgs e)
        {
            if (!_resizeInProcess)
            {
                return;
            }

            Rectangle senderRect = sender as Rectangle;
            if (senderRect?.Tag is not Window mainWindow)
            {
                return;
            }

            double width = e.GetPosition(mainWindow).X;
            double height = e.GetPosition(mainWindow).Y;
            senderRect.CaptureMouse();
            if (senderRect.Name.Contains("right", StringComparison.OrdinalIgnoreCase))
            {
                width += 5;
                if (width > 0)
                {
                    mainWindow.Width = width;
                }
            }

            double temp;
            if (senderRect.Name.Contains("left", StringComparison.OrdinalIgnoreCase))
            {
                width -= 5;
                temp = mainWindow.Width - width;
                if ((temp > mainWindow.MinWidth) && (temp < mainWindow.MaxWidth))
                {
                    mainWindow.Width = temp;
                    mainWindow.Left += width;
                }
            }

            if (senderRect.Name.Contains("bottom", StringComparison.OrdinalIgnoreCase))
            {
                height += 5;
                if (height > 0)
                {
                    mainWindow.Height = height;
                }
            }

            if (senderRect.Name.ToLower().Contains("top", StringComparison.OrdinalIgnoreCase))
            {
                height -= 5;
                temp = mainWindow.Height - height;
                if ((temp > mainWindow.MinHeight) && (temp < mainWindow.MaxHeight))
                {
                    mainWindow.Height = temp;
                    mainWindow.Top += height;
                }
            }
        }

        private void MainTabControl_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is MainWindowViewModel mainWindowViewModel)
            {
                var addedItems = e.AddedItems;
                mainWindowViewModel.SelectionChanged.Execute(addedItems);
            }
        }
    }
}