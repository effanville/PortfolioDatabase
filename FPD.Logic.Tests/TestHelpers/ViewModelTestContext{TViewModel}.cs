using System;

using Common.Structure.DataEdit;
using Common.UI;
using Common.UI.ViewModelBases;

using FinancialStructures.Database;
using FinancialStructures.NamingStructures;

namespace FPD.Logic.Tests.TestHelpers
{
    internal sealed class ViewModelTestContext<TData, TViewModel>
        : ViewModelTestContext<TData, TViewModel, IPortfolio>
        where TViewModel : ViewModelBase<TData, IPortfolio>
        where TData : class
    {
        public IPortfolio Portfolio => DataStore;

        public ViewModelTestContext(
            TData data,
            NameData name,
            IPortfolio dataStore,
            Func<TData, UiGlobals, IPortfolio, NameData, IUpdater<IPortfolio>, TViewModel> vmGenerator)
            : base(data, name, dataStore, vmGenerator)
        {
        }
    }
}
