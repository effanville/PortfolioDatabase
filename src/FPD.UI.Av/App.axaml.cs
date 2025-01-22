using System.IO.Abstractions;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using Effanville.Common.Structure.Reporting;
using Effanville.Common.UI;
using Effanville.Common.UI.Services;
using Effanville.FinancialStructures.Database;
using Effanville.FPD.AvaloniaUI.Services;
using Effanville.FPD.AvaloniaUI.Windows;
using Effanville.FPD.Logic.DependencyInjection;
using Effanville.FPD.Logic.ViewModels;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace Effanville.FPD.AvaloniaUI
{
    public partial class App : Application
    {
        private readonly IHost _host;
        public App()
        {
            var hostBuilder = Host.CreateApplicationBuilder();
            _ = hostBuilder.Services
                .AddSingleton<MainWindow>()
                .AddSingleton<Window>(x => x.GetService<MainWindow>())
                .AddSingleton<IDispatcher, DispatcherInstance>()
                .AddSingleton<IFileSystem, FileSystem>()
                .AddSingleton<IFileInteractionService, FileInteractionService>()
                .AddSingleton<DialogCreationService>()
                .AddSingleton<IBaseDialogCreationService>(x => x.GetService<DialogCreationService>())
                .AddSingleton<IDialogCreationService>(x => x.GetService<DialogCreationService>())
                .AddSingleton<UiGlobals>()
                .AddSingleton(_ => PortfolioFactory.GenerateEmpty())
                .AddSingleton(ConfigurationFactory.LoadConfig)
                .AddViewModelDependencies();
            _ = hostBuilder.Logging
                .ClearProviders()
                .AddReportLogger(UpdateReport);

            _host = hostBuilder.Build();
        }
        public override void Initialize() => AvaloniaXamlLoader.Load(this);

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var mainWindow = _host.Services.GetService<MainWindow>();
                var viewModel = _host.Services.GetService<MainWindowViewModel>();
                desktop.MainWindow = mainWindow;
                desktop.MainWindow!.DataContext = viewModel;
            }

            base.OnFrameworkInitializationCompleted();
        }
        private async void UpdateReport(ReportSeverity severity, ReportType type, string location, string message)
        {
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                var viewModel = _host.Services.GetService<MainWindowViewModel>();
                viewModel?.UpdateReport(severity, type, location, message);
            });
        }
    }
}