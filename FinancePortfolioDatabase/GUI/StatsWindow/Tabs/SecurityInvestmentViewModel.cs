﻿using System.Collections.Generic;
using FinancialStructures.Database;
using FinancialStructures.Database.Statistics;
using FinancialStructures.DataStructures;

namespace FinanceViewModels.StatsViewModels
{
    internal class SecurityInvestmentViewModel : TabViewModelBase
    {
        private List<DayValue_Named> fSecuritiesInvestments;

        public List<DayValue_Named> SecuritiesInvestments
        {
            get
            {
                return fSecuritiesInvestments;
            }
            set
            {
                fSecuritiesInvestments = value;
                OnPropertyChanged();
            }
        }

        public override void GenerateStatistics(bool displayValueFunds)
        {
            DisplayValueFunds = displayValueFunds;
            SecuritiesInvestments = fPortfolio.TotalInvestments(Totals.Security);
        }
        public SecurityInvestmentViewModel(IPortfolio portfolio, bool displayValueFunds)
            : base(portfolio, displayValueFunds)
        {
            Header = "Security Investments";
            GenerateStatistics(displayValueFunds);
        }
    }
}
