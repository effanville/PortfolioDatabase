using System.Windows.Controls;
using System.Windows.Input;

using Effanville.FPD.Logic.ViewModels.Security;

namespace Effanville.FPD.UI.Windows.Security
{
    /// <summary>
    /// Interaction logic for SelectedSecurityView.xaml
    /// </summary>
    public partial class SelectedSecurityView : UserControl
    {
        /// <summary>
        /// Construct an instance.
        /// </summary>
        public SelectedSecurityView()
        {
            InitializeComponent();
        }


        private void DataGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete || e.Key == Key.Back)
            {
                if (e.OriginalSource is DataGridCell)
                {
                    if (DataContext is SelectedSecurityViewModel vm)
                    {
                        vm.DeleteTrade();
                    }
                }
            }
        }

        private void DataGrid_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            if (DataContext is SelectedSecurityViewModel vm)
            {
                e.NewItem = vm.DefaultTradeValue();
            }
        }

        private void UC_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            string bridgeName = "bridge";
            if (Resources.Contains(bridgeName)
                && DataContext is SelectedSecurityViewModel dc
                && Resources[bridgeName] is Bridge bridge)
            {
                bridge.Styles = dc.Styles;
            }
        }
    }
}
