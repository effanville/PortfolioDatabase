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
            if (e.Key != Key.Delete && e.Key != Key.Back)
            {
                return;
            }

            if (e.OriginalSource is not DataGridCell)
            {
                return;
            }

            if (DataContext is SelectedSecurityViewModel vm)
            {
                vm.DeleteTrade();
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
            if (Resources.Contains(DisplayConstants.StyleBridgeName)
                && DataContext is SelectedSecurityViewModel dc
                && Resources[DisplayConstants.StyleBridgeName] is Bridge bridge)
            {
                bridge.Styles = dc.Styles;
            }
        }
    }
}
