using Effanville.Common.Structure.DataEdit;
using Effanville.Common.UI;

using FinancialStructures.Database;

using FPD.Logic.TemplatesAndStyles;
using FPD.Logic.ViewModels.Common;

namespace FPD.Logic.Tests.Context;

public sealed class DataNamesViewModelTestContext : ViewModelTestContext<IPortfolio, DataNamesViewModel>
{
    public bool LoadDataCalled { get; set; }

    public override void Reset()
    {
        base.Reset();
        LoadDataCalled = false;
    }

    public DataNamesViewModelTestContext(
        UiStyles uiStyles,
        UiGlobals globals,
        IUpdater<IPortfolio> updater)
        : base(uiStyles, globals, updater, null)
    {
    }
}