using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

using Common.Structure.DataStructures;
using Common.UI.Commands;
using Common.UI.ViewModelBases;

using FinancialStructures.Database;

using FPD.Logic.TemplatesAndStyles;

namespace FPD.Logic.ViewModels.Common
{
    /// <summary>
    /// View model for displaying a <see cref="TimeList"/>
    /// </summary>
    public sealed class TimeListViewModel : ViewModelBase<IPortfolio>
    {
        private readonly Action<DailyValuation> DeleteValueAction;
        private readonly Action<DailyValuation, DailyValuation> AddEditValueAction;

        internal DailyValuation fOldSelectedValuation;
        internal DailyValuation SelectedValuation;

        private List<DailyValuation> fValuations = new List<DailyValuation>();

        private UiStyles fStyles;

        /// <summary>
        /// The style object containing the style for the ui.
        /// </summary>
        public UiStyles Styles
        {
            get => fStyles;
            set => SetAndNotify(ref fStyles, value, nameof(Styles));
        }

        /// <summary>
        /// The list of values to display.
        /// </summary>
        public List<DailyValuation> Valuations
        {
            get => fValuations;
            set => SetAndNotify(ref fValuations, value, nameof(Valuations));
        }

        private string fValueName;

        /// <summary>
        /// The name of the type of value displayed.
        /// </summary>
        public string ValueName
        {
            get => fValueName;
            set => SetAndNotify(ref fValueName, value, nameof(ValueName));
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TimeListViewModel(
            TimeList timeList,
            string valueName,
            UiStyles styles,
            Action<DailyValuation> deleteValueAction,
            Action<DailyValuation, DailyValuation> addEditValueAction)
            : base("TLVM", null)
        {
            DeleteValueAction = deleteValueAction;
            AddEditValueAction = addEditValueAction;
            ValueName = valueName;
            Styles = styles;
            PreEditCommand = new RelayCommand(ExecutePreEdit);
            AddEditDataCommand = new RelayCommand(ExecuteAddEditData);
            SelectionChangedCommand = new RelayCommand<object>(ExecuteSelectionChanged);
            UpdateData(timeList);
        }

        /// <summary>
        /// Routine to update the data in the display.
        /// </summary>
        public void UpdateData(TimeList timeList)
        {
            Valuations = null;
            Valuations = timeList?.Values() ?? new List<DailyValuation>();
        }

        /// <summary>
        /// Create a new item with the default values.
        /// </summary>
        /// <returns></returns>
        public DailyValuation DefaultNewItem()
        {
            DailyValuation latest = null;
            if (Valuations != null && Valuations.Any())
            {
                latest = Valuations.Last();
            }

            return new DailyValuation()
            {
                Day = DateTime.Today,
                Value = latest?.Value ?? 0.0m
            };
        }

        /// <summary>
        /// Command to update the selected item.
        /// </summary>
        public ICommand SelectionChangedCommand
        {
            get;
            set;
        }
        private void ExecuteSelectionChanged(object obj)
        {
            if (Valuations != null && obj is DailyValuation data)
            {
                SelectedValuation = data;
            }
        }

        /// <summary>
        /// Called prior to an edit occurring in a row. This is used
        /// to record the state of the row before editing.
        /// </summary>
        public ICommand PreEditCommand
        {
            get;
            set;
        }

        private void ExecutePreEdit() => fOldSelectedValuation = SelectedValuation?.Copy();

        /// <summary>
        /// Command to add or edit data to the <see cref="TimeList"/>
        /// </summary>
        public ICommand AddEditDataCommand
        {
            get;
            set;
        }

        private void ExecuteAddEditData()
        {
            if (SelectedValuation != null)
            {
                AddEditValueAction(fOldSelectedValuation, SelectedValuation);
            }
        }

        /// <summary>
        /// Command to delete values from the <see cref="TimeList"/>
        /// </summary>
        public void DeleteValuation()
        {
            if (SelectedValuation != null)
            {
                DeleteValueAction(SelectedValuation);
            }
        }
    }
}
