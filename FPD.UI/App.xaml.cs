using System;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Windows.Threading;

using Effanville.FPD.UI.Windows;

namespace Effanville.FPD.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Constructor for the application.
        /// </summary>
        public App()
        {
            Timeline.DesiredFrameRateProperty.OverrideMetadata(
                typeof(Timeline),
                new FrameworkPropertyMetadata { DefaultValue = 10 }
            );
        }

        /// <summary>
        /// This fires on startup of the application. Used to set the culture of the program.
        /// </summary>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            var window = new MainWindow();
            window.Show();
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

            _ = MessageBox.Show(e.Exception?.Message + Environment.NewLine + e.Exception?.StackTrace,
                "Exception Caught", MessageBoxButton.OK, MessageBoxImage.Error);

            MainWindow main = Current.MainWindow as MainWindow;
            main?.PrintErrorLog(e.Exception);

            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                _ = MessageBox.Show(ex.Message, "Uncaught Thread Exception", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            MainWindow main = Current.MainWindow as MainWindow;
            main?.PrintErrorLog(ex);
        }

        private void App_OnExit(object sender, ExitEventArgs e)
        {
        }
    }
}