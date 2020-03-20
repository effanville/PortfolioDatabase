﻿using FinanceCommonViewModels;
using FinanceWindows;
using FinancialStructures.Database;
using FinancialStructures.PortfolioAPI;
using FinancialStructures.ReportLogging;
using GUISupport;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Input;

namespace FinanceWindowsViewModels
{
    internal class OptionsToolbarViewModel : ViewModelBase
    {
        private string fFileName;
        private string fDirectory;
        private readonly Action<Action<Portfolio>> DataUpdateCallback;
        private readonly LogReporter ReportLogger;
        private string fBaseCurrency;

        public string BaseCurrency
        {
            get { return fBaseCurrency; }
            set { fBaseCurrency = value; OnPropertyChanged(); }
        }

        private List<string> fCurrencies;

        public List<string> Currencies;

        public OptionsToolbarViewModel(Portfolio portfolio, Action<Action<Portfolio>> updateData, LogReporter reportLogger)
            : base("Options")
        {
            ReportLogger = reportLogger;
            DataUpdateCallback = updateData;
            UpdateData(portfolio);

            OpenHelpCommand = new BasicCommand(OpenHelpDocsCommand);
            NewDatabaseCommand = new BasicCommand(ExecuteNewDatabase);
            SaveDatabaseCommand = new BasicCommand(ExecuteSaveDatabase);
            LoadDatabaseCommand = new BasicCommand(ExecuteLoadDatabase);
            UpdateDataCommand = new BasicCommand(ExecuteUpdateData);
            RefreshCommand = new BasicCommand(ExecuteRefresh);
            SetCurrencyCommand = new BasicCommand(ExecuteSetCurrency);
        }

        private void ExecuteSetCurrency(object obj)
        {
            DataUpdateCallback(portfolio => portfolio.BaseCurrency = BaseCurrency);
        }

        public override void UpdateData(Portfolio portfolio)
        {
            fFileName = portfolio.DatabaseName + portfolio.Extension;
            fDirectory = portfolio.Directory;
            BaseCurrency = portfolio.BaseCurrency;
            Currencies = portfolio.Names(AccountType.Currency);
        }

        public ICommand OpenHelpCommand { get; }
        private void OpenHelpDocsCommand(Object obj)
        {
            var helpwindow = new HelpWindow(ReportLogger);
            helpwindow.Show();
        }

        public ICommand NewDatabaseCommand { get; }
        private void ExecuteNewDatabase(Object obj)
        {
            DialogResult result = MessageBox.Show("Do you want to load a new database?", "New Database?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                DataUpdateCallback(programPortfolio => programPortfolio.SetFilePath(""));
                DataUpdateCallback(programPortfolio => programPortfolio.LoadPortfolio("", ReportLogger));
            }
        }

        public ICommand SaveDatabaseCommand { get; }
        private void ExecuteSaveDatabase(Object obj)
        {
            SaveFileDialog saving = new SaveFileDialog() { DefaultExt = "xml", FileName = fFileName, InitialDirectory = fDirectory };
            saving.Filter = "XML Files|*.xml|All Files|*.*";
            if (saving.ShowDialog() == DialogResult.OK)
            {
                DataUpdateCallback(programPortfolio => programPortfolio.SetFilePath(saving.FileName));
                DataUpdateCallback(programPortfolio => programPortfolio.SavePortfolio(saving.FileName, ReportLogger));
            }

            saving.Dispose();
        }

        public ICommand LoadDatabaseCommand { get; }
        private void ExecuteLoadDatabase(Object obj)
        {
            OpenFileDialog openFile = new OpenFileDialog() { DefaultExt = "xml" };
            openFile.Filter = "XML Files|*.xml|All Files|*.*";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                DataUpdateCallback(programPortfolio => programPortfolio.SetFilePath(openFile.FileName));
                DataUpdateCallback(programPortfolio => programPortfolio.LoadPortfolio(openFile.FileName, ReportLogger));

            }
            openFile.Dispose();
        }

        public ICommand UpdateDataCommand { get; }
        private void ExecuteUpdateData(Object obj)
        {
            DataUpdateCallback(async programPortfolio => await PortfolioDataUpdater.Downloader(programPortfolio, ReportLogger).ConfigureAwait(false));
        }

        public ICommand RefreshCommand { get; }
        public BasicCommand SetCurrencyCommand { get; }

        private void ExecuteRefresh(Object obj)
        {
            DataUpdateCallback(programPortfolio => programPortfolio.SetFilePath(fFileName));
        }
    }
}
