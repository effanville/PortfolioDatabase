using Common.Structure.DataEdit;
using Common.Structure.DataStructures;
using Common.UI;

using FinancialStructures.Database;

using FPD.Logic.TemplatesAndStyles;
using FPD.Logic.ViewModels.Common;

namespace FPD.Logic.Tests.Context;

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
        UiStyles uiStyles,
        UiGlobals globals,
        IUpdater<IPortfolio> updater) 
        : base(uiStyles, globals, updater, null)
    {
    }
}