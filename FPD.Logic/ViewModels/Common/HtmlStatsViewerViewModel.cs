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
    public sealed class HtmlStatsViewerViewModel : DataDisplayViewModelBase
    {
        private Uri fDisplayStats;

        /// <summary>
        /// The uri path of the html file to display.
        /// </summary>
        public Uri DisplayStats
        {
            get => fDisplayStats;
            set => SetAndNotify(ref fDisplayStats, value);
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
        public HtmlStatsViewerViewModel(UiStyles styles, UiGlobals globals, string header, string filePath)
            : base(globals, styles, null, header, Account.All, closable: true)
        {
            DisplayStats = new Uri(filePath);
            FileSelect = new RelayCommand(ExecuteFileSelect);
        }

        private void ExecuteFileSelect()
        {
            FileInteractionResult result = DisplayGlobals.FileInteractionService.OpenFile("html", filter: "HTML file|*.html;*.htm|All files|*.*");
            if (result.Success)
            {
                DisplayStats = new Uri(result.FilePath);
            }
        }
    }
}
