using System;
using System.Collections.ObjectModel;

using Common.UI;
using Common.UI.ViewModelBases;

using FPD.Logic.TemplatesAndStyles;

using FinancialStructures.Database;
using FinancialStructures.FinanceStructures;
using FinancialStructures.NamingStructures;

using Common.Structure.DataEdit;

namespace FPD.Logic.ViewModels.Common
{
    /// <summary>
    /// A view model for displaying a collect of <see cref="IValueList"/>
    /// </summary>
    public class ValueListWindowViewModel : DataDisplayViewModelBase
    {
        private readonly IUpdater<IPortfolio> _dataUpdater;

        private readonly Func<IPortfolio, UiStyles,
            UiGlobals,
            NameData,
            Account,
            IUpdater<IPortfolio>, ClosableViewModelBase<IPortfolio>> _viewModelFactory;

        /// <summary>
        /// The tabs to display.
        /// </summary>
        public ObservableCollection<object> Tabs { get; set; } = new ObservableCollection<object>();

        private int _selectedIndex;

        /// <summary>
        /// Index of the selected tab.
        /// </summary>
        public int SelectedIndex
        {
            get => _selectedIndex;
            set => SetAndNotify(ref _selectedIndex, value);
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ValueListWindowViewModel(
            UiGlobals globals,
            UiStyles styles,
            IPortfolio portfolio,
            string title,
            Account accountType,
            IUpdater<IPortfolio> dataUpdater,
            Func<IPortfolio, UiStyles,
                UiGlobals,
                NameData,
                Account,
                IUpdater<IPortfolio>, ClosableViewModelBase<IPortfolio>> viewModelFactory)
            : base(globals, styles, portfolio, title, accountType)
        {
            _dataUpdater = dataUpdater;
            _viewModelFactory = viewModelFactory;
            var dataNames = new DataNamesViewModel(
                ModelData,
                DisplayGlobals,
                styles,
                dataUpdater,
                LoadTabFunc,
                accountType);
            Tabs.Add(dataNames);
            dataNames.UpdateRequest += dataUpdater.PerformUpdate;
            SelectedIndex = 0;
        }

        /// <inheritdoc/>
        public override void UpdateData(IPortfolio modelData)
        {
            base.UpdateData(modelData);
            if (Tabs == null)
            {
                return;
            }

            foreach (object item in Tabs)
            {
                if (item is ViewModelBase<IPortfolio> viewModel)
                {
                    viewModel.UpdateData(modelData);
                }
            }
        }

        internal void LoadTabFunc(object obj)
        {
            if (obj is not NameData name)
            {
                return;
            }

            var newVM = _viewModelFactory(
                ModelData,
                Styles,
                DisplayGlobals,
                name,
                DataType,
                _dataUpdater);
            newVM.RequestClose += RemoveTab;
            Tabs.Add(newVM);
        }

        /// <summary>
        /// Removes a tab from the collection of tabs controlled by this view model.
        /// </summary>
        private void RemoveTab(object obj, EventArgs args) => Tabs.Remove(obj);
    }
}