﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Input;
using FinancialStructures.DataStructures;
using FinancialStructures.FinanceInterfaces;
using FinancialStructures.StatisticStructures;
using FinancialStructures.StatsMakers;
using StructureCommon.Extensions;
using StructureCommon.Reporting;
using UICommon.Commands;
using UICommon.Interfaces;
using UICommon.Services;
using UICommon.ViewModelBases;

namespace FinanceWindowsViewModels
{
    internal class StatsOptionsViewModel : PropertyChangedBase
    {
        private readonly IPortfolio Portfolio;
        private bool fDisplayValueFunds = true;
        public bool displayValueFunds
        {
            get
            {
                return fDisplayValueFunds;
            }
            set
            {
                fDisplayValueFunds = value;
                OnPropertyChanged();
            }
        }
        private UserOptions fSelectOptions;

        public UserOptions SelectOptions
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

        private List<VisibleName> fDisplayConditions = new List<VisibleName>();

        public List<VisibleName> DisplayConditions
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

        private List<VisibleName> fSecurityColumnNames = new List<VisibleName>();

        public List<VisibleName> SecurityColumnNames
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

        private string fBankSortingField;
        public string BankSortingField
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

        private List<VisibleName> fBankColumnNames = new List<VisibleName>();

        public List<VisibleName> BankColumnNames
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

        private List<VisibleName> fSectorColumnNames = new List<VisibleName>();

        public List<VisibleName> SectorColumnNames
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

        public ICommand ExportCommand
        {
            get;
        }

        private void ExecuteExportCommand(ICloseable window)
        {
            FileInteractionResult result = fFileService.SaveFile(ExportType.Html.ToString(), Portfolio.DatabaseName + "-" + ExportType.Html, Portfolio.Directory, "Html Files|*.html|CSV Files|*.csv|All Files|*.*");
            string path = null;

            if (result.Success != null && (bool)result.Success)
            {
                path = result.FilePath;
                List<string> securitySelected = new List<string>();
                foreach (VisibleName column in SecurityColumnNames)
                {
                    if (column.Visible || column.Name == "Name" || column.Name == "Company")
                    {
                        securitySelected.Add(column.Name);
                    }
                }
                List<string> BankSelected = new List<string>();
                foreach (VisibleName column in BankColumnNames)
                {
                    if (column.Visible || column.Name == "Name" || column.Name == "Company")
                    {
                        BankSelected.Add(column.Name);
                    }
                }

                List<string> sectorSelected = new List<string>();
                foreach (VisibleName column in SectorColumnNames)
                {
                    if (column.Visible || column.Name == "Name" || column.Name == "Company")
                    {
                        sectorSelected.Add(column.Name);
                    }
                }

                UserOptions options = new UserOptions(securitySelected, BankSelected, sectorSelected, DisplayConditions, bankSortingField: BankSortingField);
                var stats = new PortfolioStatistics(Portfolio);
                string extension = Path.GetExtension(result.FilePath).Trim('.');
                ExportType type = extension.ToEnum<ExportType>();

                stats.ExportToFile(result.FilePath, type, options, ReportLogger);

                _ = ReportLogger.LogUsefulWithStrings("Report", "StatisticsPage", "Created statistics page");
            }
            else
            {
                _ = ReportLogger.LogWithStrings("Critical", "Error", "StatisticsPage", "Was not able to create page in place specified.");
            }

            CloseWindowAction(path);
            window.Close();
        }

        private readonly IFileInteractionService fFileService;
        private readonly IDialogCreationService fDialogCreationService;
        private readonly IReportLogger ReportLogger;
        private readonly Action<string> CloseWindowAction;

        public StatsOptionsViewModel(IPortfolio portfolio, IReportLogger reportLogger, Action<string> CloseWindow, IFileInteractionService fileService, IDialogCreationService dialogCreation)
        {
            Portfolio = portfolio;
            ReportLogger = reportLogger;
            fFileService = fileService;
            fDialogCreationService = dialogCreation;
            CloseWindowAction = CloseWindow;
            ExportCommand = new RelayCommand<ICloseable>(ExecuteExportCommand);

            SecurityStatistics totals = new SecurityStatistics();
            PropertyInfo[] properties = totals.GetType().GetProperties();
            foreach (PropertyInfo info in properties)
            {
                SecurityColumnNames.Add(new VisibleName(info.Name, true));
                SectorColumnNames.Add(new VisibleName(info.Name, true));
            }

            DayValue_Named BankNames = new DayValue_Named();
            PropertyInfo[] props = BankNames.GetType().GetProperties();
            foreach (PropertyInfo info in props)
            {
                if (info.Name == "Day")
                {
                    BankColumnNames.Add(new VisibleName(info.Name, false));
                }
                else
                {
                    BankColumnNames.Add(new VisibleName(info.Name, true));
                }
            }

            UserOptions fish = new UserOptions();
            PropertyInfo[] optionsInfo = fish.GetType().GetProperties();
            foreach (PropertyInfo info in optionsInfo)
            {
                if (info.PropertyType == typeof(bool))
                {
                    DisplayConditions.Add(new VisibleName(info.Name, true));
                }
            }
        }
    }
}
