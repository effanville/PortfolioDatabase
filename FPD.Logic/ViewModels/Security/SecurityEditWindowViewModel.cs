﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Common.UI;
using Common.UI.ViewModelBases;
using FPD.Logic.TemplatesAndStyles;
using FPD.Logic.ViewModels.Common;
using FinancialStructures.Database;
using FinancialStructures.NamingStructures;

namespace FPD.Logic.ViewModels.Security
{
    /// <summary>
    /// View model for the listing of all securities and their data.
    /// </summary>
    public class SecurityEditWindowViewModel : DataDisplayViewModelBase
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
        public SecurityEditWindowViewModel(UiGlobals globals, UiStyles styles, IPortfolio portfolio, string header, Account account, Action<Action<IPortfolio>> updateData)
            : base(globals, styles, portfolio, header, account)
        {
            UpdateDataAction = updateData;
            Tabs.Add(new DataNamesViewModel(DataStore, updateData, ReportLogger, styles, (name) => LoadTabFunc(name), account));
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
                Tabs.Add(new SelectedSecurityViewModel(DataStore, UpdateDataAction, ReportLogger, Styles, fUiGlobals, name, DataType));
            }
        }

        /// <summary>
        /// Removes a tab from the collection of tabs controlled by this view model.
        /// </summary>
        public bool RemoveTab(object obj)
        {
            return Tabs.Remove(obj);
        }
    }
}
