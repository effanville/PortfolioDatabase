using Effanville.Common.Structure.DataEdit;
using Effanville.Common.UI;
using Effanville.FinancialStructures.Database;
using Effanville.FPD.Logic.TemplatesAndStyles;
using Effanville.FPD.Logic.ViewModels;

namespace Effanville.FPD.Logic.Tests.Context;

public class ViewModelTestContext<TModelData, TViewModel> where TModelData : class where TViewModel : class
{
    public UiStyles Styles { get; }
    public UiGlobals Globals { get; }
    public IUpdater<IPortfolio> Updater { get; }
    public TModelData ModelData { get; set; }
    public TViewModel ViewModel { get; set; }
    
    public IViewModelFactory ViewModelFactory { get; }
    
    public ViewModelTestContext(
        UiStyles uiStyles, 
        UiGlobals globals,
        IUpdater<IPortfolio> updater, 
        IViewModelFactory viewModelFactory)
    {
        Styles = uiStyles;
        Globals = globals;
        Updater = updater;
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