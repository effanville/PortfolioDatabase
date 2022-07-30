using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using Common.Structure.DataStructures;
using Common.Structure.DisplayClasses;
using Common.Structure.Reporting;
using Common.UI.Commands;
using Common.UI.ViewModelBases;
using FPD.Logic.TemplatesAndStyles;
using FinancialStructures.Database;
using FinancialStructures.Database.Download;
using FinancialStructures.Database.Extensions.Values;
using FinancialStructures.NamingStructures;
using System.ComponentModel;

namespace FPD.Logic.ViewModels.Common
{
    public class RowData : SelectableEquatable<NameData>, IEditableObject
    {
        private readonly Account TypeOfAccount;
        private NameData fPreEditSelectedName;
        public bool IsNew
        {
            get; set;
        }

        /// <summary>
        /// Function which updates the main data store.
        /// </summary>
        private readonly Action<Action<IPortfolio>> UpdateDataCallback;

        public RowData(NameData name, bool isThis, Account accType, Action<Action<IPortfolio>> update)
            : base(name, isThis)
        {
            TypeOfAccount = accType;
            UpdateDataCallback = update;
        }

        public RowData()
        {
        }


        /// <inheritdoc/>
        public void BeginEdit()
        {
            fPreEditSelectedName = Instance?.Copy();
        }

        /// <inheritdoc/>
        public void CancelEdit()
        {
            fPreEditSelectedName = null;
        }

        /// <inheritdoc/>
        public void EndEdit()
        {
            NameData selectedInstance = Instance; //rowName.Instance;

            // maybe fired from editing stuff. Try that
            if (!string.IsNullOrEmpty(selectedInstance.Name) || !string.IsNullOrEmpty(selectedInstance.Company))
            {
                NameData name = new NameData(selectedInstance.Company, selectedInstance.Name, selectedInstance.Currency, selectedInstance.Url, selectedInstance.Sectors, selectedInstance.Notes);
                UpdateDataCallback(programPortfolio => programPortfolio.TryEditName(TypeOfAccount, fPreEditSelectedName, name, null));
            }
        }
    }

    /// <summary>
    /// Data store behind view for a list of names and associated update name methods.
    /// </summary>
    public sealed class DataNamesViewModel : TabViewModelBase<IPortfolio>
    {
        /// <summary>
        /// Function which updates the main data store.
        /// </summary>
        internal readonly Action<Action<IPortfolio>> UpdateDataCallback;

        /// <summary>
        /// Logs any possible issues in the routines here back to the user.
        /// </summary>
        private readonly IReportLogger ReportLogger;

        private UiStyles fStyles;

        /// <summary>
        /// The style object containing the style for the ui.
        /// </summary>
        public UiStyles Styles
        {
            get => fStyles;
            set => SetAndNotify(ref fStyles, value, nameof(Styles));
        }

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
            set => SetAndNotify(ref fDataNames, value, nameof(DataNames));
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
                SetAndNotify(ref fSelectedName, value, nameof(SelectedName));
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
            set => SetAndNotify(ref fSelectedValueHistory, value, nameof(SelectedValueHistory));
        }

        public RowData DefaultRow()
        {
            var thing = new RowData(new NameData(), false, TypeOfAccount, UpdateDataCallback);

            thing.IsNew = true;
            return thing;
        }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        public DataNamesViewModel(IPortfolio portfolio, Action<Action<IPortfolio>> updateDataCallback, IReportLogger reportLogger, UiStyles styles, Action<object> loadSelectedData, Account accountType)
            : base("Accounts", portfolio, loadSelectedData)
        {
            Styles = styles;
            UpdateDataCallback = updateDataCallback;
            TypeOfAccount = accountType;
            ReportLogger = reportLogger;
            DataNames = portfolio.NameDataForAccount(accountType).Select(name => new RowData(name, IsUpdated(portfolio, name), accountType, updateDataCallback)).ToList();
            DataNames.Sort();

            SelectionChangedCommand = new RelayCommand<object>(ExecuteSelectionChanged);
            CreateCommand = new RelayCommand<object>(ExecuteCreateEdit);
            DeleteCommand = new RelayCommand(ExecuteDelete);
            DownloadCommand = new RelayCommand(ExecuteDownloadCommand);
            OpenTabCommand = new RelayCommand(() => LoadSelectedTab(SelectedName?.Instance));
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
        public override void UpdateData(IPortfolio dataToDisplay, Action<object> removeTab)
        {
            base.UpdateData(dataToDisplay);

            List<RowData> values = dataToDisplay.NameDataForAccount(TypeOfAccount).Select(name => new RowData(name, IsUpdated(dataToDisplay, name), TypeOfAccount, UpdateDataCallback)).ToList();
            DataNames = null;
            DataNames = values;
            DataNames.Sort((a, b) => a.Instance.CompareTo(b.Instance));
            if (!DataNames.Contains(SelectedName))
            {
                SelectedName = null;
            }

            OnPropertyChanged(nameof(DisplayCompany));
        }

        /// <summary>
        /// Updates the data in this view model from the given portfolio.
        /// </summary>
        public override void UpdateData(IPortfolio dataToDisplay)
        {
            UpdateData(dataToDisplay, null);
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
                UpdateDataCallback(async programPortfolio => await PortfolioDataUpdater.Download(TypeOfAccount, programPortfolio, names, ReportLogger).ConfigureAwait(false));
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
            if (DataNames != null && args is RowData selectableName && selectableName.Instance != null)
            {
                SelectedName = selectableName;
                _ = ReportLogger.Log(ReportSeverity.Detailed, ReportType.Information, ReportLocation.AddingData, $"Current item is a name {SelectedName.Instance}");
                SelectedValueHistory = DataStore.NumberData(TypeOfAccount, SelectedName.Instance, ReportLogger).ToList();

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
                UpdateDataCallback(programPortfolio => edited = programPortfolio.TryAdd(TypeOfAccount, name, ReportLogger));
            }
        }

        /// <summary>
        /// Deletes the selected entry.
        /// </summary>
        public ICommand DeleteCommand
        {
            get;
        }

        private void ExecuteDelete()
        {
            _ = ReportLogger.Log(ReportSeverity.Detailed, ReportType.Information, ReportLocation.DeletingData, $"Deleting {SelectedName} from the database");
            if (SelectedName != null)
            {
                UpdateDataCallback(programPortfolio => programPortfolio.TryRemove(TypeOfAccount, SelectedName.Instance, ReportLogger));
            }
            else
            {
                _ = ReportLogger.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.DeletingData, "Nothing was selected when trying to delete.");
            }
        }
    }
}
