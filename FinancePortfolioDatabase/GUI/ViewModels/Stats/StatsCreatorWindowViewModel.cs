﻿using System;
using System.Windows.Input;
using FinancePortfolioDatabase.GUI.ViewModels.Common;
using FinancialStructures.Database;
using FinancialStructures.DataExporters;
using Common.Structure.Reporting;
using Common.UI.Commands;
using Common.UI.Services;
using Common.UI;
using FinancePortfolioDatabase.GUI.Configuration;
using FinancePortfolioDatabase.GUI.TemplatesAndStyles;

namespace FinancePortfolioDatabase.GUI.ViewModels.Stats
{
    /// <summary>
    /// View model for the statistics display.
    /// </summary>
    public class StatsCreatorWindowViewModel : DataDisplayViewModelBase
    {
        private readonly IConfiguration fUserConfiguration;
        private readonly UiGlobals fUiGlobals;

        private ExportHistoryViewModel fHistoryExport;

        /// <summary>
        /// The options for exporting the history of the portfolio.
        /// </summary>
        public ExportHistoryViewModel ExportHistoryOptions
        {
            get => fHistoryExport;
            set => SetAndNotify(ref fHistoryExport, value, nameof(ExportHistoryOptions));
        }

        private ExportStatsViewModel fDisplay;

        /// <summary>
        /// The options for exporting an html page.
        /// </summary>
        public ExportStatsViewModel StatsPageExportOptions
        {
            get => fDisplay;
            set => SetAndNotify(ref fDisplay, value, nameof(StatsPageExportOptions));
        }

        private readonly Action<object> fLoadTab;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public StatsCreatorWindowViewModel(IPortfolio portfolio, UiStyles styles, UiGlobals globals, IConfiguration userConfiguration, Action<object> loadTab)
            : base(styles, "Stats Creator", Account.All, portfolio)
        {
            fUserConfiguration = userConfiguration;
            if (fUserConfiguration.HasLoaded)
            {
                fUserConfiguration.RestoreFromConfiguration(this);
            }
            else
            {
                fUserConfiguration.HasLoaded = true;
            }

            fLoadTab = loadTab;
            fUiGlobals = globals;

            StatsPageExportOptions = new ExportStatsViewModel(DataStore, obj => fLoadTab(obj), Styles, fUiGlobals, fUserConfiguration.ChildConfigurations[UserConfiguration.StatsOptions]);
            ExportHistoryOptions = new ExportHistoryViewModel(DataStore, obj => fLoadTab(obj), Styles, fUiGlobals, fUserConfiguration.ChildConfigurations[UserConfiguration.HistoryOptions]);
            CreateInvestmentListCommand = new RelayCommand(ExecuteInvestmentListCommand);
        }

        /// <summary>
        /// Command to create a csv list of all investments.
        /// </summary>
        public ICommand CreateInvestmentListCommand
        {
            get;
        }

        private void ExecuteInvestmentListCommand()
        {
            FileInteractionResult result = fUiGlobals.FileInteractionService.SaveFile(".csv", DataStore.DatabaseName(fUiGlobals.CurrentFileSystem) + "-CSVStats.csv", DataStore.Directory(fUiGlobals.CurrentFileSystem), "CSV file|*.csv|All files|*.*");
            if (result.Success != null && (bool)result.Success)
            {
                if (!result.FilePath.EndsWith(".csv"))
                {
                    result.FilePath += ".csv";
                }

                InvestmentsExporter.Export(DataStore, result.FilePath, fUiGlobals.ReportLogger);
                fLoadTab(new SecurityInvestmentViewModel(DataStore, Styles));
            }
            else
            {
                _ = fUiGlobals.ReportLogger.LogUseful(ReportType.Error, ReportLocation.StatisticsPage, $"Was not able to create Investment list page at {result.FilePath}");
            }
        }

        /// <inheritdoc/>
        public override void UpdateData(IPortfolio dataToDisplay = null)
        {
            base.UpdateData(dataToDisplay);
            StatsPageExportOptions.UpdateData(dataToDisplay);
            ExportHistoryOptions.UpdateData(dataToDisplay);
        }
    }
}
