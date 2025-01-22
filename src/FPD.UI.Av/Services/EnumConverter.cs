using System.Windows;

using Effanville.Common.UI.Services;

namespace Effanville.FPD.AvaloniaUI.Services;

/// <summary>
/// Converts internal types into System.Windows types
/// </summary>
internal static class EnumConverter
{
    public static MessageBoxOutcome ToResult(this MessageBoxResult outcome)
    {
        switch (outcome)
        {
            case MessageBoxResult.OK:
                return MessageBoxOutcome.OK;
            case MessageBoxResult.Cancel:
                return MessageBoxOutcome.Cancel;
            case MessageBoxResult.Yes:
                return MessageBoxOutcome.Yes;
            case MessageBoxResult.No:
                return MessageBoxOutcome.No;
            case MessageBoxResult.None:
            default:
                return MessageBoxOutcome.None;
        }
    }

    public static MessageBoxButton ToMessageBoxButton(this BoxButton button)
    {
        switch (button)
        {
            case BoxButton.OKCancel:
                return MessageBoxButton.OKCancel;
            case BoxButton.YesNoCancel:
                return MessageBoxButton.YesNoCancel;
            case BoxButton.YesNo:
                return MessageBoxButton.YesNo;
            default:
            case BoxButton.OK:
                return MessageBoxButton.OK;
        }
    }

    public static MessageBoxImage ToMessageBoxImage(this BoxImage image)
    {
        switch (image)
        {
            default:
            case BoxImage.None:
                return MessageBoxImage.None;
            case BoxImage.Error:
                return MessageBoxImage.Error;
            case BoxImage.Hand:
                return MessageBoxImage.Hand;
            case BoxImage.Stop:
                return MessageBoxImage.Stop;
            case BoxImage.Question:
                return MessageBoxImage.Question;
            case BoxImage.Exclamation:
                return MessageBoxImage.Exclamation;
            case BoxImage.Warning:
                return MessageBoxImage.Warning;
            case BoxImage.Asterisk:
                return MessageBoxImage.Asterisk;
            case BoxImage.Information:
                return MessageBoxImage.Information;
        }
    }
}
