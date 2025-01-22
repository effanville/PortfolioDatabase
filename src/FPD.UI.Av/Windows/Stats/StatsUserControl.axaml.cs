using System.ComponentModel;
using System.Data;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

using Effanville.FPD.Logic.ViewModels.Stats;

namespace Effanville.FPD.UI.Av.Windows.Stats
{
    /// <summary>
    /// Interaction logic for StatsUserControl.axaml
    /// </summary>
    public partial class StatsUserControl : UserControl
    {
        /// <summary>
        /// Construct an instance.
        /// </summary>
        public StatsUserControl()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {

            if (change.Property == IsVisibleProperty || change.Property.Name == nameof(IsEffectivelyVisible))
            {
                // Handle the visibility change here
                bool newVisibilityValue = change.NewValue as bool? ?? false;
                if (newVisibilityValue)
                {
                    UpdateDataGrid(null, null);
                }
            }
            base.OnPropertyChanged(change);
        }

        private void OnDataContextChanged(object? sender, EventArgs e)
        {
            if (DataContext is StatsViewModel vm)
            {
                vm.StatisticsChanged += UpdateDataGrid;
            }

            DataContextChanged -= OnDataContextChanged;
            UpdateDataGrid(null, null);
        }

        /// <summary>
        /// Updates the data displayed in the grid.
        /// </summary>
        private async void UpdateDataGrid(object? sender, PropertyChangedEventArgs? e)
        {
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
}
