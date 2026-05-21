using System.Linq;

using Effanville.Common.UI.ViewModelBases;
using Effanville.FinancialStructures.Database;
using Effanville.FPD.Logic.ViewModels;
using Effanville.FPD.Logic.ViewModels.Common;

namespace Effanville.FPD.Logic.Tests.UserInteractions
{
    public static class MainWindowUserInteractions
    {
        public static DataNamesViewModel OpenAccountTab(this MainWindowViewModel viewModel, Account account)
        {
            object desiredTab = viewModel.Tabs.First(view => view is DataNamesViewModel vm && vm.DataType == account);
            return desiredTab as DataNamesViewModel;
        }

        public static ViewModelBase<T> OpenTab<T>(this MainWindowViewModel viewModel, T modelData) where T : class
        {
            object desiredTab = viewModel.Tabs.First(v => v is ViewModelBase<T> vm && vm.ModelData == modelData);
            return desiredTab as ViewModelBase<T>;
        }
    }
}
