using System.Collections.Generic;
using System.Linq;

using Common.UI.ViewModelBases;

using FinancialStructures.Database.Statistics;

using FPD.Logic.TemplatesAndStyles;

namespace FPD.Logic.ViewModels.Stats
{
    public sealed class AccountStatsViewModel : PropertyChangedBase
    {
        private UiStyles fStyles;

        /// <summary>
        /// The style object containing the style for the ui.
        /// </summary>
        public UiStyles Styles
        {
            get => fStyles;
            set => SetAndNotify(ref fStyles, value, nameof(Styles));
        }

        private List<IStatistic> fStatistics;
        public List<IStatistic> Statistics
        {
            get => fStatistics;
            set => SetAndNotify(ref fStatistics, value, nameof(Statistics));
        }

        public AccountStatsViewModel(AccountStatistics accStats, UiStyles styles)
        {
            Styles = styles;
            UpdateData(accStats);
        }

        /// <summary>
        /// Routine to update the data in the display.
        /// </summary>
        public void UpdateData(AccountStatistics accStats)
        {
            Statistics = null;
            Statistics = accStats?.Statistics.ToList() ?? new List<IStatistic>();
        }
    }
}
