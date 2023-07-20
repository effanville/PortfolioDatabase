using System.Windows.Controls;
using System.Windows.Input;

using FPD.Logic.ViewModels.Common;

namespace FPD.UI.Windows
{
    /// <summary>
    /// Interaction logic for DataNamesView.xaml
    /// </summary>
    public partial class DataNamesView : UserControl
    {
        /// <summary>
        /// Construct an instance.
        /// </summary>
        public DataNamesView()
        {
            InitializeComponent();
        }

        private void DataGrid_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            if (DataContext != null && DataContext is DataNamesViewModel vm && vm.DataNames != null)
            {
                e.NewItem = vm.DefaultRow();
            }
        }

        private void DataGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete || e.Key == Key.Back)
            {
                if (e.OriginalSource is DataGridCell)
                {
                    if (DataContext != null && DataContext is DataNamesViewModel vm)
                    {
                        vm.ExecuteDelete();
                    }
                }
            }
        }
    }
}
