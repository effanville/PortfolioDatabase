﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        private readonly Action<Action<IPortfolio>> UpdateDataAction;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public AssetEditWindowViewModel(UiGlobals globals, UiStyles styles, IPortfolio portfolio, Action<Action<IPortfolio>> updateData)
            : base(globals, styles, portfolio, "Assets", Account.Asset)
        {
            UpdateDataAction = updateData;
            Tabs.Add(new DataNamesViewModel(DataStore, updateData, ReportLogger, styles, (name) => LoadTabFunc(name), DataType));
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
                Tabs.Add(new SelectedAssetViewModel(DataStore, UpdateDataAction, ReportLogger, Styles, fUiGlobals, name));
            }
        }
    }
}