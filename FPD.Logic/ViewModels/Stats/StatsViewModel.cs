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
using FinancialStructures.Database.Extensions.Statistics;
using FinancialStructures.Database.Statistics;
using System.Threading.Tasks;

namespace FPD.Logic.ViewModels.Stats
{
    /// <summary>
    /// View model for the statistics display.
    /// </summary>
    public sealed class StatsViewModel : DataDisplayViewModelBase
    {
        private List<AccountStatistics> fStats;

        private bool fDisplayValueFunds = true;
        private List<Selectable<Statistic>> fStatisticNames;

        /// <summary>
        /// Event handler controlling property changed
        /// </summary>
        public event PropertyChangedEventHandler StatisticsChanged;

        /// <summary>
        /// Should statistics for funds with non zero value be displayed.
        /// </summary>
        public bool DisplayValueFunds
        {
            get => fDisplayValueFunds;
            set => SetAndNotify(ref fDisplayValueFunds, value, nameof(DisplayValueFunds));
        }

        /// <summary>
        /// The values of the statistics being displayed.
        /// </summary>
        public List<AccountStatistics> Stats
        {
            get => fStats;
            set
            {
                SetAndNotify(ref fStats, value, nameof(Stats));
                StatisticsChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Stats)));
            }
        }

        /// <summary>
        /// The statistics to display.
        /// </summary>
        public List<Selectable<Statistic>> StatisticNames
        {
            get => fStatisticNames;
            set => SetAndNotify(ref fStatisticNames, value, nameof(StatisticNames));
        }

        private Statistic[] StatsToView;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public StatsViewModel(UiGlobals globals, UiStyles styles, IConfiguration userConfiguration, IPortfolio portfolio, Account account = Account.All, Statistic[] statsToView = null)
            : base(globals, styles, userConfiguration, portfolio, "Statistics", account)
        {
            StatisticNames = statsToView != null
                ? statsToView.Select(stat => new Selectable<Statistic>(stat, true)).ToList()
                : AccountStatisticsHelpers.AllStatistics().Select(stat => new Selectable<Statistic>(stat, true)).ToList();
            if (fUserConfiguration.HasLoaded)
            {
                fUserConfiguration.RestoreFromConfiguration(this);
            }
            else
            {
                fUserConfiguration.StoreConfiguration(this);
                fUserConfiguration.HasLoaded = true;
            }

            StatsToView = StatisticNames.Where(stat => stat.Selected).Select(stat => stat.Instance).ToArray();
            UpdateData(portfolio);

            StatisticNames.ForEach(stat => stat.SelectedChanged += OnSelectedChanged);
            PropertyChanged += OnPropertyChanged;
        }

        /// <summary>
        /// Update to regenerate the statistics displayed if required.
        /// </summary>
        private void OnSelectedChanged(object sender, EventArgs e)
        {
            fUserConfiguration.StoreConfiguration(this);
            StatsToView = StatisticNames.Where(stat => stat.Selected).Select(stat => stat.Instance).ToArray();
            UpdateData();
        }

        /// <summary>
        /// Update to regenerate the statistics displayed if required.
        /// </summary>
        private async void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(StatisticNames) || e.PropertyName == nameof(DisplayValueFunds))
            {
                fUserConfiguration.StoreConfiguration(this);
                await Task.Run(() => UpdateData());
            }
        }

        /// <inheritdoc/>
        public override void UpdateData(IPortfolio dataToDisplay = null)
        {
            if (dataToDisplay != null)
            {
                base.UpdateData(dataToDisplay);
            }

            var stats = DataStore.GetStats(DateTime.Today, DataType, DisplayValueFunds, statisticsToDisplay: StatsToView);
            fUiGlobals.CurrentDispatcher?.BeginInvoke(() => AssignStats(stats));
            void AssignStats(List<AccountStatistics> stats)
            {
                Stats = stats;
            }
        }
    }
}
