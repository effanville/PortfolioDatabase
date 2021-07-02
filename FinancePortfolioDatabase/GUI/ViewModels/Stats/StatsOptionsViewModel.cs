﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using FinancialStructures.Database;
using FinancialStructures.DataExporters;
using FinancialStructures.DataExporters.ExportOptions;
using FinancialStructures.Statistics;
using Common.Structure.DisplayClasses;
using Common.Structure.Extensions;
using Common.Structure.FileAccess;
using Common.Structure.Reporting;
using Common.UI.Commands;
using Common.UI.Interfaces;
using Common.UI.Services;
using Common.UI.ViewModelBases;

namespace FinancePortfolioDatabase.GUI.ViewModels.Stats
{
    internal class StatsOptionsViewModel : PropertyChangedBase
    {
        private readonly IPortfolio Portfolio;

        private UserDisplayOptions fSelectOptions;

        public UserDisplayOptions SelectOptions
        {
            get
            {
                return fSelectOptions;
            }
            set
            {
                fSelectOptions = value;
                OnPropertyChanged();
            }
        }

        private List<Selectable<string>> fDisplayConditions = new List<Selectable<string>>();

        public List<Selectable<string>> DisplayConditions
        {
            get
            {
                return fDisplayConditions;
            }
            set
            {
                fDisplayConditions = value;
                OnPropertyChanged();
            }
        }

        private Statistic fSecuritySortingField;
        public Statistic SecuritySortingField
        {
            get
            {
                return fSecuritySortingField;
            }
            set
            {
                fSecuritySortingField = value;
                OnPropertyChanged(nameof(SecuritySortingField));
            }
        }

        private SortDirection fSecurityDirection;
        public SortDirection SecurityDirection
        {
            get
            {
                return fSecurityDirection;
            }
            set
            {
                fSecurityDirection = value;
                OnPropertyChanged(nameof(SecurityDirection));
            }
        }

        private List<Selectable<Statistic>> fSecurityColumnNames = new List<Selectable<Statistic>>();

        public List<Selectable<Statistic>> SecurityColumnNames
        {
            get
            {
                return fSecurityColumnNames;
            }
            set
            {
                fSecurityColumnNames = value;
                OnPropertyChanged();
            }
        }

        private Statistic fBankSortingField;
        public Statistic BankSortingField
        {
            get
            {
                return fBankSortingField;
            }
            set
            {
                fBankSortingField = value;
                OnPropertyChanged(nameof(BankSortingField));
            }
        }

        private SortDirection fBankDirection;
        public SortDirection BankDirection
        {
            get
            {
                return fBankDirection;
            }
            set
            {
                fBankDirection = value;
                OnPropertyChanged(nameof(BankDirection));
            }
        }

        private List<Selectable<Statistic>> fBankColumnNames = new List<Selectable<Statistic>>();

        public List<Selectable<Statistic>> BankColumnNames
        {
            get
            {
                return fBankColumnNames;
            }
            set
            {
                fBankColumnNames = value;
                OnPropertyChanged();
            }
        }

        private Statistic fSectorSortingField;
        public Statistic SectorSortingField
        {
            get
            {
                return fSectorSortingField;
            }
            set
            {
                fSectorSortingField = value;
                OnPropertyChanged(nameof(SectorSortingField));
            }
        }


        private SortDirection fSectorDirection;
        public SortDirection SectorDirection
        {
            get
            {
                return fSectorDirection;
            }
            set
            {
                fSectorDirection = value;
                OnPropertyChanged(nameof(SectorDirection));
            }
        }

        private List<Selectable<Statistic>> fSectorColumnNames = new List<Selectable<Statistic>>();

        public List<Selectable<Statistic>> SectorColumnNames
        {
            get
            {
                return fSectorColumnNames;
            }
            set
            {
                fSectorColumnNames = value;
                OnPropertyChanged();
            }
        }

        public static List<Statistic> SecurityFieldNames
        {
            get
            {
                return AccountStatisticsHelpers.AllStatistics().ToList();
            }
        }

        public static List<Statistic> BankFieldNames
        {
            get
            {
                return AccountStatisticsHelpers.DefaultBankAccountStats().ToList();
            }
        }

        public static List<Statistic> SectorFieldNames
        {
            get
            {
                return AccountStatisticsHelpers.DefaultSectorStats().ToList();
            }
        }

        public static List<SortDirection> SortDirections
        {
            get
            {
                return Enum.GetValues(typeof(SortDirection)).Cast<SortDirection>().ToList();
            }
        }

        public ICommand ExportCommand
        {
            get;
        }

        private void ExecuteExportCommand(ICloseable window)
        {
            FileInteractionResult result = fUiGlobals.FileInteractionService.SaveFile(ExportType.Html.ToString(), Portfolio.DatabaseName(fUiGlobals.CurrentFileSystem), Portfolio.Directory(fUiGlobals.CurrentFileSystem), "Html Files|*.html|CSV Files|*.csv|All Files|*.*");
            string path = null;

            if (result.Success != null && (bool)result.Success)
            {
                path = result.FilePath;

                List<Statistic> securitySelected = new List<Statistic>();
                foreach (Selectable<Statistic> column in SecurityColumnNames)
                {
                    if (column.Selected || column.Instance == Statistic.Company || column.Instance == Statistic.Name)
                    {
                        securitySelected.Add(column.Instance);
                    }
                }

                List<Statistic> BankSelected = new List<Statistic>();
                foreach (Selectable<Statistic> column in BankColumnNames)
                {
                    if (column.Selected || column.Instance == Statistic.Company || column.Instance == Statistic.Name)
                    {
                        BankSelected.Add(column.Instance);
                    }
                }

                List<Statistic> sectorSelected = new List<Statistic>();
                foreach (Selectable<Statistic> column in SectorColumnNames)
                {
                    if (column.Selected || column.Instance == Statistic.Company || column.Instance == Statistic.Name)
                    {
                        sectorSelected.Add(column.Instance);
                    }
                }

                UserDisplayOptions options = new UserDisplayOptions(securitySelected, BankSelected, sectorSelected, DisplayConditions, SecuritySortingField, BankSortingField, SectorSortingField, SecurityDirection, BankDirection, SectorDirection);

                PortfolioStatistics stats = new PortfolioStatistics(Portfolio, options, fUiGlobals.CurrentFileSystem);
                string extension = fUiGlobals.CurrentFileSystem.Path.GetExtension(result.FilePath).Trim('.');
                ExportType type = extension.ToEnum<ExportType>();

                stats.ExportToFile(fUiGlobals.CurrentFileSystem, result.FilePath, type, options, ReportLogger);

                _ = ReportLogger.LogUseful(ReportType.Information, ReportLocation.StatisticsPage, "Created statistics page");
            }
            else
            {
                _ = ReportLogger.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.StatisticsPage, "Was not able to create page in place specified.");
            }

            CloseWindowAction(path);
            window.Close();
        }

        private readonly IReportLogger ReportLogger;
        private readonly Action<string> CloseWindowAction;
        private readonly UiGlobals fUiGlobals;

        public StatsOptionsViewModel(IPortfolio portfolio, IReportLogger reportLogger, Action<string> CloseWindow, UiGlobals uiGlobals)
        {
            fUiGlobals = uiGlobals;
            Portfolio = portfolio;
            ReportLogger = reportLogger;
            CloseWindowAction = CloseWindow;
            ExportCommand = new RelayCommand<ICloseable>(ExecuteExportCommand);

            foreach (Statistic stat in AccountStatisticsHelpers.AllStatistics())
            {
                SecurityColumnNames.Add(new Selectable<Statistic>(stat, true));
            }

            SecuritySortingField = Statistic.Company;

            foreach (Statistic stat in AccountStatisticsHelpers.DefaultSectorStats())
            {
                SectorColumnNames.Add(new Selectable<Statistic>(stat, true));
            }

            SectorSortingField = Statistic.Name;

            foreach (Statistic stat in AccountStatisticsHelpers.DefaultBankAccountStats())
            {
                BankColumnNames.Add(new Selectable<Statistic>(stat, true));
            }

            BankSortingField = Statistic.Company;

            DisplayConditions.Add(new Selectable<string>("DisplayValueFunds", true));
            DisplayConditions.Add(new Selectable<string>("Spacing", true));
            DisplayConditions.Add(new Selectable<string>("Colours", true));
            DisplayConditions.Add(new Selectable<string>(UserDisplayOptions.ShowSecurities, true));
            DisplayConditions.Add(new Selectable<string>(UserDisplayOptions.ShowBankAccounts, true));
            DisplayConditions.Add(new Selectable<string>(UserDisplayOptions.ShowSectors, true));
            DisplayConditions.Add(new Selectable<string>(UserDisplayOptions.ShowBenchmarks, false));
        }
    }
}
