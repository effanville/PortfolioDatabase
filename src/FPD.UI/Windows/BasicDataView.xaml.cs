using System;
using System.Windows.Controls;
using System.Windows.Input;

using Effanville.FinancialStructures.DataStructures;
using Effanville.FPD.Logic.ViewModels;

namespace Effanville.FPD.UI.Windows
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class BasicDataView : UserControl
    {
        /// <summary>
        /// Construct an instance.
        /// </summary>
        public BasicDataView()
        {
            InitializeComponent();
        }

        private void DataGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete || e.Key == Key.Back)
            {
                if (e.OriginalSource is DataGridCell)
                {
                    if (DataContext is BasicDataViewModel vm)
                    {
                        vm.DeleteSelectedNote();
                    }
                }
            }
        }

        private void DataGrid_AddingNewItem(object sender, AddingNewItemEventArgs e) 
            => e.NewItem = new Note()
            {
                TimeStamp = DateTime.Today
            };

        private void UC_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (Resources.Contains(DisplayConstants.StyleBridgeName)
                && DataContext is BasicDataViewModel dc
                && Resources[DisplayConstants.StyleBridgeName] is Bridge bridge)
            {
                bridge.Styles = dc.Styles;
            }
        }
    }
}
