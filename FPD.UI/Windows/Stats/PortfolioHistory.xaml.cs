using System.Windows;
using System.Windows.Controls;

using FPD.Logic.ViewModels.Stats;

namespace FPD.UI.Windows.Stats
{
    /// <summary>
    /// Interaction logic for PortfolioHistory.xaml
    /// </summary>
    public partial class PortfolioHistory : UserControl
    {
        /// <summary>
        /// Construct an instance.
        /// </summary>
        public PortfolioHistory()
        {
            InitializeComponent();
        }
        private void UC_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            string bridgeName = "bridge";
            if (Resources.Contains(bridgeName)
                && DataContext is PortfolioHistoryViewModel dc
                && Resources[bridgeName] is Bridge bridge)
            {
                bridge.Styles = dc.Styles;
            }
        }
    }
}
