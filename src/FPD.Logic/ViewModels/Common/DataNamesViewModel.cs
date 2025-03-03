using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using Effanville.Common.Structure.DataEdit;
using Effanville.Common.Structure.DataStructures;
using Effanville.Common.Structure.Reporting;
using Effanville.Common.UI;
using Effanville.Common.UI.Commands;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.Download;
using Effanville.FinancialStructures.Database.Extensions.Values;
using Effanville.FinancialStructures.NamingStructures;
using Effanville.FPD.Logic.TemplatesAndStyles;

namespace Effanville.FPD.Logic.ViewModels.Common
{
    /// <summary>
    /// Data store behind view for a list of names and associated update name methods.
    /// </summary>
    public sealed class DataNamesViewModel : StyledClosableViewModelBase<IPortfolio>
    {
        private readonly IUpdater _updater;
        private readonly IPortfolioDataDownloader _portfolioDataDownloader;
        internal readonly Account DataType;

        /// <summary>
        /// Whether a company column should be displayed.
        /// </summary>
        public bool DisplayCompany => DataType != Account.Benchmark;

        /// <summary>
        /// Backing field for <see cref="DataNames"/>.
        /// </summary>
        private ObservableCollection<NameDataViewModel> _dataNames = new ObservableCollection<NameDataViewModel>();

        /// <summary>
        /// Name data of the names to be displayed in this view.
        /// </summary>
        public ObservableCollection<NameDataViewModel> DataNames
        {
            get => _dataNames;
            set => SetAndNotify(ref _dataNames, value);
        }

        private NameDataViewModel _selectedName;

        /// <summary>
        /// The selected name with any alterations made by the user.
        /// These alterations update the database when certain commands are executed.
        /// </summary>
        public NameDataViewModel SelectedName
        {
            get => _selectedName;
            set
            {
                SetAndNotify(ref _selectedName, value);
                if (SelectedName != null)
                {
                    OnPropertyChanged(nameof(SelectedNameSet));
                }
            }
        }

        /// <summary>
        /// Whether a selection has been made.
        /// </summary>
        public bool SelectedNameSet => SelectedName != null;

        private List<DailyValuation> _selectedValueHistory;

        /// <summary>
        /// The evolution of the selected accounts total value.
        /// </summary>
        public List<DailyValuation> SelectedValueHistory
        {
            get => _selectedValueHistory;
            set => SetAndNotify(ref _selectedValueHistory, value);
        }

        /// <summary>
        /// Calculate default RowData for a row in the Datanames table.
        /// </summary>
        /// <returns></returns>
        public NameDataViewModel DefaultRow() =>
            new NameDataViewModel("", new NameData(), true, UpdateNameData, DisplayGlobals, Styles)
            {
                IsNew = true
            };

        /// <summary>
        /// Construct an instance.
        /// </summary>
        public DataNamesViewModel(
            IPortfolio portfolio,
            UiGlobals uiGlobals,
            IUiStyles styles,
            IUpdater updater,
            IPortfolioDataDownloader portfolioDataDownloader,
            Action<object> loadSelectedData,
            Account dataType)
            : base("Accounts", portfolio, uiGlobals, styles, closable: false)
        {
            DataType = dataType;
            _updater = updater;
            _portfolioDataDownloader = portfolioDataDownloader;
            SelectionChangedCommand = new RelayCommand<object>(ExecuteSelectionChanged);
            CreateCommand = new RelayCommand<object>(CreateEdit);
            DeleteCommand = new RelayCommand(ExecuteDelete);
            DownloadCommand = new RelayCommand(ExecuteDownloadCommand);
            OpenTabCommand = new RelayCommand(() => loadSelectedData(SelectedName?.ModelData));
        }

        /// <summary>
        /// Command that opens a tab associated to the selected entry.
        /// </summary>
        public ICommand OpenTabCommand { get; }

        private bool IsUpdated(IPortfolio dataToDisplay, NameData name)
            => dataToDisplay.LatestDate(DataType, name) == DateTime.Today
               || dataToDisplay.LatestValue(DataType, name).Equals(0.0m);

        /// <summary>
        /// Updates the data in this view model from the given portfolio.
        /// </summary>
        public override void UpdateData(IPortfolio modelData, bool force)
        {
            base.UpdateData(modelData, force);

            List<NameDataViewModel> values = modelData
                .NameDataForAccount(DataType)
                .Select(name => new NameDataViewModel("", name.Copy(), IsUpdated(modelData, name), UpdateNameData, DisplayGlobals, Styles)).ToList();
            values.Sort((a, b) => a.ModelData.CompareTo(b.ModelData));
            DisplayGlobals.CurrentDispatcher.BeginInvoke(() =>
            {
                DataNames.Clear();
                DataNames = new ObservableCollection<NameDataViewModel>(values);
            });

            if (SelectedName != null && !DataNames.Contains(SelectedName))
            {
                SelectedName = null;
                SelectedValueHistory = null;
            }

            OnPropertyChanged(nameof(DisplayCompany));
        }

        /// <summary>
        /// Downloads the latest data for the selected entry.
        /// </summary>
        public ICommand DownloadCommand { get; }

        private async void ExecuteDownloadCommand()
        {
            ReportLogger?.Log(ReportType.Information, nameof(ExecuteDownloadCommand), $"Download selected for account {SelectedName.ModelData} - a {DataType}");
            if (SelectedName == null)
            {
                return;
            }

            NameData names = SelectedName.ModelData;
            await _updater.PerformUpdate(
                ModelData,
                new UpdateRequestArgs<IPortfolio>(
                    true,
                    portfolio => _portfolioDataDownloader.Download(portfolio, ReportLogger).ConfigureAwait(false)));
        }

        /// <summary>
        /// Enacts the selected item in the data grid has changed.
        /// </summary>
        public ICommand SelectionChangedCommand { get; set; }

        private void ExecuteSelectionChanged(object args) => SelectionChanged(args);

        private async void SelectionChanged(object args)
        {
            // object reference issue in following line
            if (DataNames != null && args is NameDataViewModel selectableName && selectableName.ModelData != null)
            {
                SelectedName = selectableName;
                ReportLogger?.Log(ReportType.Information, nameof(SelectionChanged), $"Current item is a name {SelectedName.ModelData}");
                var history = await Task.Run(() => ModelData.NumberData(DataType, SelectedName.ModelData, ReportLogger).ToList());
                SelectedValueHistory = history;
                ReportLogger?.Log(ReportType.Information, nameof(SelectionChanged), $"Successfully updated SelectedItem.");
            }
            else
            {
                if (args == null)
                {
                    SelectedName = null;
                }
            }
        }

        /// <summary>
        /// Adds a new entry if the view has more than the repository, or edits an entry if these are the same.
        /// </summary>
        public ICommand CreateCommand { get; set; }

        private async void CreateEdit(object obj)
        {
            if (obj is not NameDataViewModel rowData || rowData.ModelData == null || !rowData.IsNew)
            {
                return;
            }

            NameData name = new NameData(rowData.Company, rowData.Name, rowData.Currency, rowData.Url, notes: rowData.Notes)
            {
                SectorsFlat = rowData.Sectors
            };
            UpdateResult<(Account, NameData)> result = await _updater.PerformUpdate(
                ModelData,
                new UpdateRequestArgs<IPortfolio, (Account, NameData)>(
                    true,
                    portfolio => portfolio.TryAdd(DataType, name)));
            ReportLogger.Log(ReportType.Information, nameof(CreateEdit), result.ToString());
        }

        /// <summary>
        /// Deletes the selected entry.
        /// </summary>
        public ICommand DeleteCommand { get; }

        public async void ExecuteDelete()
        {
            ReportLogger?.Log(ReportType.Information, nameof(ExecuteDelete), $"Deleting {SelectedName} from the database");
            if (SelectedName != null)
            {
                _ = DataNames.Remove(SelectedName);
                UpdateResult<(Account, NameData)> result = await _updater.PerformUpdate(
                    ModelData,
                    new UpdateRequestArgs<IPortfolio, (Account, NameData)>(
                        true,
                        portfolio => portfolio.TryRemove(DataType, SelectedName.ModelData)));
                ReportLogger?.Log(ReportType.Information, nameof(ExecuteDelete), result.ToString());
            }
            else
            {
                ReportLogger?.Log(ReportType.Error, nameof(ExecuteDelete), "Nothing was selected when trying to delete.");
            }
        }

        internal async void UpdateNameData(NameData _preEditSelectedName, NameData name)
        {
            UpdateResult<(Account, NameData)> result = await _updater.PerformUpdate(
                ModelData,
                new UpdateRequestArgs<IPortfolio, (Account, NameData)>(
                    true,
                    portfolio => portfolio.TryEditName(DataType, _preEditSelectedName, name)));
            ReportLogger?.Log(ReportType.Information, nameof(UpdateNameData), result.ToString());
        }
    }
}
