using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Effanville.Common.Structure.DisplayClasses;
using Effanville.Common.UI;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.Database.Extensions.Statistics;
using Effanville.FinancialStructures.Database.Statistics;
using Effanville.FPD.Logic.Configuration;
using Effanville.FPD.Logic.ViewModels.Common;

namespace Effanville.FPD.Logic.ViewModels.Stats
{
    /// <summary>
    /// View model for the statistics display.
    /// </summary>
    public sealed class StatsViewModel : DataDisplayViewModelBase
    {
        private bool _updateDataInProgress;

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

        private ObservableCollection<List<string>> _statsDisplay;

        public ObservableCollection<List<string>> StatsDisplay
        {
            get => _statsDisplay;
            set
            {
                if (SetAndNotify(ref _statsDisplay, value))
                    StatisticsChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(StatsDisplay)));
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

        public List<Statistic> StatsToView { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public StatsViewModel(UiGlobals globals, IConfiguration userConfiguration, IPortfolio portfolio, Account account = Account.All)
            : base(globals, userConfiguration, portfolio, null, "Statistics", account)
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

            StatsToView = StatisticNames
                .Where(stat => stat.Selected)
                .Select(stat => stat.Instance)
                .ToList();

            StatisticNames.ForEach(stat => stat.SelectedChanged += OnSelectedChanged);
            PropertyChanged += OnPropertyChanged;
        }

        /// <summary>
        /// Update to regenerate the statistics displayed if required.
        /// </summary>
        private async void OnSelectedChanged(object sender, EventArgs e)
        {
            UserConfiguration.StoreConfiguration(this);
            StatsToView = StatisticNames.Where(stat => stat.Selected).Select(stat => stat.Instance).ToList();
            if (!_updateDataInProgress)
            {
                await Task.Run(() => UpdateData(null, true));
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

        /// <inheritdoc/>
        public override async void UpdateData(IPortfolio modelData, bool force)
        {
            if (!_updateDataInProgress)
            {
                _updateDataInProgress = true;
                if (modelData != null)
                {
                    base.UpdateData(modelData, false);
                }

                if (!force
                    && (StatsDisplay?.Count > 4
                        && (!ModelData?.IsAlteredSinceSave ?? true)))
                {
                    return;
                }

                List<AccountStatistics> stats = ModelData.GetStats(DateTime.Today, DataType, DisplayValueFunds, statisticsToDisplay: StatsToView);
                List<List<string>> statsDisplay = stats.Select(x => x.Statistics.Select(y => y?.ToString()).ToList()).ToList();
                DisplayGlobals.CurrentDispatcher?.BeginInvoke(() => AssignStatsDisplay(statsDisplay));

                _updateDataInProgress = false;
                return;

                void AssignStatsDisplay(List<List<string>> statistics)
                    => StatsDisplay = new ObservableCollection<List<string>>(statistics);
            }
        }
    }
}
