using System.Collections.Generic;
using GUIAccessorFunctions;
using FinancialStructures.GUIFinanceStructures;
using GUISupport;
using System;

namespace FinanceWindowsViewModels
{
    public class BasicDataViewModel : PropertyChangedBase
    {
        private List<NameCompDate> fFundNames;
        public List<NameCompDate> FundNames
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

        private List<NameComp> fSectorNames;
        public List<NameComp> SectorNames
        {
            get { return fSectorNames; }
            set { fSectorNames = value; OnPropertyChanged(); }
        }

        public void DataUpdate()
        {
            FundNames = DatabaseAccessor.GetSecurityNamesAndCompanies();
            FundNames.Sort();
            AccountNames = DatabaseAccessor.GetBankAccountNamesAndCompanies();
            AccountNames.Sort();
            SectorNames = DatabaseAccessor.GetSectorNames();
            SectorNames.Sort();
        }

        Action<bool> UpdateMainWindow;

        public BasicDataViewModel(Action<bool> updateWindow)
        {
            UpdateMainWindow = updateWindow;
            FundNames = DatabaseAccessor.GetSecurityNamesAndCompanies();
            FundNames.Sort();
            AccountNames = DatabaseAccessor.GetBankAccountNamesAndCompanies();
            AccountNames.Sort();
            SectorNames = DatabaseAccessor.GetSectorNames();
            SectorNames.Sort();
        }
    }
}
