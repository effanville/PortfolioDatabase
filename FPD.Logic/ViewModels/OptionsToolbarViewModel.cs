using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Common.Structure.Reporting;
using Common.UI;
using Common.UI.Commands;
using Common.UI.Services;
using FPD.Logic.TemplatesAndStyles;
using FPD.Logic.ViewModels.Common;
using FinancialStructures.Database;
using FinancialStructures.Database.Download;
using FinancialStructures.Database.Extensions;
using System.ComponentModel;

namespace FPD.Logic.ViewModels
{
    /// <summary>
    /// View model for the top toolbar.
    /// </summary>
    public class OptionsToolbarViewModel : DataDisplayViewModelBase
    {
        private string fFileName;
        private string fDirectory;
        private readonly Action<Action<IPortfolio>> DataUpdateCallback;
        private string fBaseCurrency;

        /// <summary>
        /// The base currency to display in the top dropdown.
        /// </summary>
        public string BaseCurrency
        {
            get => fBaseCurrency;
            set
            {
                SetAndNotify(ref fBaseCurrency, value, nameof(BaseCurrency));
                _ = ReportLogger.Log(ReportSeverity.Detailed, ReportType.Information, ReportLocation.DatabaseAccess, $"Editing BaseCurrency.");
            }
        }

        private List<string> fCurrencies;

        /// <summary>
        /// The currencies to populate the dropdown with.
        /// </summary>
        public List<string> Currencies
        {
            get => fCurrencies;
            set => SetAndNotify(ref fCurrencies, value, nameof(Currencies));
        }

        private bool fIsLightTheme;
        public bool IsLightTheme
        {
            get => fIsLightTheme;
            set => SetAndNotify(ref fIsLightTheme, value, nameof(IsLightTheme));
        }

        private void UpdateColours(object sender, PropertyChangedEventArgs e)
        {
            Styles.UpdateTheme(IsLightTheme);
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public OptionsToolbarViewModel(UiGlobals globals, UiStyles styles, IPortfolio portfolio, Action<Action<IPortfolio>> updateData)
            : base(globals, styles, portfolio, "Options")
        {
            DataUpdateCallback = updateData;
            UpdateData(portfolio);

            NewDatabaseCommand = new RelayCommand(ExecuteNewDatabase);
            SaveDatabaseCommand = new RelayCommand(ExecuteSaveDatabase);
            LoadDatabaseCommand = new RelayCommand(ExecuteLoadDatabase);
            UpdateDataCommand = new RelayCommand(ExecuteUpdateData);
            CleanDataCommand = new RelayCommand(ExecuteCleanData);
            RepriceResetCommand = new RelayCommand(ExecuteRepriceReset);
            RefreshCommand = new RelayCommand(ExecuteRefresh);
            CurrencyDropDownClosed = new RelayCommand(DropDownClosed);
            PropertyChanged += UpdateColours;
        }

        /// <inheritdoc/>
        public override void UpdateData(IPortfolio portfolio)
        {
            base.UpdateData(portfolio);
            fFileName = portfolio.Name;
            Currencies = portfolio.Names(Account.Currency).Concat(portfolio.Companies(Account.Currency)).Distinct().ToList();
            if (!Currencies.Contains(portfolio.BaseCurrency))
            {
                Currencies.Add(portfolio.BaseCurrency);
            }

            BaseCurrency = portfolio.BaseCurrency;
        }

        /// <summary>
        /// Command to reset database and load empty one.
        /// </summary>
        public ICommand NewDatabaseCommand { get; }
        private void ExecuteNewDatabase()
        {
            _ = ReportLogger.Log(ReportSeverity.Detailed, ReportType.Information, ReportLocation.AddingData, $"ExecuteNewDatabase called.");
            MessageBoxOutcome result;
            if (DataStore.IsAlteredSinceSave)
            {
                result = fUiGlobals.DialogCreationService.ShowMessageBox("Current database has unsaved alterations. Are you sure you want to load a new database?", "New Database?", BoxButton.YesNo, BoxImage.Warning);
            }
            else
            {
                result = fUiGlobals.DialogCreationService.ShowMessageBox("Do you want to load a new database?", "New Database?", BoxButton.YesNo, BoxImage.Warning);
            }
            if (result == MessageBoxOutcome.Yes)
            {
                DataUpdateCallback(programPortfolio => programPortfolio.Clear(ReportLogger));
                fUiGlobals.CurrentWorkingDirectory = "";
            }
        }

        /// <summary>
        /// Command to save the current database to file.
        /// </summary>
        public ICommand SaveDatabaseCommand { get; }
        private void ExecuteSaveDatabase()
        {
            ReportLogger.Log(ReportSeverity.Detailed, ReportType.Information, "Saving", $"Saving database {fFileName} called.");
            FileInteractionResult result = fUiGlobals.FileInteractionService.SaveFile("xml", fFileName, fDirectory, "XML Files|*.xml|All Files|*.*");
            if (result.Success)
            {
                fFileName = fUiGlobals.CurrentFileSystem.Path.GetFileName(result.FilePath);

                fDirectory = fUiGlobals.CurrentFileSystem.Path.GetDirectoryName(result.FilePath);
                DataUpdateCallback(portfo => portfo.Name = fUiGlobals.CurrentFileSystem.Path.GetFileNameWithoutExtension(result.FilePath));
                DataUpdateCallback(portfo => portfo.SavePortfolio(result.FilePath, fUiGlobals.CurrentFileSystem, ReportLogger));
                fUiGlobals.CurrentWorkingDirectory = fDirectory;
            }
        }

        /// <summary>
        /// Command to open file load dialog and load database from file.
        /// </summary>
        public ICommand LoadDatabaseCommand { get; }

        private void ExecuteLoadDatabase()
        {
            ReportLogger.Log(ReportSeverity.Detailed, ReportType.Information, "Loading", $"Loading database.");
            FileInteractionResult result = fUiGlobals.FileInteractionService.OpenFile("xml", filter: "XML Files|*.xml|All Files|*.*");
            if (result.Success)
            {
                DataUpdateCallback(programPortfolio => programPortfolio.FillDetailsFromFile(fUiGlobals.CurrentFileSystem, result.FilePath, ReportLogger));
                DataUpdateCallback(programPortfolio => programPortfolio.SavePortfolio($"{result.FilePath}.bak", fUiGlobals.CurrentFileSystem, ReportLogger));
                fUiGlobals.CurrentWorkingDirectory = fUiGlobals.CurrentFileSystem.Path.GetDirectoryName(result.FilePath);
                fFileName = fUiGlobals.CurrentFileSystem.Path.GetFileName(result.FilePath);
                fDirectory = fUiGlobals.CurrentFileSystem.Path.GetDirectoryName(result.FilePath);

                ReportLogger.Log(ReportSeverity.Detailed, ReportType.Information, "Loading", $"Loaded database {fFileName} successfully.");
            }
        }

        /// <summary>
        /// Command to instantiate the auto update of database values.
        /// </summary>
        public ICommand UpdateDataCommand { get; }
        private void ExecuteUpdateData()
        {
            ReportLogger.Log(ReportSeverity.Detailed, ReportType.Information, "Downloading", $"Execute update data for  database {fFileName} called.");
            DataUpdateCallback(async programPortfolio => await PortfolioDataUpdater.Download(Account.All, programPortfolio, null, ReportLogger).ConfigureAwait(false));
        }

        /// <summary>
        /// Command to remove unnecessary data from the database.
        /// </summary>
        public ICommand CleanDataCommand { get; }
        private void ExecuteCleanData()
        {
            ReportLogger.Log(ReportSeverity.Detailed, ReportType.Information, "EditingData", $"Execute clean database for database {fFileName} called.");
            DataUpdateCallback(programPortfolio => programPortfolio.CleanData());
        }

        /// <summary>
        /// Command to replace old trade types from the database.
        /// </summary>
        public ICommand RepriceResetCommand { get; }
        private void ExecuteRepriceReset()
        {
            ReportLogger.Log(ReportSeverity.Detailed, ReportType.Information, "EditingData", $"Execute clean database for database {fFileName} called.");
            DataUpdateCallback(programPortfolio => programPortfolio.MigrateRepriceToReset());
        }

        /// <summary>
        /// Command to call refresh on the ui windows.
        /// </summary>
        public ICommand RefreshCommand { get; }

        private void ExecuteRefresh()
        {
            ReportLogger.Log(ReportSeverity.Detailed, ReportType.Information, "DatabaseAccess", $"Execute refresh on the window fo database {fFileName} called.");
            DataUpdateCallback(programPortfolio => programPortfolio.OnPortfolioChanged(false, new PortfolioEventArgs(Account.All)));
        }

        /// <summary>
        /// Command to update the base currency of the database.
        /// </summary>
        public ICommand CurrencyDropDownClosed { get; }
        private void DropDownClosed() => DataUpdateCallback(portfolio => portfolio.BaseCurrency = BaseCurrency);
    }
}
