using FPD.Logic.ViewModels.Common;
using FinancialStructures.NamingStructures;

namespace FPD.Logic.Tests.ViewModelExtensions
{
    /// <summary>
    /// Contains user like interaction behaviours with the <see cref="DataNamesViewModel"/>.
    /// </summary>
    public static class DataNamesViewModelExtensions
    {
        public static void SelectItem(this DataNamesViewModel viewModel, NameData name)
        {
            viewModel.SelectionChangedCommand?.Execute(new RowData(name, false, viewModel.TypeOfAccount, viewModel._updater, null));
        }

        public static void BeginRowEdit(this DataNamesViewModel viewModel, RowData row)
        {
            row.BeginEdit();
        }

        public static void CompleteEdit(this DataNamesViewModel viewModel, RowData row)
        {
            viewModel.CreateCommand?.Execute(row);
            row.EndEdit();
        }

        public static void CompleteCreate(this DataNamesViewModel viewModel, RowData row)
        {
            viewModel.CreateCommand?.Execute(row);
        }

        public static RowData AddNewItem(this DataNamesViewModel viewModel)
        {
            viewModel.SelectItem(null);
            var row = viewModel.DefaultRow();
            viewModel.DataNames.Add(row);
            viewModel.SelectItem(row.Instance);
            return row;
        }

        public static void DownloadSelected(this DataNamesViewModel viewModel)
        {
            viewModel.DownloadCommand.Execute(null);
        }

        public static void DeleteSelected(this DataNamesViewModel viewModel)
        {
            viewModel.DeleteCommand?.Execute(null);
        }
    }
}
