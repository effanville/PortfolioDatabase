using Effanville.Common.Structure.DataEdit;
using Effanville.Common.UI;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.Download;
using Effanville.FPD.Logic.TemplatesAndStyles;
using Effanville.FPD.Logic.ViewModels;

namespace Effanville.FPD.Logic.Tests.Context;

public class MainViewModelTestContext : ViewModelTestContext<IPortfolio, MainWindowViewModel>
{
    public object FocusedTab { get; set; }
    public MainViewModelTestContext(IUiStyles uiStyles, UiGlobals globals, IUpdater updater, IViewModelFactory viewModelFactory, IPortfolioDataDownloader portfolioDataDownloader)
        : base(uiStyles, globals, updater, viewModelFactory, portfolioDataDownloader)
    {
    }
}
