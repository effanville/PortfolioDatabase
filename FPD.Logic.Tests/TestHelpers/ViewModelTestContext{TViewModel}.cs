using System;

using Common.Structure.DataEdit;
using Common.UI;
using Common.UI.ViewModelBases;

using FinancialStructures.Database;
using FinancialStructures.NamingStructures;

namespace FPD.Logic.Tests.TestHelpers
{
    internal sealed class ViewModelTestContext<TViewModel>
        : ViewModelTestContext<TViewModel, IPortfolio>
        where TViewModel : ViewModelBase<IPortfolio>
    {
        public IPortfolio Portfolio => DataStore;

        public ViewModelTestContext(
            NameData name,
            IPortfolio dataStore,
            Func<UiGlobals, IPortfolio, NameData, IUpdater<IPortfolio>, TViewModel> vmGenerator)
            : base(name, dataStore, vmGenerator)
        {
        }
    }
}
