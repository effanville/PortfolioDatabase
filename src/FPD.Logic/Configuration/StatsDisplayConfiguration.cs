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
    public sealed class StatsDisplayConfiguration : IConfiguration
    {
        [DataMember]
        internal bool DisplayValueFunds;
        [DataMember(EmitDefaultValue = false)]
        internal List<Selectable<Statistic>> StatisticNames;

        /// <inheritdoc/>
        [DataMember]
        public bool HasLoaded { get; set; }

        /// <inheritdoc/>
        [DataMember(EmitDefaultValue = false)]
        public Dictionary<string, IConfiguration> ChildConfigurations { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public StatsDisplayConfiguration()
        {
            ChildConfigurations = new Dictionary<string, IConfiguration>();
        }

        /// <inheritdoc/>
        public void StoreConfiguration(object viewModel)
        {
            if (viewModel is StatsViewModel vm)
            {
                DisplayValueFunds = vm.DisplayValueFunds;
                StatisticNames = vm.StatisticNames;
            }
        }

        /// <inheritdoc/>
        public void RestoreFromConfiguration(object viewModel)
        {
            if (viewModel is StatsViewModel vm)
            {
                if (StatisticNames != null && StatisticNames.Any())
                {
                    if (vm.StatisticNames == null)
                    {
                        vm.StatisticNames = StatisticNames;
                    }
                    else
                    {
                        foreach (Selectable<Statistic> name in vm.StatisticNames)
                        {
                            Selectable<Statistic> configName = StatisticNames.FirstOrDefault(config => config.Instance == name.Instance);
                            if (configName != null)
                            {
                                name.Selected = configName.Selected;
                            }
                        }
                    }
                }

                vm.DisplayValueFunds = DisplayValueFunds;
            }
        }

        public void SaveConfiguration(IReportLogger logger = null) => throw new System.NotImplementedException();
    }
}
