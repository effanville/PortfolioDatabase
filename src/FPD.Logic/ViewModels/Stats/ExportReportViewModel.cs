﻿using System;
using System.Windows.Input;

using Effanville.Common.ReportWriting.Documents;
using Effanville.Common.Structure.Reporting;
using Effanville.Common.UI;
using Effanville.Common.UI.Commands;
using Effanville.Common.UI.Services;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.Database.Export.Report;
using Effanville.FPD.Logic.Configuration;
using Effanville.FPD.Logic.TemplatesAndStyles;
using Effanville.FPD.Logic.ViewModels.Common;

namespace Effanville.FPD.Logic.ViewModels.Stats
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
        public ExportReportViewModel(UiGlobals globals, IUiStyles styles, IConfiguration userConfiguration, IPortfolio portfolio, Action<object> CloseWindow)
            : base(globals, styles, userConfiguration, portfolio, null, "", Account.All)
        {
            _closeWindowAction = CloseWindow;
            if (UserConfiguration.HasLoaded)
            {
                UserConfiguration.RestoreFromConfiguration(this);
            }
            else
            {
                DisplayValueFunds = true;
                UserConfiguration.StoreConfiguration(this);
                UserConfiguration.HasLoaded = true;
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

        private async void ExecuteCreateReport()
        {
            UserConfiguration.StoreConfiguration(this);
            FileInteractionResult result = await DisplayGlobals.FileInteractionService.SaveFile(
                DocumentType.Html.ToString().ToLower(),
                $"{ModelData.Name}-report.html",
                filter: "Html file|*.html|All files|*.*");
            if (result.Success)
            {
                if (!result.FilePath.EndsWith(".html"))
                {
                    result.FilePath += ".html";
                }
                PortfolioReport portfolioInvestments = new PortfolioReport(ModelData, PortfolioReport.Settings.Default());
                portfolioInvestments.ExportToFile(DisplayGlobals.CurrentFileSystem, result.FilePath, PortfolioReport.ExportSettings.Default(), ReportLogger);
                _closeWindowAction(new HtmlViewerViewModel(Styles, DisplayGlobals, "Exported Report", result.FilePath));
            }
            else
            {
                DisplayGlobals.ReportLogger.Error(nameof(ExportReportViewModel), $"Was not able to create Investment list page at {result.FilePath}");
            }
        }
    }
}
