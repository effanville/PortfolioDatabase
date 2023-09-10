using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Common.Structure.DisplayClasses;
using Common.UI;

using FPD.Logic.Configuration;
using FPD.Logic.TemplatesAndStyles;
using FPD.Logic.ViewModels.Common;

using FinancialStructures.Database;
using FinancialStructures.Database.Statistics;
using System.Threading.Tasks;

using FinancialStructures.Database.Extensions.Statistics;

namespace FPD.Logic.ViewModels.Stats
{
    /// <summary>
    /// View model for the statistics display.
    /// </summary>
    public sealed class StatsViewModel : DataDisplayViewModelBase
    {
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
            set => SetAndNotify(ref _displayValueFunds, value, nameof(DisplayValueFunds));
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
        public StatsViewModel(UiGlobals globals, UiStyles styles, IConfiguration userConfiguration, IPortfolio portfolio, Account account = Account.All, Statistic[] statsToView = null)
            : base(globals, styles, userConfiguration, portfolio, "Statistics", account)
        {
            StatisticNames = statsToView != null
                ? statsToView.Select(stat => new Selectable<Statistic>(stat, true)).ToList()
                : AccountStatisticsHelpers.AllStatistics().Select(stat => new Selectable<Statistic>(stat, true)).ToList();
            if (UserConfiguration.HasLoaded)
            {
                UserConfiguration.RestoreFromConfiguration(this);
            }
            else
            {
                UserConfiguration.StoreConfiguration(this);
                UserConfiguration.HasLoaded = true;
            }

            _statsToView = StatisticNames.Where(stat => stat.Selected).Select(stat => stat.Instance).ToArray();
            UpdateData(portfolio);

            StatisticNames.ForEach(stat => stat.SelectedChanged += OnSelectedChanged);
            PropertyChanged += OnPropertyChanged;
        }

        /// <summary>
        /// Update to regenerate the statistics displayed if required.
        /// </summary>
        private void OnSelectedChanged(object sender, EventArgs e)
        {
            UserConfiguration.StoreConfiguration(this);
            _statsToView = StatisticNames.Where(stat => stat.Selected).Select(stat => stat.Instance).ToArray();
            UpdateData(null);
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
            await Task.Run(() => UpdateData(null));
        }

        /// <inheritdoc/>
        public override void UpdateData(IPortfolio modelData)
        {
            if (modelData != null)
            {
                base.UpdateData(modelData);
            }

            var stats = ModelData.GetStats(DateTime.Today, DataType, DisplayValueFunds, statisticsToDisplay: _statsToView);
            DisplayGlobals.CurrentDispatcher?.BeginInvoke(() => AssignStats(stats));
            return;

            void AssignStats(List<AccountStatistics> statistics)
            {
                Stats = statistics;
            }
        }
    }
}
