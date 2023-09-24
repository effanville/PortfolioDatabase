using System;
using System.Windows.Input;
using Common.UI;
using Common.UI.Commands;
using Common.UI.Services;
using FPD.Logic.TemplatesAndStyles;
using FPD.Logic.ViewModels.Common;
using FinancialStructures.Database;

namespace FPD.Logic.ViewModels
{
    /// <summary>
    /// Enables the display and selection of a html file in a closable tab.
    /// </summary>
    public sealed class HtmlViewerViewModel : DataDisplayViewModelBase
    {
        private Uri _htmlPath;

        /// <summary>
        /// The uri path of the html file to display.
        /// </summary>
        public Uri HtmlPath
        {
            get => _htmlPath;
            set => SetAndNotify(ref _htmlPath, value);
        }

        /// <summary>
        /// Command to select the path of the html file.
        /// </summary>
        public ICommand FileSelect
        {
            get;
        }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        public HtmlViewerViewModel(UiStyles styles, UiGlobals globals, string header, string filePath)
            : base(globals, styles, null, header, Account.All, closable: true)
        {
            HtmlPath = new Uri(filePath);
            FileSelect = new RelayCommand(ExecuteFileSelect);
        }

        private void ExecuteFileSelect()
        {
            FileInteractionResult result = DisplayGlobals.FileInteractionService.OpenFile("html", filter: "HTML file|*.html;*.htm|All files|*.*");
            if (result.Success)
            {
                HtmlPath = new Uri(result.FilePath);
            }
        }
    }
}
