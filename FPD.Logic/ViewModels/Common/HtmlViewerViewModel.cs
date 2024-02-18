using System;
using System.Windows.Input;

using Effanville.Common.UI;
using Effanville.Common.UI.Commands;
using Effanville.Common.UI.Services;

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
        private string _urlTextPath;

        public string UrlTextPath
        {
            get => _urlTextPath;
            set
            {
                if (SetAndNotify(ref _urlTextPath, value))
                {
                    if (UriHelpers.IsValidUri(_urlTextPath, out Uri newUri))
                    {
                        HtmlPath = newUri;
                    }
                }
            }
        }

        private Uri _htmlPath;

        /// <summary>
        /// The uri path of the html file to display.
        /// </summary>
        public Uri HtmlPath
        {
            get => _htmlPath;
            private set => SetAndNotify(ref _htmlPath, value);
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
            if (!string.IsNullOrEmpty(filePath))
            {
                UrlTextPath = filePath;
            }

            FileSelect = new RelayCommand(ExecuteFileSelect);
        }

        private void ExecuteFileSelect()
        {
            FileInteractionResult result = DisplayGlobals.FileInteractionService.OpenFile("html", filter: "HTML file|*.html;*.htm|All files|*.*");
            if (result.Success)
            {
                UrlTextPath = result.FilePath;
            }
        }
    }
}