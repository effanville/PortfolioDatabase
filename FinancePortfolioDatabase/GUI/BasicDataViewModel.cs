using FinancialStructures.GUIFinanceStructures;
using GUIAccessorFunctions;
using GUISupport;
using System;
using System.Collections.Generic;

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

        private List<NameComp> fCurrencyNames;
        public List<NameComp> CurrencyNames
        {
            get { return fCurrencyNames; }
            set { fCurrencyNames = value; OnPropertyChanged(); }
        }

        public void DataUpdate()
        {
            FundNames = DatabaseAccessor.GetSecurityNamesAndCompanies();
            FundNames.Sort();
            AccountNames = DatabaseAccessor.GetBankAccountNamesAndCompanies();
            AccountNames.Sort();
            SectorNames = DatabaseAccessor.GetSectorNames();
            SectorNames.Sort();
            CurrencyNames = DatabaseAccessor.GetCurrencyNames();
            CurrencyNames.Sort();
        }

        Action<bool> UpdateMainWindow;

        public BasicDataViewModel(Action<bool> updateWindow)
        {
            UpdateMainWindow = updateWindow;
            DataUpdate();
        }
    }
}
