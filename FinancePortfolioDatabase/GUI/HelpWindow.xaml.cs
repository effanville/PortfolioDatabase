using FinancialStructures.ReportingStructures;
using System;
using System.IO;
using System.Windows;

namespace FinanceWindows
{
    /// <summary>
    /// Interaction logic for HelpWindow.xaml
    /// </summary>
    public partial class HelpWindow : Window
    {
        public HelpWindow()
        {
            InitializeComponent();
            string helpPath = Path.GetFullPath("GUI\\GuiSupport\\help.html");
            if (!File.Exists(helpPath))
            {
                ErrorReports.AddGeneralReport(ReportType.Error, "Could not find help documentation.");
                return;
            }
            Uri path = new Uri(helpPath);
            webBrowser.Source = path;
        }
    }
}
