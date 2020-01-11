using System;
using System.Windows.Controls;

namespace FinanceWindows
{
    /// <summary>
    /// Interaction logic for StatsCreatorWindow.xaml
    /// </summary>
    public partial class StatsCreatorWindow : Grid
    {
        public StatsCreatorWindow()
        {
            InitializeComponent();
        }

        private void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(System.DateTime))
                (e.Column as DataGridTextColumn).Binding.StringFormat = "dd/MM/yyyy";
        }
    }
}
