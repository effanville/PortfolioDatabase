using System;
using System.Collections.Generic;

using Effanville.FinancialStructures.Database.Statistics;
using Effanville.FinancialStructures.FinanceStructures;

namespace Effanville.FPD.Logic.ViewModels;

public interface IAccountStatisticsProvider
{
    AccountStatistics GetStats(IValueList valueList, DateTime time, IReadOnlyList<Statistic> stats);
}