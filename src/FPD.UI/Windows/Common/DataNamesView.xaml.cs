using System.Windows.Controls;
using System.Windows.Input;

using Effanville.FPD.Logic.ViewModels.Common;

namespace Effanville.FPD.UI.Windows.Common;

/// <summary>
/// Interaction logic for DataNamesView.xaml
/// </summary>
public partial class DataNamesView : UserControl
{
    /// <summary>
    /// Construct an instance.
    /// </summary>
    public DataNamesView() => InitializeComponent();

    private async void DataGrid_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Delete && e.Key != Key.Back)
        {
            return;
        }

        if (e.OriginalSource is not DataGridCell)
        {
            return;
        }

        if (DataContext is DataNamesViewModel vm)
        {
            await vm.ExecuteDelete();
        }
    }

    private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DataContext is not DataNamesViewModel dc || sender is not DataGrid dataGrid)
        {
            return;
        }

        dc.SelectionChangedCommand.Execute(dataGrid.CurrentItem);
    }
}
