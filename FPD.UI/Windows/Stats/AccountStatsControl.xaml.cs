using System.Windows.Controls;
using System.Windows.Input;

using Effanville.FPD.Logic.ViewModels.Common;
using Effanville.FPD.Logic.ViewModels.Stats;

namespace Effanville.FPD.UI.Windows.Stats
{
    /// <summary>
    /// Interaction logic for TimeListView.xaml
    /// </summary>
    public partial class AccountStatsControl : ContentControl
    {
        /// <summary>
        /// Construct an instance.
        /// </summary>
        public AccountStatsControl()
        {
            InitializeComponent();
        }

        private void UC_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (Resources.Contains(DisplayConstants.StyleBridgeName)
                && DataContext is AccountStatsViewModel dc
                && Resources[DisplayConstants.StyleBridgeName] is Bridge bridge)
            {
                bridge.Styles = dc.Styles;
            }
        }
    }
}
