using FinanceWindowsViewModels;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Media;

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
            DataContextChanged += OnDataContextChanged;

        }
        private Random rnd = new Random();
        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is StatsCreatorWindow window)
            {
                if (window.DataContext is StatsCreatorWindowViewModel viewModel)
                {
                    securityCarChart.Series.Clear();
                    var stats = viewModel.HistoryStats;
                    if (stats.Count > 1)
                    {
                        for (int sectorIndex = 0; sectorIndex < stats[0].SectorCar.Count; sectorIndex++)
                        {
                            if (stats[stats.Count - 1].SectorValues[sectorIndex].Value > 5000)
                            {
                                var pc = new List<KeyValuePair<DateTime, double>>();
                                for (int time = 0; time < stats.Count; time++)
                                {
                                    pc.Add(new KeyValuePair<DateTime, double>(stats[time].SectorCar[sectorIndex].Day, stats[time].SectorCar[sectorIndex].Value));
                                }

                                LineSeries series1 = new LineSeries();

                                series1.DependentValuePath = "Value";
                                series1.IndependentValuePath = "Key";
                                series1.ItemsSource = pc;
                                series1.Title = stats[0].SectorCar[sectorIndex].Company;
                                Style style = new Style(typeof(DataPoint));
                                Setter st1 = new Setter(DataPoint.TemplateProperty, null);
                                Byte[] b = new Byte[3];
                                rnd.NextBytes(b);
                                Setter back = new Setter(DataPoint.BackgroundProperty, new SolidColorBrush(Color.FromRgb(b[0], b[1], b[2])));
                                style.Setters.Add(st1);
                                style.Setters.Add(back);
                                series1.DataPointStyle = style;
                                securityCarChart.Series.Add(series1);
                            }
                        }
                    }
                }
            }
        }

        private void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(System.DateTime))
                (e.Column as DataGridTextColumn).Binding.StringFormat = "dd/MM/yyyy";
        }
    }
}
