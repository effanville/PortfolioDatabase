using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FPDconsole
{
    public static class ArgumentParser
    {
        public static List<TextToken> Parse(string[] args)
        {
            List<TextToken> tokens = new List<TextToken>();
            if (args.Length > 0)
            {
                tokens.Add(PrepareFilePath(args[0]));
            }
            else { }

            return tokens;
        }

        public static TextToken PrepareFilePath(string expectedFilePath)
        {
            if (File.Exists(expectedFilePath))
            {
                return new TextToken(TokenType.FilePath, expectedFilePath);
            }

            return new TextToken(TokenType.Error, expectedFilePath);
        }
    }

    public enum TokenType
    {
        FilePath,
        Argument,
        Parameter,
        Error
    }

    public class TextToken
    {
        public TokenType sort;
        public string field;

        public TextToken(TokenType type, string value)
        { 
            sort = type;
            field = value;
        }
    }
}
