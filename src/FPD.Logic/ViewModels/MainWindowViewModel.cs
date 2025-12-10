using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Effanville.Common.Structure.Reporting;
using Effanville.Common.UI;
using Effanville.Common.UI.ViewModelBases;
using Effanville.FinancialStructures.Database;
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
            "Securities",
            Account.Security,
            nameof(ValueListWindowViewModel)));
        Tabs.Add(viewModelFactory.GenerateViewModel(
            ProgramPortfolio,
            "Bank Accounts",
            Account.BankAccount,
            nameof(ValueListWindowViewModel)));
        Tabs.Add(viewModelFactory.GenerateViewModel(
            ProgramPortfolio,
            "Pensions",
            Account.Pension,
            nameof(ValueListWindowViewModel)));
        Tabs.Add(viewModelFactory.GenerateViewModel(
            ProgramPortfolio,
            "Benchmarks",
            Account.Benchmark,
            nameof(ValueListWindowViewModel)));
        Tabs.Add(viewModelFactory.GenerateViewModel(
            ProgramPortfolio,
            "Currencies",
            Account.Currency,
            nameof(ValueListWindowViewModel)));
        Tabs.Add(viewModelFactory.GenerateViewModel(
            ProgramPortfolio,
            "Assets",
            Account.Asset,
            nameof(ValueListWindowViewModel)));
        Tabs.Add(viewModelFactory.GenerateViewModel(
            ProgramPortfolio,
            "",
            Account.All,
            nameof(StatsViewModel)));
        if (statisticsChartsViewModel != null)
        {
            Tabs.Add(statisticsChartsViewModel);
        }

        DataDisplayViewModelBase statsCreatorWindow = viewModelFactory.GenerateViewModel(
            ProgramPortfolio,
            "",
            Account.All,
            nameof(StatsCreatorWindowViewModel));
        ((StatsCreatorWindowViewModel)statsCreatorWindow).RequestAddTab += AddTab;
        Tabs.Add(statsCreatorWindow);


        foreach (object tab in Tabs)
        {
            if (tab is not DataDisplayViewModelBase vmb)
            {
                continue;
            }

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

    private void OnModelUpdated(object sender, PortfolioEventArgs e)
    {
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
        lock (_tabsLock)
        {
            if (obj is DataDisplayViewModelBase vmb)
            {
                vmb.RequestClose += RemoveTab;
            }

            Tabs.Add(obj);
        }
    }

    private void RemoveTab(object obj, EventArgs args)
    {
        lock (_tabsLock)
        {
            Tabs.Remove(obj);
        }

        if (obj is not DataDisplayViewModelBase vmb)
        {
            return;
        }

        vmb.RequestClose -= RemoveTab;
        vmb.ModelUpdated -= OnModelUpdated;
    }

    private bool UpdateTab(object item, IPortfolio modelData, Account changedAccount, bool force)
    {
        if (item is not DataDisplayViewModelBase vmb)
        {
            return false;
        }

        if (!PortfolioEventArgs.ShouldUpdate(changedAccount, vmb.DataType))
        {
            return true;
        }

        vmb.UpdateData(modelData, force);
        return true;
    }
}