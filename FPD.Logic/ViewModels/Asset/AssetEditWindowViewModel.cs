using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Common.Structure.DataEdit;
using Common.UI;
using Common.UI.ViewModelBases;

using FinancialStructures.Database;
using FinancialStructures.NamingStructures;

using FPD.Logic.TemplatesAndStyles;
using FPD.Logic.ViewModels.Common;

namespace FPD.Logic.ViewModels.Asset
{
    /// <summary>
    /// View model for the listing of all assets and their data.
    /// </summary>
    public class AssetEditWindowViewModel : DataDisplayViewModelBase
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

        private int fSelectedIndex;

        /// <summary>
        /// Index of the selected tab.
        /// </summary>
        public int SelectedIndex
        {
            get => fSelectedIndex;
            set => SetAndNotify(ref fSelectedIndex, value, nameof(SelectedIndex));
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public AssetEditWindowViewModel(UiGlobals globals, UiStyles styles, IPortfolio portfolio, IUpdater<IPortfolio> dataUpdater)
            : base(globals, styles, portfolio, "Assets", Account.Asset)
        {
            _dataUpdater = dataUpdater;
            Tabs.Add(new DataNamesViewModel(DataStore, ReportLogger, styles, dataUpdater, (name) => LoadTabFunc(name), DataType));
            foreach (var tab in Tabs)
            {
                if (tab is DataDisplayViewModelBase vmb)
                {
                    vmb.UpdateRequest += _dataUpdater.PerformUpdate;
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
                        _ = Tabs.Remove(tab);
                    }

                    removableTabs.Clear();
                }
            }
        }

        internal void LoadTabFunc(object obj)
        {
            if (obj is NameData name)
            {
                var newVM = new SelectedAssetViewModel(DataStore, ReportLogger, Styles, fUiGlobals, name);
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
