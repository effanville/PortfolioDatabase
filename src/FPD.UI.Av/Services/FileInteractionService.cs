using Avalonia.Controls;
using Avalonia.Platform.Storage;

using Effanville.Common.UI.Services;

namespace Effanville.FPD.AvaloniaUI.Services
{
    /// <summary>
    /// Interaction service for the file system. Note this lives in the UI, but is a service for the view models.
    /// </summary>
    public class FileInteractionService : IFileInteractionService
    {
        /// <summary>
        /// The default parent to use if non are selected.
        /// </summary>
        private readonly Window fDefaultParent;

        /// <summary>
        /// The standard constructor to use.
        /// </summary>
        public FileInteractionService(Window defaultParent)
        {
            fDefaultParent = defaultParent;
        }

        //"XML Files|*.xml|Bin Files|*.bin|All Files|*.*"
        private List<FilePickerFileType> ParseFilters(string filter)
        {
            string[] filterStrings = filter.Split(new char[] { '|' });
            List<FilePickerFileType> filters = new List<FilePickerFileType>();
            for (int index = 0; index < filterStrings.Length; index += 2)
            {
                string name = filterStrings[index];
                string filterString = filterStrings[index + 1];
                var filterFormats = filterString.Split(new char[] { ';' }).ToList();

                filters.Add(new FilePickerFileType(name)
                {
                    Patterns = filterFormats
                });
            }

            return filters;
        }

        /// <summary>
        /// Interaction with an opening file dialog.
        /// </summary>
        /// <inheritdoc/>
        public async Task<FileInteractionResult> OpenFile(string defaultExt, string initialDirectory = null, string filter = null)
        {
            // Get a reference to our TopLevel (in our case the parent Window)
            var topLevel = TopLevel.GetTopLevel(fDefaultParent);

            var initialFolder = await topLevel!.StorageProvider.TryGetFolderFromPathAsync(initialDirectory);
            // Try to get the files
            var storageFiles = await topLevel!.StorageProvider.OpenFilePickerAsync(
                            new FilePickerOpenOptions()
                            {
                                AllowMultiple = false,
                                SuggestedStartLocation = initialFolder,
                                FileTypeFilter = ParseFilters(filter),
                                Title = "Open File"
                            });

            if (storageFiles?.Count > 0)
            {
                return new FileInteractionResult(true, storageFiles.FirstOrDefault()?.TryGetLocalPath());
            }

            return new FileInteractionResult(false, null);
        }

        public async Task<FileInteractionResult> SaveFile(string defaultExt, string fileName, string initialDirectory = null, string filter = null)
        {
            // Get a reference to our TopLevel (in our case the parent Window)
            var topLevel = TopLevel.GetTopLevel(fDefaultParent);

            var initialFolder = await topLevel!.StorageProvider.TryGetFolderFromPathAsync(initialDirectory);
            // Try to get the files
            var storageFile = await topLevel!.StorageProvider.SaveFilePickerAsync(
                            new FilePickerSaveOptions()
                            {
                                SuggestedStartLocation = initialFolder,
                                FileTypeChoices = ParseFilters(filter),
                                Title = "Open File"
                            });
            return new FileInteractionResult(true, storageFile?.Path.ToString());
        }
    }
}
