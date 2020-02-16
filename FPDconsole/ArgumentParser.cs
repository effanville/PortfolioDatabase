using FinancialStructures.ReportingStructures;
using System.Collections.Generic;
using System.IO;

namespace FPDconsole
{
    public static class ArgumentParser
    {
        private static string ParameterSpecifier = "--";

        private static HashSet<string> downloadArguments = new HashSet<string>() { "download", "d" };
        private static HashSet<string> helpArguments = new HashSet<string>() { "help", "h" };
        public static List<TextToken> Parse(string[] args, ErrorReports reports)
        {
            List<TextToken> tokens = new List<TextToken>();
            if (args.Length > 1)
            {
                tokens.Add(PrepareFilePath(args[0], reports));
                for (int index = 1; index < args.Length; index++)
                {
                    tokens.Add(ParseNonFilePathToken(args[index], reports));
                }
            }
            else
            {
                reports.AddError("Insufficient parameters specified for program to run.", Location.Parsing);
            }

            return tokens;
        }

        private static TextToken PrepareFilePath(string expectedFilePath, ErrorReports reports)
        {
            if (File.Exists(expectedFilePath))
            {
                return new TextToken(TextTokenType.FilePath, expectedFilePath);
            }
            reports.AddError("Specified Text not valid.", Location.Parsing);
            return new TextToken(TextTokenType.Error, expectedFilePath);
        }

        private static TextToken ParseNonFilePathToken(string tokenText, ErrorReports reports)
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

            reports.AddError("Specified Text not valid.", Location.Parsing);
            return new TextToken(TextTokenType.Error, tokenText);
        }
    }
}
