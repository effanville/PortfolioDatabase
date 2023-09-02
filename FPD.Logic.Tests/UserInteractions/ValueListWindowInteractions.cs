using System.Collections.Generic;
using System.Linq;
using FPD.Logic.ViewModels.Common;
using FinancialStructures.NamingStructures;

namespace FPD.Logic.Tests.UserInteractions
{
    /// <summary>
    /// Contains user like interaction behaviours with the <see cref="ViewModels.Common.ValueListWindowViewModel"/>.
    /// </summary>
    public static class ValueListWindowInteractions
    {
        public static DataNamesViewModel GetDataNamesViewModel(this ViewModels.Common.ValueListWindowViewModel viewModel)
            => (DataNamesViewModel)viewModel.Tabs.First(tab => tab is DataNamesViewModel);

        public static SelectedSingleDataViewModel SelectedTab(this ViewModels.Common.ValueListWindowViewModel viewModel, NameData name)
        {
            IEnumerable<object> desiredViewModel = viewModel.Tabs.Where(vm => vm is SelectedSingleDataViewModel svm && svm.SelectedName.Equals(name));
            return desiredViewModel.First() as SelectedSingleDataViewModel;
        }
    }
}
