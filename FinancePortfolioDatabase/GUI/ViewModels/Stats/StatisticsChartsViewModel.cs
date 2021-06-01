﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Media;
using FinancialStructures.Database;
using FinancialStructures.Database.Statistics;
using FinancialStructures.DataStructures;
using StructureCommon.DataStructures;

namespace FinancePortfolioDatabase.GUI.ViewModels.Stats
{
    internal class StatisticsChartsViewModel : TabViewModelBase
    {
        private readonly Random rnd = new Random();
        private int fHistoryGapDays = 25;
        public int HistoryGapDays
        {
            get
            {
                return fHistoryGapDays;
            }
            set
            {
                fHistoryGapDays = value;
                OnPropertyChanged();
            }
        }

        private List<PortfolioDaySnapshot> fHistoryStats;
        public List<PortfolioDaySnapshot> HistoryStats
        {
            get
            {
                return fHistoryStats;
            }
            set
            {
                fHistoryStats = value;
                OnPropertyChanged();
            }
        }

        private Dictionary<string, DailyValuation> fDistributionValues;
        public Dictionary<string, DailyValuation> SecurityValues
        {
            get
            {
                return fDistributionValues;
            }
            set
            {
                fDistributionValues = value;
                OnPropertyChanged();
            }
        }


        private Dictionary<string, DailyValuation> fDistributionValues2;
        public Dictionary<string, DailyValuation> BankAccountValues
        {
            get
            {
                return fDistributionValues2;
            }
            set
            {
                fDistributionValues2 = value;
                OnPropertyChanged();
            }
        }

        private Dictionary<string, DailyValuation> fDistributionValues3;

        public Dictionary<string, DailyValuation> SectorValues
        {
            get
            {
                return fDistributionValues3;
            }
            set
            {
                fDistributionValues3 = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<LineSeries> fIRRlines = new ObservableCollection<LineSeries>();
        public ObservableCollection<LineSeries> IRRLines
        {
            get
            {
                return fIRRlines;
            }
            set
            {
                fIRRlines = value;
                OnPropertyChanged();
            }
        }

        public override async void GenerateStatistics(bool displayValueFunds)
        {
            DisplayValueFunds = displayValueFunds;
            HistoryStats = await fPortfolio.GenerateHistoryStats(HistoryGapDays).ConfigureAwait(true);
            if (DisplayValueFunds)
            {
                SecurityValues = HistoryStats[HistoryStats.Count - 1].SecurityValues.Where(x => x.Value.Value > 0).ToDictionary(x => x.Key, x => x.Value);
                BankAccountValues = HistoryStats[HistoryStats.Count - 1].BankAccValues.Where(x => x.Value.Value > 0).ToDictionary(x => x.Key, x => x.Value);
                SectorValues = HistoryStats[HistoryStats.Count - 1].SectorValues.Where(x => x.Value.Value > 0).ToDictionary(x => x.Key, x => x.Value);
            }
            else
            {
                SecurityValues = HistoryStats[HistoryStats.Count - 1].SecurityValues;
                BankAccountValues = HistoryStats[HistoryStats.Count - 1].BankAccValues;
                SectorValues = HistoryStats[HistoryStats.Count - 1].SectorValues;
            }

            UpdateChart();
        }

        public void UpdateChart()
        {
            IRRLines = null;
            var newValues = new ObservableCollection<LineSeries>();
            if (HistoryStats.Count > 1)
            {
                var sectorNames = fPortfolio.GetSecuritiesSectors();
                foreach (var name in sectorNames)
                {
                    var total = HistoryStats[HistoryStats.Count - 1].SecurityValue;
                    if (HistoryStats[HistoryStats.Count - 1].SectorValues[name].Value > 0.1 * total.Value)
                    {
                        List<KeyValuePair<DateTime, double>> pc = new List<KeyValuePair<DateTime, double>>();
                        for (int time = 0; time < HistoryStats.Count; time++)
                        {
                            pc.Add(new KeyValuePair<DateTime, double>(HistoryStats[time].SectorCar[name].Day, HistoryStats[time].SectorCar[name].Value));
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
                        Byte[] b = new Byte[3];
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

        public StatisticsChartsViewModel(IPortfolio portfolio, bool displayValueFunds)
            : base(portfolio, displayValueFunds)
        {
            Header = "Charts";
            GenerateStatistics(displayValueFunds);
        }
    }
}