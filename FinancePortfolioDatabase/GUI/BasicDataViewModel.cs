using FinancialStructures.Database;
using FinancialStructures.FinanceStructures;
using FinancialStructures.GUIFinanceStructures;
using GUIAccessorFunctions;
using GUISupport;
using System.Collections.Generic;

namespace FinanceWindowsViewModels
{
    public class BasicDataViewModel : PropertyChangedBase
    {
        private Portfolio Portfolio;
        private List<Sector> Sectors;

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
            FundNames = Portfolio.SecurityNamesAndCompanies();
            FundNames.Sort();
            AccountNames = Portfolio.GetBankAccountNamesAndCompanies();
            AccountNames.Sort();
            SectorNames = DatabaseAccessor.GetSectorNames();
            SectorNames.Sort();
            CurrencyNames = Portfolio.GetCurrencyNames();
            CurrencyNames.Sort();
        }

        public BasicDataViewModel(Portfolio portfolio, List<Sector> sectors)
        {
            Portfolio = portfolio;
            Sectors = sectors;
            DataUpdate();
        }
    }
}
