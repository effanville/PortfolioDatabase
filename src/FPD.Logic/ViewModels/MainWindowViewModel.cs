using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Effanville.Common.Structure.Reporting;
using Effanville.Common.UI;
using Effanville.Common.UI.ViewModelBases;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.FinanceStructures;
using Effanville.FPD.Logic.Configuration;
using Effanville.FPD.Logic.TemplatesAndStyles;
using Effanville.FPD.Logic.ViewModels.Common;
using Effanville.FPD.Logic.ViewModels.Stats;

namespace Effanville.FPD.Logic.ViewModels;

/// <summary>
/// View model for the entire display.
/// </summary>
public sealed class MainWindowViewModel : PropertyChangedBase
{
    private readonly object _tabsLock = new object();
    private readonly object _updatingLock = new object();
    internal readonly IConfiguration UserConfiguration;

    public UiGlobals Globals { get; }

    private IUiStyles _styles;

    /// <summary>
    /// The styles for the Ui.
    /// </summary>
    public IUiStyles Styles
    {
        get => _styles;
        set => SetAndNotify(ref _styles, value);
    }

    /// <summary>
    /// The portfolio for the view model instance.
    /// </summary>
    public IPortfolio ProgramPortfolio { get; }

    private readonly OptionsToolbarViewModel _optionsToolbarCommands;

    /// <summary>
    /// view model for the top toolbar.
    /// </summary>
    public OptionsToolbarViewModel OptionsToolbarCommands
    {
        get => _optionsToolbarCommands;
        private init => SetAndNotify(ref _optionsToolbarCommands, value);
    }

    private readonly ReportingWindowViewModel _reports;

    /// <summary>
    /// View model for the reports view.
    /// </summary>
    public ReportingWindowViewModel ReportsViewModel
    {
        get => _reports;
        private init => SetAndNotify(ref _reports, value);
    }

    /// <summary>
    /// The collection of tabs to hold the data and interactions for the various sub-windows.
    /// </summary>
    public ObservableCollection<object> Tabs { get; } = new();

    /// <summary>
    /// Default constructor.
    /// </summary>
    public MainWindowViewModel(
        UiGlobals globals,
        IUiStyles styles,
        IPortfolio portfolio,
        IViewModelFactory viewModelFactory,
        IConfiguration configuration,
        ReportingWindowViewModel reportsViewModel,
        OptionsToolbarViewModel optionsViewModel,
        BasicDataViewModel basicDataViewModel,
        StatisticsChartsViewModel statisticsChartsViewModel)
    {
        ProgramPortfolio = portfolio;
        _styles = styles;
        Globals = globals;
        UserConfiguration = configuration;

        ReportsViewModel = reportsViewModel;

        OptionsToolbarCommands = optionsViewModel;
        if (OptionsToolbarCommands != null)
        {
            OptionsToolbarCommands.ModelUpdated += OnModelUpdated;
            OptionsToolbarCommands.IsLightTheme = styles.IsLightTheme;
        }

        if (basicDataViewModel != null)
        {
            Tabs.Add(basicDataViewModel);
        }

        Tabs.Add(viewModelFactory.GenerateViewModel(
            ProgramPortfolio,
            "",
            Account.All,
            nameof(StatsViewModel)));
        if (statisticsChartsViewModel != null)
        {
            Tabs.Add(statisticsChartsViewModel);
        }

        Tabs.Add(viewModelFactory.GenerateViewModel(
            ProgramPortfolio,
            "",
            Account.All,
            nameof(StatsCreatorWindowViewModel)));

        Tabs.Add(viewModelFactory.GenerateViewModel(
            ProgramPortfolio,
            "",
            Account.Security,
            nameof(DataNamesViewModel)));
        Tabs.Add(viewModelFactory.GenerateViewModel(
            ProgramPortfolio,
            "",
            Account.BankAccount,
            nameof(DataNamesViewModel)));
        Tabs.Add(viewModelFactory.GenerateViewModel(
            ProgramPortfolio,
            "",
            Account.Pension,
            nameof(DataNamesViewModel)));
        Tabs.Add(viewModelFactory.GenerateViewModel(
            ProgramPortfolio,
            "",
            Account.Benchmark,
            nameof(DataNamesViewModel)));
        Tabs.Add(viewModelFactory.GenerateViewModel(
            ProgramPortfolio,
            "",
            Account.Currency,
            nameof(DataNamesViewModel)));
        Tabs.Add(viewModelFactory.GenerateViewModel(
            ProgramPortfolio,
            "",
            Account.Asset,
            nameof(DataNamesViewModel)));

        foreach (object tab in Tabs)
        {
            if (tab is not DataDisplayViewModelBase vmb)
            {
                continue;
            }

            vmb.RequestAddTab += AddTab;
            vmb.RequestClose += RemoveTab;
            vmb.ModelUpdated += OnModelUpdated;
        }
    }

    /// <summary>
    /// Saves the user configuration to the local appData folder.
    /// </summary>
    public void SaveConfig() => UserConfiguration.SaveConfiguration();

    public void UpdateReport(ReportSeverity severity, ReportType type, string location, string message)
        => ReportsViewModel?.UpdateReport(severity, type, location, message);

    internal void OnModelUpdated(object sender, EventArgs eventArgs)
    {
        if (eventArgs is not PortfolioEventArgs e)
            return;

        if (e.ChangedAccount == Account.Unknown)
        {
            return;
        }

        lock (_updatingLock)
        {
            var tabs = TabsShallowCopy();
            List<object> tabsToRemove = new List<object>();
            foreach (object tab in tabs)
            {
                if (!UpdateTab(tab, ProgramPortfolio, e.ChangedAccount, e.UserInitiated))
                {
                    tabsToRemove.Add(tab);
                }
            }

            foreach (object tab in tabsToRemove)
            {
                Globals.CurrentDispatcher.BeginInvoke(() => RemoveTab(tab, EventArgs.Empty));
            }

            OptionsToolbarCommands.UpdateData(ProgramPortfolio, e.UserInitiated);

            if (e.ChangedPortfolio)
            {
                ReportsViewModel?.ClearReportsCommand.Execute(null);
            }
        }
    }

    private List<object> TabsShallowCopy()
    {
        lock (_tabsLock)
        {
            return Tabs.ToList();
        }
    }

    private void AddTab(object obj, EventArgs args)
    {
        if (obj == null)
            return;

        if (obj is IClosableViewModel vmb)
        {
            vmb.RequestClose += RemoveTab;
        }

        switch (obj)
        {
            case DataDisplayViewModelBase vmb5:
            {
                vmb5.ModelUpdated -= OnModelUpdated;
                break;
            }
            case StyledClosableViewModelBase<IPortfolio> viewModel1:
            {
                viewModel1.ModelUpdated += (x, y) => OnSubViewModelDataUpdated(x, y, Account.All);
                break;
            }
            case StyledClosableViewModelBase<ISecurity> viewModel2:
            {
                viewModel2.ModelUpdated += (x, y) => OnSubViewModelDataUpdated(x, y, Account.Security);
                break;
            }
            case StyledClosableViewModelBase<IAmortisableAsset> viewModel3:
            {
                viewModel3.ModelUpdated += (x, y) => OnSubViewModelDataUpdated(x, y, Account.Asset);
                break;
            }
            case StyledClosableViewModelBase<IValueList> viewModel4:
            {
                viewModel4.ModelUpdated += (x, y) => OnSubViewModelDataUpdated(x, y, Account.BankAccount);
                break;
            }
            default:
                break;
        }

        lock (_tabsLock)
        {
            Tabs.Add(obj);
        }
    }

    private void OnSubViewModelDataUpdated(object obj, EventArgs args, Account dataType)
        => OnModelUpdated(obj, new PortfolioEventArgs(dataType));

    private void RemoveTab(object obj, EventArgs args)
    {
        lock (_tabsLock)
        {
            Tabs.Remove(obj);
        }

        if (obj is IClosableViewModel vmb)
        {
            vmb.RequestClose += RemoveTab;
        }

        switch (obj)
        {
            case DataDisplayViewModelBase vmb5:
            {
                vmb5.ModelUpdated -= OnModelUpdated;
                break;
            }
            case StyledClosableViewModelBase<IPortfolio> viewModel1:
            {
                viewModel1.ModelUpdated -= (x, y) => OnSubViewModelDataUpdated(x, y, Account.All);
                break;
            }
            case StyledClosableViewModelBase<ISecurity> viewModel2:
            {
                viewModel2.ModelUpdated -= (x, y) => OnSubViewModelDataUpdated(x, y, Account.Security);
                break;
            }
            case StyledClosableViewModelBase<IAmortisableAsset> viewModel3:
            {
                viewModel3.ModelUpdated -= (x, y) => OnSubViewModelDataUpdated(x, y, Account.Asset);
                break;
            }
            case StyledClosableViewModelBase<IValueList> viewModel4:
            {
                viewModel4.ModelUpdated -= (x, y) => OnSubViewModelDataUpdated(x, y, Account.BankAccount);
                break;
            }
            default:
                break;
        }

        if (obj is IDisposable disposable)
            disposable.Dispose();
    }

    private bool UpdateTab(object item, IPortfolio modelData, Account changedAccount, bool force)
    {
        switch (item)
        {
            case DataDisplayViewModelBase vmb5:
            {
                if (!PortfolioEventArgs.ShouldUpdate(changedAccount, vmb5.DataType))
                {
                    return true;
                }

                vmb5.UpdateData(modelData, force);
                return true;
            }
            case StyledClosableViewModelBase<IPortfolio> viewModel1:
            {
                viewModel1.UpdateData(modelData, false);
                return true;
            }
            case StyledClosableViewModelBase<ISecurity> viewModel2:
            {
                if (!modelData.TryGetAccount(Account.Security, viewModel2.ModelData.Names, out ISecurity security))
                {
                    return false;
                }

                viewModel2.UpdateData(security, false);
                return true;

            }
            case StyledClosableViewModelBase<IAmortisableAsset> viewModel3:
            {
                if (!modelData.TryGetAccount(Account.Asset, viewModel3.ModelData.Names, out IAmortisableAsset asset))
                {
                    return false;
                }

                viewModel3.UpdateData(asset, false);
                return true;

            }
            case StyledClosableViewModelBase<IValueList> viewModel4:
            {
                if (!modelData.TryGetAccount(Account.BankAccount, viewModel4.ModelData.Names, out IValueList vl))
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
}