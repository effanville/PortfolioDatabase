using Effanville.Common.Structure.DataEdit;
using Effanville.Common.UI;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.Download;
using Effanville.FPD.Logic.TemplatesAndStyles;
using Effanville.FPD.Logic.ViewModels.Common;

namespace Effanville.FPD.Logic.Tests.Context;

public sealed class DataNamesViewModelTestContext : ViewModelTestContext<IPortfolio, DataNamesViewModel>
{
    public bool LoadDataCalled { get; set; }

    public override void Reset()
    {
        base.Reset();
        LoadDataCalled = false;
    }

    public DataNamesViewModelTestContext(
        IUiStyles uiStyles,
        UiGlobals globals,
        IUpdater updater,
        IPortfolioDataDownloader downloader)
        : base(uiStyles, globals, updater, null, downloader)
    {
    }
}