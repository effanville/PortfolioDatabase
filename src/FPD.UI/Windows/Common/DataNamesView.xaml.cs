using System.Windows.Controls;
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
        public DataNamesView()
        {
            InitializeComponent();
        }

        private void DataGrid_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            if (DataContext  is DataNamesViewModel vm 
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
    }
}
