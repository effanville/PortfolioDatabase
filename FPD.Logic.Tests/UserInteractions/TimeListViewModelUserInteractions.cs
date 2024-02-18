using Effanville.Common.Structure.DataStructures;

using FPD.Logic.ViewModels.Common;

namespace FPD.Logic.Tests.UserInteractions;

public static class TimeListViewModelUserInteractions
{
    public static void AddValuation(this TimeListViewModel viewModel, DailyValuation valuation)
    {
        viewModel.SelectValuation(null);
        var newItem = viewModel.DefaultNewItem();
        viewModel.Valuations.Add(newItem);
        viewModel.SelectValuation(newItem);
        viewModel.BeginEdit();

        newItem.Day = valuation.Day;
        newItem.Value = valuation.Value;
        viewModel.CompleteEdit();
    }

    public static void EditValuation(
        this TimeListViewModel viewModel, 
        DailyValuation oldValuation,
        DailyValuation newValuation)
    {
        viewModel.SelectValuation(oldValuation);
        viewModel.BeginEdit();

        oldValuation.Day = newValuation.Day;
        oldValuation.Value = newValuation.Value;
        viewModel.CompleteEdit();
    }

    public static void DeleteValuation(this TimeListViewModel viewModel, DailyValuation valuation)
    {
        viewModel.SelectValuation(valuation);
        viewModel.DeleteValuation();
    }

    private static void SelectValuation(this TimeListViewModel viewModel, DailyValuation valuation)
        => viewModel.SelectionChangedCommand.Execute(valuation);
    
    private static void BeginEdit(this TimeListViewModel viewModel) 
        => viewModel.PreEditCommand.Execute(null);

    private static void CompleteEdit(this TimeListViewModel viewModel) 
        => viewModel.AddEditDataCommand.Execute(null);
}