using Effanville.Common.Structure.DataEdit;
using Effanville.Common.Structure.WebAccess;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.Download;
using Effanville.FinancialStructures.Persistence;
using Effanville.FPD.Logic.ViewModels;
using Effanville.FPD.Logic.ViewModels.Stats;

using Microsoft.Extensions.DependencyInjection;

namespace Effanville.FPD.Logic.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddViewModelDependencies(this IServiceCollection services)
    {
        _ = services
            .AddSingleton<IPersistence<IPortfolio>, PortfolioPersistence>()
            .AddSingleton<IViewModelFactory, ViewModelFactory>()
            .AddSingleton<IAccountStatisticsProvider, StatisticsProvider>()
            .AddSingleton<IUpdater, BackgroundUpdater>()
            .AddSingleton<IPriceDownloaderFactory, PriceDownloaderFactory>()
            .AddSingleton<WebDownloader>()
            .AddSingleton<IPortfolioDataDownloader, PortfolioDataDownloader>()
            .AddSingleton<ReportingWindowViewModel>()
            .AddSingleton<OptionsToolbarViewModel>()
            .AddSingleton<BasicDataViewModel>()
            .AddSingleton<StatisticsChartsViewModel>()
            .AddSingleton<MainWindowViewModel>();
        return services;
    }
}
