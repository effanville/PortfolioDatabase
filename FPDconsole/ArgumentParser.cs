using FinancialStructures.Reporting;
using System.Collections.Generic;
using System.IO;

namespace FPDconsole
{
    public static class ArgumentParser
    {
        private static string ParameterSpecifier = "--";

        private static HashSet<string> downloadArguments = new HashSet<string>() { "download", "d" };
        private static HashSet<string> helpArguments = new HashSet<string>() { "help", "h" };

        public static List<TextToken> Parse(string[] args, LogReporter reportLogger)
        {
            List<TextToken> tokens = new List<TextToken>();
            if (args.Length > 1)
            {
                tokens.Add(PrepareFilePath(args[0], reportLogger));
                for (int index = 1; index < args.Length; index++)
                {
                    tokens.Add(ParseNonFilePathToken(args[index], reportLogger));
                }
            }
            else
            {
                _ = reportLogger.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.Parsing, "Insufficient parameters specified for program to run.");
            }

            return tokens;
        }

        private static TextToken PrepareFilePath(string expectedFilePath, LogReporter reportLogger)
        {
            if (File.Exists(expectedFilePath))
            {
                return new TextToken(TextTokenType.FilePath, expectedFilePath);
            }

            _ = reportLogger.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.Parsing, "Specified Text not valid.");
            return new TextToken(TextTokenType.Error, expectedFilePath);
        }

        private static TextToken ParseNonFilePathToken(string tokenText, LogReporter reportLogger)
        {
            if (tokenText.StartsWith(ParameterSpecifier))
            {
                return new TextToken(TextTokenType.Parameter, tokenText);
            }
            else if (downloadArguments.Contains(tokenText))
            {
                return new TextToken(TextTokenType.Download, tokenText);
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
