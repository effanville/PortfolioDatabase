using Effanville.Common.Structure.DataEdit;
using Effanville.Common.UI;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.Download;
using Effanville.FPD.Logic.ViewModels;
using Effanville.FPD.Logic.ViewModels.Common;

namespace Effanville.FPD.Logic.Tests.Context;

public sealed class DataNamesViewModelTestContext : ViewModelTestContext<IPortfolio, DataNamesViewModel>
{
    public bool LoadDataCalled { get; set; }

    public override DataNamesViewModel ViewModel
    {
        get => base.ViewModel;
        set
        {
            if (base.ViewModel != value && ViewModel != null)
            {
                ViewModel.RequestAddTab -= Value_RequestAddTab;
            }

            base.ViewModel = value;
            base.ViewModel?.RequestAddTab += Value_RequestAddTab;
        }
    }

    private void Value_RequestAddTab(object sender, System.EventArgs e)
        => LoadDataCalled = true;

    public override void Reset()
    {
        base.Reset();
        LoadDataCalled = false;
    }

    public DataNamesViewModelTestContext(
        UiGlobals globals,
        IUpdater updater,
        IViewModelFactory viewModelFactory,
        IPortfolioDataDownloader downloader)
        : base(globals, updater, viewModelFactory, downloader)
    {
    }
}