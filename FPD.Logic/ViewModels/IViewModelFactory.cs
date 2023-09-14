using FinancialStructures.Database;
using FinancialStructures.NamingStructures;

using FPD.Logic.ViewModels.Common;

namespace FPD.Logic.ViewModels;

public interface IViewModelFactory
{
    StyledClosableViewModelBase<T, IPortfolio> GenerateViewModel<T>(
        T modelData, 
        NameData names,
        Account account,
        
        IPortfolio portfolio)
        where T : class;
}