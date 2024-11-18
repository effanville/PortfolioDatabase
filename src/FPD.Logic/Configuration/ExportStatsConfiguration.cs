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
        private Statistic SecuritySortingField;
        [DataMember]
        private SortDirection SecurityDirection;
        [DataMember(EmitDefaultValue = false)]
        private List<Selectable<Statistic>> SecurityColumnNames = new List<Selectable<Statistic>>();
        [DataMember]
        private Statistic BankSortingField;

        [DataMember]
        private SortDirection BankDirection;
        [DataMember(EmitDefaultValue = false)]
        private List<Selectable<Statistic>> BankColumnNames = new List<Selectable<Statistic>>();
        [DataMember]
        private Statistic SectorSortingField;
        [DataMember]
        private SortDirection SectorDirection;
        [DataMember(EmitDefaultValue = false)]
        private List<Selectable<Statistic>> SectorColumnNames = new List<Selectable<Statistic>>();

        [DataMember]
        private Statistic AssetSortingField;
        [DataMember]
        private SortDirection AssetDirection;
        [DataMember(EmitDefaultValue = false)]
        private List<Selectable<Statistic>> AssetColumnNames = new List<Selectable<Statistic>>();

        [DataMember]
        private Statistic CurrencySortingField;
        [DataMember]
        private SortDirection CurrencyDirection;
        [DataMember(EmitDefaultValue = false)]
        private List<Selectable<Statistic>> CurrencyColumnNames = new List<Selectable<Statistic>>();

        /// <inheritdoc/>
        [DataMember]
        public bool HasLoaded
        {
            get;
            set;
        }

        /// <inheritdoc/>
        [DataMember(EmitDefaultValue = false)]
        public Dictionary<string, IConfiguration> ChildConfigurations
        {
            get;
            set;
        }


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
                SecurityColumnNames = vm.SecurityColumnNames;
                SecuritySortingField = vm.SecuritySortingField;
                SecurityDirection = vm.SecurityDirection;
                SectorColumnNames = vm.SectorColumnNames;
                SectorSortingField = vm.SectorSortingField;
                SectorDirection = vm.SectorDirection;
                BankColumnNames = vm.BankColumnNames;
                BankSortingField = vm.BankSortingField;
                BankDirection = vm.BankDirection;
                AssetColumnNames = vm.AssetColumnNames;
                AssetSortingField = vm.AssetSortingField;
                AssetDirection = vm.AssetDirection;
                CurrencyColumnNames = vm.CurrencyColumnNames;
                CurrencySortingField = vm.CurrencySortingField;
                CurrencyDirection = vm.CurrencyDirection;
                DisplayConditions = vm.DisplayConditions;
            }
        }

        /// <inheritdoc/>
        public void RestoreFromConfiguration(object viewModel)
        {
            if (HasLoaded && viewModel is ExportStatsViewModel vm)
            {
                if (SecurityColumnNames != null && SecurityColumnNames.Any())
                {                    
                    if(vm.SecurityColumnNames == null)
                    {
                        vm.SecurityColumnNames = SecurityColumnNames;
                    }
                    else
                    {
                        foreach (Selectable<Statistic> name in vm.SecurityColumnNames)
                        {
                            Selectable<Statistic> configName = SecurityColumnNames.FirstOrDefault(config => config.Instance == name.Instance);
                            if (configName != null)
                            {
                                name.Selected = configName.Selected;
                            }
                        }
                    }
                }
                vm.SecuritySortingField = SecuritySortingField;
                vm.SecurityDirection = SecurityDirection;

                if (SectorColumnNames != null && SectorColumnNames.Any())
                {                    
                    if(vm.SectorColumnNames == null)
                    {
                        vm.SectorColumnNames = SectorColumnNames;
                    }
                    else
                    {
                        foreach (Selectable<Statistic> name in vm.SectorColumnNames)
                        {
                            Selectable<Statistic> configName = SectorColumnNames.FirstOrDefault(config => config.Instance == name.Instance);
                            if (configName != null)
                            {
                                name.Selected = configName.Selected;
                            }
                        }
                    }
                }
                vm.SectorSortingField = SectorSortingField;
                vm.SectorDirection = SectorDirection;

                if (BankColumnNames != null && BankColumnNames.Any())
                {
                    if(vm.BankColumnNames == null)
                    {
                        vm.BankColumnNames = BankColumnNames;
                    }
                    else
                    {
                        foreach (Selectable<Statistic> name in vm.BankColumnNames)
                        {
                            Selectable<Statistic> configName = BankColumnNames.FirstOrDefault(config => config.Instance == name.Instance);
                            if (configName != null)
                            {
                                name.Selected = configName.Selected;
                            }
                        }
                    }
                }
                vm.BankSortingField = BankSortingField;
                vm.BankDirection = BankDirection;

                if (AssetColumnNames != null && AssetColumnNames.Any())
                {                    
                    if(vm.AssetColumnNames == null)
                    {
                        vm.AssetColumnNames = AssetColumnNames;
                    }
                    else
                    {
                        foreach (Selectable<Statistic> name in vm.AssetColumnNames)
                        {
                            Selectable<Statistic> configName = AssetColumnNames.FirstOrDefault(config => config.Instance == name.Instance);
                            if (configName != null)
                            {
                                name.Selected = configName.Selected;
                            }
                        }
                    }
                }
                vm.AssetSortingField = AssetSortingField;
                vm.AssetDirection = AssetDirection;
                
                
                if (CurrencyColumnNames != null && CurrencyColumnNames.Any())
                {                    
                    if(vm.CurrencyColumnNames == null)
                    {
                        vm.CurrencyColumnNames = CurrencyColumnNames;
                    }
                    else
                    {
                        foreach (Selectable<Statistic> name in vm.CurrencyColumnNames)
                        {
                            Selectable<Statistic> configName = CurrencyColumnNames.FirstOrDefault(config => config.Instance == name.Instance);
                            if (configName != null)
                            {
                                name.Selected = configName.Selected;
                            }
                        }
                    }
                }
                vm.CurrencySortingField = CurrencySortingField;
                vm.CurrencyDirection = CurrencyDirection;

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

        public void SaveConfiguration(IReportLogger logger = null) => throw new System.NotImplementedException();
    }
}
