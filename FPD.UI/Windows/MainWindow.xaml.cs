using System;
using System.ComponentModel;
using System.IO;
using System.IO.Abstractions;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;

using Effanville.Common.Structure.Reporting;
using Effanville.Common.UI;
using Effanville.Common.UI.Services;
using Effanville.Common.UI.Wpf;
using Effanville.Common.UI.Wpf.Services;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.Persistence;
using Effanville.FPD.Logic.ViewModels;
using Effanville.FPD.Logic.ViewModels.Common;

using Microsoft.Win32;

namespace Effanville.FPD.UI.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _resizeInProcess;
        private readonly UiGlobals _uiGlobals;

        /// <summary>
        /// Construct an instance of the main window.
        /// </summary>
        public MainWindow()
        {
            FileInteractionService fileInteractionService = new FileInteractionService(this);
            DialogCreationService dialogCreationService = new DialogCreationService(this);
            _uiGlobals = new UiGlobals(
                null,
                new DispatcherInstance(),
                new FileSystem(),
                fileInteractionService,
                dialogCreationService, null);
            MainWindowViewModel viewModel = new MainWindowViewModel(
                _uiGlobals,
                new BackgroundUpdater<IPortfolio>());
            InitializeComponent();

            Assembly assembly = Assembly.GetExecutingAssembly();
            string informationVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion;
            Title = "Financial Database v" + informationVersion;

            DataContext = viewModel;
        }

        /// <summary>
        /// Prints all error reports from the report logger instance.
        /// </summary>
        /// <param name="exception"></param>
        public void PrintErrorLog(Exception exception)
        {
            FileInteractionResult result = _uiGlobals.FileInteractionService.SaveFile("log", string.Empty,
                _uiGlobals.CurrentWorkingDirectory, filter: "log Files|*.log|All Files|*.*");
            if (!result.Success)
            {
                return;
            }

            using (Stream stream = _uiGlobals.CurrentFileSystem.FileStream.Create(result.FilePath, FileMode.Create))
            using (TextWriter writer = new StreamWriter(stream))
            {
                foreach (ErrorReport report in _uiGlobals.ReportLogger.Reports.GetReports())
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
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (DataContext is not MainWindowViewModel viewModel)
            {
                return;
            }

            viewModel.SaveConfig();
            MessageBoxOutcome result = viewModel.ProgramPortfolio.IsAlteredSinceSave
                ? _uiGlobals.DialogCreationService.ShowMessageBox(
                    "Data has changed since last saved. Would you like to save changes before closing?",
                    $"Closing {Title}.", BoxButton.YesNoCancel, BoxImage.Warning)
                : _uiGlobals.DialogCreationService.ShowMessageBox(
                    "There is a small chance that the data has changed since last save (due to neglect on my part). Would you like to save before closing?",
                    $"Closing {Title}.", BoxButton.YesNoCancel, BoxImage.Warning);

            if (result == MessageBoxOutcome.Yes)
            {
                FileInteractionResult savingResult = _uiGlobals.FileInteractionService.SaveFile("xml",
                    viewModel.ProgramPortfolio.Name, filter: "XML Files|*.xml|All Files|*.*");
                if (savingResult.Success)
                {
                    viewModel.ProgramPortfolio.Name =
                        _uiGlobals.CurrentFileSystem.Path.GetFileNameWithoutExtension(savingResult.FilePath);
                    var xmlPersistence = new XmlPortfolioPersistence();
                    var options = new XmlFilePersistenceOptions(savingResult.FilePath, _uiGlobals.CurrentFileSystem);
                    xmlPersistence.Save(viewModel.ProgramPortfolio, options, viewModel.ReportLogger);
                }
            }

            if (result == MessageBoxOutcome.Cancel)
            {
                e.Cancel = true;
            }
        }

        private void CloseTabCommand(object sender, RoutedEventArgs e)
        {
            if (DataContext is not MainWindowViewModel viewModel
                || MainTabControl.SelectedIndex == 0)
            {
                return;
            }

            if (viewModel.Tabs[MainTabControl.SelectedIndex] is DataDisplayViewModelBase vmBase
                && vmBase.Closable)
            {
                viewModel.Tabs.RemoveAt(MainTabControl.SelectedIndex);
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

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e) =>
            Application.Current.MainWindow?.DragMove();

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
    }
}