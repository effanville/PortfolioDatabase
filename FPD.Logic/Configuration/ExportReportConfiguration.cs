﻿using System.Collections.Generic;
using System.Runtime.Serialization;

using FPD.Logic.ViewModels.Stats;

namespace FPD.Logic.Configuration
{
    /// <summary>
    /// Configuration for the stats display
    /// </summary>
    [DataContract]
    public sealed class ExportReportConfiguration : IConfiguration
    {
        [DataMember]
        internal bool DisplayValueFunds;

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
        public ExportReportConfiguration()
        {
            ChildConfigurations = new Dictionary<string, IConfiguration>();
        }

        /// <inheritdoc/>
        public void StoreConfiguration(object viewModel)
        {
            if (viewModel is ExportReportViewModel vm)
            {
                DisplayValueFunds = vm.DisplayValueFunds;
            }
        }

        /// <inheritdoc/>
        public void RestoreFromConfiguration(object viewModel)
        {
            if (viewModel is ExportReportViewModel vm)
            {
                vm.DisplayValueFunds = DisplayValueFunds;
            }
        }
    }
}