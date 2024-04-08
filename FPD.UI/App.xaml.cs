using System;
using System.IO.Abstractions;
using System.Windows;
using System.Windows.Threading;

using Effanville.Common.Structure.DataEdit;
using Effanville.Common.Structure.Reporting;
using Effanville.Common.UI;
using Effanville.Common.UI.Services;
using Effanville.Common.UI.Wpf;
using Effanville.Common.UI.Wpf.Services;
using Effanville.FinancialStructures.Database;
using Effanville.FPD.Logic.TemplatesAndStyles;
using Effanville.FPD.Logic.ViewModels;
using Effanville.FPD.UI.Windows;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Win32;

namespace Effanville.FPD.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private readonly IHost _host;
        
        /// <summary>
        /// Constructor for the application.
        /// </summary>
        public App()
        {
            _host = new HostBuilder()
                .ConfigureServices(serviceCollection =>
                {
                    serviceCollection.AddSingleton<MainWindow>()
                        .AddSingleton<Window>(x => x.GetService<MainWindow>())
                        .AddSingleton<IDispatcher, DispatcherInstance>()
                        .AddSingleton<IFileSystem, FileSystem>()
                        .AddSingleton<IFileInteractionService, FileInteractionService>()
                        .AddSingleton<DialogCreationService>()
                        .AddSingleton<IBaseDialogCreationService>(x => x.GetService<DialogCreationService>())
                        .AddSingleton<IDialogCreationService>(x => x.GetService<DialogCreationService>())
                        .AddSingleton<UiGlobals>()
                        .AddSingleton(_ => new UiStyles(IsLightTheme()))
                        .AddSingleton(_ => PortfolioFactory.GenerateEmpty())
                        .AddSingleton<IViewModelFactory, ViewModelFactory>()
                        .AddSingleton<IUpdater<IPortfolio>, BackgroundUpdater<IPortfolio>>()
                        .AddSingleton<MainWindowViewModel>();
                })
                .ConfigureLogging(loggingBuilder =>
                {
                    loggingBuilder.AddReportLogger();
                })
                .Build();
        }

        private static bool IsLightTheme()
        {
            using var key =
                Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            object value = key?.GetValue("AppsUseLightTheme");
            return value is int i && i > 0;
        }
        /// <summary>
        /// This fires on startup of the application. Used to set the culture of the program.
        /// </summary>
        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            DispatcherUnhandledException += Application_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            await _host.StartAsync();
            var mainWindow = _host.Services.GetService<MainWindow>();
            var viewModel = _host.Services.GetService<MainWindowViewModel>();
            mainWindow.DataContext = viewModel;
            mainWindow.Show();
        }

        /// <summary>
        /// Fires when unhandled exceptions occur. Opens a dialog box, and hopefully the program continues.
        /// </summary>
        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // Handling the exception within the UnhandledException handler.
            if (e == null)
            {
                return;
            }

            _ = MessageBox.Show(
                e.Exception?.Message + Environment.NewLine + e.Exception?.StackTrace,
                "Exception Caught", 
                MessageBoxButton.OK, 
                MessageBoxImage.Error);

            MainWindow main = _host.Services.GetService<MainWindow>();
            main?.PrintErrorLog(e.Exception);

            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                _ = MessageBox.Show(
                    ex.Message, 
                    "Uncaught Thread Exception",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            MainWindow main = _host.Services.GetService<MainWindow>();
            main?.PrintErrorLog(ex);
        }

        private async void App_OnExit(object sender, ExitEventArgs e)
        {
            using (_host)
            {
                await _host.StopAsync();
            }
        }
    }
}