using FinancialStructures.NamingStructures;

using FPD.Logic.ViewModels.Common;

namespace Effanville.FPD.Logic.Tests.UserInteractions;

public static class DataNamesViewModelUserInteractions
{
    public static void AddName(this DataNamesViewModel viewModel, NameData name)
    {
        viewModel.SelectName(null);
        var newItem = viewModel.DefaultRow();
        viewModel.DataNames.Add(newItem);
        viewModel.SelectRow(newItem);

        newItem.BeginEdit();
        newItem.Instance.Company = name.Company;
        newItem.Instance.Name = name.Name;
        newItem.Instance.Url = name.Url;
        newItem.Instance.Currency = name.Currency;
        newItem.Instance.Sectors = name.Sectors;
        viewModel.CompleteEdit(newItem);
    }

    public static void EditName(this DataNamesViewModel viewModel, RowData row, NameData newName)
    {
        viewModel.SelectRow(row);
        row.BeginEdit();
        row.Instance.Company = newName.Company;
        row.Instance.Name = newName.Name;
        row.Instance.Url = newName.Url;
        row.Instance.Currency = newName.Currency;
        row.Instance.Sectors = newName.Sectors;
        viewModel.CompleteEdit(row);
    }

    public static void SelectName(this DataNamesViewModel viewModel, NameData name)
        => viewModel.SelectionChangedCommand?.Execute(new RowData(name, false, viewModel.DataType, viewModel._updater,
            null));

    public static void SelectRow(this DataNamesViewModel viewModel, RowData row)
        => viewModel.SelectionChangedCommand?.Execute(row);

    public static void ViewData(this DataNamesViewModel viewModel)
        => viewModel.OpenTabCommand.Execute(null);

    public static void CompleteEdit(this DataNamesViewModel viewModel, RowData row)
    {
        viewModel.CreateCommand?.Execute(row);
        row.EndEdit();
    }

    public static void DeleteName(this DataNamesViewModel viewModel, NameData name)
    {
        viewModel.SelectName(name);
        viewModel.DeleteCommand.Execute(name);
    }

    public static void DownloadSelected(this DataNamesViewModel viewModel)
        => viewModel.DownloadCommand.Execute(null);
}