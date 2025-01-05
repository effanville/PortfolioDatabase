using Effanville.FinancialStructures.NamingStructures;
using Effanville.FPD.Logic.ViewModels.Common;

namespace Effanville.FPD.Logic.Tests.UserInteractions;

public static class DataNamesViewModelUserInteractions
{
    public static void AddName(this DataNamesViewModel viewModel, NameData name)
    {
        viewModel.SelectName(null);
        NameDataViewModel newItem = viewModel.DefaultRow();
        viewModel.DataNames.Add(newItem);
        viewModel.SelectRow(newItem);

        newItem.BeginEdit();
        newItem.Company = name.Company;
        newItem.Name = name.Name;
        newItem.Url = name.Url;
        newItem.Currency = name.Currency;
        newItem.Sectors = name.SectorsFlat;
        viewModel.CompleteEdit(newItem);
    }

    public static void EditName(this DataNamesViewModel viewModel, NameDataViewModel row, NameData newName)
    {
        viewModel.SelectRow(row);
        row.BeginEdit();
        row.Company = newName.Company;
        row.Name = newName.Name;
        row.Url = newName.Url;
        row.Currency = newName.Currency;
        row.Sectors = newName.SectorsFlat;
        viewModel.CompleteEdit(row);
    }

    public static void SelectName(this DataNamesViewModel viewModel, NameData name)
        => viewModel.SelectionChangedCommand?.Execute(new NameDataViewModel("", name, false, viewModel.DataType, viewModel._updater,
            null, null));

    public static void SelectRow(this DataNamesViewModel viewModel, NameDataViewModel row)
        => viewModel.SelectionChangedCommand?.Execute(row);

    public static void ViewData(this DataNamesViewModel viewModel)
        => viewModel.OpenTabCommand.Execute(null);

    public static void CompleteEdit(this DataNamesViewModel viewModel, NameDataViewModel row)
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