using Avalonia.Controls;
using Avalonia.Input;

using Effanville.Common.Structure.Reporting;
using Effanville.FPD.Logic.ViewModels;

namespace Effanville.FPD.AvaloniaUI.Windows;

/// <summary>
/// Interaction logic for ReportingWindow.axaml
/// </summary>
public partial class ReportingWindow : Expander
{
    /// <summary>
    /// Construct an instance.
    /// </summary>
    public ReportingWindow() => InitializeComponent();

    private void DataGrid_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Delete && e.Key != Key.Back)
        {
            return;
        }

        if (sender is not DataGrid dg)
        {
            return;
        }

        if (DataContext == null
            || DataContext is not ReportingWindowViewModel vm)
        {
            return;
        }

        if (dg.SelectedItems.Count > 1)
        {
            var reports = new List<ErrorReport>();
            foreach (object item in dg.SelectedItems)
            {
                if (item is ErrorReport report)
                {
                    reports.Add(report);
                }
            }

            vm.DeleteReports(reports);
        }
        else if (dg.SelectedItem is ErrorReport selectedItem)
        {
            vm.DeleteReport(selectedItem);
        }
    }
}