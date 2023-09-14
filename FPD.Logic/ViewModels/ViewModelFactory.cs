using Common.Structure.DataEdit;
using Common.UI;

using FinancialStructures.Database;
using FinancialStructures.FinanceStructures;
using FinancialStructures.NamingStructures;

using FPD.Logic.TemplatesAndStyles;
using FPD.Logic.ViewModels.Asset;
using FPD.Logic.ViewModels.Common;
using FPD.Logic.ViewModels.Security;

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

    public StyledClosableViewModelBase<T, IPortfolio> GenerateViewModel<T>(
        T modelData, 
        NameData names,
        Account account, 
        IPortfolio portfolio) 
        where T : class
    {
        switch (modelData)
        {
            case IAmortisableAsset asset:
                return new SelectedAssetViewModel(portfolio, asset, _styles, _globals, asset.Names, account, _updater)
                    as StyledClosableViewModelBase<T, IPortfolio>;
            case ISecurity security:
                return new SelectedSecurityViewModel(portfolio, security, _styles, _globals, names, account, _updater) 
                    as StyledClosableViewModelBase<T, IPortfolio>;
            case IExchangableValueList exchangeableValueList:
                return new SelectedSingleDataViewModel(portfolio, exchangeableValueList, _styles, _globals, exchangeableValueList.Names, account, _updater) 
                    as StyledClosableViewModelBase<T, IPortfolio>;
            case IValueList valueList:
                return new SelectedSingleDataViewModel(portfolio, valueList, _styles, _globals, valueList.Names, account, _updater)
                    as StyledClosableViewModelBase<T, IPortfolio>;
            default:
                return null;
        }
    }
}