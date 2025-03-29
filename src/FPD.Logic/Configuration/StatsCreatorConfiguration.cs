using System.Collections.Generic;
using System.Runtime.Serialization;

using Effanville.Common.Structure.Reporting;
using Effanville.FPD.Logic.ViewModels.Stats;

namespace Effanville.FPD.Logic.Configuration
{
    /// <summary>
    /// Configuration object for the <see cref="StatsCreatorWindowViewModel"/>
    /// </summary>
    [DataContract]
    public sealed class StatsCreatorConfiguration : IConfiguration
    {
        /// <inheritdoc/>
        [DataMember(EmitDefaultValue = false)]
        public Dictionary<string, IConfiguration> ChildConfigurations { get; set; }

        /// <inheritdoc/>
        [DataMember]
        public bool HasLoaded { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public StatsCreatorConfiguration()
        {
            ChildConfigurations = new Dictionary<string, IConfiguration>
            {
                { UserConfiguration.StatsOptions, new ExportStatsConfiguration() },
                { UserConfiguration.HistoryOptions, new ExportHistoryConfiguration() },
                { UserConfiguration.ReportOptions, new ExportReportConfiguration() }
            };
        }

        /// <inheritdoc/>
        public void RestoreFromConfiguration(object viewModel)
        {
        }

        public void SaveConfiguration(IReportLogger logger = null) => throw new System.NotImplementedException();

        /// <inheritdoc/>
        public void StoreConfiguration(object viewModel)
        {
        }
    }
}
