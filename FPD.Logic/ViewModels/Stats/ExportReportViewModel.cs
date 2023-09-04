using System;
using System.Windows.Input;
using Common.Structure.Reporting;
using Common.UI;
using Common.UI.Commands;
using Common.UI.Services;
using FPD.Logic.Configuration;
using FPD.Logic.TemplatesAndStyles;
using FPD.Logic.ViewModels.Common;
using FinancialStructures.Database;
using FinancialStructures.Database.Export.Report;
using Common.Structure.ReportWriting;

namespace FPD.Logic.ViewModels.Stats
{
    /// <summary>
    /// Export to report options and routine.
    /// </summary>
    public sealed class ExportReportViewModel : DataDisplayViewModelBase
    {
        private readonly Action<object> _closeWindowAction;
        private bool _displayValueFunds;

        /// <summary>
        /// The number of days to have between history stats.
        /// </summary>
        public bool DisplayValueFunds
        {
            get => _displayValueFunds;
            set => SetAndNotify(ref _displayValueFunds, value);
        }

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public ExportReportViewModel(UiGlobals globals, UiStyles styles, IConfiguration userConfiguration, IPortfolio portfolio, Action<object> CloseWindow)
            : base(globals, styles, userConfiguration, portfolio, "", Account.All)
        {
            _closeWindowAction = CloseWindow;
            if (fUserConfiguration.HasLoaded)
            {
                fUserConfiguration.RestoreFromConfiguration(this);
            }
            else
            {
                DisplayValueFunds = true;
                fUserConfiguration.StoreConfiguration(this);
                fUserConfiguration.HasLoaded = true;
            }

            ExportReportCommand = new RelayCommand(ExecuteCreateReport);
        }

        /// <summary>
        /// Command for exporting history data.
        /// </summary>
        public ICommand ExportReportCommand
        {
            get;
        }

        private void ExecuteCreateReport()
        {
            fUserConfiguration.StoreConfiguration(this);
            FileInteractionResult result = DisplayGlobals.FileInteractionService.SaveFile(DocumentType.Html.ToString().ToLower(), $"{ModelData.Name}-report.html", filter: "Html file|*.html|All files|*.*");
            if (result.Success)
            {
                if (!result.FilePath.EndsWith(".html"))
                {
                    result.FilePath += ".html";
                }
                PortfolioReport portfolioInvestments = new PortfolioReport(ModelData, PortfolioReport.Settings.Default());
                portfolioInvestments.ExportToFile(DisplayGlobals.CurrentFileSystem, result.FilePath, PortfolioReport.ExportSettings.Default(), ReportLogger);
                _closeWindowAction(new HtmlStatsViewerViewModel(Styles, DisplayGlobals, "Exported Report", result.FilePath));
            }
            else
            {
                DisplayGlobals.ReportLogger.Log(ReportType.Error, ReportLocation.StatisticsPage.ToString(), $"Was not able to create Investment list page at {result.FilePath}");
            }
        }
    }
}
