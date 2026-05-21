using Effanville.Common.Structure.DataEdit;
using Effanville.Common.UI;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.Download;
using Effanville.FPD.Logic.ViewModels;

namespace Effanville.FPD.Logic.Tests.Context;

public class MainViewModelTestContext : ViewModelTestContext<IPortfolio, MainWindowViewModel>
{
    public object FocusedTab { get; set; }
    public MainViewModelTestContext(UiGlobals globals, IUpdater updater, IViewModelFactory viewModelFactory, IPortfolioDataDownloader portfolioDataDownloader)
        : base(globals, updater, viewModelFactory, portfolioDataDownloader)
    {
    }
}
