using System.Windows.Controls;

using Effanville.FPD.Logic.ViewModels.Common;

namespace Effanville.FPD.UI.Windows.Common
{
    /// <summary>
    /// Interaction logic for SingleValueEditWindow.xaml
    /// </summary>
    public partial class SingleValueEditWindow : Grid
    {
        /// <summary>
        /// Construct an instance.
        /// </summary>
        public SingleValueEditWindow()
        {
            InitializeComponent();
        }

        private void TabMain_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is ValueListWindowViewModel valueListWindowViewModel)
            {
                var addedItems = e.AddedItems;
                valueListWindowViewModel.SelectionChanged.Execute(addedItems);
            }
        }
    }
}
