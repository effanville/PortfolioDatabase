using System;
using System.Windows.Input;

using Effanville.Common.Structure.Reporting;
using Effanville.Common.UI;
using Effanville.Common.UI.Commands;
using Effanville.Common.UI.Services;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.Database.Export.Investments;

using FPD.Logic.Configuration;
using FPD.Logic.TemplatesAndStyles;
using FPD.Logic.ViewModels.Common;

namespace FPD.Logic.ViewModels.Stats
{
    /// <summary>
    /// View model for the statistics display.
    /// </summary>
    public sealed class StatsCreatorWindowViewModel : DataDisplayViewModelBase
    {
        private ExportHistoryViewModel _historyExport;

        /// <summary>
        /// The options for exporting the history of the portfolio.
        /// </summary>
        public ExportHistoryViewModel ExportHistoryOptions
        {
            get => _historyExport;
            set => SetAndNotify(ref _historyExport, value);
        }

        private ExportStatsViewModel _display;

        /// <summary>
        /// The options for exporting an html page.
        /// </summary>
        public ExportStatsViewModel StatsPageExportOptions
        {
            get => _display;
            set => SetAndNotify(ref _display, value);
        }

        private ExportReportViewModel _exportReport;

        /// <summary>
        /// The options for exporting an html page.
        /// </summary>
        public ExportReportViewModel ExportReportOptions
        {
            get => _exportReport;
            set => SetAndNotify(ref _exportReport, value);
        }

        private readonly Action<object> _loadTab;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public StatsCreatorWindowViewModel(UiGlobals globals, UiStyles styles, IConfiguration userConfiguration, IPortfolio portfolio, Action<object> loadTab)
            : base(globals, styles, userConfiguration, portfolio, "Stats Creator", Account.All)
        {
            UserConfiguration = userConfiguration;
            if (UserConfiguration.HasLoaded)
            {
                UserConfiguration.RestoreFromConfiguration(this);
            }
            else
            {
                UserConfiguration.StoreConfiguration(this);
                UserConfiguration.HasLoaded = true;
            }

            _loadTab = loadTab;

            StatsPageExportOptions = new ExportStatsViewModel(DisplayGlobals, Styles, UserConfiguration.ChildConfigurations[Configuration.UserConfiguration.StatsOptions], ModelData, obj => _loadTab(obj));
            if (!UserConfiguration.ChildConfigurations.TryGetValue(Configuration.UserConfiguration.ReportOptions, out _))
            {
                UserConfiguration.ChildConfigurations.Add(Configuration.UserConfiguration.ReportOptions, new ExportReportConfiguration());
            }

            ExportReportOptions = new ExportReportViewModel(DisplayGlobals, Styles, UserConfiguration.ChildConfigurations[Configuration.UserConfiguration.ReportOptions], ModelData, obj => _loadTab(obj));
            ExportHistoryOptions = new ExportHistoryViewModel(DisplayGlobals, Styles, UserConfiguration.ChildConfigurations[Configuration.UserConfiguration.HistoryOptions], ModelData, obj => _loadTab(obj));
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
            FileInteractionResult result = DisplayGlobals.FileInteractionService.SaveFile(".csv", ModelData.Name + "-CSVStats.csv", filter: "CSV file|*.csv|All files|*.*");
            if (result.Success)
            {
                if (!result.FilePath.EndsWith(".csv"))
                {
                    result.FilePath += ".csv";
                }
                PortfolioInvestments portfolioInvestments = new PortfolioInvestments(ModelData, new PortfolioInvestmentSettings());
                portfolioInvestments.ExportToFile(result.FilePath, DisplayGlobals.CurrentFileSystem, ReportLogger);
                _loadTab(new SecurityInvestmentViewModel(ModelData, Styles));
            }
            else
            {
                DisplayGlobals.ReportLogger.Log(ReportType.Error, ReportLocation.StatisticsPage.ToString(), $"Was not able to create Investment list page at {result.FilePath}");
            }
        }

        /// <inheritdoc/>
        public override void UpdateData(IPortfolio modelData)
        {
            base.UpdateData(modelData);
            StatsPageExportOptions.UpdateData(modelData);
            ExportHistoryOptions.UpdateData(modelData);
        }
    }
}
