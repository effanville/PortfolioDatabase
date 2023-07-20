using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Common.UI;
using Common.UI.ViewModelBases;
using FPD.Logic.TemplatesAndStyles;
using FPD.Logic.ViewModels.Common;
using FinancialStructures.Database;
using FinancialStructures.NamingStructures;
using Common.Structure.DataEdit;

namespace FPD.Logic.ViewModels.Security
{
    /// <summary>
    /// View model for the listing of all securities and their data.
    /// </summary>
    public class SecurityEditWindowViewModel : DataDisplayViewModelBase
    {
        private readonly IUpdater<IPortfolio> _dataUpdater;
        /// <summary>
        /// The tabs to display, with the Security names, and
        /// selected security data.
        /// </summary>
        public ObservableCollection<object> Tabs
        {
            get;
            set;
        } = new ObservableCollection<object>();

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
        public SecurityEditWindowViewModel(UiGlobals globals, UiStyles styles, IPortfolio portfolio, string header, Account account, IUpdater<IPortfolio> dataUpdater)
            : base(globals, styles, portfolio, header, account)
        {
            _dataUpdater = dataUpdater;
            Tabs.Add(new DataNamesViewModel(DataStore, ReportLogger, styles, dataUpdater, (name) => LoadTabFunc(name), account));
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
                for (int tabIndex = 0; tabIndex < Tabs.Count; tabIndex++)
                {
                    if (Tabs[tabIndex] is TabViewModelBase<IPortfolio> viewModel)
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
                var newVM = new SelectedSecurityViewModel(DataStore, ReportLogger, Styles, fUiGlobals, name, DataType);
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
