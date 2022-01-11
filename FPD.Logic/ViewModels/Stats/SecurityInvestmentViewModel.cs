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
    public sealed class SecurityInvestmentViewModel : DataDisplayViewModelBase
    {
        public override bool Closable => true;
        private List<Labelled<TwoName, DailyValuation>> fSecuritiesInvestments;

        public List<Labelled<TwoName, DailyValuation>> SecuritiesInvestments
        {
            get => fSecuritiesInvestments;
            set => SetAndNotify(ref fSecuritiesInvestments, value, nameof(SecuritiesInvestments));
        }

        public override void UpdateData(IPortfolio portfolio)
        {
            base.UpdateData(portfolio);
            SecuritiesInvestments = DataStore.TotalInvestments(Totals.Security);
        }

        public SecurityInvestmentViewModel(IPortfolio portfolio, UiStyles styles)
            : base(null, styles, portfolio, "Investments")
        {
            UpdateData(portfolio);
        }
    }
}
