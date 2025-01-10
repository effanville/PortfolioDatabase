using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

using Effanville.FPD.Logic.ViewModels.Common;

namespace Effanville.FPD.UI.Windows.Common
{
    /// <summary>
    /// Interaction logic for DataNamesView.xaml
    /// </summary>
    public partial class DataNamesView : UserControl
    {
        /// <summary>
        /// Construct an instance.
        /// </summary>
        public DataNamesView() => InitializeComponent();

        private void DataGrid_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            if (DataContext is DataNamesViewModel vm
                && vm.DataNames != null)
            {
                e.NewItem = vm.DefaultRow();
            }
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

            if (DataContext is DataNamesViewModel vm)
            {
                vm.ExecuteDelete();
            }
        }

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is not DataNamesViewModel dc || sender is not DataGrid dataGrid)
            {
                return;
            }

            object currentItem = dataGrid.CurrentItem;
            dc.SelectionChangedCommand.Execute(currentItem == CollectionView.NewItemPlaceholder
                ? null
                : dataGrid.CurrentItem);
        }

        private void DataGrid_RowDetailsVisibilityChanged(object sender, DataGridRowDetailsEventArgs e)
        {
            if (e.Row.DataContext is not NameDataViewModel rd)
            {
                return;
            }

            if (!rd.IsEditing)
            {
                rd.BeginEdit();
            }
            else
            {
                rd.EndEdit();
            }
        }
    }
}
