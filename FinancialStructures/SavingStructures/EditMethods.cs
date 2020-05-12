﻿using FinancialStructures.NamingStructures;
using FinancialStructures.PortfolioAPI;
using StructureCommon.DataStructures;
using StructureCommon.Reporting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinancialStructures.FinanceInterfaces
{
    public enum FunctionType
    {
        Download,
        NameUpdate,
        Create,
        Edit,
        Delete,
        SelectData,
        AddData,
        EditData,
        DeleteData
    }

    public class EditMethods
    {
        public async Task<object> ExecuteFunction(FunctionType functionType, params object[] functionInputs)
        {
            if (functionInputs.Length < 1)
            {
                return null;
            }
            object output = null;
            var portfolio = (IPortfolio)functionInputs[0];
            NameData name = functionInputs.Length > 1 ? (NameData)functionInputs[1] : null;

            switch (functionType)
            {
                case (FunctionType.Download):
                {
                    if (functionInputs.Length != 3)
                    {
                        break;
                    }

                    await DownloadMethod(portfolio, (NameData)functionInputs[1], (IReportLogger)functionInputs[2]).ConfigureAwait(false);
                    break;
                }
                case (FunctionType.NameUpdate):
                {
                    if (functionInputs.Length != 1)
                    {
                        break;
                    }

                    output = UpdateNameMethod(portfolio);
                    break;
                }
                case (FunctionType.Create):
                {
                    if (functionInputs.Length != 3)
                    {
                        break;
                    }
                    output = CreateMethod(portfolio, name, (IReportLogger)functionInputs[2]);
                    break;
                }
                case (FunctionType.Edit):
                {
                    if (functionInputs.Length != 4)
                    {
                        break;

                    }
                    output = EditMethod(portfolio, name, (NameData)functionInputs[2], (IReportLogger)functionInputs[3]);
                    break;
                }
                case (FunctionType.Delete):
                {
                    if (functionInputs.Length != 3)
                    {
                        break;
                    }
                    output = DeleteMethod(portfolio, name, (IReportLogger)functionInputs[2]);
                    break;
                }
                case (FunctionType.SelectData):
                {
                    if (functionInputs.Length != 3)
                    {
                        break;
                    }
                    output = SelectedDataMethod(portfolio, name, (IReportLogger)functionInputs[2]);
                    break;
                }
                case (FunctionType.AddData):
                {
                    if (functionInputs.Length != 4)
                    {
                        break;
                    }
                    output = AddDataMethod(portfolio, name, (DayValue_ChangeLogged)functionInputs[2], (IReportLogger)functionInputs[3]);
                    break;
                }
                case (FunctionType.EditData):
                {
                    if (functionInputs.Length != 5)
                    {
                        break;
                    }
                    output = EditDataMethod(portfolio, name, (DayValue_ChangeLogged)functionInputs[2], (DayValue_ChangeLogged)functionInputs[3], (IReportLogger)functionInputs[4]);
                    break;
                }
                case (FunctionType.DeleteData):
                {
                    if (functionInputs.Length != 4)
                    {
                        break;
                    }
                    output = DeleteDataMethod(portfolio, name, (DayValue_ChangeLogged)functionInputs[2], (IReportLogger)functionInputs[3]);
                    break;
                }
                default:
                    break;
            }
            return output;
        }

        private readonly Func<IPortfolio, NameData, IReportLogger, Task> DownloadMethod;
        private readonly Func<IPortfolio, List<NameCompDate>> UpdateNameMethod;

        private readonly Func<IPortfolio, NameData, IReportLogger, bool> CreateMethod;

        private readonly Func<IPortfolio, NameData, NameData, IReportLogger, bool> EditMethod;

        private readonly Func<IPortfolio, NameData, IReportLogger, bool> DeleteMethod;

        private readonly Func<IPortfolio, NameData, IReportLogger, List<DayValue_ChangeLogged>> SelectedDataMethod;

        private readonly Func<IPortfolio, NameData, DayValue_ChangeLogged, IReportLogger, bool> AddDataMethod;

        private readonly Func<IPortfolio, NameData, DayValue_ChangeLogged, DayValue_ChangeLogged, IReportLogger, bool> EditDataMethod;

        private readonly Func<IPortfolio, NameData, DayValue_ChangeLogged, IReportLogger, bool> DeleteDataMethod;

        /// <summary>
        /// Generates collection of methods to edit/create object of type accountType. Note this will not work with Security type as
        /// these data are special.
        /// </summary>
        public static EditMethods GenerateEditMethods(AccountType accountType)
        {
            return new EditMethods(
              (portfolio, name, reportUpdate) => PortfolioDataUpdater.DownloadOfType(accountType, portfolio, name, reportUpdate),
              (portfolio) => portfolio.NameData(accountType),
              (portfolio, name, reports) => portfolio.TryAdd(accountType, name, reports),
              (portfolio, oldName, newName, reports) => portfolio.TryEditName(accountType, oldName, newName, reports),
              (portfolio, name, reports) => portfolio.TryRemove(accountType, name, reports),
              (portfolio, name, reports) => portfolio.NumberData(accountType, name, reports),
              (portfolio, name, data, reports) => portfolio.TryAddData(accountType, name, data, reports),
              (portfolio, name, oldData, newData, reports) => portfolio.TryEditData(accountType, name, oldData, newData, reports),
              (portfolio, name, data, reports) => portfolio.TryDeleteData(accountType, name, data, reports));
        }

        private EditMethods(
            Func<IPortfolio, NameData, IReportLogger, Task> downloadMethod,
            Func<IPortfolio, List<NameCompDate>> updateNameMethod = null,
            Func<IPortfolio, NameData, IReportLogger, bool> createMethod = null,
            Func<IPortfolio, NameData, NameData, IReportLogger, bool> editMethod = null,
            Func<IPortfolio, NameData, IReportLogger, bool> deleteMethod = null,
            Func<IPortfolio, NameData, IReportLogger, List<DayValue_ChangeLogged>> selectedDataMethod = null,
            Func<IPortfolio, NameData, DayValue_ChangeLogged, IReportLogger, bool> addDataMethod = null,
            Func<IPortfolio, NameData, DayValue_ChangeLogged, DayValue_ChangeLogged, IReportLogger, bool> editDataMethod = null,
            Func<IPortfolio, NameData, DayValue_ChangeLogged, IReportLogger, bool> deleteDataMethod = null)
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
        {
        }
    }
}
