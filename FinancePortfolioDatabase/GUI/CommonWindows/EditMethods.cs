using FinancialStructures.Database;
using FinancialStructures.FinanceStructures;
using FinancialStructures.GUIFinanceStructures;
using FinancialStructures.ReportingStructures;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceCommonViewModels
{

    public class EditMethods
    {
        public Func<Portfolio, List<Sector>, NameData, Action<ErrorReports>, ErrorReports, Task> DownloadMethod;
        public Func<Portfolio, List<Sector>, List<NameData>> UpdateNameMethod;

        public Func<Portfolio, List<Sector>, NameData, ErrorReports, bool> CreateMethod;

        public Func<Portfolio, List<Sector>, NameData, NameData, ErrorReports, bool> EditMethod;

        public Func<Portfolio, List<Sector>, NameData, ErrorReports, bool> DeleteMethod;

        public Func<Portfolio, List<Sector>, NameData, ErrorReports, List<AccountDayDataView>> SelectedDataMethod;

        public Func<Portfolio, List<Sector>, NameData, AccountDayDataView, ErrorReports, bool> AddDataMethod;

        public Func<Portfolio, List<Sector>, NameData, AccountDayDataView, AccountDayDataView, ErrorReports, bool> EditDataMethod;

        public Func<Portfolio, List<Sector>, NameData, AccountDayDataView, ErrorReports, bool> DeleteDataMethod;

        public EditMethods(
            Func<Portfolio, List<Sector>, NameData, Action<ErrorReports>, ErrorReports, Task>  downloadMethod,
            Func<Portfolio, List<Sector>, List<NameData>> updateNameMethod = null,
            Func<Portfolio, List<Sector>, NameData, ErrorReports, bool> createMethod = null,
            Func<Portfolio, List<Sector>, NameData, NameData, ErrorReports, bool> editMethod = null, 
            Func<Portfolio, List<Sector>, NameData, ErrorReports, bool> deleteMethod = null,
            Func<Portfolio, List<Sector>, NameData, ErrorReports, List<AccountDayDataView> >selectedDataMethod = null,
            Func<Portfolio, List<Sector>, NameData, AccountDayDataView, ErrorReports, bool> addDataMethod = null,
            Func<Portfolio, List<Sector>, NameData, AccountDayDataView, AccountDayDataView, ErrorReports, bool> editDataMethod = null,
            Func<Portfolio, List<Sector>, NameData, AccountDayDataView, ErrorReports, bool> deleteDataMethod = null)
        {
            DownloadMethod = downloadMethod;
            UpdateNameMethod = updateNameMethod;
            CreateMethod = createMethod;
            EditMethod = editMethod;
            DeleteMethod = deleteMethod;
            SelectedDataMethod = selectedDataMethod;
            AddDataMethod = addDataMethod;
            EditDataMethod = editDataMethod;
            DeleteDataMethod = deleteDataMethod;
        }

        public EditMethods()
        { }
    }
}
