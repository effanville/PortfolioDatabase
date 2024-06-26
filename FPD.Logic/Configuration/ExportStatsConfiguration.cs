﻿using System.Collections.Generic;
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
                    vm.SecurityColumnNames = SecurityColumnNames;
                }
                vm.SecuritySortingField = SecuritySortingField;
                vm.SecurityDirection = SecurityDirection;

                if (SectorColumnNames != null && SectorColumnNames.Any())
                {
                    vm.SectorColumnNames = SectorColumnNames;
                }
                vm.SectorSortingField = SectorSortingField;
                vm.SectorDirection = SectorDirection;

                if (BankColumnNames != null && BankColumnNames.Any())
                {
                    vm.BankColumnNames = BankColumnNames;
                }
                vm.BankSortingField = BankSortingField;
                vm.BankDirection = BankDirection;

                if (AssetColumnNames != null && AssetColumnNames.Any())
                {
                    vm.AssetColumnNames = AssetColumnNames;
                }
                vm.AssetSortingField = AssetSortingField;
                vm.AssetDirection = AssetDirection;

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
