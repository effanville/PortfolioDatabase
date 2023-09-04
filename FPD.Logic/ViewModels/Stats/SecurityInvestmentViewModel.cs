using System.Collections.Generic;

using Common.Structure.DataStructures;
using Common.Structure.NamingStructures;

using FPD.Logic.TemplatesAndStyles;
using FPD.Logic.ViewModels.Common;
using FinancialStructures.Database;
using FinancialStructures.Database.Extensions.Values;
using FinancialStructures.NamingStructures;

namespace FPD.Logic.ViewModels.Stats
{
    /// <summary>
    /// Contains data and ability to export the investments into the portfolio.
    /// </summary>
    public sealed class SecurityInvestmentViewModel : DataDisplayViewModelBase
    {
        /// <inheritdoc/>
        public override bool Closable => true;

        private List<Labelled<TwoName, DailyValuation>> fSecuritiesInvestments;

        /// <summary>
        /// The investments into the securities.
        /// </summary>
        public List<Labelled<TwoName, DailyValuation>> SecuritiesInvestments
        {
            get => fSecuritiesInvestments;
            set => SetAndNotify(ref fSecuritiesInvestments, value, nameof(SecuritiesInvestments));
        }

        /// <summary>
        /// Construct an instance
        /// </summary>
        public SecurityInvestmentViewModel(IPortfolio portfolio, UiStyles styles)
            : base(null, styles, portfolio, "Investments")
        {
            UpdateData(portfolio);
        }

        /// <summary>
        /// The update routine. Updates the values stored.
        /// <inheritdoc/>
        /// </summary>
        public override void UpdateData(IPortfolio modelData)
        {
            base.UpdateData(modelData);
            SecuritiesInvestments = ModelData.TotalInvestments(Totals.Security);
        }

    }
}
