using Effanville.Common.Structure.DataEdit;
using Effanville.Common.UI;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.FinanceStructures;
using Effanville.FinancialStructures.NamingStructures;

using FPD.Logic.Configuration;
using FPD.Logic.TemplatesAndStyles;
using FPD.Logic.ViewModels.Asset;
using FPD.Logic.ViewModels.Common;
using FPD.Logic.ViewModels.Security;
using FPD.Logic.ViewModels.Stats;

namespace FPD.Logic.ViewModels;

public class ViewModelFactory : IViewModelFactory
{
    private readonly UiGlobals _globals;
    private readonly IUpdater<IPortfolio> _updater;
    private readonly UiStyles _styles;

    public ViewModelFactory(UiStyles styles, UiGlobals globals, IUpdater<IPortfolio> updater)
    {
        _styles = styles;
        _globals = globals;
        _updater = updater;
    }

    public DataDisplayViewModelBase GenerateViewModel(
        IPortfolio portfolio, 
        TwoName names,
        Account account,
        IConfiguration configuration,
        string vmType) 
        => vmType switch
        {
            nameof(StatsViewModel) => new StatsViewModel(_globals, _styles, configuration, portfolio, account),
            nameof(BasicDataViewModel) => new BasicDataViewModel(_globals, _styles, portfolio),
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
            IExchangableValueList exchangeableValueList => new SelectedSingleDataViewModel(portfolio,
                exchangeableValueList, _styles, _globals, exchangeableValueList.Names, account,
                _updater) as StyledClosableViewModelBase<T, IPortfolio>,
            IValueList valueList => new SelectedSingleDataViewModel(portfolio, valueList, _styles, _globals,
                valueList.Names, account, _updater) as StyledClosableViewModelBase<T, IPortfolio>,
            _ => null
        };
}