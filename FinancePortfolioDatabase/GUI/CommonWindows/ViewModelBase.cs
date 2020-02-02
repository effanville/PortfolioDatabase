using FinancialStructures.Database;
using FinancialStructures.FinanceStructures;
using FinancialStructures.GUIFinanceStructures;
using GUISupport;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FinanceCommonViewModels
{
    public abstract class ViewModelBase : PropertyChangedBase
    {
        public abstract void UpdateData(Portfolio portfolio, List<Sector> sectors);

        public abstract Action<NameData> LoadSelectedTab { get; set; }
    }
}
