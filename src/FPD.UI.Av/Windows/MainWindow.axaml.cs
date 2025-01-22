using System.ComponentModel;
using System.IO;
using System.Reflection;

using Avalonia.Controls;

using Effanville.Common.Structure.Reporting;
using Effanville.Common.UI.Services;
using Effanville.FinancialStructures.Persistence;
using Effanville.FPD.Logic.ViewModels;
using Avalonia.Interactivity;
using Effanville.FPD.Logic.ViewModels.Common;

namespace Effanville.FPD.AvaloniaUI.Windows;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        Assembly assembly = Assembly.GetExecutingAssembly();
        string informationVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion;
        Title = "FPD v" + informationVersion;
    }
    /// <summary>
    /// Prints all error reports from the report logger instance.
    /// </summary>
    /// <param name="exception"></param>
    public async void PrintErrorLog(Exception exception)
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
                var options = new XmlFilePersistenceOptions(savingResult.FilePath, viewModel.Globals.CurrentFileSystem, "1.0.0.0");
                xmlPersistence.Save(viewModel.ProgramPortfolio, options);
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
}
