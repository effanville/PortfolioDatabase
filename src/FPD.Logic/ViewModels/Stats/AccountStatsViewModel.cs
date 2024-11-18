using System.Collections.Generic;
using System.Linq;

using Effanville.FinancialStructures.Database.Statistics;
using Effanville.FPD.Logic.TemplatesAndStyles;
using Effanville.FPD.Logic.ViewModels.Common;

namespace Effanville.FPD.Logic.ViewModels.Stats
{
    public sealed class AccountStatsViewModel : StyledViewModelBase<AccountStatistics>
    {
        private List<IStatistic> _statistics;
        public List<IStatistic> Statistics
        {
            get => _statistics;
            set => SetAndNotify(ref _statistics, value);
        }

        public AccountStatsViewModel(AccountStatistics accStats, IUiStyles styles)
        : base("Account Stats", accStats, null, styles)
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
