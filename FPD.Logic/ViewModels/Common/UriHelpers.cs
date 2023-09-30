using System;

namespace FPD.Logic.ViewModels.Common;

public static class UriHelpers
{
    public static bool IsValidUri(string text, out Uri uri)
    {
        if (Uri.TryCreate(text, UriKind.Absolute, out uri))
        {
            return true;
        }

        if (Uri.TryCreate(text, UriKind.Relative, out uri))
        {
            try
            {
                if (uri.IsFile || uri.IsAbsoluteUri)
                {
                    return true;
                }
            }
            catch (Exception)
            {
                uri = null;
                return false;
            }
        }

        uri = null;
        return false;
    }
}