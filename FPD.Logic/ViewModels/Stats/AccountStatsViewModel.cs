﻿using System.Collections.Generic;
using System.Linq;

using FinancialStructures.Database.Statistics;

using FPD.Logic.TemplatesAndStyles;
using FPD.Logic.ViewModels.Common;

namespace FPD.Logic.ViewModels.Stats
{
    public sealed class AccountStatsViewModel : StyledViewModelBase<AccountStatistics>
    {
        private List<IStatistic> _statistics;
        public List<IStatistic> Statistics
        {
            get => _statistics;
            set => SetAndNotify(ref _statistics, value);
        }

        public AccountStatsViewModel(AccountStatistics accStats, UiStyles styles)
        : base("Account Stats", accStats, null, styles)
        {
        }

        /// <summary>
        /// Routine to update the data in the display.
        /// </summary>
        public override void UpdateData(AccountStatistics accStats)
        {
            Statistics = null;
            Statistics = accStats?.Statistics.ToList() ?? new List<IStatistic>();
        }
    }
}
