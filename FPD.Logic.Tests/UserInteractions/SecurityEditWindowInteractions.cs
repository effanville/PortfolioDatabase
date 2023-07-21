using System.Linq;
using FPD.Logic.ViewModels.Common;
using FPD.Logic.ViewModels.Security;
using FinancialStructures.NamingStructures;

namespace FPD.Logic.Tests.UserInteractions
{
    /// <summary>
    /// Contains user like interaction behaviours with the <see cref="AssetEditWindowViewModel"/>.
    /// </summary>
    public static class SecurityEditWindowInteractions
    {
        public static DataNamesViewModel GetDataNamesViewModel(this SecurityEditWindowViewModel viewModel)
            => (DataNamesViewModel)viewModel.Tabs.First(tab => tab is DataNamesViewModel);

        public static SelectedSecurityViewModel SelectedViewModel(this SecurityEditWindowViewModel viewModel, NameData name)
        {
            return (SelectedSecurityViewModel)viewModel.Tabs.First(
                tab => tab is SelectedSecurityViewModel vm
                && vm.SelectedName.Equals(name)
                && vm.Header == name.ToString());
        }
    }
}
