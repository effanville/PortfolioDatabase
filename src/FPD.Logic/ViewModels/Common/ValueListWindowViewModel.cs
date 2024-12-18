﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

using Effanville.Common.Structure.DataEdit;
using Effanville.Common.UI;
using Effanville.Common.UI.Commands;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.FinanceStructures;
using Effanville.FinancialStructures.NamingStructures;
using Effanville.FPD.Logic.TemplatesAndStyles;

namespace Effanville.FPD.Logic.ViewModels.Common
{
    /// <summary>
    /// A view model for displaying a collect of <see cref="IValueList"/>
    /// </summary>
    public class ValueListWindowViewModel : DataDisplayViewModelBase
    {
        private readonly IViewModelFactory _viewModelFactory;

        public ObservableCollection<object> Tabs { get; set; } = new ObservableCollection<object>();

        public ICommand SelectionChanged { get; }

        private void ExecuteSelectionChanged(IList source)
        {
            if (source is not object[] list || list.Length != 1)
            {
                return;
            }

            UpdateTab(list[0], ModelData);
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ValueListWindowViewModel(
            UiGlobals globals,
            IUiStyles styles,
            IPortfolio portfolio,
            string title,
            Account accountType,
            IUpdater<IPortfolio> dataUpdater,
            IViewModelFactory viewModelFactory)
            : base(globals, styles, portfolio, title, accountType)
        {
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
            dataNames.RequestClose += RemoveTab;
            SelectionChanged = new RelayCommand<IList>(ExecuteSelectionChanged);
        }

        /// <inheritdoc/>
        public override void UpdateData(IPortfolio modelData, bool force)
        {
            base.UpdateData(modelData, force);
            if (Tabs == null)
            {
                return;
            }

            List<object> tabsToRemove = new List<object>();
            foreach (object item in Tabs)
            {
                if (!UpdateTab(item, modelData))
                {
                    tabsToRemove.Add(item);
                }
            }

            foreach (object tab in tabsToRemove)
            {
                DisplayGlobals.CurrentDispatcher.BeginInvoke(() => _ = Tabs.Remove(tab));
            }
        }

        private bool UpdateTab(object item, IPortfolio modelData)
        {
            switch (item)
            {
                case StyledClosableViewModelBase<IPortfolio, IPortfolio> viewModel1:
                {
                    viewModel1.UpdateData(modelData, false);
                    return true;
                }
                case StyledClosableViewModelBase<ISecurity, IPortfolio> viewModel2:
                {
                    if (!modelData.TryGetAccount(DataType, viewModel2.ModelData.Names, out ISecurity security))
                    {
                        return false;
                    }

                    viewModel2.UpdateData(security, false);
                    return true;

                }
                case StyledClosableViewModelBase<IAmortisableAsset, IPortfolio> viewModel3:
                {
                    if (!modelData.TryGetAccount(DataType, viewModel3.ModelData.Names, out IAmortisableAsset asset))
                    {
                        return false;
                    }

                    viewModel3.UpdateData(asset, false);
                    return true;

                }
                case StyledClosableViewModelBase<IValueList, IPortfolio> viewModel4:
                {
                    if (!modelData.TryGetAccount(DataType, viewModel4.ModelData.Names, out IValueList vl))
                    {
                        return false;
                    }

                    viewModel4.UpdateData(vl, false);
                    return true;

                }
                default:
                    return false;
            }
        }

        internal void LoadTabFunc(object obj)
        {
            if (obj is not NameData name)
            {
                return;
            }

            if (ModelData.TryGetAccount(DataType, name, out IValueList valueList))
            {
                switch (valueList)
                {
                    case ISecurity security:
                    {
                        var newViewModel = _viewModelFactory.GenerateViewModel(
                            security, 
                            security.Names,
                            DataType,
                            ModelData);
                        newViewModel.RequestClose += RemoveTab;
                        Tabs.Add(newViewModel);
                        break;
                    }
                    case IAmortisableAsset asset:
                    {
                        var newViewModel = _viewModelFactory.GenerateViewModel(
                            asset, 
                            asset.Names,
                            DataType, 
                            ModelData);
                        newViewModel.RequestClose += RemoveTab;
                        Tabs.Add(newViewModel);
                        break;
                    }
                    default:
                    {
                        var newViewModel = _viewModelFactory.GenerateViewModel(
                            valueList,
                            valueList.Names,
                            DataType,
                            ModelData);
                        newViewModel.RequestClose += RemoveTab;
                        Tabs.Add(newViewModel);
                        break;
                    }
                }
            }
            else
            {
                var newViewModel =  _viewModelFactory.GenerateViewModel(
                    ModelData, 
                    null,
                    DataType, 
                    ModelData);
                Tabs.Add(newViewModel);
            }

        }
        
        /// <summary>
        /// Removes a tab from the collection of tabs controlled by this view model.
        /// </summary>
        private void RemoveTab(object obj, EventArgs args) => Tabs.Remove(obj);
    }
}