using Effanville.Common.Structure.DataEdit;
using Effanville.Common.Structure.DataStructures;
using Effanville.Common.UI;
using Effanville.FinancialStructures.Database;
using Effanville.FPD.Logic.TemplatesAndStyles;
using Effanville.FPD.Logic.ViewModels.Common;

namespace Effanville.FPD.Logic.Tests.Context;

public sealed class TimeListTestContext
    : ViewModelTestContext<TimeList, TimeListViewModel>
{
    public bool UpdateCalled { get; set; }
    public object DeleteCalled { get; set; }

    public override void Reset()
    {
        base.Reset();
        UpdateCalled = false;
        DeleteCalled = false;
    }

    public TimeListTestContext(
        IUiStyles uiStyles,
        UiGlobals globals,
        IDataStoreUpdater<IPortfolio> updater,
        IUpdater dataUpdater)
        : base(uiStyles, globals, updater, dataUpdater, null)
    {
    }
}