using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContactToVCard.Services;

public interface IFilePickerService
{
    Task<IReadOnlyList<string>> PickFilesAsync();

    Task<string?> PickOutputFolderAsync();
}