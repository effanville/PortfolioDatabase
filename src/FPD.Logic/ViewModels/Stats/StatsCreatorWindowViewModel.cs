using System;
using System.Windows.Input;

using Effanville.Common.Structure.Reporting;
using Effanville.Common.UI;
using Effanville.Common.UI.Commands;
using Effanville.Common.UI.Services;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.Database.Export.Investments;
using Effanville.FPD.Logic.Configuration;
using Effanville.FPD.Logic.TemplatesAndStyles;
using Effanville.FPD.Logic.ViewModels.Common;

namespace Effanville.FPD.Logic.ViewModels.Stats
{
    /// <summary>
    /// View model for the statistics display.
    /// </summary>
    public sealed class StatsCreatorWindowViewModel : DataDisplayViewModelBase
    {
        private ExportHistoryViewModel _historyExport;
        private IViewModelFactory _viewModelFactory;

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

        public event EventHandler RequestAddTab;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public StatsCreatorWindowViewModel(UiGlobals globals, IUiStyles styles, IConfiguration userConfiguration, IPortfolio portfolio, IViewModelFactory viewModelFactory)
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

            StatsPageExportOptions = new ExportStatsViewModel(DisplayGlobals, Styles, UserConfiguration.ChildConfigurations[Configuration.UserConfiguration.StatsOptions], ModelData, obj => RequestAddTab?.Invoke(obj, EventArgs.Empty));
            if (!UserConfiguration.ChildConfigurations.TryGetValue(Configuration.UserConfiguration.ReportOptions, out _))
            {
                UserConfiguration.ChildConfigurations.Add(Configuration.UserConfiguration.ReportOptions, new ExportReportConfiguration());
            }

            ExportReportOptions = new ExportReportViewModel(DisplayGlobals, Styles, UserConfiguration.ChildConfigurations[Configuration.UserConfiguration.ReportOptions], ModelData, obj => RequestAddTab?.Invoke(obj, EventArgs.Empty));
            ExportHistoryOptions = new ExportHistoryViewModel(DisplayGlobals, Styles, UserConfiguration.ChildConfigurations[Configuration.UserConfiguration.HistoryOptions], ModelData, obj => RequestAddTab?.Invoke(obj, EventArgs.Empty));
            CreateInvestmentListCommand = new RelayCommand(ExecuteInvestmentListCommand);
            _viewModelFactory = viewModelFactory;
        }

        /// <summary>
        /// Command to create a csv list of all investments.
        /// </summary>
        public ICommand CreateInvestmentListCommand
        {
            get;
        }

        private async void ExecuteInvestmentListCommand()
        {
            ReportLogger.Log(ReportType.Information, nameof(ExecuteInvestmentListCommand), "Execute called");
            FileInteractionResult result = await DisplayGlobals.FileInteractionService.SaveFile(
                ".csv",
                $"{ModelData.Name}-CSVStats.csv",
                filter: "CSV file|*.csv|All files|*.*");
            if (result.Success)
            {
                if (!result.FilePath.EndsWith(".csv"))
                {
                    result.FilePath += ".csv";
                }
                PortfolioInvestments portfolioInvestments = new PortfolioInvestments(ModelData, new PortfolioInvestmentSettings());
                portfolioInvestments.ExportToFile(result.FilePath, DisplayGlobals.CurrentFileSystem, ReportLogger);
                RequestAddTab?.Invoke(_viewModelFactory.GenerateViewModel(ModelData, "", Account.All, nameof(SecurityInvestmentViewModel)), EventArgs.Empty);

            }
            else
            {
                ReportLogger.Log(ReportType.Error, nameof(ExecuteInvestmentListCommand), $"Was not able to create Investment list page at {result.FilePath}");
            }
        }

        /// <inheritdoc/>
        public override void UpdateData(IPortfolio modelData, bool force)
        {
            base.UpdateData(modelData, force);
            StatsPageExportOptions.UpdateData(modelData, force);
            ExportHistoryOptions.UpdateData(modelData, force);
        }
    }
}
