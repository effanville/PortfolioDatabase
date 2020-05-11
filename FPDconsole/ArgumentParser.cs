using StructureCommon.Reporting;
using System.Collections.Generic;
using System.IO;

namespace FPDconsole
{
    public class ArgumentParser
    {
        private readonly LogReporter logReports;

        private string ParameterSpecifier = "--";

        private HashSet<string> downloadArguments = new HashSet<string>() { "download", "d" };
        private HashSet<string> downloadStatisticsArguments = new HashSet<string>() { "downloadUpdateStatistics", "u" };
        private HashSet<string> helpArguments = new HashSet<string>() { "help", "h" };

        public ArgumentParser(LogReporter reportLogger)
        {
            logReports = reportLogger;
        }

        public List<TextToken> Parse(string[] args)
        {
            List<TextToken> tokens = new List<TextToken>();
            if (args.Length > 1)
            {
                tokens.Add(PrepareFilePath(args[0], logReports));
                for (int index = 1; index < args.Length; index++)
                {
                    tokens.Add(ParseNonFilePathToken(args[index], logReports));
                }
            }
            else
            {
                _ = logReports.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.Parsing, "Insufficient parameters specified for program to run.");
            }

            return tokens;
        }

        private TextToken PrepareFilePath(string expectedFilePath, LogReporter reportLogger)
        {
            if (File.Exists(expectedFilePath))
            {
                return new TextToken(TextTokenType.FilePath, expectedFilePath);
            }

            _ = reportLogger.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.Parsing, "Specified Text not valid.");
            return new TextToken(TextTokenType.Error, expectedFilePath);
        }

        private TextToken ParseNonFilePathToken(string tokenText, LogReporter reportLogger)
        {
            if (tokenText.StartsWith(ParameterSpecifier))
            {
                return new TextToken(TextTokenType.Parameter, tokenText);
            }
            else if (downloadArguments.Contains(tokenText))
            {
                return new TextToken(TextTokenType.Download, tokenText);
            }
            else if (downloadStatisticsArguments.Contains(tokenText))
            {
                return new TextToken(TextTokenType.DownloadUpdateStats, tokenText);
            }
            else if (helpArguments.Contains(tokenText))
            {
                return new TextToken(TextTokenType.Help, tokenText);
            }

            _ = reportLogger.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.Parsing, "Specified Text not valid.");
            return new TextToken(TextTokenType.Error, tokenText);
        }
    }
}
