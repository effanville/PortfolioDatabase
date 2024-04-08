using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
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

using Microsoft.Win32;

namespace Effanville.FPD.Logic.ViewModels
{
    /// <summary>
    /// View model for the entire display.
    /// </summary>
    public class MainWindowViewModel : PropertyChangedBase
    {
        private readonly Timer _timer = new Timer(100);

        private PortfolioEventArgs AggEventArgs = new PortfolioEventArgs(Account.Unknown);

        private Action<object> AddObjectAsMainTab => obj => AddTabAction(obj);

        private void AddTabAction(object obj)
        {
            lock (TabsLock)
            {
                if (obj is DataDisplayViewModelBase vmb)
                {
                    vmb.RequestClose += RemoveTab;
                }

                Tabs.Add(obj);
            }
        }

        public UiGlobals Globals { get; }
        internal UserConfiguration _userConfiguration;
        private string _configLocation;
        private readonly IUpdater<IPortfolio> _updater;

        /// <summary>
        /// The logging mechanism for the program.
        /// </summary>
        public IReportLogger ReportLogger
        {
            get;
        }

        private UiStyles fStyles;

        /// <summary>
        /// The styles for the Ui.
        /// </summary>
        public UiStyles Styles
        {
            get => fStyles;
            set => SetAndNotify(ref fStyles, value, nameof(Styles));
        }

        /// <summary>
        /// The portfolio for the view model instance.
        /// </summary>
        public IPortfolio ProgramPortfolio
        {
            get;
            set;
        } = PortfolioFactory.GenerateEmpty();

        private OptionsToolbarViewModel fOptionsToolbarCommands;

        /// <summary>
        /// view model for the top toolbar.
        /// </summary>
        public OptionsToolbarViewModel OptionsToolbarCommands
        {
            get => fOptionsToolbarCommands;
            set => SetAndNotify(ref fOptionsToolbarCommands, value, nameof(OptionsToolbarCommands));
        }

        private ReportingWindowViewModel fReports;

        /// <summary>
        /// View model for the reports view.
        /// </summary>
        public ReportingWindowViewModel ReportsViewModel
        {
            get => fReports;
            set => SetAndNotify(ref fReports, value, nameof(ReportsViewModel));
        }

        /// <summary>
        /// The collection of tabs to hold the data and interactions for the various subwindows.
        /// </summary>
        public ObservableCollection<object> Tabs
        {
            get;
            set;
        } = new ObservableCollection<object>();

        private readonly object TabsLock = new object();

        private List<object> TabsShallowCopy()
        {
            lock (TabsLock)
            {
                return Tabs.ToList();
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MainWindowViewModel(UiGlobals globals, IUpdater<IPortfolio> updater)
        {
            bool isLightTheme = IsLightTheme();
            Styles = new UiStyles(isLightTheme);
            ReportsViewModel = new ReportingWindowViewModel(globals, Styles);
            ReportLogger = new LogReporter(UpdateReport);
            Globals = globals;
            Globals.ReportLogger = ReportLogger;
            _updater = updater;
            _updater.Database = ProgramPortfolio;

            SelectionChanged = new RelayCommand<SelectionChangedEventArgs>(ExecuteSelectionChanged);
            LoadConfig();
            var viewModelFactory = new ViewModelFactory(Styles, Globals, _updater);
            OptionsToolbarCommands = new OptionsToolbarViewModel(Globals, Styles, ProgramPortfolio);
            OptionsToolbarCommands.UpdateRequest += _updater.PerformUpdate;
            OptionsToolbarCommands.IsLightTheme = isLightTheme;
            Tabs.Add(new BasicDataViewModel(Globals, Styles, ProgramPortfolio));
            Tabs.Add(new ValueListWindowViewModel(Globals, Styles, ProgramPortfolio, "Securities", Account.Security,
                _updater, viewModelFactory));
            Tabs.Add(new ValueListWindowViewModel(Globals, Styles, ProgramPortfolio, "Bank Accounts",
                Account.BankAccount, _updater, viewModelFactory));
            Tabs.Add(new ValueListWindowViewModel(Globals, Styles, ProgramPortfolio, "Pensions", Account.Pension,
                _updater, viewModelFactory));
            Tabs.Add(new ValueListWindowViewModel(Globals, Styles, ProgramPortfolio, "Benchmarks", Account.Benchmark,
                _updater, viewModelFactory));
            Tabs.Add(new ValueListWindowViewModel(Globals, Styles, ProgramPortfolio, "Currencies", Account.Currency,
                _updater, viewModelFactory));
            Tabs.Add(new ValueListWindowViewModel(Globals, Styles, ProgramPortfolio, "Assets", Account.Asset,
                _updater, viewModelFactory));
            Tabs.Add(new StatsViewModel(Globals, Styles,
                _userConfiguration.ChildConfigurations[UserConfiguration.StatsDisplay], ProgramPortfolio, Account.All));
            Tabs.Add(new StatisticsChartsViewModel(Globals, ProgramPortfolio, Styles));
            Tabs.Add(new StatsCreatorWindowViewModel(Globals, Styles,
                _userConfiguration.ChildConfigurations[UserConfiguration.StatsCreator], ProgramPortfolio,
                AddObjectAsMainTab));

            foreach (object tab in Tabs)
            {
                if (tab is DataDisplayViewModelBase vmb)
                {
                    vmb.UpdateRequest += _updater.PerformUpdate;
                    vmb.RequestClose += RemoveTab;
                }
            }

            ProgramPortfolio.PortfolioChanged += AllData_portfolioChanged;
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
        }

        private static bool IsLightTheme()
        {
            using var key =
                Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            object value = key?.GetValue("AppsUseLightTheme");
            return value is int i && i > 0;
        }
        
        private void LoadConfig()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            AssemblyName name = assembly.GetName();
            _configLocation = Globals.CurrentFileSystem.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), name.Name, "user.config");
            _userConfiguration =
                UserConfiguration.LoadFromUserConfigFile(_configLocation, Globals.CurrentFileSystem, ReportLogger);
        }

        /// <summary>
        /// Saves the user configuration to the local appData folder.
        /// </summary>
        public void SaveConfig() => SaveConfig(_configLocation, Globals.CurrentFileSystem);

        internal void SaveConfig(string filePath, IFileSystem fileSystem) =>
            _userConfiguration.SaveConfiguration(filePath, fileSystem);

        private void AllData_portfolioChanged(object sender, PortfolioEventArgs e)
        {
            var changeType =
                AggEventArgs.ChangedAccount == Account.All
                || (AggEventArgs.ChangedAccount != Account.Unknown && AggEventArgs.ChangedAccount != e.ChangedAccount)
                    ? Account.All
                    : e.ChangedAccount;
            AggEventArgs = e.ChangedPortfolio
                ? new PortfolioEventArgs(Account.All, e.UserInitiated)
                : new PortfolioEventArgs(changeType, e.UserInitiated);
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e) =>
            Task.Run(() => UpdateChildViewModels(AggEventArgs));

        private bool isUpdating = false;

        private async void UpdateChildViewModels(PortfolioEventArgs e)
        {
            if (e.ChangedAccount == Account.Unknown)
            {
                return;
            }

            if (isUpdating)
            {
                return;
            }

            isUpdating = true;

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

            AggEventArgs = new PortfolioEventArgs(Account.Unknown);
            isUpdating = false;
        }

        private void UpdateReport(ReportSeverity severity, ReportType type, string location, string message)
            => ReportsViewModel?.UpdateReport(severity, type, location, message);

        private void RemoveTab(object obj, EventArgs args)
        {
            lock (TabsLock)
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