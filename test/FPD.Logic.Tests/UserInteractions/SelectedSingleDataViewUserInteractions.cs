﻿using System.Linq;

using Effanville.Common.Structure.DataStructures;
using Effanville.FinancialStructures.FinanceStructures;
using Effanville.FPD.Logic.ViewModels.Common;

namespace Effanville.FPD.Logic.Tests.UserInteractions
{
    /// <summary>
    /// Contains user like interaction behaviours with the <see cref="SelectedSingleDataViewModel"/>.
    /// </summary>
    public static class SelectedSingleDataViewUserInteractions
    {
        public static DailyValuation AddNewItem(this SelectedSingleDataViewModel viewModel)
        {
            viewModel.SelectItem(null);
            viewModel.TLVM.Valuations.Add(viewModel.TLVM.DefaultNewItem());
            DailyValuation newItem = viewModel.TLVM.Valuations.Last();
            viewModel.SelectItem(newItem);
            viewModel.BeginEdit();

            return newItem;
        }

        public static void SelectItem(this SelectedSingleDataViewModel viewModel, DailyValuation valueToSelect)
        {
            viewModel.TLVM.SelectionChangedCommand?.Execute(valueToSelect);
        }
        
        public static void BeginEdit(this SelectedSingleDataViewModel viewModel)
        {
            viewModel.TLVM.PreEditCommand?.Execute(null);
        }

        public static void CompleteEdit(this SelectedSingleDataViewModel viewModel, IValueList valueList)
        {
            viewModel.TLVM.AddEditDataCommand?.Execute(null);
            viewModel.UpdateData(valueList, false);
        }

        public static void DeleteSelected(this SelectedSingleDataViewModel viewModel, IValueList valueList)
        {
            viewModel.DeleteValuationCommand?.Execute(null);
            viewModel.UpdateData(valueList, false);
        }
    }
}
