using FinancialStructures.Database;
using FinancialStructures.NamingStructures;

using FPD.Logic.Configuration;
using FPD.Logic.ViewModels.Common;

namespace FPD.Logic.ViewModels;

public interface IViewModelFactory
{
    DataDisplayViewModelBase GenerateViewModel(
        IPortfolio portfolio,
        NameData names,
        Account account,
        IConfiguration configuration,
        string vmType);
    
    StyledClosableViewModelBase<T, IPortfolio> GenerateViewModel<T>(
        T modelData, 
        NameData names,
        Account account,
        IPortfolio portfolio)
        where T : class;
}