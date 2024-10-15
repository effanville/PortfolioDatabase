using Effanville.Common.Structure.DataEdit;
using Effanville.Common.UI;
using Effanville.FinancialStructures.Database;
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
    private readonly IUpdater<IPortfolio> _updater;
    private readonly IConfiguration _configuration;
    private readonly UiStyles _styles;

    public ViewModelFactory(
        UiStyles styles,
        UiGlobals globals,
        IUpdater<IPortfolio> updater,
        IConfiguration configuration)
    {
        _styles = styles;
        _globals = globals;
        _updater = updater;
        _configuration = configuration;
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
            nameof(ValueListWindowViewModel) => new ValueListWindowViewModel(_globals, _styles, portfolio, title, account, _updater, this),
            nameof(StatsCreatorWindowViewModel) => new StatsCreatorWindowViewModel(_globals, _styles, _configuration.ChildConfigurations[nameof(StatsCreatorWindowViewModel)], portfolio),
            _ => null
        };

    public StyledClosableViewModelBase<T, IPortfolio> GenerateViewModel<T>(
        T modelData, 
        TwoName names,
        Account account, 
        IPortfolio portfolio) 
        where T : class 
        => modelData switch
        {
            IAmortisableAsset asset => new SelectedAssetViewModel(portfolio, asset, _styles, _globals, asset.Names,
                account, _updater) as StyledClosableViewModelBase<T, IPortfolio>,
            ISecurity security => new SelectedSecurityViewModel(portfolio, security, _styles, _globals, names, account,
                _updater) as StyledClosableViewModelBase<T, IPortfolio>,
            IExchangeableValueList exchangeableValueList => new SelectedSingleDataViewModel(portfolio,
                exchangeableValueList, _styles, _globals, exchangeableValueList.Names, account,
                _updater) as StyledClosableViewModelBase<T, IPortfolio>,
            IValueList valueList => new SelectedSingleDataViewModel(portfolio, valueList, _styles, _globals,
                valueList.Names, account, _updater) as StyledClosableViewModelBase<T, IPortfolio>,
            _ => null
        };
}