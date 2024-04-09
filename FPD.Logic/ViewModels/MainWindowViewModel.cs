using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Input;

using Effanville.Common.Structure.DataEdit;
using Effanville.Common.Structure.Reporting;
using Effanville.Common.UI;
using Effanville.Common.UI.Commands;
using Effanville.Common.UI.ViewModelBases;
using Effanville.FinancialStructures.Database;
using Effanville.FPD.Logic.Configuration;
using Effanville.FPD.Logic.TemplatesAndStyles;
using Effanville.FPD.Logic.ViewModels.Common;
using Effanville.FPD.Logic.ViewModels.Stats;

namespace Effanville.FPD.Logic.ViewModels
{
    /// <summary>
    /// View model for the entire display.
    /// </summary>
    public class MainWindowViewModel : PropertyChangedBase
    {
        private readonly object _tabsLock = new object();
        private readonly Timer _timer = new Timer(100);
        private bool _isUpdating;
        private PortfolioEventArgs _aggEventArgs = new PortfolioEventArgs(Account.Unknown);
        internal readonly IConfiguration UserConfiguration;

        public UiGlobals Globals { get; }

        private UiStyles _styles;

        /// <summary>
        /// The styles for the Ui.
        /// </summary>
        public UiStyles Styles
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
        public ObservableCollection<object> Tabs { get; } = new ();

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MainWindowViewModel(UiGlobals globals,
            UiStyles styles,
            IPortfolio portfolio,
            IUpdater<IPortfolio> updater,
            IViewModelFactory viewModelFactory,
            IConfiguration configuration)
        {
            ProgramPortfolio = portfolio;
            _styles = styles;
            Globals = globals;
            UserConfiguration = configuration;
            
            ReportsViewModel = new ReportingWindowViewModel(globals, Styles);

            SelectionChanged = new RelayCommand<SelectionChangedEventArgs>(ExecuteSelectionChanged);
            OptionsToolbarCommands = new OptionsToolbarViewModel(Globals, Styles, ProgramPortfolio);
            OptionsToolbarCommands.UpdateRequest += updater.PerformUpdate;
            OptionsToolbarCommands.IsLightTheme = styles.IsLightTheme;
            Tabs.Add(new BasicDataViewModel(Globals, Styles, ProgramPortfolio));
            Tabs.Add(new ValueListWindowViewModel(Globals, Styles, ProgramPortfolio, "Securities", Account.Security,
                updater, viewModelFactory));
            Tabs.Add(new ValueListWindowViewModel(Globals, Styles, ProgramPortfolio, "Bank Accounts",
                Account.BankAccount, updater, viewModelFactory));
            Tabs.Add(new ValueListWindowViewModel(Globals, Styles, ProgramPortfolio, "Pensions", Account.Pension,
                updater, viewModelFactory));
            Tabs.Add(new ValueListWindowViewModel(Globals, Styles, ProgramPortfolio, "Benchmarks", Account.Benchmark,
                updater, viewModelFactory));
            Tabs.Add(new ValueListWindowViewModel(Globals, Styles, ProgramPortfolio, "Currencies", Account.Currency,
                updater, viewModelFactory));
            Tabs.Add(new ValueListWindowViewModel(Globals, Styles, ProgramPortfolio, "Assets", Account.Asset,
                updater, viewModelFactory));
            Tabs.Add(new StatsViewModel(Globals, Styles,
                UserConfiguration.ChildConfigurations[Configuration.UserConfiguration.StatsDisplay], ProgramPortfolio));
            Tabs.Add(new StatisticsChartsViewModel(Globals, ProgramPortfolio, Styles));
            Tabs.Add(new StatsCreatorWindowViewModel(Globals, Styles,
                UserConfiguration.ChildConfigurations[Configuration.UserConfiguration.StatsCreator], ProgramPortfolio,
                AddTab));

            foreach (object tab in Tabs)
            {
                if (tab is not DataDisplayViewModelBase vmb)
                {
                    continue;
                }

                vmb.UpdateRequest += updater.PerformUpdate;
                vmb.RequestClose += RemoveTab;
            }

            ProgramPortfolio.PortfolioChanged += AllData_portfolioChanged;
            _timer.Elapsed += OnTimerElapsed;
            _timer.Start();
        }

        /// <summary>
        /// Saves the user configuration to the local appData folder.
        /// </summary>
        public void SaveConfig() => UserConfiguration.SaveConfiguration();

        public void UpdateReport(ReportSeverity severity, ReportType type, string location, string message)
            => ReportsViewModel?.UpdateReport(severity, type, location, message);
        
        private void AllData_portfolioChanged(object sender, PortfolioEventArgs e)
        {
            var changeType =
                _aggEventArgs.ChangedAccount == Account.All
                || (_aggEventArgs.ChangedAccount != Account.Unknown && _aggEventArgs.ChangedAccount != e.ChangedAccount)
                    ? Account.All
                    : e.ChangedAccount;
            _aggEventArgs = e.ChangedPortfolio
                ? new PortfolioEventArgs(Account.All, e.UserInitiated)
                : new PortfolioEventArgs(changeType, e.UserInitiated);
        }
        
        private void OnTimerElapsed(object sender, ElapsedEventArgs e) =>
            Task.Run(() => UpdateChildViewModels(_aggEventArgs));
        
        private void UpdateChildViewModels(PortfolioEventArgs e)
        {
            if (e.ChangedAccount == Account.Unknown)
            {
                return;
            }

            if (_isUpdating)
            {
                return;
            }

            _isUpdating = true;

            var tabs = TabsShallowCopy();
            List<object> tabsToRemove = new List<object>();
            foreach (object tab in tabs)
            {
                if (!UpdateTab(tab, ProgramPortfolio, e.ChangedAccount))
                {
                    tabsToRemove.Add(tab);
                }
            }
            
            foreach (object tab in tabsToRemove)
            {
                Globals.CurrentDispatcher.BeginInvoke(() => RemoveTab(tab, EventArgs.Empty));
            }
            
            OptionsToolbarCommands.UpdateData(ProgramPortfolio);

            if (e.ChangedPortfolio)
            {
                ReportsViewModel?.ClearReportsCommand.Execute(null);
            }

            _aggEventArgs = new PortfolioEventArgs(Account.Unknown);
            _isUpdating = false;
        }

        private List<object> TabsShallowCopy()
        {
            lock (_tabsLock)
            {
                return Tabs.ToList();
            }
        }
        
        private void AddTab(object obj)
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
        }

        public ICommand SelectionChanged { get; }

        private void ExecuteSelectionChanged(SelectionChangedEventArgs e)
        {
            var source = e.AddedItems;
            if (source is not object[] list || list.Length != 1)
            {
                return;
            }

            UpdateTab(list[0], ProgramPortfolio, Account.All);
        }

        private bool UpdateTab(object item, IPortfolio modelData, Account changedAccount)
        {
            if (item is not DataDisplayViewModelBase vmb)
            {
                return false;
            }

            if (!PortfolioEventArgs.ShouldUpdate(changedAccount, vmb.DataType))
            {
                return true;
            }

            vmb.UpdateData(modelData);
            return true;
        }
    }
}