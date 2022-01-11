using System.Windows.Controls;
using FPD.Logic.ViewModels.Common;

namespace FPD.UI.Windows
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

        private void CloseTabCommand(object sender, System.Windows.RoutedEventArgs e)
        {
            ValueListWindowViewModel VM = DataContext as ValueListWindowViewModel;
            if (TabMain.SelectedIndex != 0)
            {
                VM.Tabs.RemoveAt(TabMain.SelectedIndex);
            }
        }
    }
}
