using System.Linq;

using Effanville.FinancialStructures.Database;

using FPD.Logic.ViewModels;
using FPD.Logic.ViewModels.Common;

namespace Effanville.FPD.Logic.Tests.ViewModelExtensions
{
    public static class MainWindowViewModelExtensions
    {

        public static ValueListWindowViewModel SecurityWindow(this MainWindowViewModel viewModel)
            => viewModel.Window(Account.Security);

        public static ValueListWindowViewModel Window(this MainWindowViewModel viewModel, Account account)
        {
            object desiredTab = viewModel.Tabs.First(view => view is ValueListWindowViewModel vm && vm.DataType == account);
            return desiredTab as ValueListWindowViewModel;
        }
    }
}
