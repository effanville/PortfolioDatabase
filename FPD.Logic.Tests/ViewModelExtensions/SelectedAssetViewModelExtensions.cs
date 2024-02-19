using System.Linq;

using Effanville.Common.Structure.DataStructures;
using Effanville.FinancialStructures.FinanceStructures;

using FPD.Logic.ViewModels.Asset;
using FPD.Logic.ViewModels.Common;

namespace Effanville.FPD.Logic.Tests.ViewModelExtensions
{
    /// <summary>
    /// Contains user like interaction behaviours with the <see cref="SelectedSingleDataViewModel"/>.
    /// </summary>
    public static class SelectedAssetViewModelExtensions
    {
        public static DailyValuation AddNewDebt(this SelectedAssetViewModel viewModel)
        {
            viewModel.SelectDebt(null);
            viewModel.DebtTLVM.Valuations.Add(viewModel.DebtTLVM.DefaultNewItem());
            DailyValuation newItem = viewModel.DebtTLVM.Valuations.Last();
            viewModel.SelectDebt(newItem);
            viewModel.BeginEdit();

            return newItem;
        }

        public static void SelectDebt(this SelectedAssetViewModel viewModel, DailyValuation valueToSelect) 
            => viewModel.DebtTLVM.SelectionChangedCommand?.Execute(valueToSelect);

        public static void BeginEditDebt(this SelectedAssetViewModel viewModel) 
            => viewModel.DebtTLVM.PreEditCommand?.Execute(null);

        public static void CompleteEditTrade(this SelectedAssetViewModel viewModel, IAmortisableAsset asset)
        {
            viewModel.DebtTLVM.AddEditDataCommand?.Execute(null);
            viewModel.UpdateData(asset);
        }

        public static void DeleteSelectedDebt(this SelectedAssetViewModel viewModel, IAmortisableAsset asset)
        {
            viewModel.DebtTLVM.DeleteValuation();
            viewModel.UpdateData(asset);
        }

        public static DailyValuation AddNewUnitPrice(this SelectedAssetViewModel viewModel)
        {
            viewModel.SelectValue(null);
            viewModel.ValuesTLVM.Valuations.Add(viewModel.ValuesTLVM.DefaultNewItem());
            DailyValuation newItem = viewModel.ValuesTLVM.Valuations.Last();
            viewModel.SelectValue(newItem);
            viewModel.BeginEdit();

            return newItem;
        }

        public static void SelectValue(this SelectedAssetViewModel viewModel, DailyValuation valueToSelect) 
            => viewModel.ValuesTLVM.SelectionChangedCommand?.Execute(valueToSelect);

        public static void BeginEdit(this SelectedAssetViewModel viewModel) 
            => viewModel.ValuesTLVM.PreEditCommand?.Execute(null);

        public static void CompleteEdit(this SelectedAssetViewModel viewModel, IAmortisableAsset asset)
        {
            viewModel.ValuesTLVM.AddEditDataCommand?.Execute(null);
            viewModel.UpdateData(asset);
        }

        public static void DeleteSelected(this SelectedAssetViewModel viewModel, IAmortisableAsset asset)
        {
            viewModel.ValuesTLVM.DeleteValuation();
            viewModel.UpdateData(asset);
        }
    }
}
