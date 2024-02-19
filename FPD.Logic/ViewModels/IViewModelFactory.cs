using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.NamingStructures;
using Effanville.FPD.Logic.Configuration;
using Effanville.FPD.Logic.ViewModels.Common;

namespace Effanville.FPD.Logic.ViewModels;

public interface IViewModelFactory
{
    DataDisplayViewModelBase GenerateViewModel(
        IPortfolio portfolio,
        TwoName names,
        Account account,
        IConfiguration configuration,
        string vmType);
    
    StyledClosableViewModelBase<T, IPortfolio> GenerateViewModel<T>(
        T modelData, 
        TwoName names,
        Account account,
        IPortfolio portfolio)
        where T : class;
}