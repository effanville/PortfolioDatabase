using System.Collections.Generic;
using System.Linq;

using Effanville.Common.UI.ViewModelBases;
using Effanville.FinancialStructures.Database.Statistics;

namespace Effanville.FPD.Logic.ViewModels.Stats
{
    public sealed class AccountStatsViewModel : ViewModelBase<AccountStatistics>
    {
        private List<IStatistic> _statistics;
        public List<IStatistic> Statistics
        {
            get => _statistics;
            set => SetAndNotify(ref _statistics, value);
        }

        public AccountStatsViewModel(AccountStatistics accStats)
        : base("Account Stats", accStats, null)
        {
        }

        /// <summary>
        /// Routine to update the data in the display.
        /// </summary>
        public override void UpdateData(AccountStatistics accStats, bool force)
        {
            Statistics = null;
            Statistics = accStats?.Statistics?.ToList() ?? new List<IStatistic>();
        }
    }
}
