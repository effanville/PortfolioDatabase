using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Common.UI.Wpf.Controls;
using FPD.Logic.ViewModels.Stats;
using FinancialStructures.Database.Statistics;
using System.Threading.Tasks;

namespace FPD.UI.Windows.Stats
{
    /// <summary>
    /// Interaction logic for StatsUserControl.xaml
    /// </summary>
    public partial class StatsUserControl : AutoGenColumnControl
    {
        private bool _isDataGridSet = false;
        
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
            if(e.NewValue is bool isVisibleChanged && !isVisibleChanged)
            {
                return;
            }

            UpdateDataGrid(null, null);
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is StatsViewModel vm)
            {
                vm.StatisticsChanged += UpdateDataGrid;
            }
            string bridgeName = "bridge";
            if (Resources.Contains(bridgeName)
                && DataContext is StatsViewModel dc
                && Resources[bridgeName] is Bridge bridge)
            {
                bridge.Styles = dc.Styles;
            }
        }
        
        /// <summary>
        /// Updates the data displayed in the grid.
        /// </summary>
        private async void UpdateDataGrid(object sender, PropertyChangedEventArgs e)
        {
            if (DataContext is StatsViewModel vm)
            {
                if (_isDataGridSet && (!vm.ModelData?.IsAlteredSinceSave ?? true))
                {
                    return;
                }

                DataTable dt = await Task.Run(() => GetTable(vm.Stats));
                DataTable GetTable(List<AccountStatistics> statistics)
                {
                    DataTable dt = new DataTable();

                    // First add the column header names from the statistics names.
                    IReadOnlyList<Statistic> statisticNames = vm.Stats.First().StatisticNames;
                    foreach (Statistic statisticName in statisticNames)
                    {
                        dt.Columns.Add(new DataColumn(statisticName.ToString(), typeof(string)));
                    }

                    // Now add the statistics values.
                    foreach (AccountStatistics val in vm.Stats)
                    {
                        _ = dt.Rows.Add(val.StatValuesAsObjects.ToArray());
                    }
                    
                    return dt;
                }

                StatsBox.ItemsSource = dt.DefaultView;
                _isDataGridSet = true;
            }
        }

        private SortDirection ConvertFromVisual(ListSortDirection sD)
        {
            switch (sD)
            {
                case ListSortDirection.Ascending:
                    return SortDirection.Ascending;
                case ListSortDirection.Descending:
                default:
                    return SortDirection.Descending;
            }
        }

        private ListSortDirection ConvertToVisual(SortDirection sD)
        {
            switch (sD)
            {
                case SortDirection.Ascending:
                    return ListSortDirection.Ascending;
                case SortDirection.Descending:
                default:
                    return ListSortDirection.Descending;
            }
        }

        private void dg_Sorting(object sender, DataGridSortingEventArgs e)
        {
            Statistic sortColumnStatistic = System.Enum.Parse<Statistic>(e.Column.Header.ToString());
            ListSortDirection? previousVisualSortDirection = e.Column.SortDirection;
            SortDirection sortDirection = SortDirectionHelpers.Invert(ConvertFromVisual(previousVisualSortDirection ?? ListSortDirection.Descending));
            if (DataContext is StatsViewModel vm)
            {
                vm.Stats.Sort((x, y) => x.CompareTo(y, sortColumnStatistic, sortDirection));
            }

            UpdateDataGrid(null, null);

            // Remove sorting indicators from other columns
            foreach (var dgColumn in StatsBox.Columns)
            {
                if (dgColumn.Header.ToString() != e.Column.Header.ToString())
                {
                    dgColumn.SortDirection = null;
                }
            }

            //Show the ascending icon when acending sort is done
            e.Column.SortDirection = ConvertToVisual(sortDirection);
            if (sender is DataGrid dg)
            {
                var sameColumn = dg.Columns.First(col => col.Header == e.Column.Header);
                sameColumn.SortDirection = ConvertToVisual(sortDirection);
            }

            e.Handled = true;
        }
    }
}
