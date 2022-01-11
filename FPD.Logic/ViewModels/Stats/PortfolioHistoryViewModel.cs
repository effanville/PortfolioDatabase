using System.Collections.Generic;
using FPD.Logic.TemplatesAndStyles;
using FPD.Logic.ViewModels.Common;
using FinancialStructures.Database;
using FinancialStructures.Database.Export.History;

namespace FPD.Logic.ViewModels.Stats
{
    public sealed class PortfolioHistoryViewModel : DataDisplayViewModelBase
    {
        public override bool Closable => true;

        private int fHistoryGapDays = 20;
        public int HistoryGapDays
        {
            get => fHistoryGapDays;
            set
            {
                fHistoryGapDays = value;
                OnPropertyChanged();
            }
        }

        private List<PortfolioDaySnapshot> fHistoryStats;
        public List<PortfolioDaySnapshot> HistoryStats
        {
            get => fHistoryStats;
            set
            {
                fHistoryStats = value;
                OnPropertyChanged();
            }
        }

        public override void UpdateData(IPortfolio DataStore)
        {
            base.UpdateData(DataStore);
            PortfolioHistory history = new PortfolioHistory(DataStore, new PortfolioHistorySettings(snapshotIncrement: HistoryGapDays, generateSecurityRates: false, generateSectorRates: false));
            HistoryStats = history.Snapshots;
        }

        public PortfolioHistoryViewModel(IPortfolio portfolio, UiStyles styles)
            : base(null, styles, portfolio, "History")
        {
            UpdateData(portfolio);
        }
    }
}
