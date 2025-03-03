using System;

using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.NamingStructures;
using Effanville.FPD.Logic.ViewModels.Common;

namespace Effanville.FPD.Logic.ViewModels;

public interface IViewModelFactory
{
    DataDisplayViewModelBase GenerateViewModel(
        IPortfolio portfolio,
        string title,
        Account account,
        string vmType);

    StyledClosableViewModelBase<TModel> GenerateViewModel<TModel>(
        TModel modelData,
        TwoName names,
        Account account)
        where TModel : class;

    DataNamesViewModel GenerateViewModel(
        IPortfolio portfolio,
        Action<object> loadSelectedData,
        Account dataType);
}