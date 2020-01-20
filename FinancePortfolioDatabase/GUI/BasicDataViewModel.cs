using FinancialStructures.GUIFinanceStructures;
using FinancialStructures.Database;
using GlobalHeldData;
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

        private List<NameData> fAccountNames;
        public List<NameData> AccountNames
        {
            get { return fAccountNames; }
            set { fAccountNames = value; OnPropertyChanged(); }
        }

        private List<NameData> fSectorNames;
        public List<NameData> SectorNames
        {
            get { return fSectorNames; }
            set { fSectorNames = value; OnPropertyChanged(); }
        }

        private List<NameData> fCurrencyNames;
        public List<NameData> CurrencyNames
        {
            get { return fCurrencyNames; }
            set { fCurrencyNames = value; OnPropertyChanged(); }
        }

        public void DataUpdate()
        {
            FundNames = GlobalData.Finances.GetSecurityNamesAndCompanies();
            FundNames.Sort();
            AccountNames = GlobalData.Finances.GetBankAccountNamesAndCompanies();
            AccountNames.Sort();
            SectorNames = DatabaseAccessor.GetSectorNames();
            SectorNames.Sort();
            CurrencyNames = GlobalData.Finances.GetCurrencyNames();
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
