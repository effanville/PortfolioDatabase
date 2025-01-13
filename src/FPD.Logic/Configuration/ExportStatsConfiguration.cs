using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using Effanville.Common.Structure.DisplayClasses;
using Effanville.Common.Structure.Reporting;
using Effanville.FinancialStructures.Database.Statistics;
using Effanville.FPD.Logic.ViewModels.Stats;

namespace Effanville.FPD.Logic.Configuration
{
    /// <summary>
    /// Configuration for the stats display
    /// </summary>
    [DataContract]
    public sealed class ExportStatsConfiguration : IConfiguration
    {
        // Internal configurations to store.
        [DataMember(EmitDefaultValue = false)]
        private List<Selectable<string>> DisplayConditions = new List<Selectable<string>>();
        [DataMember]
        private bool ShowSecurities;
        [DataMember]
        private Statistic SecuritySortingField;
        [DataMember]
        private SortDirection SecurityDirection;
        [DataMember(EmitDefaultValue = false)]
        private List<Selectable<Statistic>> SecurityColumnNames = new List<Selectable<Statistic>>();

        [DataMember]
        private bool ShowBankAccounts;
        [DataMember]
        private Statistic BankSortingField;
        [DataMember]
        private SortDirection BankDirection;
        [DataMember(EmitDefaultValue = false)]
        private List<Selectable<Statistic>> BankColumnNames = new List<Selectable<Statistic>>();

        [DataMember]
        private bool ShowSectors;
        [DataMember]
        private Statistic SectorSortingField;
        [DataMember]
        private SortDirection SectorDirection;
        [DataMember(EmitDefaultValue = false)]
        private List<Selectable<Statistic>> SectorColumnNames = new List<Selectable<Statistic>>();

        [DataMember]
        private bool ShowAssets;
        [DataMember]
        private Statistic AssetSortingField;
        [DataMember]
        private SortDirection AssetDirection;
        [DataMember(EmitDefaultValue = false)]
        private List<Selectable<Statistic>> AssetColumnNames = new List<Selectable<Statistic>>();

        [DataMember]
        private bool ShowCurrencies;
        [DataMember]
        private Statistic CurrencySortingField;
        [DataMember]
        private SortDirection CurrencyDirection;
        [DataMember(EmitDefaultValue = false)]
        private List<Selectable<Statistic>> CurrencyColumnNames = new List<Selectable<Statistic>>();

        /// <inheritdoc/>
        [DataMember]
        public bool HasLoaded { get; set; }

        /// <inheritdoc/>
        [DataMember(EmitDefaultValue = false)]
        public Dictionary<string, IConfiguration> ChildConfigurations { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ExportStatsConfiguration()
        {
        }

        /// <inheritdoc/>
        public void StoreConfiguration(object viewModel)
        {
            if (viewModel is ExportStatsViewModel vm)
            {
                ShowSecurities = vm.SecuritySortingData.ShouldDisplay;
                SecurityColumnNames = vm.SecuritySortingData.ColumnNames;
                SecuritySortingField = vm.SecuritySortingData.SortingField;
                SecurityDirection = vm.SecuritySortingData.SortingDirection;

                ShowSectors = vm.SectorSortingData.ShouldDisplay;
                SectorColumnNames = vm.SectorSortingData.ColumnNames;
                SectorSortingField = vm.SectorSortingData.SortingField;
                SectorDirection = vm.SectorSortingData.SortingDirection;

                ShowBankAccounts = vm.BankAccountSortingData.ShouldDisplay;
                BankColumnNames = vm.BankAccountSortingData.ColumnNames;
                BankSortingField = vm.BankAccountSortingData.SortingField;
                BankDirection = vm.BankAccountSortingData.SortingDirection;

                ShowAssets = vm.AssetSortingData.ShouldDisplay;
                AssetColumnNames = vm.AssetSortingData.ColumnNames;
                AssetSortingField = vm.AssetSortingData.SortingField;
                AssetDirection = vm.AssetSortingData.SortingDirection;

                ShowCurrencies = vm.CurrencySortingData.ShouldDisplay;
                CurrencyColumnNames = vm.CurrencySortingData.ColumnNames;
                CurrencySortingField = vm.CurrencySortingData.SortingField;
                CurrencyDirection = vm.CurrencySortingData.SortingDirection;

                DisplayConditions = vm.DisplayConditions;
            }
        }

        /// <inheritdoc/>
        public void RestoreFromConfiguration(object viewModel)
        {
            if (HasLoaded && viewModel is ExportStatsViewModel vm)
            {
                RestoreSubViewModel(vm.SecuritySortingData, SecurityColumnNames, SecuritySortingField, SecurityDirection, ShowSecurities);
                RestoreSubViewModel(vm.BankAccountSortingData, BankColumnNames, BankSortingField, BankDirection, ShowBankAccounts);
                RestoreSubViewModel(vm.SectorSortingData, SectorColumnNames, SectorSortingField, SectorDirection, ShowSectors);
                RestoreSubViewModel(vm.AssetSortingData, AssetColumnNames, AssetSortingField, AssetDirection, ShowAssets);
                RestoreSubViewModel(vm.CurrencySortingData, CurrencyColumnNames, CurrencySortingField, CurrencyDirection, ShowCurrencies);

                if (DisplayConditions != null && DisplayConditions.Any())
                {
                    foreach (var condition in DisplayConditions)
                    {
                        var displayedCond = vm.DisplayConditions.FirstOrDefault(cond => cond.Instance == condition.Instance);
                        displayedCond.Selected = condition.Selected;
                    }
                }
            }
        }

        private static void RestoreSubViewModel(
            ExportDataViewModel dataViewModel,
            List<Selectable<Statistic>> columnNames,
            Statistic sortingField,
            SortDirection sortingDirection,
            bool shouldDisplay)
        {
            if (columnNames != null && columnNames.Any())
            {
                if (dataViewModel.ColumnNames == null)
                {
                    dataViewModel.ColumnNames = columnNames;
                }
                else
                {
                    foreach (Selectable<Statistic> name in dataViewModel.ColumnNames)
                    {
                        Selectable<Statistic> configName = columnNames.FirstOrDefault(config => config.Instance == name.Instance);
                        if (configName != null)
                        {
                            name.Selected = configName.Selected;
                        }
                    }
                }
            }
            dataViewModel.ShouldDisplay = shouldDisplay;
            dataViewModel.SortingField = sortingField;
            dataViewModel.SortingDirection = sortingDirection;
        }

        public void SaveConfiguration(IReportLogger logger = null) => throw new System.NotImplementedException();
    }
}
