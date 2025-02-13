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
        internal readonly IDataStoreUpdater<IPortfolio> _updater;
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
            new NameDataViewModel("", new NameData(), true, DataType, _updater, DisplayGlobals, Styles)
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
            IDataStoreUpdater<IPortfolio> dataUpdater,
            IPortfolioDataDownloader portfolioDataDownloader,
            Action<object> loadSelectedData,
            Account dataType)
            : base("Accounts", portfolio, uiGlobals, styles, closable: false)
        {
            DataType = dataType;
            _updater = dataUpdater;
            _portfolioDataDownloader = portfolioDataDownloader;
            SelectionChangedCommand = new RelayCommand<object>(ExecuteSelectionChanged);
            CreateCommand = new RelayCommand<object>(ExecuteCreateEdit);
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
                .Select(name => new NameDataViewModel("", name.Copy(), IsUpdated(modelData, name), DataType, _updater, DisplayGlobals, Styles)).ToList();
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

        private void ExecuteDownloadCommand()
        {
            _ = ReportLogger.Log(ReportSeverity.Detailed, ReportType.Information, ReportLocation.DatabaseAccess, $"Download selected for account {SelectedName.ModelData} - a {DataType}");
            if (SelectedName == null)
            {
                return;
            }

            NameData names = SelectedName.ModelData;
            OnUpdateRequest(new UpdateRequestArgs<IPortfolio>(true, async programPortfolio => await _portfolioDataDownloader.Download(ModelData, ReportLogger).ConfigureAwait(false)));
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
                _ = ReportLogger.Log(ReportSeverity.Detailed, ReportType.Information, ReportLocation.AddingData, $"Current item is a name {SelectedName.ModelData}");
                var history = await Task.Run(() => ModelData.NumberData(DataType, SelectedName.ModelData, ReportLogger).ToList());
                SelectedValueHistory = history;
                _ = ReportLogger.Log(ReportSeverity.Detailed, ReportType.Information, ReportLocation.AddingData, $"Successfully updated SelectedItem.");
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

        private void ExecuteCreateEdit(object obj)
        {
            if (obj is not NameDataViewModel rowData || rowData.ModelData == null || !rowData.IsNew)
            {
                return;
            }

            NameData name = new NameData(rowData.Company, rowData.Name, rowData.Currency, rowData.Url, notes: rowData.Notes)
            {
                SectorsFlat = rowData.Sectors
            };
            OnUpdateRequest(new UpdateRequestArgs<IPortfolio>(true, programPortfolio => programPortfolio.TryAdd(DataType, name)));
        }

        /// <summary>
        /// Deletes the selected entry.
        /// </summary>
        public ICommand DeleteCommand { get; }

        public void ExecuteDelete()
        {
            _ = ReportLogger?.Log(ReportSeverity.Detailed, ReportType.Information, ReportLocation.DeletingData, $"Deleting {SelectedName} from the database");
            if (SelectedName != null)
            {
                _ = DataNames.Remove(SelectedName);
                OnUpdateRequest(new UpdateRequestArgs<IPortfolio>(true, programPortfolio => programPortfolio.TryRemove(DataType, SelectedName.ModelData)));
            }
            else
            {
                _ = ReportLogger?.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.DeletingData, "Nothing was selected when trying to delete.");
            }
        }
    }
}
