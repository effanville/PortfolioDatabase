﻿using System;
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
        private List<Labelled<TwoName, DailyValuation>> _securitiesInvestments;

        /// <summary>
        /// The investments into the securities.
        /// </summary>
        public List<Labelled<TwoName, DailyValuation>> SecuritiesInvestments
        {
            get => _securitiesInvestments;
            set => SetAndNotify(ref _securitiesInvestments, value);
        }

        /// <summary>
        /// Construct an instance
        /// </summary>
        public SecurityInvestmentViewModel(IPortfolio portfolio, UiStyles styles)
            : base(null, styles, portfolio, "Investments", closable: true)
        {
            UpdateData(portfolio);
        }

        /// <summary>
        /// The update routine. Updates the values stored.
        /// <inheritdoc/>
        /// </summary>
        public override void UpdateData(IPortfolio modelData)
        {            
            if (!modelData.Equals(ModelData))
            {
                OnRequestClose(EventArgs.Empty);
                return;
            }
            
            base.UpdateData(modelData);
            SecuritiesInvestments = ModelData.TotalInvestments(Totals.Security);
        }
    }
}
