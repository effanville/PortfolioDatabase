using System;
using System.Collections.ObjectModel;
using System.IO.Abstractions;
using System.Reflection;
using Common.Structure.Reporting;
using Common.UI;
using Common.UI.ViewModelBases;
using FPD.Logic.ViewModels.Asset;
using FPD.Logic.Configuration;
using FPD.Logic.TemplatesAndStyles;
using FPD.Logic.ViewModels.Common;
using FPD.Logic.ViewModels.Security;
using FPD.Logic.ViewModels.Stats;
using FinancialStructures.Database;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace FPD.Logic.ViewModels
{
    /// <summary>
    /// View model for the entire display.
    /// </summary>
    public class MainWindowViewModel : PropertyChangedBase
    {
        /// <summary>
        /// The mechanism by which the data in <see cref="ProgramPortfolio"/> is updated. This includes a GUI update action.
        /// </summary>
        private Action<Action<IPortfolio>> UpdateDataCallback => action => fUpdater.PerformPortfolioAction(action, ProgramPortfolio);

        private Action<object> AddObjectAsMainTab => obj => AddTabAction(obj);

        private void AddTabAction(object obj)
        {
            lock (TabsLock)
            {
                Tabs.Add(obj);
            }
        }

        private readonly UiGlobals fUiGlobals;
        internal UserConfiguration fUserConfiguration;
        private string fConfigLocation;
        private readonly IPortfolioUpdater fUpdater;

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
        public MainWindowViewModel(UiGlobals globals, IPortfolioUpdater updater)
        {
            Styles = new UiStyles();
            ReportsViewModel = new ReportingWindowViewModel(globals, Styles);
            ReportLogger = new LogReporter(UpdateReport);
            fUiGlobals = globals;
            fUiGlobals.ReportLogger = ReportLogger;
            fUpdater = updater;

            LoadConfig();

            OptionsToolbarCommands = new OptionsToolbarViewModel(fUiGlobals, Styles, ProgramPortfolio, UpdateDataCallback);
            Tabs.Add(new BasicDataViewModel(fUiGlobals, Styles, ProgramPortfolio, UpdateDataCallback));
            Tabs.Add(new SecurityEditWindowViewModel(fUiGlobals, Styles, ProgramPortfolio, "Securities", Account.Security, UpdateDataCallback));
            Tabs.Add(new ValueListWindowViewModel(fUiGlobals, Styles, ProgramPortfolio, "Bank Accounts", Account.BankAccount, UpdateDataCallback));
            Tabs.Add(new SecurityEditWindowViewModel(fUiGlobals, Styles, ProgramPortfolio, "Pensions", Account.Pension, UpdateDataCallback));
            Tabs.Add(new ValueListWindowViewModel(fUiGlobals, Styles, ProgramPortfolio, "Benchmarks", Account.Benchmark, UpdateDataCallback));
            Tabs.Add(new ValueListWindowViewModel(fUiGlobals, Styles, ProgramPortfolio, "Currencies", Account.Currency, UpdateDataCallback));
            Tabs.Add(new AssetEditWindowViewModel(fUiGlobals, Styles, ProgramPortfolio, UpdateDataCallback));
            Tabs.Add(new StatsViewModel(fUiGlobals, Styles, fUserConfiguration.ChildConfigurations[UserConfiguration.StatsDisplay], ProgramPortfolio, Account.All));
            Tabs.Add(new StatisticsChartsViewModel(fUiGlobals, ProgramPortfolio, Styles));
            Tabs.Add(new StatsCreatorWindowViewModel(fUiGlobals, Styles, fUserConfiguration.ChildConfigurations[UserConfiguration.StatsCreator], ProgramPortfolio, AddObjectAsMainTab));
            ProgramPortfolio.PortfolioChanged += AllData_portfolioChanged;
        }

        private void LoadConfig()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            AssemblyName name = assembly.GetName();
            fConfigLocation = fUiGlobals.CurrentFileSystem.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), name.Name, "user.config");
            fUserConfiguration = UserConfiguration.LoadFromUserConfigFile(fConfigLocation, fUiGlobals.CurrentFileSystem, ReportLogger);
        }

        /// <summary>
        /// Saves the user configuration to the local appData folder.
        /// </summary>
        public void SaveConfig() => SaveConfig(fConfigLocation, fUiGlobals.CurrentFileSystem);

        internal void SaveConfig(string filePath, IFileSystem fileSystem) => fUserConfiguration.SaveConfiguration(filePath, fileSystem);

        private async void AllData_portfolioChanged(object sender, PortfolioEventArgs e)
        {
            var tabs = TabsShallowCopy();
            foreach (object tab in tabs)
            {
                if (tab is DataDisplayViewModelBase vm && e.ShouldUpdate(vm.DataType))
                {
                    await Task.Run(() => vm.UpdateData(ProgramPortfolio));
                }
            }

            OptionsToolbarCommands.UpdateData(ProgramPortfolio);

            if (e.ChangedPortfolio)
            {
                ReportsViewModel?.ClearReportsCommand.Execute(null);
            }
        }

        private void UpdateReport(ReportSeverity severity, ReportType type, string location, string message)
            => ReportsViewModel?.UpdateReport(severity, type, location, message);
    }
}
