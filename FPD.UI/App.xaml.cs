using System;
using System.Windows;
using System.Windows.Threading;

using Effanville.FPD.UI.Windows;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
                    serviceCollection.AddSingleton<MainWindow>();
                })
                .Build();
        }

        /// <summary>
        /// This fires on startup of the application. Used to set the culture of the program.
        /// </summary>
        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            await _host.StartAsync();
            var mainWindow = _host.Services.GetService<MainWindow>();
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