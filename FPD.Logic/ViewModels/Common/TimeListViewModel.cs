using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

using Effanville.Common.Structure.DataStructures;
using Effanville.Common.UI.Commands;
using Effanville.FinancialStructures.Database;
using Effanville.FPD.Logic.TemplatesAndStyles;

namespace Effanville.FPD.Logic.ViewModels.Common
{
    /// <summary>
    /// View model for displaying a <see cref="TimeList"/>
    /// </summary>
    public sealed class TimeListViewModel : StyledViewModelBase<TimeList, IPortfolio>
    {
        private readonly Action<DailyValuation> _deleteValueAction;
        private readonly Action<DailyValuation, DailyValuation> _addEditValueAction;

        private DailyValuation _oldSelectedValuation;
        internal DailyValuation SelectedValuation;

        private List<DailyValuation> _valuations;

        /// <summary>
        /// The list of values to display.
        /// </summary>
        public List<DailyValuation> Valuations
        {
            get => _valuations;
            set => SetAndNotify(ref _valuations, value);
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
            : base(valueName, timeList, null, styles)
        {
            _deleteValueAction = deleteValueAction;
            _addEditValueAction = addEditValueAction;
            Styles = styles;
            PreEditCommand = new RelayCommand(ExecutePreEdit);
            AddEditDataCommand = new RelayCommand(ExecuteAddEditData);
            SelectionChangedCommand = new RelayCommand<object>(ExecuteSelectionChanged);
        }

        /// <summary>
        /// Routine to update the data in the display.
        /// </summary>
        public override void UpdateData(TimeList modelData)
        {
            base.UpdateData(modelData);
            Valuations = null;
            Valuations = modelData?.Values() ?? new List<DailyValuation>();
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

        private void ExecutePreEdit() => _oldSelectedValuation = SelectedValuation?.Copy();

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
                _addEditValueAction(_oldSelectedValuation, SelectedValuation);
            }
        }

        /// <summary>
        /// Command to delete values from the <see cref="TimeList"/>
        /// </summary>
        public void DeleteValuation()
        {
            if (SelectedValuation == null)
            {
                return;
            }

            Valuations.Remove(SelectedValuation);
            _deleteValueAction(SelectedValuation);
        }
    }
}
