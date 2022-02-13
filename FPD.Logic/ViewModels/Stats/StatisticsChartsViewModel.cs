using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Media;
using FPD.Logic.TemplatesAndStyles;
using FPD.Logic.ViewModels.Common;
using FinancialStructures.Database;
using FinancialStructures.Database.Export.History;
using FinancialStructures.Database.Extensions.Values;

namespace FPD.Logic.ViewModels.Stats
{
    /// <summary>
    /// Contains data for chart display.
    /// </summary>
    public sealed class StatisticsChartsViewModel : DataDisplayViewModelBase
    {
        private int fHistoryGapDays = 25;

        /// <summary>
        /// The number of days between points on the charts.
        /// </summary>
        public int HistoryGapDays
        {
            get => fHistoryGapDays;
            set => SetAndNotify(ref fHistoryGapDays, value, nameof(HistoryGapDays));
        }

        private List<PortfolioDaySnapshot> fHistoryStats;

        /// <summary>
        /// The history information.
        /// </summary>
        public List<PortfolioDaySnapshot> HistoryStats
        {
            get => fHistoryStats;
            set => SetAndNotify(ref fHistoryStats, value, nameof(HistoryStats));
        }

        /// <summary>
        /// The current value of each security.
        /// </summary>
        public Dictionary<string, decimal> SecurityValues => HistoryStats?[HistoryStats.Count - 1].SecurityValues
            .Where(x => x.Value > 0).ToDictionary(x => x.Key, x => x.Value)
            ?? new Dictionary<string, decimal>();

        /// <summary>
        /// The current value of each bank account.
        /// </summary>
        public Dictionary<string, decimal> BankAccountValues => HistoryStats?[HistoryStats.Count - 1].BankAccValues
            .Where(x => x.Value > 0).ToDictionary(x => x.Key, x => x.Value)
            ?? new Dictionary<string, decimal>();

        /// <summary>
        /// The current value of each sector.
        /// </summary>
        public Dictionary<string, decimal> SectorValues => HistoryStats?[HistoryStats.Count - 1].SectorValues
            .Where(x => x.Value > 0).ToDictionary(x => x.Key, x => x.Value)
            ?? new Dictionary<string, decimal>();

        private ObservableCollection<LineSeries> fIRRlines = new ObservableCollection<LineSeries>();

        /// <summary>
        /// Specific line values for the rate of return chart.
        /// </summary>
        public ObservableCollection<LineSeries> IRRLines
        {
            get => fIRRlines;
            set => SetAndNotify(ref fIRRlines, value, nameof(IRRLines));
        }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        public StatisticsChartsViewModel(IPortfolio portfolio, UiStyles styles)
            : base(null, styles, portfolio, "Charts", Account.All)
        {
            UpdateData(portfolio);
        }

        /// <summary>
        /// Updates the data for display in the charts.
        /// </summary>
        public override void UpdateData(IPortfolio portfolio)
        {
            if (portfolio != null)
            {
                base.UpdateData(portfolio);
            }

            DateTime firstDate = portfolio.FirstValueDate(Totals.All);
            if (firstDate < new DateTime(1980, 1, 1))
            {
                firstDate = new DateTime(1980, 1, 1);
            }

            DateTime lastDate = portfolio.LatestDate(Totals.All);
            if (lastDate == default(DateTime))
            {
                lastDate = DateTime.Today;
            }

            int numDays = (lastDate - firstDate).Days;
            int snapshotGap = numDays / 100;

            PortfolioHistory history = new PortfolioHistory(
                DataStore,
                new PortfolioHistorySettings(
                    firstDate,
                    lastDate,
                    snapshotIncrement: snapshotGap,
                    generateSecurityRates: false,
                    generateSectorRates: true));
            HistoryStats = history.Snapshots.Where(stat => stat.Date > new DateTime(1000, 1, 1)).ToList();

            UpdateChart();
        }

        /// <summary>
        /// Updates the data for the rate of return chart.
        /// </summary>
        public void UpdateChart()
        {
            var rnd = new Random(12345);
            IRRLines = null;
            ObservableCollection<LineSeries> newValues = new ObservableCollection<LineSeries>();
            if (HistoryStats.Count > 1)
            {
                IReadOnlyList<string> sectorNames = DataStore.Sectors(Account.Security);
                foreach (string name in sectorNames)
                {
                    decimal total = HistoryStats[HistoryStats.Count - 1].SecurityValue;
                    if (HistoryStats[HistoryStats.Count - 1].SectorValues[name] > 0.1m * total)
                    {
                        List<KeyValuePair<DateTime, double>> pc = new List<KeyValuePair<DateTime, double>>();
                        for (int time = 0; time < HistoryStats.Count; time++)
                        {
                            pc.Add(new KeyValuePair<DateTime, double>(HistoryStats[time].Date, HistoryStats[time].CurrentSectorTotalCar[name]));
                        }

                        LineSeries series1 = new LineSeries
                        {
                            DependentValuePath = "Value",
                            IndependentValuePath = "Key",
                            ItemsSource = pc,
                            Title = name
                        };
                        Style style = new Style(typeof(DataPoint));
                        Setter st1 = new Setter(DataPoint.TemplateProperty, null);
                        byte[] b = new byte[3];
                        rnd.NextBytes(b);
                        Setter back = new Setter(DataPoint.BackgroundProperty, new SolidColorBrush(Color.FromRgb(b[0], b[1], b[2])));
                        style.Setters.Add(st1);
                        style.Setters.Add(back);
                        series1.DataPointStyle = style;
                        newValues.Add(series1);
                    }
                }

                IRRLines = newValues;
            }
        }
    }
}
