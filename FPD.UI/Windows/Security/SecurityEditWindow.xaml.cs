using System.Windows;
using System.Windows.Controls;
using FPD.Logic.ViewModels.Security;

namespace FPD.UI.Windows.Security
{
    /// <summary>
    /// Interaction logic for SecurityEditWindow.xaml
    /// </summary>
    public partial class SecurityEditWindow : Grid
    {
        /// <summary>
        /// Construct an instance.
        /// </summary>
        public SecurityEditWindow()
        {
            InitializeComponent();
        }

        private void CloseTabCommand(object sender, RoutedEventArgs e)
        {
            SecurityEditWindowViewModel VM = DataContext as SecurityEditWindowViewModel;
            var button = e.OriginalSource as System.Windows.Controls.Button;
            if(button != null && VM != null)
            {
                _ = VM.RemoveTab(button.DataContext);
            }
        }
    }
}
