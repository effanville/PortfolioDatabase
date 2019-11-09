﻿using DataStructures;
using System;

namespace FinanceStructures
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
        private Sector()
        { }

        /// <summary>
        /// Creates a new instance of a sector.
        /// </summary>
        public Sector(string name)
        {
            fName = name;
            fValues = new TimeList();
        }

        private Sector(string name, TimeList values)
        {
            fName = name;
            fValues = values;
        }

        internal Sector Copy()
        {
            return new Sector(fName, fValues);
        }
    }
}
