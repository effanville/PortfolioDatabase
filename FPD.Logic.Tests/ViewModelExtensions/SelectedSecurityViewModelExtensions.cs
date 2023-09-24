using System.Linq;
using Common.Structure.DataStructures;
using FPD.Logic.ViewModels.Common;
using FPD.Logic.ViewModels.Security;
using FinancialStructures.DataStructures;
using FinancialStructures.FinanceStructures;

namespace FPD.Logic.Tests.ViewModelExtensions
{
    /// <summary>
    /// Contains user like interaction behaviours with the <see cref="SelectedSingleDataViewModel"/>.
    /// </summary>
    public static class SelectedSecurityViewModelExtensions
    {
        public static SecurityTrade AddNewTrade(this SelectedSecurityViewModel viewModel)
        {
            viewModel.SelectTrade(null);
            viewModel.Trades.Add(viewModel.DefaultTradeValue());
            var newItem = viewModel.Trades.Last();
            viewModel.SelectTrade(newItem);
            viewModel.BeginEdit();

            return newItem;
        }

        public static void SelectTrade(this SelectedSecurityViewModel viewModel, SecurityTrade valueToSelect)
        {
            viewModel.SelectionChangedCommand?.Execute(valueToSelect);
        }

        public static void BeginEditTrade(this SelectedSecurityViewModel viewModel)
        {
            viewModel.PreEditCommand?.Execute(null);
        }

        public static void CompleteEditTrade(this SelectedSecurityViewModel viewModel, ISecurity security)
        {
            viewModel.AddEditDataCommand?.Execute(null);
            viewModel.UpdateData(security);
        }

        public static void DeleteSelectedTrade(this SelectedSecurityViewModel viewModel, ISecurity security)
        {
            viewModel.DeleteTrade();
            viewModel.UpdateData(security);
        }

        public static DailyValuation AddNewUnitPrice(this SelectedSecurityViewModel viewModel)
        {
            viewModel.SelectUnitPrice(null);
            viewModel.TLVM.Valuations.Add(new DailyValuation());
            DailyValuation newItem = viewModel.TLVM.Valuations.Last();
            viewModel.SelectUnitPrice(newItem);
            viewModel.BeginEdit();

            return newItem;
        }

        public static void SelectUnitPrice(this SelectedSecurityViewModel viewModel, DailyValuation valueToSelect)
        {
            viewModel.TLVM.SelectionChangedCommand?.Execute(valueToSelect);
        }

        public static void BeginEdit(this SelectedSecurityViewModel viewModel)
        {
            viewModel.TLVM.PreEditCommand?.Execute(null);
        }

        public static void CompleteEdit(this SelectedSecurityViewModel viewModel, ISecurity security)
        {
            viewModel.TLVM.AddEditDataCommand?.Execute(null);
            viewModel.UpdateData(security);
        }

        public static void DeleteSelected(this SelectedSecurityViewModel viewModel, ISecurity security)
        {
            viewModel.TLVM.DeleteValuation();
            viewModel.UpdateData(security);
        }
    }
}
