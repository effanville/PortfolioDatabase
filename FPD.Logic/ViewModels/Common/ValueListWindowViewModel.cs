using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
            set => SetAndNotify(ref _selectedIndex, value, nameof(SelectedIndex));
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ValueListWindowViewModel(UiGlobals globals, UiStyles styles, IPortfolio portfolio, string title, Account accountType, IUpdater<IPortfolio> dataUpdater)
            : base(globals, styles, portfolio, title, accountType)
        {
            _dataUpdater = dataUpdater;
            UpdateData(portfolio);
            Tabs.Add(new DataNamesViewModel(DataStore, fUiGlobals.ReportLogger, styles, dataUpdater, (name) => LoadTabFunc(name), accountType));
            foreach (object tab in Tabs)
            {
                if (tab is ViewModelBase<IPortfolio> vmb)
                {
                    vmb.UpdateRequest += dataUpdater.PerformUpdate;
                }
            }
            SelectedIndex = 0;
        }

        /// <inheritdoc/>
        public override void UpdateData(IPortfolio dataToDisplay)
        {
            base.UpdateData(dataToDisplay);
            List<object> removableTabs = new List<object>();
            if (Tabs != null)
            {
                foreach (object item in Tabs)
                {
                    if (item is TabViewModelBase<IPortfolio> viewModel)
                    {
                        viewModel.UpdateData(dataToDisplay, tabItem => removableTabs.Add(tabItem));
                    }
                }

                if (removableTabs.Any())
                {
                    foreach (object tab in removableTabs)
                    {
                        fUiGlobals.CurrentDispatcher.BeginInvoke(() => _ = Tabs.Remove(tab));
                    }

                    removableTabs.Clear();
                }
            }
        }

        internal void LoadTabFunc(object obj)
        {
            if (obj is NameData name)
            {
                var newVM = new SelectedSingleDataViewModel(DataStore, Styles, fUiGlobals, name, DataType, _dataUpdater);
                newVM.UpdateRequest += _dataUpdater.PerformUpdate;
                Tabs.Add(newVM);
            }
        }

        /// <summary>
        /// Removes a tab from the collection of tabs controlled by this view model.
        /// </summary>
        public bool RemoveTab(object obj) => Tabs.Remove(obj);
    }
}
