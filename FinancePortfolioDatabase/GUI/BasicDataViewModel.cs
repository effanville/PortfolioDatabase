using System.Collections.Generic;
using GUIAccessorFunctions;
using GUIFinanceStructures;
using GuiSupport;
using System;

namespace FinanceWindowsViewModels
{
    public class BasicDataViewModel : PropertyChangedBase
    {
        private List<NameComp> fFundNames;
        public List<NameComp> FundNames
        {
            get { return fFundNames; }
            set { fFundNames = value; OnPropertyChanged(); }
        }

        private List<NameComp> fAccountNames;
        public List<NameComp> AccountNames
        {
            get { return fAccountNames; }
            set { fAccountNames = value; OnPropertyChanged(); }
        }

        public void DataUpdate()
        {
            FundNames = DatabaseAccessorHelper.GetSecurityNamesAndCompanies();
            AccountNames = DatabaseAccessorHelper.GetBankAccountNamesAndCompanies();
        }

        Action<bool> UpdateMainWindow;

        public BasicDataViewModel(Action<bool> updateWindow)
        {
            UpdateMainWindow = updateWindow;
            FundNames = DatabaseAccessorHelper.GetSecurityNamesAndCompanies();

            AccountNames = DatabaseAccessorHelper.GetBankAccountNamesAndCompanies();
        }
    }
}
