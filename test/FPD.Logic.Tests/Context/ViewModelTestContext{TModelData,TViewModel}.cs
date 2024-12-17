using Effanville.Common.Structure.DataEdit;
using Effanville.Common.UI;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.Download;
using Effanville.FPD.Logic.TemplatesAndStyles;
using Effanville.FPD.Logic.ViewModels;

namespace Effanville.FPD.Logic.Tests.Context;

public class ViewModelTestContext<TModelData, TViewModel> where TModelData : class where TViewModel : class
{
    public IUiStyles Styles { get; }
    public UiGlobals Globals { get; }
    public IDataStoreUpdater<IPortfolio> Updater { get; }
    public IUpdater DataUpdater { get; }
    public IPortfolioDataDownloader PortfolioDataDownloader { get; }
    public TModelData ModelData { get; set; }
    public TViewModel ViewModel { get; set; }

    public IViewModelFactory ViewModelFactory { get; }

    public ViewModelTestContext(
        IUiStyles uiStyles,
        UiGlobals globals,
        IDataStoreUpdater<IPortfolio> updater,
        IUpdater dataUpdater,
        IViewModelFactory viewModelFactory)
    {
        Styles = uiStyles;
        Globals = globals;
        Updater = updater;
        DataUpdater = dataUpdater;
        ViewModelFactory = viewModelFactory;
    }

    public virtual void Reset()
    {
        ModelData = null;
        ViewModel = null;
        Updater.Database = null;
        Globals.ReportLogger.Reports.Clear();
    }
}