using FinancialStructures.Database;
using FinancialStructures.FinanceStructures;
using FinancialStructures.GUIFinanceStructures;
using FinancialStructures.ReportingStructures;
using GUISupport;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FinanceWindowsViewModels
{
    public class SecurityEditWindowViewModel : PropertyChangedBase
    {
        private Portfolio Portfolio;
        private List<Sector> Sectors;

        public ObservableCollection<object> Tabs { get; set; } = new ObservableCollection<object>();

        public void UpdateFundListBox(Portfolio portfolio, List<Sector> sectors)
        {
            Portfolio = portfolio;
            Sectors = sectors;
            if (Tabs != null)
            {
                foreach (var item in Tabs)
                {
                    if (item is SecurityNamesViewModel viewModel)
                    {
                        viewModel.UpdateFundListBox(portfolio);
                    }
                    if (item is SelectedSecurityViewModel selectedVM)
                    {
                        selectedVM.UpdateData(portfolio);
                    }
                }
            }
        }

        Action<bool> UpdateMainWindow;
        Action<ErrorReports> UpdateReports;

        Action<NameData> loadTab => (name) => LoadTabFunc(name);

        private void LoadTabFunc(NameData name)
        {
            Tabs.Add(new SelectedSecurityViewModel(Portfolio, UpdateMainWindow, UpdateReports, name));
        }

        public SecurityEditWindowViewModel(Portfolio portfolio, List<Sector> sectors, Action<bool> updateWindow, Action<ErrorReports> updateReports)
        {
            Portfolio = portfolio;
            Sectors = sectors;
            UpdateMainWindow = updateWindow;
            UpdateReports = updateReports;
            Tabs.Add(new SecurityNamesViewModel(Portfolio, updateWindow, updateReports, loadTab));
        }
    }
}
