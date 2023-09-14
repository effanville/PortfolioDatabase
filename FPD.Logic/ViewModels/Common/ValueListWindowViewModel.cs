using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Common.UI;

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
        private readonly IViewModelFactory _viewModelFactory;

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

            List<object> tabsToRemove = new List<object>();
            foreach (object item in Tabs)
            {
                if (item is StyledClosableViewModelBase<IPortfolio, IPortfolio> viewModel1)
                {
                    viewModel1.UpdateData(modelData);
                }

                if (item is StyledClosableViewModelBase<ISecurity, IPortfolio> viewModel2)
                {
                    if (modelData.TryGetAccount(DataType, viewModel2.ModelData.Names, out var vl) 
                        && vl is ISecurity security)
                    {
                        viewModel2.UpdateData(security);
                    }
                    else
                    {
                        tabsToRemove.Add(item);
                    }
                }
            }
            
            foreach (object tab in tabsToRemove)
            {
                DisplayGlobals.CurrentDispatcher.BeginInvoke(() => _ = Tabs.Remove(tab));
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
                        var newVM = _viewModelFactory.GenerateViewModel(security, security.Names, DataType,  ModelData);
                        newVM.RequestClose += RemoveTab;
                        Tabs.Add(newVM);
                        break;
                    }
                    case IAmortisableAsset asset:
                    {
                        var newVM = _viewModelFactory.GenerateViewModel(asset, asset.Names, DataType,  ModelData);
                        newVM.RequestClose += RemoveTab;
                        Tabs.Add(newVM);
                        break;
                    }
                    case IExchangableValueList exchangableValueList:
                    {
                        var newVM = _viewModelFactory.GenerateViewModel(exchangableValueList, exchangableValueList.Names, DataType,  ModelData);
                        newVM.RequestClose += RemoveTab;
                        Tabs.Add(newVM);
                        break;
                    }
                    default:
                    {
                        var newVM = _viewModelFactory.GenerateViewModel(valueList,valueList.Names, DataType, ModelData);
                        newVM.RequestClose += RemoveTab;
                        Tabs.Add(newVM);
                        break;
                    }
                }
            }
            else
            {
                StyledClosableViewModelBase<IPortfolio, IPortfolio> newVM;
                newVM =  _viewModelFactory.GenerateViewModel(ModelData, null, DataType, ModelData);
            }
            
        }

        /// <summary>
        /// Removes a tab from the collection of tabs controlled by this view model.
        /// </summary>
        private void RemoveTab(object obj, EventArgs args) => Tabs.Remove(obj);
    }
}