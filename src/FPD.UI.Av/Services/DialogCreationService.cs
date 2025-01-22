using Avalonia.Controls;

using Effanville.Common.UI.Services;

using FPD.UI.Av.Dialogs;

using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace Effanville.FPD.AvaloniaUI.Services
{
    /// <summary>
    /// Created dialog boxes in the UI. Note that this should live in the UI part, but is a service that can be used in
    /// the view model area of the code.
    /// </summary>
    public class DialogCreationService : IDialogCreationService
    {
        /// <summary>
        /// The default parent to use if non are selected.
        /// </summary>
        private readonly Window fDefaultParent;

        /// <summary>
        /// The standard constructor to use.
        /// </summary>
        public DialogCreationService(Window defaultParent)
        {
            fDefaultParent = defaultParent;
        }

        /// <summary>
        /// Shows a standard message box with the specified parameters.
        /// </summary>       
        /// <inheritdoc/>
        public MessageBoxOutcome ShowMessageBox(string text, string title, BoxButton buttons, BoxImage imageType)
        {
            Icon messageImageType = imageType.ToIcon();
            ButtonEnum messageButtons = buttons.ToButtonEnum();

            var messagebox = MessageBoxManager.GetMessageBoxStandard(text, title, messageButtons, messageImageType);
            var outcome = messagebox.ShowAsync().Result;
            return outcome.ToResult();
        }

        /// <summary>
        /// Shows a standard message box with the specified parameters with non-default owner.
        /// </summary>
        /// <inheritdoc/>
        public MessageBoxOutcome ShowMessageBox(Window owner, string text, string title, BoxButton buttons, BoxImage imageType)
        {
            Icon messageImageType = imageType.ToIcon();
            ButtonEnum messageButtons = buttons.ToButtonEnum();
            var messagebox = MessageBoxManager.GetMessageBoxStandard(text, title, messageButtons, messageImageType);
            var outcome = messagebox.ShowAsync().Result;
            return outcome.ToResult();
        }

        /// <summary>
        /// Displays an arbitrary dialog window, populated from an object which is
        /// either a window itself, or is a viewModel.
        /// In the latter case one should add a template into the DialogTemplate.xaml file.
        /// </summary>
        /// <inheritdoc/>
        public void DisplayCustomDialog(object obj)
        {
            // If obj is a window, then display that window.
            if (obj is Window window)
            {
                _ = window.ShowDialog(fDefaultParent);
            }
            else
            {
                // if obj isnt a window, guess it is a view model, so try to display as such.
                DialogWindow dialog = new DialogWindow() { DataContext = obj };
                dialog.ShowInTaskbar = true;
                _ = dialog.ShowDialog(fDefaultParent);
            }
        }
    }

    public static class Converter
    {
        public static MessageBoxOutcome ToResult(this ButtonResult outcome)
        {
            switch (outcome)
            {
                case ButtonResult.Ok:
                    return MessageBoxOutcome.OK;
                case ButtonResult.Cancel:
                    return MessageBoxOutcome.Cancel;
                case ButtonResult.Yes:
                    return MessageBoxOutcome.Yes;
                case ButtonResult.No:
                    return MessageBoxOutcome.No;
                case ButtonResult.None:
                default:
                    return MessageBoxOutcome.None;
            }
        }
        public static ButtonEnum ToButtonEnum(this BoxButton button)
        {
            switch (button)
            {
                case BoxButton.OKCancel:
                    return ButtonEnum.OkCancel;
                case BoxButton.YesNoCancel:
                    return ButtonEnum.YesNoCancel;
                case BoxButton.YesNo:
                    return ButtonEnum.YesNo;
                default:
                case BoxButton.OK:
                    return ButtonEnum.Ok;
            }
        }

        public static Icon ToIcon(this BoxImage image)
        {
            switch (image)
            {
                default:
                case BoxImage.None:
                    return Icon.None;
                case BoxImage.Error:
                    return Icon.Error;
                case BoxImage.Stop:
                    return Icon.Stop;
                case BoxImage.Question:
                    return Icon.Question;
                case BoxImage.Warning:
                    return Icon.Warning;
                case BoxImage.Information:
                    return Icon.Info;
            }
        }
    }
}
