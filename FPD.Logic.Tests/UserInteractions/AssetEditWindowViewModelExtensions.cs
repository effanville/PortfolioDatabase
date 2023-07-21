using System.Linq;
using FPD.Logic.ViewModels.Common;
using FPD.Logic.ViewModels.Asset;
using FinancialStructures.NamingStructures;

namespace FPD.Logic.Tests.UserInteractions
{
    /// <summary>
    /// Contains user like interaction behaviours with the <see cref="AssetEditWindowViewModel"/>.
    /// </summary>
    public static class AssetEditWindowInteractions
    {
        public static DataNamesViewModel GetDataNamesViewModel(this AssetEditWindowViewModel viewModel)
            => (DataNamesViewModel)viewModel.Tabs.First(tab => tab is DataNamesViewModel);

        public static SelectedAssetViewModel SelectedViewModel(this AssetEditWindowViewModel viewModel, NameData name)
        {
            return (SelectedAssetViewModel)viewModel.Tabs.First(
                tab => tab is SelectedAssetViewModel vm
                && vm.fSelectedName.Equals(name)
                && vm.Header == name.ToString());
        }
    }
}
