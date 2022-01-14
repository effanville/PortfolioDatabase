﻿using System;
using System.Windows.Input;
using Common.Structure.Reporting;
using Common.UI;
using Common.UI.Commands;
using Common.UI.Services;

using FPD.Logic.Configuration;
using FPD.Logic.TemplatesAndStyles;
using FPD.Logic.ViewModels.Common;

using FinancialStructures.Database;
using FinancialStructures.Database.Extensions;
using FinancialStructures.Database.Export.Investments;

namespace FPD.Logic.ViewModels.Stats
{
    /// <summary>
    /// View model for the statistics display.
    /// </summary>
    public sealed class StatsCreatorWindowViewModel : DataDisplayViewModelBase
    {
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

        private ExportReportViewModel fExportReport;

        /// <summary>
        /// The options for exporting an html page.
        /// </summary>
        public ExportReportViewModel ExportReportOptions
        {
            get => fExportReport;
            set => SetAndNotify(ref fExportReport, value, nameof(ExportReportOptions));
        }

        private readonly Action<object> fLoadTab;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public StatsCreatorWindowViewModel(UiGlobals globals, UiStyles styles, IConfiguration userConfiguration, IPortfolio portfolio, Action<object> loadTab)
            : base(globals, styles, userConfiguration, portfolio, "Stats Creator", Account.All)
        {
            fUserConfiguration = userConfiguration;
            if (fUserConfiguration.HasLoaded)
            {
                fUserConfiguration.RestoreFromConfiguration(this);
            }
            else
            {
                fUserConfiguration.StoreConfiguration(this);
                fUserConfiguration.HasLoaded = true;
            }

            fLoadTab = loadTab;

            StatsPageExportOptions = new ExportStatsViewModel(fUiGlobals, Styles, fUserConfiguration.ChildConfigurations[UserConfiguration.StatsOptions], DataStore, obj => fLoadTab(obj));
            if (!fUserConfiguration.ChildConfigurations.TryGetValue(UserConfiguration.ReportOptions, out _))
            {
                fUserConfiguration.ChildConfigurations.Add(UserConfiguration.ReportOptions, new ExportReportConfiguration());
            }

            ExportReportOptions = new ExportReportViewModel(fUiGlobals, Styles, fUserConfiguration.ChildConfigurations[UserConfiguration.ReportOptions], DataStore, obj => fLoadTab(obj));
            ExportHistoryOptions = new ExportHistoryViewModel(fUiGlobals, Styles, fUserConfiguration.ChildConfigurations[UserConfiguration.HistoryOptions], DataStore, obj => fLoadTab(obj));
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
            if (result.Success)
            {
                if (!result.FilePath.EndsWith(".csv"))
                {
                    result.FilePath += ".csv";
                }
                PortfolioInvestments portfolioInvestments = new PortfolioInvestments(DataStore, new PortfolioInvestmentSettings());
                portfolioInvestments.ExportToFile(result.FilePath, fUiGlobals.CurrentFileSystem, ReportLogger);
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