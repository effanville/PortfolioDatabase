using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Effanville.FPD.Logic.ViewModels.Common;

namespace Effanville.FPD.UI.Windows.Common
{
    /// <summary>
    /// Interaction logic for TimeListView.xaml
    /// </summary>
    public partial class TimeListView : ContentControl
    {
        /// <summary>
        /// Construct an instance.
        /// </summary>
        public TimeListView()
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

        private void UC_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            string bridgeName = "bridge";
            if (Resources.Contains(bridgeName)
                && DataContext is TimeListViewModel dc
                && Resources[bridgeName] is Bridge bridge)
            {
                bridge.Styles = dc.Styles;
            }
        }
    }
}
