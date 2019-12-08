﻿using FinancialStructures.DataStructures;
using System;

namespace FinancialStructures.FinanceStructures
{
    /// <summary>
    /// Acts as an overall change of an area of funds.
    /// </summary>
    /// <example>
    /// e.g. FTSE100 or MSCI-Asia
    /// </example>
    public partial class Sector
    {
        /// <summary>
        /// The name of the sector. 
        /// </summary>
        /// <remarks>
        /// These names must be unique
        /// </remarks>
        private string fName;

        /// <summary>
        /// This should only be used for serialisation.
        /// </summary>
        public string Name
        {
            get => fName;
            set => fName = value;
        }

        private string fUrl;

        public string Url
        {
            get => fUrl;
            set => fUrl = value;
        }

        /// <summary>
        /// The values of the sector.
        /// </summary>
        private TimeList fValues;

        /// <summary>
        /// This should only be used for serialisation.
        /// </summary>
        public TimeList Values
        {
            get => fValues;
            set => fValues = value;
        }

        /// <summary>
        /// default constructor.
        /// </summary>
        public Sector()
        { }

        /// <summary>
        /// Creates a new instance of a sector.
        /// </summary>
        public Sector(string name)
        {
            fName = name;
            fValues = new TimeList();
        }

        public Sector(string name, string url)
        {
            fName = name;
            fUrl = url;
            fValues = new TimeList();
        }

        private Sector(string name, TimeList values)
        {
            fName = name;
            fValues = values;
        }

        private Sector(string name, string url, TimeList values)
        {
            fName = name;
            fUrl = url;
            fValues = values;
        }

        public Sector Copy()
        {
            return new Sector(fName, fValues);
        }
    }
}
