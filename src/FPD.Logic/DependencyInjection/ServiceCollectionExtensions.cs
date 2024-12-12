using Effanville.Common.Structure.DataEdit;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.Download;
using Effanville.FPD.Logic.ViewModels;
using Effanville.FPD.Logic.ViewModels.Stats;

using Microsoft.Extensions.DependencyInjection;

namespace Effanville.FPD.Logic.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddViewModelDependencies(this IServiceCollection services)
    {
        _ = services
            .AddSingleton<IViewModelFactory, ViewModelFactory>()
            .AddSingleton<IAccountStatisticsProvider, StatisticsProvider>()
            .AddSingleton<IUpdater<IPortfolio>, BackgroundUpdater<IPortfolio>>()
            .AddSingleton<IPriceDownloaderFactory, PriceDownloaderFactory>()
            .AddSingleton<IPortfolioDataDownloader, PortfolioDataDownloader>()
            .AddSingleton<ReportingWindowViewModel>()
            .AddSingleton<OptionsToolbarViewModel>()
            .AddSingleton<BasicDataViewModel>()
            .AddSingleton<StatisticsChartsViewModel>()
            .AddSingleton<MainWindowViewModel>();
        return services;
    }
}
