using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using GlobalHeldData;
using FinanceStructures;
using GUIFinanceStructures;
using SavingDummyClasses;
using ReportingStructures;
using DataStructures;

namespace GUIAccessorFunctions
{
    /// <summary>
    /// Class holding functions for User Interfaces to edit the global database.
    /// </summary>
    public static class DatabaseAccessor
    {
        /// <summary>
        /// Returns a copy of the currently held portfolio. 
        /// Note one cannot use this portfolio to edit as it makes a copy.
        /// </summary>
        public static Portfolio GetPortfolio()
        {
            var PortfoCopy = new Portfolio();

            foreach (var security in GlobalData.Finances.Funds)
            {
                PortfoCopy.Funds.Add(security);
            }
            foreach (var bankAcc in GlobalData.Finances.BankAccounts)
            {
                PortfoCopy.BankAccounts.Add(bankAcc);
            }

            return PortfoCopy;
        }

        /// <summary>
        /// returns a copy of the 
        /// </summary>
        public static List<Sector> GetBenchMarks()
        {
            var output = new List<Sector>();
            foreach (var sector in GlobalData.BenchMarks)
            { 
                output.Add(sector);
            }
            return output;
        }

        public static Sector GetSectorFromName(string name)
        {
            var benchmarks = GetBenchMarks();
            foreach (var sector in benchmarks)
            {
                if (sector.GetName() == name)
                {
                    return sector.Copy();
                }
            }

            return null;
        }

        public static Security GetSecurityFromName(string name, string company)
        {
            foreach (var security in GlobalData.Finances.Funds)
            {
                if (security.GetName() == name && security.GetCompany() == company)
                {
                    return security.Copy();
                }
            }

            return null;
        }

        public static List<string> GetSecurityNames()
        {
            if (GlobalData.Finances != null)
            {
                return GlobalData.Finances.GetSecurityNames();
            }
            return new List<string>();
        }
        public static List<NameComp> GetSectorNames()
        {
            var outputs = new List<NameComp>();
            if (GlobalData.BenchMarks != null)
            {
                foreach (Sector thing in GlobalData.BenchMarks)
                {
                    outputs.Add(new NameComp(thing.GetName(), string.Empty, false));
                }
            }
            return outputs;
        }

        public static void SetFilePath(string path)
        {
            GlobalData.fDatabaseFilePath = path;
        }
        public static List<NameComp> GetSecurityNamesAndCompanies()
        {
            if (GlobalData.Finances != null)
            {
                return GlobalData.Finances.GetSecurityNamesAndCompanies();
            }
            return new List<NameComp>();
        }

        public static List<SecurityStatsHolder> GenerateSecurityStatistics()
        {
            if (GlobalData.Finances != null)
            {
                return GlobalData.Finances.GenerateSecurityStatistics();
            }
            return new List<SecurityStatsHolder>();
        }

        public static List<string> GetBankAccountNames()
        {
            if (GlobalData.Finances != null)
            {
                return GlobalData.Finances.GetBankAccountNames();
            }
            return new List<string>();
        }

        public static List<BankAccountStatsHolder> GenerateBankAccountStatistics()
        {
            if (GlobalData.Finances != null)
            {
                return GlobalData.Finances.GenerateBankAccountStatistics();
            }
            return new List<BankAccountStatsHolder>();
        }

        public static List<NameComp> GetBankAccountNamesAndCompanies()
        {
            if (GlobalData.Finances != null)
            {
                return GlobalData.Finances.GetBankAccountNamesAndCompanies();
            }
            return new List<NameComp>();
        }

        public static List<DailyValuation_Named> AllSecuritiesInvestments()
        {
            return GlobalData.Finances.AllSecuritiesInvestments();
        }

        public static void ClearPortfolio()
        {
            GlobalData.ClearDatabase();
        }

        public static void LoadPortfolio()
        {
            ErrorReports.Configure();
            
            if (File.Exists(GlobalData.fDatabaseFilePath))
            {
                var database = ReadFromXmlFile<AllData>(GlobalData.fDatabaseFilePath);
                GlobalData.LoadDatabase(database.MyFunds, database.myBenchMarks);
                return;
            }
            ErrorReports.AddReport("Loaded Empty New Database.");
            GlobalData.LoadDatabase(null, null);
        }

        public static void SavePortfolio()
        {
            var toSave = new AllData(GetPortfolio(), GetBenchMarks());
            if (GlobalData.fDatabaseFilePath != null)
            {
                WriteToXmlFile(GlobalData.fDatabaseFilePath, toSave);
                ErrorReports.AddReport("Saved Database.");
            }
        }

        /// <summary>
        /// Writes the given object instance to an XML file.
        /// <para>Only Public properties and variables will be written to the file. These can be any type though, even other classes.</para>
        /// <para>If there are public properties/variables that you do not want written to the file, decorate them with the [XmlIgnore] attribute.</para>
        /// <para>Object type must have a parameterless constructor.</para>
        /// </summary>
        /// <typeparam name="T">The type of object being written to the file.</typeparam>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the file.</param>
        /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
        public static void WriteToXmlFile<T>(string filePath, T objectToWrite, bool append = false) where T : new()
        {
            TextWriter writer = null;
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                writer = new StreamWriter(filePath, append);
                serializer.Serialize(writer, objectToWrite);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        /// <summary>
        /// Reads an object instance from an XML file.
        /// <para>Object type must have a parameterless constructor.</para>
        /// </summary>
        /// <typeparam name="T">The type of object to read from the file.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the XML file.</returns>
        public static T ReadFromXmlFile<T>(string filePath) where T : new()
        {
            TextReader reader = null;
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                reader = new StreamReader(filePath);
                return (T)serializer.Deserialize(reader);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }
    }
}
