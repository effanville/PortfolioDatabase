using System.Windows.Controls;
using System.Windows.Input;

using FPD.Logic.ViewModels.Common;
using FPD.Logic.ViewModels.Stats;

namespace FPD.UI.Windows
{
    /// <summary>
    /// Interaction logic for TimeListView.xaml
    /// </summary>
    public partial class AccountStatsControl : ContentControl
    {
        /// <summary>
        /// Construct an instance.
        /// </summary>
        public AccountStatsControl()
        {
            InitializeComponent();
        }

        private void DataGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete || e.Key == Key.Back)
            {
                if (e.OriginalSource is DataGridCell)
                {
                    if (DataContext != null && DataContext is TimeListViewModel vm)
                    {
                        vm.DeleteValuation();
                    }
                }
            }
        }

        private void DataGrid_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            if (DataContext != null && DataContext is TimeListViewModel vm)
            {
                e.NewItem = vm.DefaultNewItem();
            }
        }

        private void UC_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            string bridgeName = "bridge";
            if (Resources.Contains(bridgeName)
                && DataContext is AccountStatsViewModel dc
                && Resources[bridgeName] is Bridge bridge)
            {
                bridge.Styles = dc.Styles;
            }
        }
    }
}
