using System.Linq;

using Effanville.Common.Structure.NamingStructures;
using Effanville.FinancialStructures.NamingStructures;
using Effanville.FPD.Logic.Tests.TestHelpers;
using Effanville.FPD.Logic.ViewModels.Common;

namespace Effanville.FPD.Logic.Tests.UserInteractions;

public static class DataNamesViewModelUserInteractions
{
    public static void AddName(this DataNamesViewModel viewModel, TestDialogService service, NameData name)
    {
        service.SetupCustomDialogAction(obj =>
        {
            if (obj is AddEditNameViewModel vm)
            {
                vm.UpdateData(name, true);
                vm.CompleteCommand.Execute(null);
            }
        });
        viewModel.AddCommand.Execute(null);
    }

    public static void EditName(this DataNamesViewModel viewModel, TestDialogService service, NameDataViewModel row, NameData newName)
    {
        viewModel.SelectRow(row);
        service.SetupCustomDialogAction(obj =>
        {
            if (obj is AddEditNameViewModel vm)
            {
                vm.UpdateData(newName, true);
                vm.CompleteCommand.Execute(null);
            }
        });
        viewModel.EditCommand.Execute(null);
    }

    public static void SelectName(this DataNamesViewModel viewModel, NameData name)
    {
        var selectedRow = viewModel.DataNames.FirstOrDefault(v => v.ModelData.IsEqualTo(name));
        if (name != null && selectedRow == null)
        {
            throw new System.Exception("Cannot attempt to select a non-existent row");
        }

        viewModel.SelectionChangedCommand?.Execute(selectedRow);
    }

    public static void SelectRow(this DataNamesViewModel viewModel, NameDataViewModel row)
        => viewModel.SelectionChangedCommand?.Execute(row);

    public static void ViewData(this DataNamesViewModel viewModel)
        => viewModel.OpenTabCommand.Execute(null);

    public static void DeleteName(this DataNamesViewModel viewModel, NameData name)
    {
        viewModel.SelectName(name);
        viewModel.DeleteCommand.Execute(name);
    }

    public static void DownloadSelected(this DataNamesViewModel viewModel)
        => viewModel.DownloadCommand.Execute(null);
}