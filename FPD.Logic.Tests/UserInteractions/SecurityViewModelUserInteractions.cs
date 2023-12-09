using FinancialStructures.DataStructures;

using FPD.Logic.ViewModels.Security;

namespace FPD.Logic.Tests.UserInteractions;

public static class SecurityViewModelUserInteractions
{
    public static void AddNewTrade(this SelectedSecurityViewModel viewModel, SecurityTrade trade)
    {
        viewModel.SelectTrade(null);
        var newTrade = viewModel.DefaultTradeValue();
        viewModel.Trades.Add(newTrade);
        viewModel.SelectTrade(newTrade);

        viewModel.BeginEditTrade();
        newTrade.TradeType = trade.TradeType;
        newTrade.Names = trade.Names;
        newTrade.Day = trade.Day;
        newTrade.NumberShares = trade.NumberShares;
        newTrade.UnitPrice = trade.UnitPrice;
        newTrade.TradeCosts = trade.TradeCosts;
        viewModel.CompleteEditTrade();
    }

    public static void EditTrade(this SelectedSecurityViewModel viewModel, SecurityTrade oldTrade, SecurityTrade trade)
    {
        viewModel.SelectTrade(oldTrade);
        viewModel.BeginEditTrade();
        oldTrade.TradeType = trade.TradeType;
        oldTrade.Names = trade.Names;
        oldTrade.Day = trade.Day;
        oldTrade.NumberShares = trade.NumberShares;
        oldTrade.UnitPrice = trade.UnitPrice;
        oldTrade.TradeCosts = trade.TradeCosts;
        viewModel.CompleteEditTrade();
    }

    public static void SelectTrade(this SelectedSecurityViewModel viewModel, SecurityTrade valueToSelect)
        => viewModel.SelectionChangedCommand.Execute(valueToSelect);

    public static void BeginEditTrade(this SelectedSecurityViewModel viewModel)
        => viewModel.PreEditCommand.Execute(null);

    public static void CompleteEditTrade(this SelectedSecurityViewModel viewModel)
        => viewModel.AddEditDataCommand.Execute(null);

    public static void DeleteTrade(this SelectedSecurityViewModel viewModel, SecurityTrade valuation)
    {
        viewModel.SelectTrade(valuation);
        viewModel.DeleteTrade();
    }
}