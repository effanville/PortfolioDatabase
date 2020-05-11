using FinancialStructures.Reporting;
using StringFunctions;
using System;
using System.Collections.Generic;
using System.IO;

namespace FPDconsole
{
    class Program
    {
        public static ConsoleStreamWriter ReportWriter;
        static void Main(string[] args)
        {
            try
            {
                MemoryStream errorStream = new MemoryStream();
                ReportWriter = new ConsoleStreamWriter(errorStream);

                void WriteReport(ReportSeverity detailLevel, ReportType errorType, ReportLocation location, string message)
                {
                    string toOutput = errorType + " - " + location + " - " + message;
                    ReportWriter.Write(toOutput);
                }

                LogReporter ReportLogger = new LogReporter(WriteReport);
                var commandRunner = new ExecuteCommands(ReportLogger, ReportWriter);
                ReportWriter.Write("FPDconsole.exe - version 1");
                if (args.Length == 0)
                {
                    commandRunner.DisplayHelp();
                    return;
                }

                var parser = new ArgumentParser(ReportLogger);
                List<TextToken> values = parser.Parse(args);

                commandRunner.RunCommands(values);

                TextToken filePath = values.Find(token => token.TokenType == TextTokenType.FilePath);
                ReportWriter.filePath = Path.GetDirectoryName(filePath.Value) + "\\" + DateTime.Now.FileSuitableDateTimeValue() + "-" + Path.GetFileNameWithoutExtension(filePath.Value) + "-output.log";
            }
            catch (Exception ex)
            {
                ReportWriter.Write(ex.Message);
            }
            finally
            {
                ReportWriter.Write("Program finished");
                ReportWriter.SaveToFile();
            }
        }
    }
}
