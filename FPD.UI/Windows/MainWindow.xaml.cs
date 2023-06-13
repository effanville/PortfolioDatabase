using System;
using System.ComponentModel;
using System.IO;
using System.IO.Abstractions;
using System.Reflection;
using System.Windows;
using Common.Structure.Reporting;
using Common.UI;
using Common.UI.Services;
using Common.UI.Wpf.Services;
using FPD.Logic.ViewModels;
using FPD.Logic.ViewModels.Common;
using FinancialStructures.Database;
using Common.UI.Wpf;
using System.Windows.Input;
using System.Windows.Shapes;

namespace FPD.UI.Windows
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
            bool isLightTheme = true;
            FileInteractionService FileInteractionService = new FileInteractionService(this);
            DialogCreationService DialogCreationService = new DialogCreationService(this);
            fUiGlobals = new UiGlobals(null, new DispatcherInstance(), new FileSystem(), FileInteractionService, DialogCreationService, null);
            MainWindowViewModel viewModel = new MainWindowViewModel(fUiGlobals, new BackgroundPortfolioUpdater());
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
            MessageBoxOutcome result = VM.ProgramPortfolio.IsAlteredSinceSave
                ? fUiGlobals.DialogCreationService.ShowMessageBox("Data has changed since last saved. Would you like to save changes before closing?", $"Closing {Title}.", BoxButton.YesNoCancel, BoxImage.Warning)
                : fUiGlobals.DialogCreationService.ShowMessageBox("There is a small chance that the data has changed since last save (due to neglect on my part). Would you like to save before closing?", $"Closing {Title}.", BoxButton.YesNoCancel, BoxImage.Warning);

            if (result == MessageBoxOutcome.Yes)
            {
                FileInteractionResult savingResult = fUiGlobals.FileInteractionService.SaveFile("xml", VM.ProgramPortfolio.Name, filter: "XML Files|*.xml|All Files|*.*");
                if (savingResult.Success)
                {
                    VM.ProgramPortfolio.Name = fUiGlobals.CurrentFileSystem.Path.GetFileNameWithoutExtension(savingResult.FilePath);
                    MainWindowViewModel vm = DataContext as MainWindowViewModel;
                    vm.ProgramPortfolio.SavePortfolio(savingResult.FilePath, fUiGlobals.CurrentFileSystem, vm.ReportLogger);
                }
            }
            if (result == MessageBoxOutcome.Cancel)
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
        private void MinimizeButton_Click(object sender, RoutedEventArgs e) => App.Current.MainWindow.WindowState = WindowState.Minimized;

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (App.Current.MainWindow.WindowState == WindowState.Maximized)
            {
                App.Current.MainWindow.WindowState = WindowState.Normal;
            }
            else if (App.Current.MainWindow.WindowState == WindowState.Normal)
            {
                App.Current.MainWindow.WindowState = WindowState.Maximized;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) => Application.Current.MainWindow.Close();

        private void UserControl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            App.Current.MainWindow.DragMove();
        }

        bool ResizeInProcess = false;
        private void Resize_Init(object sender, MouseButtonEventArgs e)
        {
            Rectangle senderRect = sender as Rectangle;
            if (senderRect != null)
            {
                ResizeInProcess = true;
                senderRect.CaptureMouse();
            }
        }

        private void Resize_End(object sender, MouseButtonEventArgs e)
        {
            Rectangle senderRect = sender as Rectangle;
            if (senderRect != null)
            {
                ResizeInProcess = false; ;
                senderRect.ReleaseMouseCapture();
            }
        }

        private void Resizeing_Form(object sender, MouseEventArgs e)
        {
            if (ResizeInProcess)
            {
                double temp = 0;
                Rectangle senderRect = sender as Rectangle;
                Window mainWindow = senderRect.Tag as Window;
                if (senderRect != null)
                {
                    double width = e.GetPosition(mainWindow).X;
                    double height = e.GetPosition(mainWindow).Y;
                    senderRect.CaptureMouse();
                    if (senderRect.Name.Contains("right", StringComparison.OrdinalIgnoreCase))
                    {
                        width += 5;
                        if (width > 0)
                            mainWindow.Width = width;
                    }
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
                            mainWindow.Height = height;
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
            }
        }
    }
}
