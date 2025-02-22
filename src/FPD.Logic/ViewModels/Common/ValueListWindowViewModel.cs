using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

using Effanville.Common.Structure.DataEdit;
using Effanville.Common.UI;
using Effanville.Common.UI.Commands;
using Effanville.Common.UI.ViewModelBases;
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
            IUpdater updater,
            IViewModelFactory viewModelFactory)
            : base(globals, styles, portfolio, updater, title, accountType)
        {
            _viewModelFactory = viewModelFactory;
            var dataNames = viewModelFactory.GenerateViewModel(portfolio, LoadTabFunc, accountType);
            Tabs.Add(dataNames);
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
                case StyledClosableViewModelBase<IPortfolio> viewModel1:
                {
                    viewModel1.UpdateData(modelData, false);
                    return true;
                }
                case StyledClosableViewModelBase<ISecurity> viewModel2:
                {
                    if (!modelData.TryGetAccount(DataType, viewModel2.ModelData.Names, out ISecurity security))
                    {
                        return false;
                    }

                    viewModel2.UpdateData(security, false);
                    return true;

                }
                case StyledClosableViewModelBase<IAmortisableAsset> viewModel3:
                {
                    if (!modelData.TryGetAccount(DataType, viewModel3.ModelData.Names, out IAmortisableAsset asset))
                    {
                        return false;
                    }

                    viewModel3.UpdateData(asset, false);
                    return true;

                }
                case StyledClosableViewModelBase<IValueList> viewModel4:
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
                        if (Tabs.Where(x => x is ViewModelBase<ISecurity>)
                            .Any(y => (y as ViewModelBase<ISecurity>).ModelData == security))
                        {
                            return;
                        }

                        var newViewModel = _viewModelFactory.GenerateViewModel(
                            security,
                            security.Names,
                            DataType);
                        if (newViewModel == null)
                        {
                            break;
                        }

                        newViewModel.RequestClose += RemoveTab;
                        Tabs.Add(newViewModel);
                        break;
                    }
                    case IAmortisableAsset asset:
                    {
                        if (Tabs.Where(x => x is ViewModelBase<IAmortisableAsset>)
                            .Any(y => (y as ViewModelBase<IAmortisableAsset>).ModelData == asset))
                        {
                            return;
                        }
                        var newViewModel = _viewModelFactory.GenerateViewModel(
                            asset,
                            asset.Names,
                            DataType);
                        newViewModel.RequestClose += RemoveTab;
                        Tabs.Add(newViewModel);
                        break;
                    }
                    default:
                    {
                        if (Tabs.Where(x => x is ViewModelBase<IValueList>)
                            .Any(y => (y as ViewModelBase<IValueList>).ModelData == valueList))
                        {
                            return;
                        }
                        var newViewModel = _viewModelFactory.GenerateViewModel(
                            valueList,
                            valueList.Names,
                            DataType);
                        newViewModel.RequestClose += RemoveTab;
                        Tabs.Add(newViewModel);
                        break;
                    }
                }
            }
            else
            {
                StyledClosableViewModelBase<IPortfolio> newViewModel = _viewModelFactory.GenerateViewModel(
                    ModelData,
                    (TwoName)null,
                    DataType);
                if (newViewModel != null)
                {
                    Tabs.Add(newViewModel);
                }
            }
        }

        /// <summary>
        /// Removes a tab from the collection of tabs controlled by this view model.
        /// </summary>
        private void RemoveTab(object obj, EventArgs args) => Tabs.Remove(obj);
    }
}