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
            var button = e.OriginalSource as System.Windows.Controls.Button;
            if(button != null && VM != null)
            {
                _ = VM.RemoveTab(button.DataContext);
            }
        }
    }
}
