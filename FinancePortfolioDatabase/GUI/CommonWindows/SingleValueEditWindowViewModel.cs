﻿using FinancialStructures.Database;
using FinancialStructures.FinanceStructures;
using FinancialStructures.GUIFinanceStructures;
using FinancialStructures.ReportingStructures;
using GUISupport;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FinanceCommonViewModels
{
    public class SingleValueEditWindowViewModel : PropertyChangedBase
    {
        private Portfolio Portfolio;
        private List<Sector> Sectors;
        public ObservableCollection<object> Tabs { get; set; } = new ObservableCollection<object>();

        public void UpdateListBoxes(Portfolio portfolio, List<Sector> sectors)
        {
            Portfolio = portfolio;
            Sectors = sectors;
            if (Tabs != null)
            {
                foreach (var item in Tabs)
                {
                    if (item is ViewModelBase viewModel)
                    {
                        viewModel.UpdateData(portfolio, sectors);
                    }
                }
            }
        }

        Action<NameData> loadTab => (name) => LoadTabFunc(name);

        private void LoadTabFunc(NameData name)
        {
            Tabs.Add(new SelectedSingleDataViewModel(Portfolio, Sectors, UpdateMainWindow, UpdateReports, EditMethods, name));
        }


        Action UpdateMainWindow;
        Action<ErrorReports> UpdateReports;
        EditMethods EditMethods;

        public SingleValueEditWindowViewModel(Portfolio portfolio, List<Sector> sectors, Action updateWindow, Action<ErrorReports> updateReports, EditMethods editMethods)
        {
            UpdateMainWindow = updateWindow;
            UpdateReports = updateReports;
            EditMethods = editMethods;
            UpdateListBoxes(portfolio, sectors);

            Tabs.Add(new DataNamesViewModel(Portfolio, sectors, updateWindow, updateReports, loadTab, editMethods));
        }
    }
}
