using System.Collections.Generic;
using System.Linq;
using FPD.Logic.ViewModels.Common;
using FinancialStructures.NamingStructures;

using FPD.Logic.ViewModels.Asset;
using FPD.Logic.ViewModels.Security;

namespace FPD.Logic.Tests.UserInteractions
{
    /// <summary>
    /// Contains user like interaction behaviours with the <see cref="ViewModels.Common.ValueListWindowViewModel"/>.
    /// </summary>
    public static class ValueListWindowInteractions
    {
        public static DataNamesViewModel GetDataNamesViewModel(this ValueListWindowViewModel viewModel)
            => (DataNamesViewModel)viewModel.Tabs.First(tab => tab is DataNamesViewModel);

        public static SelectedSingleDataViewModel SelectedTab(this ValueListWindowViewModel viewModel, NameData name)
        {
            IEnumerable<object> desiredViewModel = viewModel.Tabs.Where(vm => vm is SelectedSingleDataViewModel svm && svm.SelectedName.Equals(name));
            return desiredViewModel.First() as SelectedSingleDataViewModel;
        }
        
        public static SelectedAssetViewModel SelectedAssetTab(this ValueListWindowViewModel viewModel, NameData name)
        {
            IEnumerable<object> desiredViewModel = viewModel.Tabs.Where(vm => vm is SelectedAssetViewModel svm && svm.SelectedName.Equals(name));
            return desiredViewModel.First() as SelectedAssetViewModel;
        }
        
        public static SelectedSecurityViewModel SelectedSecurityTab(this ValueListWindowViewModel viewModel, NameData name)
        {
            IEnumerable<object> desiredViewModel = viewModel.Tabs.Where(vm => vm is SelectedSecurityViewModel svm && svm.SelectedName.Equals(name));
            return desiredViewModel.First() as SelectedSecurityViewModel;
        }
    }
}
