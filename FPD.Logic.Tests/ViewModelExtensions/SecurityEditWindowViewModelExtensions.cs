using System.Collections.Generic;
using System.Linq;
using FPD.Logic.ViewModels.Common;
using FPD.Logic.ViewModels.Security;
using FinancialStructures.NamingStructures;

namespace FPD.Logic.Tests.ViewModelExtensions
{
    /// <summary>
    /// Contains user like interaction behaviours with the <see cref="AssetEditWindowViewModel"/>.
    /// </summary>
    public static class SecurityEditWindowViewModelExtensions
    {
        public static DataNamesViewModel DataNames(this AssetEditWindowViewModel viewModel)
        {
            IEnumerable<object> desiredViewModel = viewModel.Tabs.Where(vm => vm is DataNamesViewModel svm);
            return desiredViewModel.First() as DataNamesViewModel;
        }

        public static AssetSecurityViewModel SelectedTab(this AssetEditWindowViewModel viewModel, NameData name)
        {
            IEnumerable<object> desiredViewModel = viewModel.Tabs.Where(vm => vm is AssetSecurityViewModel svm && svm.SelectedName.Equals(name));
            return desiredViewModel.First() as AssetSecurityViewModel;
        }
    }
}
