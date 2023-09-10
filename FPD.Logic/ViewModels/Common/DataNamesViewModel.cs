using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using Common.Structure.DataStructures;
using Common.Structure.Reporting;
using Common.UI.Commands;
using FPD.Logic.TemplatesAndStyles;
using FinancialStructures.Database;
using FinancialStructures.Database.Download;
using FinancialStructures.Database.Extensions.Values;
using FinancialStructures.NamingStructures;
using Common.Structure.DataEdit;
using Common.UI;

namespace FPD.Logic.ViewModels.Common
{
    /// <summary>
    /// Data store behind view for a list of names and associated update name methods.
    /// </summary>
    public sealed class DataNamesViewModel : StyledClosableViewModelBase<IPortfolio>
    {
        internal readonly IUpdater<IPortfolio> _updater;

        internal readonly Account TypeOfAccount;

        /// <summary>
        /// Whether a company column should be displayed.
        /// </summary>
        public bool DisplayCompany => TypeOfAccount != Account.Benchmark;

        /// <summary>
        /// Backing field for <see cref="DataNames"/>.
        /// </summary>
        private List<RowData> fDataNames = new List<RowData>();

        /// <summary>
        /// Name data of the names to be displayed in this view.
        /// </summary>
        public List<RowData> DataNames
        {
            get => fDataNames;
            set => SetAndNotify(ref fDataNames, value);
        }

        private RowData fSelectedName;

        /// <summary>
        /// The selected name with any alterations made by the user.
        /// These alterations update the database when certain commands are executed.
        /// </summary>
        public RowData SelectedName
        {
            get => fSelectedName;
            set
            {
                SetAndNotify(ref fSelectedName, value);
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

        private List<DailyValuation> fSelectedValueHistory;

        /// <summary>
        /// The evolution of the selected accounts total value.
        /// </summary>
        public List<DailyValuation> SelectedValueHistory
        {
            get => fSelectedValueHistory;
            set => SetAndNotify(ref fSelectedValueHistory, value);
        }

        /// <summary>
        /// Calculate default RowData for a row in the Datanames table.
        /// </summary>
        /// <returns></returns>
        public RowData DefaultRow()
        {
            var defaultRow = new RowData(new NameData(), true, TypeOfAccount, _updater, Styles)
            {
                IsNew = true
            };
            return defaultRow;
        }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        public DataNamesViewModel(IPortfolio portfolio, UiGlobals uiGlobals, UiStyles styles, IUpdater<IPortfolio> dataUpdater, Action<object> loadSelectedData, Account accountType)
            : base("Accounts", portfolio, uiGlobals, styles, closable: false)
        {
            TypeOfAccount = accountType;
            _updater = dataUpdater;
            UpdateData(portfolio);
            SelectionChangedCommand = new RelayCommand<object>(ExecuteSelectionChanged);
            CreateCommand = new RelayCommand<object>(ExecuteCreateEdit);
            DeleteCommand = new RelayCommand(ExecuteDelete);
            DownloadCommand = new RelayCommand(ExecuteDownloadCommand);
            OpenTabCommand = new RelayCommand(() => loadSelectedData(SelectedName?.Instance));
        }

        /// <summary>
        /// Command that opens a tab associated to the selected entry.
        /// </summary>
        public ICommand OpenTabCommand
        {
            get;
        }

        private bool IsUpdated(IPortfolio dataToDisplay, NameData name)
        {
            bool result = dataToDisplay.LatestDate(TypeOfAccount, name) == DateTime.Today || dataToDisplay.LatestValue(TypeOfAccount, name).Equals(0.0m);

            if (!result)
            {
            }
            return result;
        }

        /// <summary>
        /// Updates the data in this view model from the given portfolio.
        /// </summary>
        public override void UpdateData(IPortfolio modelData)
        {
            base.UpdateData(modelData);

            List<RowData> values = modelData.NameDataForAccount(TypeOfAccount).Select(name => new RowData(name, IsUpdated(modelData, name), TypeOfAccount, _updater, Styles)).ToList();
            DataNames = null;
            DataNames = values;
            DataNames ??= new List<RowData>();

            DataNames.Sort((a, b) => a.Instance.CompareTo(b.Instance));
            if (SelectedName != null && !DataNames.Contains(SelectedName))
            {
                SelectedName = null;
            }

            OnPropertyChanged(nameof(DisplayCompany));
        }

        /// <summary>
        /// Downloads the latest data for the selected entry.
        /// </summary>
        public ICommand DownloadCommand
        {
            get;
        }
        private void ExecuteDownloadCommand()
        {
            _ = ReportLogger.Log(ReportSeverity.Detailed, ReportType.Information, ReportLocation.DatabaseAccess, $"Download selected for account {SelectedName.Instance} - a {TypeOfAccount}");
            if (SelectedName != null)
            {
                NameData names = SelectedName.Instance;
                OnUpdateRequest(new UpdateRequestArgs<IPortfolio>(true, async programPortfolio => await PortfolioDataUpdater.Download(TypeOfAccount, programPortfolio, names, ReportLogger).ConfigureAwait(false)));
            }
        }

        /// <summary>
        /// Enacts the selected item in the datagrid has changed.
        /// </summary>
        public ICommand SelectionChangedCommand
        {
            get;
            set;
        }

        private void ExecuteSelectionChanged(object args)
        {
            // object reference issue in following line
            if (DataNames != null && args is RowData selectableName && selectableName.Instance != null)
            {
                SelectedName = selectableName;
                _ = ReportLogger.Log(ReportSeverity.Detailed, ReportType.Information, ReportLocation.AddingData, $"Current item is a name {SelectedName.Instance}");
                SelectedValueHistory = ModelData.NumberData(TypeOfAccount, SelectedName.Instance, ReportLogger).ToList();

                _ = ReportLogger.Log(ReportSeverity.Detailed, ReportType.Information, ReportLocation.AddingData, $"Successfully updated SelectedItem.");
            }
            else
            {
                if (args == CollectionView.NewItemPlaceholder)
                {
                    SelectedName = null;
                }
            }
        }

        /// <summary>
        /// Adds a new entry if the view has more than the repository, or edits an entry if these are the same.
        /// </summary>
        public ICommand CreateCommand
        {
            get;
            set;
        }

        private void ExecuteCreateEdit(object obj)
        {
            _ = ReportLogger.Log(ReportSeverity.Detailed, ReportType.Information, ReportLocation.DatabaseAccess, $"ExecuteCreateEdit called.");
            bool edited = false;
            if (obj is RowData rowData && rowData != null && rowData.Instance != null && rowData.IsNew)
            {
                NameData selectedInstance = rowData.Instance; //rowName.Instance;
                _ = ReportLogger.Log(ReportSeverity.Detailed, ReportType.Information, ReportLocation.AddingData, $"Adding {selectedInstance} to the database");
                NameData name = new NameData(selectedInstance.Company, selectedInstance.Name, selectedInstance.Currency, selectedInstance.Url, selectedInstance.Sectors, selectedInstance.Notes);
                OnUpdateRequest(new UpdateRequestArgs<IPortfolio>(true, programPortfolio => edited = programPortfolio.TryAdd(TypeOfAccount, name, ReportLogger)));
            }
        }

        /// <summary>
        /// Deletes the selected entry.
        /// </summary>
        public ICommand DeleteCommand
        {
            get;
        }

        public void ExecuteDelete()
        {
            _ = ReportLogger.Log(ReportSeverity.Detailed, ReportType.Information, ReportLocation.DeletingData, $"Deleting {SelectedName} from the database");
            if (SelectedName != null)
            {
                OnUpdateRequest(new UpdateRequestArgs<IPortfolio>(true, programPortfolio => programPortfolio.TryRemove(TypeOfAccount, SelectedName.Instance, ReportLogger)));
            }
            else
            {
                _ = ReportLogger.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.DeletingData, "Nothing was selected when trying to delete.");
            }
        }
    }
}
