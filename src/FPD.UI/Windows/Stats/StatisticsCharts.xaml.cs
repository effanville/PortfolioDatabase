using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Media;

using Effanville.FPD.Logic.ViewModels.Stats;

namespace Effanville.FPD.UI.Windows.Stats
{
    /// <summary>
    /// Interaction logic for StatisticsCharts.xaml
    /// </summary>
    public partial class StatisticsCharts : UserControl
    {
        /// <summary>
        /// Construct an instance.
        /// </summary>
        public StatisticsCharts()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is StatisticsChartsViewModel vm)
            {
                vm.PropertyChanged += VmOnPropertyChanged;
            }
            VmOnPropertyChanged(null, new PropertyChangedEventArgs("IRRLines"));
        }

        private void VmOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IRRLines" && DataContext is StatisticsChartsViewModel vm)
            {
                var rnd = new Random(12345);
                if (vm.IRRLines == null)
                {
                    return;
                }
                foreach (var line in vm.IRRLines)
                {
                    Style style = new Style(typeof(DataPoint));
                    Setter st1 = new Setter(Control.TemplateProperty, null);
                    byte[] b = new byte[3];
                    rnd.NextBytes(b);
                    Setter back = new Setter(Control.BackgroundProperty,
                        new SolidColorBrush(Color.FromRgb(b[0], b[1], b[2])));
                    style.Setters.Add(st1);
                    style.Setters.Add(back);
                    line.DataPointStyle = style;
                }
            }
        }
    }
}
