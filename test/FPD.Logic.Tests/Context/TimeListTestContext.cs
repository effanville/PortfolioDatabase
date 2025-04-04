using Effanville.Common.Structure.DataEdit;
using Effanville.Common.Structure.DataStructures;
using Effanville.Common.UI;
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
        IUpdater updater)
        : base(uiStyles, globals, updater, null, null)
    {
    }
}