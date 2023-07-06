using System.Windows.Controls;
using System.Windows.Input;
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
            if (e.Key == Key.Delete || e.Key == Key.Back)
            {
                if (DataContext != null && DataContext is ReportingWindowViewModel vm)
                {
                    vm.DeleteReport();
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
