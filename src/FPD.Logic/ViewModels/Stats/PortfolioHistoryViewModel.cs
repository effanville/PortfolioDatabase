﻿using System;
using System.Collections.Generic;

using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.Database.Export.History;
using Effanville.FPD.Logic.TemplatesAndStyles;
using Effanville.FPD.Logic.ViewModels.Common;

namespace Effanville.FPD.Logic.ViewModels.Stats
{
    /// <summary>
    /// Contains settings and ability to generate a history of portfolio object.
    /// </summary>
    public sealed class PortfolioHistoryViewModel : DataDisplayViewModelBase
    {
        private int _historyGapDays = 20;

        /// <summary>
        /// The number of days between history calculations.
        /// </summary>
        public int HistoryGapDays
        {
            get => _historyGapDays;
            set => SetAndNotify(ref _historyGapDays, value);
        }

        private List<PortfolioDaySnapshot> _historyStats;

        /// <summary>
        /// The store of the historical values.
        /// </summary>
        public List<PortfolioDaySnapshot> HistoryStats
        {
            get => _historyStats;
            set => SetAndNotify(ref _historyStats, value);
        }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        public PortfolioHistoryViewModel(IPortfolio portfolio, IUiStyles styles)
            : base(null, styles, portfolio, null, "History", closable: true)
        {
            UpdateData(portfolio, false);
        }

        /// <summary>
        /// Updates the data stored in the history.
        /// </summary>
        public override void UpdateData(IPortfolio modelData, bool force)
        {
            if (!modelData.Equals(ModelData))
            {
                OnRequestClose(EventArgs.Empty);
                return;
            }

            base.UpdateData(modelData, force);
            PortfolioHistory history = new PortfolioHistory(
                modelData,
                new PortfolioHistory.Settings(snapshotIncrement: HistoryGapDays, generateSecurityRates: false,
                    generateSectorRates: false));
            HistoryStats = history.Snapshots;
        }
    }
}