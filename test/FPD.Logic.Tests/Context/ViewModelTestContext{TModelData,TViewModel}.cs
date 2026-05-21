using Effanville.Common.Structure.DataEdit;
using Effanville.Common.UI;
using Effanville.FinancialStructures.Download;
using Effanville.FPD.Logic.ViewModels;

namespace Effanville.FPD.Logic.Tests.Context;

public class ViewModelTestContext<TModelData, TViewModel> where TModelData : class where TViewModel : class
{
    public UiGlobals Globals { get; }
    public IUpdater Updater { get; }
    public IPortfolioDataDownloader PortfolioDataDownloader { get; }
    public TModelData ModelData { get; set; }
    public virtual TViewModel ViewModel { get; set; }

    public IViewModelFactory ViewModelFactory { get; }

    public ViewModelTestContext(
        UiGlobals globals,
        IUpdater updater,
        IViewModelFactory viewModelFactory,
        IPortfolioDataDownloader portfolioDataDownloader)
    {
        Globals = globals;
        Updater = updater;
        ViewModelFactory = viewModelFactory;
        PortfolioDataDownloader = portfolioDataDownloader;
    }

    public virtual void Reset()
    {
        ModelData = null;
        ViewModel = null;
        Globals.ReportLogger.Reports.Clear();
    }
}