using System;
using System.Collections.Generic;

using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.Database.Statistics;
using Effanville.FinancialStructures.FinanceStructures;

namespace Effanville.FPD.Logic.ViewModels;

public sealed class StatisticsProvider : IAccountStatisticsProvider
{
    private readonly IPortfolio _portfolio;
        
    public StatisticsProvider(IPortfolio portfolio)
    {
        _portfolio = portfolio;
    }
    public AccountStatistics GetStats(IValueList valueList, DateTime time, IReadOnlyList<Statistic> stats) 
        => new AccountStatistics(
            _portfolio,
            time,
            valueList,
            stats);
}