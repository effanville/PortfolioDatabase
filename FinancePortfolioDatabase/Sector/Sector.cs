
namespace FinanceStructures
{
    public partial class Sector
    {
        private string fName;
        private TimeList fValues;

        public Sector()
        { }

        public Sector(string name, TimeList values)
        {
            fName = name;
            fValues = values;
        }
    }
}
