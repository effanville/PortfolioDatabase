using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Effanville.Common.Structure.DisplayClasses;
using Effanville.Common.Structure.Reporting;
using Effanville.Common.UI;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.Database.Extensions.Statistics;
using Effanville.FinancialStructures.Database.Statistics;
using Effanville.FPD.Logic.Configuration;
using Effanville.FPD.Logic.TemplatesAndStyles;
using Effanville.FPD.Logic.ViewModels.Common;

namespace Effanville.FPD.Logic.ViewModels.Stats
{
    /// <summary>
    /// View model for the statistics display.
    /// </summary>
    public sealed class StatsViewModel : DataDisplayViewModelBase
    {
        private bool _updateDataInProgress = false;
        private List<AccountStatistics> _stats;

        private bool _displayValueFunds = true;
        private List<Selectable<Statistic>> _statisticNames;

        /// <summary>
        /// Event handler controlling property changed
        /// </summary>
        public event PropertyChangedEventHandler StatisticsChanged;

        /// <summary>
        /// Should statistics for funds with non zero value be displayed.
        /// </summary>
        public bool DisplayValueFunds
        {
            get => _displayValueFunds;
            set => SetAndNotify(ref _displayValueFunds, value);
        }

        /// <summary>
        /// The values of the statistics being displayed.
        /// </summary>
        public List<AccountStatistics> Stats
        {
            get => _stats;
            set
            {
                SetAndNotify(ref _stats, value);
                StatisticsChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Stats)));
            }
        }

        /// <summary>
        /// The statistics to display.
        /// </summary>
        public List<Selectable<Statistic>> StatisticNames
        {
            get => _statisticNames;
            set => SetAndNotify(ref _statisticNames, value);
        }

        private Statistic[] _statsToView;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public StatsViewModel(UiGlobals globals, UiStyles styles, IConfiguration userConfiguration, IPortfolio portfolio, Account account = Account.All)
            : base(globals, styles, userConfiguration, portfolio, "Statistics", account)
        {
            StatisticNames = AccountStatisticsHelpers.AllStatistics()
                .Select(stat =>
                    {
                        if (stat == Statistic.DrawDown || stat == Statistic.MDD)
                        {
                            return new Selectable<Statistic>(stat, false);
                        }
                        
                        return new Selectable<Statistic>(stat, true);
                    })
                .ToList();
            if (UserConfiguration.HasLoaded)
            {
                UserConfiguration.RestoreFromConfiguration(this);
            }
            else
            {
                UserConfiguration.StoreConfiguration(this);
                UserConfiguration.HasLoaded = true;
            }

            _statsToView = StatisticNames
                .Where(stat => stat.Selected)
                .Select(stat => stat.Instance)
                .ToArray();

            StatisticNames.ForEach(stat => stat.SelectedChanged += OnSelectedChanged);
            PropertyChanged += OnPropertyChanged;
        }

        /// <summary>
        /// Update to regenerate the statistics displayed if required.
        /// </summary>
        private async void OnSelectedChanged(object sender, EventArgs e)
        {
            UserConfiguration.StoreConfiguration(this);
            _statsToView = StatisticNames.Where(stat => stat.Selected).Select(stat => stat.Instance).ToArray();
            if (!_updateDataInProgress)
            {
                await Task.Run(() => UpdateData(null, false));
            }
        }

        /// <summary>
        /// Update to regenerate the statistics displayed if required.
        /// </summary>
        private async void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(StatisticNames) && e.PropertyName != nameof(DisplayValueFunds))
            {
                return;
            }

            UserConfiguration.StoreConfiguration(this);
            if (!_updateDataInProgress)
            {
                _updateDataInProgress = true;
                await Task.Run(() => UpdateData(null, force: true));
                _updateDataInProgress = false;
            }
        }

        private void UpdateDataInternal(IPortfolio modelData, bool force)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            stopwatch.Start();
            if (modelData != null)
            {
                base.UpdateData(modelData, false);
            }

            if ((Stats?.Count > 4 && (!ModelData?.IsAlteredSinceSave ?? true)) && !force)
            {
                return;
            }

            var stats = ModelData.GetStats(DateTime.Today, DataType, DisplayValueFunds, statisticsToDisplay: _statsToView);
            DisplayGlobals.CurrentDispatcher?.BeginInvoke(() => AssignStats(stats));
            stopwatch.Stop();
            ReportLogger.Log(ReportSeverity.Critical, ReportType.Information, "here", $"Elapsed is {stopwatch.Elapsed.TotalMilliseconds}ms");
            return;

            void AssignStats(List<AccountStatistics> statistics)
            {
                Stats = statistics;
            }
        }

        /// <inheritdoc/>
        public override async void UpdateData(IPortfolio modelData, bool force)
        {
            if (!_updateDataInProgress)
            {
                _updateDataInProgress = true;
                await Task.Run(() => UpdateDataInternal(null, force));
                _updateDataInProgress = false;
            }
        }
    }
}
