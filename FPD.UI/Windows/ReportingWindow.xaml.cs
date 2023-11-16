using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;

using Common.Structure.Reporting;

using FPD.Logic.ViewModels;

namespace FPD.UI.Windows
{
    /// <summary>
    /// Interaction logic for ReportingWindow.xaml
    /// </summary>
    public partial class ReportingWindow : Expander
    {
        /// <summary>
        /// Construct an instance.
        /// </summary>
        public ReportingWindow()
        {
            InitializeComponent();
        }

        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Delete && e.Key != Key.Back)
            {
                return;
            }

            if (sender is not DataGrid dg)
            {
                return;
            }

            if (DataContext != null
                && DataContext is ReportingWindowViewModel vm)
            {
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
                else if (dg.CurrentItem is ErrorReport selectedItem)
                {
                    vm.DeleteReport(selectedItem);
                }
            }
        }

        private void UC_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            string bridgeName = "bridge";
            if (Resources.Contains(bridgeName)
                && DataContext is ReportingWindowViewModel dc
                && Resources[bridgeName] is Bridge bridge)
            {
                bridge.Styles = dc.Styles;
            }
        }
    }
}