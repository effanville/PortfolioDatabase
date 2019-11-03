using DataStructures;
using System;

namespace FinanceStructures
{
    public partial class Sector
    {
        private string fName;

        [Obsolete("This should only be used for serialisation or data visualisation.", false)]
        public string Name
        {
            get => fName;
            set => fName = value;
        }
        private TimeList fValues;

        [Obsolete("This should only be used for serialisation or data visualisation.", false)]
        public TimeList Values
        {
            get => fValues;
            set => fValues = value;
        }

        public Sector()
        { }
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
