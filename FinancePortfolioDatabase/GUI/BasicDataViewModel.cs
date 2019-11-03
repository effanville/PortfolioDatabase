using System.Collections.Generic;
using GUIAccessorFunctions;
using GUIFinanceStructures;
using GUISupport;
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

        private List<string> fSectorNames;
        public List<string> SectorNames
        {
            get { return fSectorNames; }
            set { fSectorNames = value; OnPropertyChanged(); }
        }

        public void DataUpdate()
        {
            FundNames = DatabaseAccessor.GetSecurityNamesAndCompanies();
            AccountNames = DatabaseAccessor.GetBankAccountNamesAndCompanies();
            SectorNames = DatabaseAccessor.GetSectorNames();
        }

        Action<bool> UpdateMainWindow;

        public BasicDataViewModel(Action<bool> updateWindow)
        {
            UpdateMainWindow = updateWindow;
            FundNames = DatabaseAccessor.GetSecurityNamesAndCompanies();

            AccountNames = DatabaseAccessor.GetBankAccountNamesAndCompanies();
        }
    }
}
