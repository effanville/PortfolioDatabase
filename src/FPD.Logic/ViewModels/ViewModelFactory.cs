using Effanville.Common.Structure.DataEdit;
using Effanville.Common.UI;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.Download;
using Effanville.FinancialStructures.FinanceStructures;
using Effanville.FinancialStructures.NamingStructures;
using Effanville.FPD.Logic.Configuration;
using Effanville.FPD.Logic.TemplatesAndStyles;
using Effanville.FPD.Logic.ViewModels.Asset;
using Effanville.FPD.Logic.ViewModels.Common;
using Effanville.FPD.Logic.ViewModels.Security;
using Effanville.FPD.Logic.ViewModels.Stats;

namespace Effanville.FPD.Logic.ViewModels;

public class ViewModelFactory : IViewModelFactory
{
    private readonly UiGlobals _globals;
    private readonly IDataStoreUpdater<IPortfolio> _updater;
    private readonly IUpdater _newUpdater;
    private readonly IPortfolioDataDownloader _portfolioDataDownloader;
    private readonly IConfiguration _configuration;
    private readonly IAccountStatisticsProvider _statisticsProvider;
    private readonly IUiStyles _styles;

    public ViewModelFactory(
        IUiStyles styles,
        UiGlobals globals,
        IDataStoreUpdater<IPortfolio> updater,
        IUpdater newUpdater,
        IPortfolioDataDownloader portfolioDataDownloader,
        IConfiguration configuration,
        IAccountStatisticsProvider statisticsProvider)
    {
        _styles = styles;
        _globals = globals;
        _updater = updater;
        _newUpdater = newUpdater;
        _portfolioDataDownloader = portfolioDataDownloader;
        _configuration = configuration;
        _statisticsProvider = statisticsProvider;
    }

    public DataDisplayViewModelBase GenerateViewModel(
        IPortfolio portfolio,
        string title,
        Account account,
        string vmType)
        => vmType switch
        {
            nameof(StatsViewModel) => new StatsViewModel(_globals, _styles, _configuration.ChildConfigurations[nameof(StatsViewModel)], portfolio, account),
            nameof(BasicDataViewModel) => new BasicDataViewModel(_globals, _styles, portfolio),
            nameof(ValueListWindowViewModel) => new ValueListWindowViewModel(_globals, _styles, portfolio, title, account, _updater, _portfolioDataDownloader, this),
            nameof(StatsCreatorWindowViewModel) => new StatsCreatorWindowViewModel(_globals, _styles, _configuration.ChildConfigurations[nameof(StatsCreatorWindowViewModel)], portfolio),
            _ => null
        };

    public StyledClosableViewModelBase<T> GenerateViewModel<T>(
        T modelData,
        TwoName names,
        Account account)
        where T : class
        => modelData switch
        {
            IAmortisableAsset asset => new SelectedAssetViewModel(
                _statisticsProvider,
                asset,
                _styles,
                _globals,
                asset.Names,
                account,
                _newUpdater,
                _portfolioDataDownloader) as StyledClosableViewModelBase<T>,
            ISecurity security => new SelectedSecurityViewModel(
                _statisticsProvider,
                security,
                _styles,
                _globals,
                names,
                account,
                _newUpdater,
                _portfolioDataDownloader) as StyledClosableViewModelBase<T>,
            IExchangeableValueList exchangeableValueList => new SelectedSingleDataViewModel(
                _statisticsProvider,
                exchangeableValueList,
                _styles,
                _globals,
                exchangeableValueList.Names,
                account,
                _newUpdater) as StyledClosableViewModelBase<T>,
            IValueList valueList => new SelectedSingleDataViewModel(
                _statisticsProvider,
                valueList,
                _styles,
                _globals,
                valueList.Names,
                account,
                _newUpdater) as StyledClosableViewModelBase<T>,
            _ => null
        };
}