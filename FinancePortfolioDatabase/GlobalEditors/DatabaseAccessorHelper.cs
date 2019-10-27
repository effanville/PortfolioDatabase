using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using GlobalHeldData;
using FinanceStructures;
using GUIFinanceStructures;

namespace GUIAccessorFunctions
{
    /// <summary>
    /// Class holding functions for User Interfaces to edit the global database.
    /// </summary>
    public static class DatabaseAccessorHelper
    {
        /// <summary>
        /// Returns a copy of the currently held portfolio. 
        /// Note one cannot use this portfolio to edit as it makes a copy.
        /// </summary>
        public static Portfolio GetPortfolio()
        {
            var PortfoCopy = new Portfolio();
            PortfoCopy = GlobalData.Finances;
            return PortfoCopy;
        }
        public static List<string> GetSecurityNames()
        {
            if (GlobalData.Finances != null)
            {
                return GlobalData.Finances.GetSecurityNames();
            }
            return new List<string>();
        }
        public static List<string> GetSectorNames()
        {
            var outputs = new List<string>();
            if (GlobalData.BenchMark != null)
            {
                foreach (Sector thing in GlobalData.BenchMark)
                {
                    outputs.Add(thing.GetName());
                }
            }
            return outputs;
        }
        public static List<NameComp> GetSecurityNamesAndCompanies()
        {
            if (GlobalData.Finances != null)
            {
                return GlobalData.Finances.GetSecurityNamesAndCompanies();
            }
            return new List<NameComp>();
        }

        public static List<string> GetBankAccountNames()
        {
            if (GlobalData.Finances != null)
            {
                return GlobalData.Finances.GetBankAccountNames();
            }
            return new List<string>();
        }

        public static List<NameComp> GetBankAccountNamesAndCompanies()
        {
            if (GlobalData.Finances != null)
            {
                return GlobalData.Finances.GetBankAccountNamesAndCompanies();
            }
            return new List<NameComp>();
        }

        public static void LoadPortfolio()
        {
            if (File.Exists(GlobalData.fDatabaseFilePath))
            {
                var database = ReadFromXmlFile<Portfolio>(GlobalData.fDatabaseFilePath);
                GlobalData.LoadDatabase(database);
                return;
            }

            GlobalData.LoadDatabase(null);
        }

        public static void SavePortfolio()
        {
            var ToSave = GetPortfolio();
            if (GlobalData.fDatabaseFilePath != null)
            {
                WriteToXmlFile<Portfolio>(GlobalData.fDatabaseFilePath, ToSave);
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
