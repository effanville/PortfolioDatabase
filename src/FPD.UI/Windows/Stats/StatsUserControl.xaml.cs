using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using Effanville.FPD.Logic.ViewModels.Stats;

namespace Effanville.FPD.UI.Windows.Stats;

/// <summary>
/// Interaction logic for StatsUserControl.xaml
/// </summary>
public partial class StatsUserControl
{
    private bool _isVisible;
    /// <summary>
    /// Construct an instance.
    /// </summary>
    public StatsUserControl()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
        IsVisibleChanged += OnIsVisibleChanged;
    }

    private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is bool isVisible and false)
        {
            _isVisible = false;
            return;
        }

        _isVisible = true;
        UpdateDataGrid(null, null);
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (DataContext is StatsViewModel vm)
        {
            vm.StatisticsChanged += UpdateDataGrid;
        }

        DataContextChanged -= OnDataContextChanged;
    }

    /// <summary>
    /// Updates the data displayed in the grid.
    /// </summary>
    private async void UpdateDataGrid(object sender, PropertyChangedEventArgs e)
    {
        if (!_isVisible)
        {
            return;
        }
        if (DataContext is not StatsViewModel vm || vm.StatsToView == null)
        {
            return;
        }

        GenerateColumns(StatsBox, vm.StatsToView.Select(x => x.ToString()));
    }

    private static void GenerateColumns(
        DataGrid dataGrid,
        IEnumerable<string> columns)
    {
        dataGrid.Columns.Clear();

        int index = 0;
        foreach (string column in columns)
        {
            dataGrid.Columns.Add(new DataGridTextColumn()
            {
                Header = column,
                Binding = new Binding(string.Format("[{0}]", index++))
            });
        }
    }
}
