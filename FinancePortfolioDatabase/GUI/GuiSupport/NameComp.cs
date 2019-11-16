using System;

namespace GUIFinanceStructures
{
    public class NameComp : IComparable
    {
        public int CompareTo(object obj)
        {
            if (obj is NameComp value)
            {
                if (Company == value.Company)
                {
                    return Name.CompareTo(value.Name);
                }

                return Company.CompareTo(value.Company);
            }

            return 0;
        }

        public NameComp()
        { }
        public NameComp(string n, string c)
        {
            Name = n;
            Company = c;
        }
        public string Name { get; set; }
        public string Company { get; set; }
    }
}
