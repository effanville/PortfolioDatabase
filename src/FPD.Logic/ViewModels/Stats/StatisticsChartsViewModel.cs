using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Media;

using Effanville.Common.UI;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.Database.Export.History;
using Effanville.FinancialStructures.Database.Extensions.Values;
using Effanville.FPD.Logic.TemplatesAndStyles;
using Effanville.FPD.Logic.ViewModels.Common;

namespace Effanville.FPD.Logic.ViewModels.Stats
{
    /// <summary>
    /// Contains data for chart display.
    /// </summary>
    public sealed class StatisticsChartsViewModel : DataDisplayViewModelBase
    {
        private int _historyGapDays;
        private DateTime _earliestViewDate = new DateTime(DateTime.Today.AddYears(-10).Year, 1, 1);
        private List<PortfolioDaySnapshot> _historyStats;

        public int HistoryGapDays
        {
            get => _historyGapDays;
            set => SetAndNotify(ref _historyGapDays, value);
        }

        public DateTime EarliestViewDate
        {
            get => _earliestViewDate;
            set => SetAndNotify(ref _earliestViewDate, value);
        }

        /// <summary>
        /// The history information.
        /// </summary>
        public List<PortfolioDaySnapshot> HistoryStats
        {
            get => _historyStats;
            set => SetAndNotify(ref _historyStats, value);
        }

        /// <summary>
        /// The current value of each security.
        /// </summary>
        public Dictionary<string, decimal> SecurityValues
            => HistoryStats?.Count > 0
                ? HistoryStats?[HistoryStats.Count - 1].SecurityValues
                    .Where(x => x.Value > 0)
                    .ToDictionary(x => x.Key, x => x.Value)
                : new Dictionary<string, decimal>();

        /// <summary>
        /// The current value of each bank account.
        /// </summary>
        public Dictionary<string, decimal> BankAccountValues
            => HistoryStats?.Count > 0
                ? HistoryStats[HistoryStats.Count - 1].BankAccValues
                    .Where(x => x.Value > 0)
                    .ToDictionary(x => x.Key, x => x.Value)
                : new Dictionary<string, decimal>();

        /// <summary>
        /// The current value of each sector.
        /// </summary>
        public Dictionary<string, decimal> SectorValues
            => HistoryStats?.Count > 0
                ? HistoryStats[HistoryStats.Count - 1].SectorValues
                    .Where(x => x.Value > 0)
                    .ToDictionary(x => x.Key, x => x.Value)
                : new Dictionary<string, decimal>();

        private ObservableCollection<LineSeries> _IRRlines = new ObservableCollection<LineSeries>();

        /// <summary>
        /// Specific line values for the rate of return chart.
        /// </summary>
        public ObservableCollection<LineSeries> IRRLines
        {
            get => _IRRlines;
            set => SetAndNotify(ref _IRRlines, value);
        }

        private ObservableCollection<StackedAreaSeries> _totalLines = new ObservableCollection<StackedAreaSeries>();

        /// <summary>
        /// Specific line values for the rate of return chart.
        /// </summary>
        public ObservableCollection<StackedAreaSeries> TotalLines
        {
            get => _totalLines;
            set => SetAndNotify(ref _totalLines, value);
        }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        public StatisticsChartsViewModel(UiGlobals uiGlobals, IPortfolio portfolio, IUiStyles styles)
            : base(uiGlobals, styles, portfolio, "Charts", Account.All)
        {
            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(EarliestViewDate)
                || e.PropertyName == nameof(HistoryGapDays))
            {
                UpdateData(force: true);
            }
        }

        /// <summary>
        /// Updates the data for display in the charts.
        /// </summary>
        public override void UpdateData(IPortfolio modelData, bool force)
        {
            if (modelData != null)
            {
                base.UpdateData(modelData, force);
            }
            
            UpdateData(force);
        }

        private void UpdateData(bool force = false)
        {
            if (!force && (HistoryStats?.Count > 4 && (!ModelData?.IsAlteredSinceSave ?? true)))
            {
                return;
            }

            DateTime firstDate = ModelData.FirstValueDate(Totals.All);
            if (firstDate < new DateTime(1980, 1, 1))
            {
                firstDate = new DateTime(1980, 1, 1);
            }

            DateTime lastDate = ModelData.LatestDate(Totals.All);
            if (lastDate == default)
            {
                lastDate = DateTime.Today;
            }

            int numDays = (lastDate - firstDate).Days;
            if (HistoryGapDays == 0 && numDays > 0)
            { 
                HistoryGapDays = numDays / 100;
            }

            firstDate = firstDate < EarliestViewDate ? EarliestViewDate : firstDate;
            PortfolioHistory history = new PortfolioHistory(
                ModelData,
                new PortfolioHistory.Settings(
                    firstDate,
                    lastDate,
                    snapshotIncrement: HistoryGapDays,
                    generateSecurityRates: false,
                    generateSectorRates: true));
            HistoryStats = history.Snapshots.Where(stat => stat.Date > new DateTime(1000, 1, 1)).ToList();

            DisplayGlobals.CurrentDispatcher?.BeginInvoke(UpdateChart);
        }

        /// <summary>
        /// Updates the data for the rate of return chart.
        /// </summary>
        private void UpdateChart()
        {
            var rnd = new Random(12345);
            IRRLines = null;
            ObservableCollection<LineSeries> newValues = new ObservableCollection<LineSeries>();
            if (HistoryStats.Count > 1)
            {
                IReadOnlyList<string> sectorNames = ModelData.Sectors(Account.Security);
                foreach (string name in sectorNames)
                {
                    decimal total = HistoryStats[HistoryStats.Count - 1].SecurityValue;
                    if (HistoryStats[HistoryStats.Count - 1].SectorValues[name] <= 0.1m * total)
                    {
                        continue;
                    }

                    List<KeyValuePair<DateTime, double>> pc = new List<KeyValuePair<DateTime, double>>();
                    for (int time = 0; time < HistoryStats.Count; time++)
                    {
                        pc.Add(new KeyValuePair<DateTime, double>(HistoryStats[time].Date,
                            HistoryStats[time].CurrentSectorTotalCar[name]));
                    }

                    LineSeries series1 = new LineSeries
                    {
                        DependentValuePath = "Value", IndependentValuePath = "Key", ItemsSource = pc, Title = name
                    };
                    Style style = new Style(typeof(DataPoint));
                    Setter st1 = new Setter(Control.TemplateProperty, null);
                    byte[] b = new byte[3];
                    rnd.NextBytes(b);
                    Setter back = new Setter(Control.BackgroundProperty,
                        new SolidColorBrush(Color.FromRgb(b[0], b[1], b[2])));
                    style.Setters.Add(st1);
                    style.Setters.Add(back);
                    series1.DataPointStyle = style;
                    newValues.Add(series1);
                }

                IRRLines = newValues;
            }

            // Now populate the total value chart.
            TotalLines = null;
            ObservableCollection<StackedAreaSeries> newTotalValues = new ObservableCollection<StackedAreaSeries>();
            if (HistoryStats.Count <= 1)
            {
                return;
            }

            List<KeyValuePair<DateTime, double>> bankAccTotalValues = new List<KeyValuePair<DateTime, double>>();
            List<KeyValuePair<DateTime, double>> secTotalValues = new List<KeyValuePair<DateTime, double>>();
            List<KeyValuePair<DateTime, double>> assetTotalValues = new List<KeyValuePair<DateTime, double>>();
            List<KeyValuePair<DateTime, double>> pensionTotalValues = new List<KeyValuePair<DateTime, double>>();
            for (int time = 0; time < HistoryStats.Count; time++)
            {
                double bankAccTotal = decimal.ToDouble(HistoryStats[time].BankAccValue);
                bankAccTotalValues.Add(new KeyValuePair<DateTime, double>(HistoryStats[time].Date, bankAccTotal));
                double secTotal = decimal.ToDouble(HistoryStats[time].SecurityValue);
                secTotalValues.Add(new KeyValuePair<DateTime, double>(HistoryStats[time].Date, secTotal));
                double assetTotal = decimal.ToDouble(HistoryStats[time].AssetValue);
                assetTotalValues.Add(new KeyValuePair<DateTime, double>(HistoryStats[time].Date, assetTotal));
                double pensionTotal = decimal.ToDouble(HistoryStats[time].PensionValue);
                pensionTotalValues.Add(new KeyValuePair<DateTime, double>(HistoryStats[time].Date, pensionTotal));
            }

            var bankAccSeries = new SeriesDefinition
            {
                ItemsSource = bankAccTotalValues,
                DependentValuePath = "Value",
                IndependentValuePath = "Key",
                Title = "Bank Accounts"
            };
            var secSeries = new SeriesDefinition
            {
                DependentValuePath = "Value",
                IndependentValuePath = "Key",
                ItemsSource = secTotalValues,
                Title = "Securities"
            };
            var pensionSeries = new SeriesDefinition
            {
                DependentValuePath = "Value",
                IndependentValuePath = "Key",
                ItemsSource = pensionTotalValues,
                Title = "Pensions"
            };
            var assetSeries = new SeriesDefinition
            {
                DependentValuePath = "Value",
                IndependentValuePath = "Key",
                ItemsSource = assetTotalValues,
                Title = "Assets"
            };

            var stackedAreaSeries = new StackedAreaSeries();
            stackedAreaSeries.SeriesDefinitions.Add(bankAccSeries);
            stackedAreaSeries.SeriesDefinitions.Add(secSeries);
            stackedAreaSeries.SeriesDefinitions.Add(pensionSeries);
            stackedAreaSeries.SeriesDefinitions.Add(assetSeries);

            newTotalValues.Add(stackedAreaSeries);
            TotalLines = newTotalValues;
        }
    }
}