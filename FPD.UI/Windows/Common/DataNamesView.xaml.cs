using System.Windows.Controls;
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
    }
}
