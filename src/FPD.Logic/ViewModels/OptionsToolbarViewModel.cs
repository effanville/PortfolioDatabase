using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Effanville.Common.Structure.DataEdit;
using Effanville.Common.UI;
using Effanville.Common.UI.Commands;
using Effanville.Common.UI.Services;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.Download;
using Effanville.FinancialStructures.Database.Extensions;
using Effanville.FinancialStructures.Persistence;
using Effanville.FPD.Logic.TemplatesAndStyles;
using Effanville.FPD.Logic.ViewModels.Common;
using Microsoft.Extensions.Logging;

namespace Effanville.FPD.Logic.ViewModels
{
    /// <summary>
    /// View model for the top toolbar.
    /// </summary>
    public sealed class OptionsToolbarViewModel : DataDisplayViewModelBase
    {
        private readonly IPersistence<IPortfolio> _portfolioPersistence;
        private readonly IPortfolioDataDownloader _portfolioDataDownloader;
        private string _fileName;
        private string _directory;
        private string _baseCurrency;

        /// <summary>
        /// The base currency to display in the top dropdown.
        /// </summary>
        public string BaseCurrency
        {
            get => _baseCurrency;
            set => SetAndNotify(ref _baseCurrency, value);
        }

        private List<string> _currencies;

        /// <summary>
        /// The currencies to populate the dropdown with.
        /// </summary>
        public List<string> Currencies
        {
            get => _currencies;
            set => SetAndNotify(ref _currencies, value);
        }

        private bool _isLightTheme;
        public bool IsLightTheme
        {
            get => _isLightTheme;
            set => SetAndNotify(ref _isLightTheme, value);
        }

        private void UpdateColours(object sender, PropertyChangedEventArgs e) => Styles.UpdateTheme(IsLightTheme);

        public event EventHandler<PortfolioEventArgs> RefreshDisplay;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public OptionsToolbarViewModel(
            UiGlobals globals,
            IUiStyles styles,
            IPortfolio portfolio,
            IPortfolioDataDownloader portfolioDataDownloader,
            IUpdater updater,
            IPersistence<IPortfolio> persistence)
            : base(globals, styles, portfolio, updater, "Options")
        {
            _portfolioDataDownloader = portfolioDataDownloader;
            _portfolioPersistence = persistence;
            NewDatabaseCommand = new RelayCommand(ExecuteNewDatabase);
            SaveDatabaseCommand = new RelayCommand(ExecuteSaveDatabase);
            LoadDatabaseCommand = new RelayCommand(ExecuteLoadDatabase);
            UpdateDataCommand = new RelayCommand(ExecuteUpdateData);
            ImportFromOtherDatabaseCommand = new RelayCommand(ImportFromOtherDatabase);
            CleanDataCommand = new RelayCommand(ExecuteCleanData);
            RepriceResetCommand = new RelayCommand(ExecuteRepriceReset);
            RefreshCommand = new RelayCommand(ExecuteRefresh);
            CurrencyDropDownClosed = new RelayCommand(DropDownClosed);
            PropertyChanged += UpdateColours;
        }

        /// <inheritdoc/>
        public override void UpdateData(IPortfolio modelData, bool force)
        {
            base.UpdateData(modelData, force);
            _fileName = modelData.Name;
            Currencies = modelData.Names(Account.Currency).Concat(modelData.Companies(Account.Currency)).Distinct().ToList();
            if (!Currencies.Contains(modelData.BaseCurrency))
            {
                Currencies.Add(modelData.BaseCurrency);
            }

            BaseCurrency = modelData.BaseCurrency;
        }

        /// <summary>
        /// Command to reset database and load empty one.
        /// </summary>
        public ICommand NewDatabaseCommand { get; }
        private async void ExecuteNewDatabase()
        {
            MessageBoxOutcome result;
            if (ModelData.IsAlteredSinceSave)
            {
                result = DisplayGlobals.DialogCreationService.ShowMessageBox("Current database has unsaved alterations. Are you sure you want to load a new database?", "New Database?", BoxButton.YesNo, BoxImage.Warning);
            }
            else
            {
                result = DisplayGlobals.DialogCreationService.ShowMessageBox("Do you want to load a new database?", "New Database?", BoxButton.YesNo, BoxImage.Warning);
            }
            if (result == MessageBoxOutcome.Yes)
            {
                await _updater.PerformUpdate(
                    ModelData,
                    new UpdateRequestArgs<IPortfolio>(
                        userInitiated: true,
                        portfolio => portfolio.Clear()));
                DisplayGlobals.CurrentWorkingDirectory = "";
            }
        }

        /// <summary>
        /// Command to save the current database to file.
        /// </summary>
        public ICommand SaveDatabaseCommand { get; }
        private async void ExecuteSaveDatabase()
        {
            FileInteractionResult result = await DisplayGlobals.FileInteractionService.SaveFile("xml", _fileName, _directory, "XML Files|*.xml|Bin Files|*.bin|All Files|*.*");
            if (!result.Success)
            {
                return;
            }

            _fileName = DisplayGlobals.CurrentFileSystem.Path.GetFileName(result.FilePath);
            _directory = DisplayGlobals.CurrentFileSystem.Path.GetDirectoryName(result.FilePath);
            await _updater.PerformUpdate(ModelData, new UpdateRequestArgs<IPortfolio>(
                true,
                portfolio => portfolio.Name = DisplayGlobals.CurrentFileSystem.Path.GetFileNameWithoutExtension(result.FilePath)));
            PersistenceOptions options = PortfolioPersistence.CreateOptions(result.FilePath, DisplayGlobals.CurrentFileSystem);
            await _updater.PerformUpdate(ModelData, new UpdateRequestArgs<IPortfolio>(
                false,
                portfolio => _portfolioPersistence.Save(portfolio, options)));
            DisplayGlobals.CurrentWorkingDirectory = _directory;
        }

        /// <summary>
        /// Command to open file load dialog and load database from file.
        /// </summary>
        public ICommand LoadDatabaseCommand { get; }

        private async void ExecuteLoadDatabase()
        {
            FileInteractionResult result = await DisplayGlobals.FileInteractionService.OpenFile("xml", filter: "XML Files|*.xml|Bin Files|*.bin|All Files|*.*");
            if (!result.Success)
            {
                return;
            }

            var options = PortfolioPersistence.CreateOptions(result.FilePath, DisplayGlobals.CurrentFileSystem);
            await _updater.PerformUpdate(ModelData, new UpdateRequestArgs<IPortfolio>(
                true,
                portfolio => _portfolioPersistence.Load(portfolio, options)));

            var backupOptions = PortfolioPersistence.CreateOptions($"{result.FilePath}.bak", DisplayGlobals.CurrentFileSystem);
            await _updater.PerformUpdate(ModelData, new UpdateRequestArgs<IPortfolio>(
                false,
                portfolio => _portfolioPersistence.Save(portfolio, backupOptions)));
            DisplayGlobals.CurrentWorkingDirectory = DisplayGlobals.CurrentFileSystem.Path.GetDirectoryName(result.FilePath);
            _fileName = DisplayGlobals.CurrentFileSystem.Path.GetFileName(result.FilePath);
            _directory = DisplayGlobals.CurrentFileSystem.Path.GetDirectoryName(result.FilePath);
        }

        /// <summary>
        /// Command to instantiate the auto update of database values.
        /// </summary>
        public ICommand UpdateDataCommand { get; }
        private async void ExecuteUpdateData()
        {
            await _updater.PerformUpdate(
                ModelData,
                new UpdateRequestArgs<IPortfolio>(
                    true,
                    portfolio => _portfolioDataDownloader.Download(portfolio, ReportLogger)));
        }

        /// <summary>
        /// Command to import data from another database.
        /// </summary>
        public ICommand ImportFromOtherDatabaseCommand { get; }
        private async void ImportFromOtherDatabase()
        {
            FileInteractionResult result = await DisplayGlobals.FileInteractionService.OpenFile("xml", filter: "XML Files|*.xml|All Files|*.*");
            if (result.Success)
            {
                PersistenceOptions options = PortfolioPersistence.CreateOptions(result.FilePath, DisplayGlobals.CurrentFileSystem);
                IPortfolio otherPortfolio = _portfolioPersistence.Load(options);
                await _updater.PerformUpdate(
                    ModelData,
                    new UpdateRequestArgs<IPortfolio>(true, portfolio => portfolio.ImportValuesFrom(otherPortfolio, ReportLogger)));
            }
        }

        /// <summary>
        /// Command to remove unnecessary data from the database.
        /// </summary>
        public ICommand CleanDataCommand { get; }
        private async void ExecuteCleanData()
        {
            await _updater.PerformUpdate(
                ModelData,
                new UpdateRequestArgs<IPortfolio>(true, portfolio => portfolio.CleanData()));
        }

        /// <summary>
        /// Command to replace old trade types from the database.
        /// </summary>
        public ICommand RepriceResetCommand { get; }
        private async void ExecuteRepriceReset()
        {
            await _updater.PerformUpdate(
                ModelData,
                new UpdateRequestArgs<IPortfolio>(true, portfolio => portfolio.MigrateRepriceToReset()));
        }

        /// <summary>
        /// Command to call refresh on the ui windows.
        /// </summary>
        public ICommand RefreshCommand { get; }

        private void ExecuteRefresh()
        {
            RefreshDisplay?.Invoke(null, new PortfolioEventArgs(Account.All, true));
        }

        /// <summary>
        /// Command to update the base currency of the database.
        /// </summary>
        public ICommand CurrencyDropDownClosed { get; }

        private async void DropDownClosed()
            => await _updater.PerformUpdate(
                ModelData,
                new UpdateRequestArgs<IPortfolio>(true, portfolio => portfolio.BaseCurrency = BaseCurrency));
    }
}
