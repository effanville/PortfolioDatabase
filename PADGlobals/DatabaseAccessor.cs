using FinancialStructures.Database;
using FinancialStructures.FinanceStructures;
using FinancialStructures.GUIFinanceStructures;
using FinancialStructures.ReportingStructures;
using GlobalHeldData;
using SavingDummyClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace GUIAccessorFunctions
{
    /// <summary>
    /// Class holding functions for User Interfaces to edit the global database.
    /// </summary>
    public static class DatabaseAccessor
    {
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

        public static List<NameData> GetSectorNames()
        {
            var outputs = new List<NameData>();
            if (GlobalData.BenchMarks != null)
            {
                foreach (Sector thing in GlobalData.BenchMarks)
                {
                    outputs.Add(new NameData(thing.GetName(), string.Empty, string.Empty, thing.GetUrl(), false));
                }
            }
            return outputs;
        }

        public static void SetFilePath(string path)
        {
            GlobalData.fDatabaseFilePath = path;
        }

        public static void ClearPortfolio()
        {
            GlobalData.ClearDatabase();
        }

        public static void LoadPortfolio(ErrorReports reports)
        {
            if (File.Exists(GlobalData.fDatabaseFilePath))
            {
                var database = ReadFromXmlFile<AllData>(GlobalData.fDatabaseFilePath);
                GlobalData.LoadDatabase(database.MyFunds, database.myBenchMarks);
                return;
            }

            reports.AddReport("Loaded Empty New Database.");
            GlobalData.LoadDatabase(null, null);
        }

        public static void SavePortfolio(ErrorReports reports)
        {
            var toSave = new AllData(GlobalData.Finances.GetPortfolio(), GetBenchMarks());
            if (GlobalData.fDatabaseFilePath != null)
            {
                WriteToXmlFile(GlobalData.fDatabaseFilePath, toSave);
                reports.AddReport("Saved Database.");
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
            catch (Exception ex)
            {
                return;
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
            catch (Exception ex)
            {
                return default(T);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }
    }
}
