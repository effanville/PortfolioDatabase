using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using Effanville.Common.Structure.Reporting;
using Effanville.FinancialStructures.Database.Statistics;
using Effanville.FPD.Logic.ViewModels.Stats;

namespace Effanville.FPD.UI.Windows.Stats
{
    /// <summary>
    /// Interaction logic for StatsUserControl.xaml
    /// </summary>
    public partial class StatsUserControl
    {
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
            
            if (Resources.Contains(DisplayConstants.StyleBridgeName)
                && DataContext is StatsViewModel dc
                && Resources[DisplayConstants.StyleBridgeName] is Bridge bridge)
            {
                bridge.Styles = dc.Styles;
            }
            
            DataContextChanged -= OnDataContextChanged;
        }
        
        /// <summary>
        /// Updates the data displayed in the grid.
        /// </summary>
        private async void UpdateDataGrid(object sender, PropertyChangedEventArgs e)
        {
            Stopwatch stopwatch = new();
            stopwatch.Start();
            if (DataContext is not StatsViewModel vm)
            {
                return;
            }

            DataTable dt = await Task.Run(GetTable);

            StatsBox.ItemsSource = dt.DefaultView;
            stopwatch.Stop();
            vm.ReportLogger.Log(ReportSeverity.Critical, ReportType.Information, "UIhere", $"Elapsed is {stopwatch.Elapsed.TotalMilliseconds}ms");

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

        private static SortDirection ConvertFromVisual(ListSortDirection sortDirection)
        {
            switch (sortDirection)
            {
                case ListSortDirection.Ascending:
                    return SortDirection.Ascending;
                case ListSortDirection.Descending:
                default:
                    return SortDirection.Descending;
            }
        }

        private static ListSortDirection ConvertToVisual(SortDirection sortDirection)
        {
            switch (sortDirection)
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
            if (e.Column == null || e.Column.Header == null)
            {
                return;
            }

            string sortColumnString = e.Column.Header.ToString() ?? Statistic.Company.ToString();
            Statistic sortColumnStatistic = System.Enum.Parse<Statistic>(sortColumnString);
            ListSortDirection? previousVisualSortDirection = e.Column.SortDirection;
            SortDirection sortDirection =
                SortDirectionHelpers.Invert(
                    ConvertFromVisual(previousVisualSortDirection ?? ListSortDirection.Descending));
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

            //Show the ascending icon when ascending sort is done
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
