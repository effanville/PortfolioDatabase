using System;
using System.Windows.Input;

using Effanville.Common.Structure.Reporting;
using Effanville.Common.UI;
using Effanville.Common.UI.Commands;
using Effanville.Common.UI.Services;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.Database.Export.History;
using Effanville.FPD.Logic.Configuration;
using Effanville.FPD.Logic.TemplatesAndStyles;
using Effanville.FPD.Logic.ViewModels.Common;

namespace Effanville.FPD.Logic.ViewModels.Stats
{
    /// <summary>
    /// View model for the options for exporting the portfolio history.
    /// </summary>
    public sealed class ExportHistoryViewModel : DataDisplayViewModelBase
    {
        private readonly Action<object> _closeWindowAction;

        private int _historyGapDays;

        /// <summary>
        /// The number of days to have between history stats.
        /// </summary>
        public int HistoryGapDays
        {
            get => _historyGapDays;
            set => SetAndNotify(ref _historyGapDays, value);
        }

        private bool _generateSecurityValues;

        /// <summary>
        /// Should values for Securities be generated.
        /// </summary>
        public bool GenerateSecurityValues
        {
            get => _generateSecurityValues;
            set => SetAndNotify(ref _generateSecurityValues, value);
        }

        private bool _generateBankAccountValues;

        /// <summary>
        /// Should values for BankAccounts be generated.
        /// </summary>
        public bool GenerateBankAccountValues
        {
            get => _generateBankAccountValues;
            set => SetAndNotify(ref _generateBankAccountValues, value);
        }

        private bool _generateSectorValues;

        /// <summary>
        /// Should values for Sectors be generated.
        /// </summary>
        public bool GenerateSectorValues
        {
            get => _generateSectorValues;
            set => SetAndNotify(ref _generateSectorValues, value);
        }

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public ExportHistoryViewModel(UiGlobals globals, UiStyles styles, IConfiguration userConfiguration, IPortfolio portfolio, Action<object> closeWindow)
            : base(globals, styles, userConfiguration, portfolio, "", Account.All, closable: true)
        {
            _closeWindowAction = closeWindow;
            if (UserConfiguration.HasLoaded)
            {
                UserConfiguration.RestoreFromConfiguration(this);
            }
            else
            {
                HistoryGapDays = 20;
                UserConfiguration.StoreConfiguration(this);
                UserConfiguration.HasLoaded = true;
            }

            ExportHistoryCommand = new RelayCommand(ExecuteCreateHistory);
        }

        /// <summary>
        /// Command for exporting history data.
        /// </summary>
        public ICommand ExportHistoryCommand
        {
            get;
        }

        private void ExecuteCreateHistory()
        {
            UserConfiguration.StoreConfiguration(this);
            FileInteractionResult result = DisplayGlobals.FileInteractionService.SaveFile(".csv", DateTime.Today.Year + "-" + DateTime.Today.Month + "-" + DateTime.Today.Day + "-" + ModelData.Name + "-History.csv", filter: "CSV file|*.csv|All files|*.*");
            if (result.Success)
            {
                if (!result.FilePath.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    result.FilePath += ".csv";
                }

                PortfolioHistory history = new PortfolioHistory(ModelData, new PortfolioHistory.Settings(default, default, HistoryGapDays, generateSecurityValues: GenerateSecurityValues, generateBankAccountValues: GenerateBankAccountValues, generateSectorValues: GenerateSectorValues, generateSecurityRates: false, generateSectorRates: false));
                history.ExportToFile(result.FilePath, DisplayGlobals.CurrentFileSystem);
                _closeWindowAction(new PortfolioHistoryViewModel(ModelData, Styles));
            }
            else
            {
                ReportLogger.Log(ReportType.Error, ReportLocation.StatisticsPage.ToString(), $"Was not able to create Investment list page at {result.FilePath}");
            }
        }
    }
}
