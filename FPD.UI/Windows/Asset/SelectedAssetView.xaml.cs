using System.Windows.Controls;
using FPD.Logic.ViewModels.Asset;

namespace FPD.UI.Windows.Asset
{
    /// <summary>
    /// Interaction logic for SelectedSecurityView.xaml
    /// </summary>
    public partial class SelectedAssetView : UserControl
    {
        /// <summary>
        /// Construct an instance.
        /// </summary>
        public SelectedAssetView()
        {
            InitializeComponent();
        }
        private void UC_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            string bridgeName = "bridge";
            if (Resources.Contains(bridgeName)
                && DataContext is SelectedAssetViewModel dc
                && Resources[bridgeName] is Bridge bridge)
            {
                bridge.Styles = dc.Styles;
            }
        }
    }
}
