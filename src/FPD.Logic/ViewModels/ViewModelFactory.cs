using Effanville.Common.Structure.DataEdit;
using Effanville.Common.UI;
using Effanville.Common.UI.ViewModelBases;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.Download;
using Effanville.FinancialStructures.FinanceStructures;
using Effanville.FinancialStructures.NamingStructures;
using Effanville.FPD.Logic.Configuration;
using Effanville.FPD.Logic.ViewModels.Asset;
using Effanville.FPD.Logic.ViewModels.Common;
using Effanville.FPD.Logic.ViewModels.Security;
using Effanville.FPD.Logic.ViewModels.Stats;

namespace Effanville.FPD.Logic.ViewModels;

public class ViewModelFactory : IViewModelFactory
{
    private readonly UiGlobals _globals;
    private readonly IUpdater _updater;
    private readonly IPortfolioDataDownloader _portfolioDataDownloader;
    private readonly IConfiguration _configuration;
    private readonly IAccountStatisticsProvider _statisticsProvider;

    public ViewModelFactory(
        UiGlobals globals,
        IUpdater updater,
        IPortfolioDataDownloader portfolioDataDownloader,
        IConfiguration configuration,
        IAccountStatisticsProvider statisticsProvider)
    {
        _globals = globals;
        _updater = updater;
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
            nameof(StatsViewModel) => new StatsViewModel(_globals, _configuration.ChildConfigurations[nameof(StatsViewModel)], portfolio, account),
            nameof(BasicDataViewModel) => new BasicDataViewModel(_globals, portfolio, _updater),
            nameof(StatsCreatorWindowViewModel) => new StatsCreatorWindowViewModel(_globals, _configuration.ChildConfigurations[nameof(StatsCreatorWindowViewModel)], portfolio, this),
            nameof(SecurityInvestmentViewModel) => new SecurityInvestmentViewModel(portfolio),
            nameof(DataNamesViewModel) => new DataNamesViewModel(portfolio, _globals, _updater, _portfolioDataDownloader, this, account),
            _ => null
        };

    public ClosableViewModelBase<T> GenerateViewModel<T>(
        T modelData,
        TwoName names,
        Account account)
        where T : class
        => modelData switch
        {
            IAmortisableAsset asset => new SelectedAssetViewModel(
                _statisticsProvider,
                asset,
                _globals,
                asset.Names,
                account,
                _updater,
                _portfolioDataDownloader) as ClosableViewModelBase<T>,
            ISecurity security => new SelectedSecurityViewModel(
                _statisticsProvider,
                security,
                _globals,
                names,
                account,
                _updater,
                _portfolioDataDownloader) as ClosableViewModelBase<T>,
            IExchangeableValueList exchangeableValueList => new SelectedSingleDataViewModel(
                _statisticsProvider,
                exchangeableValueList,
                _globals,
                exchangeableValueList.Names,
                account,
                _updater) as ClosableViewModelBase<T>,
            IValueList valueList => new SelectedSingleDataViewModel(
                _statisticsProvider,
                valueList,
                _globals,
                valueList.Names,
                account,
                _updater) as ClosableViewModelBase<T>,
            _ => null
        };
}