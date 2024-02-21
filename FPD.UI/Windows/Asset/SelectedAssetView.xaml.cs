using System.Windows.Controls;

using Effanville.FPD.Logic.ViewModels.Asset;

namespace Effanville.FPD.UI.Windows.Asset
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
            if (Resources.Contains(DisplayConstants.StyleBridgeName)
                && DataContext is SelectedAssetViewModel dc
                && Resources[DisplayConstants.StyleBridgeName] is Bridge bridge)
            {
                bridge.Styles = dc.Styles;
            }
        }
    }
}
