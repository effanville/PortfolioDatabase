using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using Effanville.FinancialStructures.Database.Statistics;
using Effanville.FPD.Logic.ViewModels.Stats;

namespace Effanville.FPD.UI.Windows.Stats
{
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
            if (DataContext is not StatsViewModel vm || vm.Stats == null)
            {
                return;
            }

            DataTable dt = await Task.Run(GetTable);

            StatsBox.ItemsSource = dt.DefaultView;
            return;

            DataTable GetTable()
            {
                DataTable dataTable = new DataTable();

                // First add the column header names from the statistics names.
                IReadOnlyList<Statistic> statisticNames = vm.Stats.First().StatisticNames;
                foreach (Statistic statisticName in statisticNames)
                {
                    dataTable.Columns.Add(new DataColumn(statisticName.ToString(), typeof(string)));
                }

                // Now add the statistics values.
                foreach (AccountStatistics val in vm.Stats)
                {
                    _ = dataTable.Rows.Add(val.StatValuesAsObjects.ToArray());
                }

                return dataTable;
            }
        }
    }
}
