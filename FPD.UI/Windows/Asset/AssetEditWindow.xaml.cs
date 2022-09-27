using System.Windows;
using System.Windows.Controls;
using FPD.Logic.ViewModels.Asset;

namespace FPD.UI.Windows.Asset
{
    /// <summary>
    /// Interaction logic for SecurityEditWindow.xaml
    /// </summary>
    public partial class AssetEditWindow : Grid
    {
        /// <summary>
        /// Construct an instance.
        /// </summary>
        public AssetEditWindow()
        {
            InitializeComponent();
        }

        private void CloseTabCommand(object sender, RoutedEventArgs e)
        {
            AssetEditWindowViewModel VM = DataContext as AssetEditWindowViewModel;
            if (TabMain.SelectedIndex != 0)
            {
                VM.Tabs.RemoveAt(TabMain.SelectedIndex);
            }
        }
    }
}
