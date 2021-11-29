using System.Linq;
using Common.Structure.DataStructures;
using FPD.Logic.ViewModels.Common;
using FPD.Logic.ViewModels.Asset;
using FinancialStructures.Database;

namespace FPD.Logic.Tests.ViewModelExtensions
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
        {
            viewModel.DebtTLVM.SelectionChangedCommand?.Execute(valueToSelect);
        }

        public static void BeginEditDebt(this SelectedAssetViewModel viewModel)
        {
            viewModel.DebtTLVM.PreEditCommand?.Execute(null);
        }

        public static void CompleteEditTrade(this SelectedAssetViewModel viewModel, IPortfolio portfolio)
        {
            viewModel.DebtTLVM.AddEditDataCommand?.Execute(null);
            viewModel.UpdateData(portfolio);
        }

        public static void DeleteSelectedDebt(this SelectedAssetViewModel viewModel, IPortfolio portfolio)
        {
            viewModel.DebtTLVM.DeleteValuation();
            viewModel.UpdateData(portfolio);
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
        {
            viewModel.ValuesTLVM.SelectionChangedCommand?.Execute(valueToSelect);
        }

        public static void BeginEdit(this SelectedAssetViewModel viewModel)
        {
            viewModel.ValuesTLVM.PreEditCommand?.Execute(null);
        }

        public static void CompleteEdit(this SelectedAssetViewModel viewModel, IPortfolio portfolio)
        {
            viewModel.ValuesTLVM.AddEditDataCommand?.Execute(null);
            viewModel.UpdateData(portfolio);
        }

        public static void DeleteSelected(this SelectedAssetViewModel viewModel, IPortfolio portfolio)
        {
            viewModel.ValuesTLVM.DeleteValuation();
            viewModel.UpdateData(portfolio);
        }
    }
}
