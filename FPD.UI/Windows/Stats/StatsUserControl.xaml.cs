using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Common.UI.DisplayObjects;
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
        /// <summary>
        /// Construct an instance.
        /// </summary>
        public StatsUserControl()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private async void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is StatsViewModel vm)
            {
                vm.StatisticsChanged += UpdateGrid;
            }

            await UpdateDataGrid();
        }

        private async void UpdateGrid(object sender, PropertyChangedEventArgs e)
        {
            await UpdateDataGrid();
        }

        /// <summary>
        /// Updates the data displayed in the grid.
        /// </summary>
        private async Task UpdateDataGrid()
        {
            if (DataContext is StatsViewModel vm)
            {
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

        private async void dg_Sorting(object sender, DataGridSortingEventArgs e)
        {
            if (sender is DataGrid dg && DataContext is StatsViewModel vm)
            {
                Statistic sortColumnStatistic = System.Enum.Parse<Statistic>(e.Column.Header.ToString());
                var sameColumn = dg.Columns.First(col => col.Header == e.Column.Header);

                SortDirection sortDirection = SortDirectionHelpers.Invert(vm.CurrentSortedDirection ?? SortDirection.Descending);

                vm.Stats.Sort((x, y) => x.CompareTo(y, sortColumnStatistic, sortDirection));


                await UpdateDataGrid();

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
                sameColumn.SortDirection = ConvertToVisual(sortDirection);
                vm.CurrentSortedDirection = sortDirection;
                vm.CurrentSortedField = sameColumn.Header.ToString();
            }

            e.Handled = true;
        }
    }
}
