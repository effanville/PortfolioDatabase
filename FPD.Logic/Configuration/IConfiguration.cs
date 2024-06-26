﻿using System.Collections.Generic;

using Effanville.Common.Structure.Reporting;

namespace Effanville.FPD.Logic.Configuration
{
    /// <summary>
    /// Configuration object for storing user interactions.
    /// </summary>
    public interface IConfiguration
    {
        /// <summary>
        /// A named collection of child configuration objects for the
        /// current configuration.
        /// </summary>
        Dictionary<string, IConfiguration> ChildConfigurations
        {
            get;
            set;
        }

        /// <summary>
        /// Whether the object of the configuration has
        /// been loaded yet or not.
        /// </summary>
        bool HasLoaded
        {
            get;
            set;
        }

        /// <summary>
        /// Records the configuration o the object.
        /// </summary>
        void StoreConfiguration(object viewModel);

        /// <summary>
        /// Restores the object using the configuration.
        /// </summary>
        /// <param name="viewModel"></param>
        void RestoreFromConfiguration(object viewModel);

        /// <summary>
        /// Save the configuration to file.
        /// </summary>
        void SaveConfiguration(IReportLogger logger = null);
    }
}
