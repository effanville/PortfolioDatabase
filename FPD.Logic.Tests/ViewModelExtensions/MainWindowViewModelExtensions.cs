using System.Linq;
using FPD.Logic.ViewModels;
using FPD.Logic.ViewModels.Common;
using FPD.Logic.ViewModels.Security;
using FinancialStructures.Database;

namespace FPD.Logic.Tests.ViewModelExtensions
{
    public static class MainWindowViewModelExtensions
    {

        public static SecurityEditWindowViewModel SecurityWindow(this MainWindowViewModel viewModel)
        {
            object securityView = viewModel.Tabs.First(view => view is SecurityEditWindowViewModel);
            return securityView as SecurityEditWindowViewModel;
        }

        public static ValueListWindowViewModel Window(this MainWindowViewModel viewModel, Account account)
        {
            object desiredTab = viewModel.Tabs.First(view => view is ValueListWindowViewModel vm && vm.DataType == account);
            return desiredTab as ValueListWindowViewModel;
        }
    }
}
