using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ContactToVCard.Services;

namespace ContactToVCard.ViewModels;

public partial class MainWindowViewModel(IFilePickerService filePickerService) : ViewModelBase
{
    public MainWindowViewModel() : this(new DesignTimeFilePickerService())
    {
    }

    [ObservableProperty]
    private string selectedFilesSummary = "No files selected.";

    [ObservableProperty]
    private string selectedOutputFolderSummary = "No output folder selected.";

    public string PickFilesText { get; } = "Select Contact Files";
    public string PickOutputFolderText { get; } = "Select Where To Save";
    public string ConvertButtonText { get; } = "Convert Contacts";
    public string IntroductionText { get; } = "Use this app to convert your .CONTACT files to .VCF files. Start by selecting the files, the output folder, and then press process.";
    public string TitleText { get; set; } = "Contact To VCard";
    
    private List<string> selectedFiles { get; set; }
    
    private string selectedOutputFolder { get; set; }

    
    [RelayCommand]
    private async Task HandlePickFilesAsync()
    {
        var files = await filePickerService.PickFilesAsync();
        SetSelectedFiles(files);
    }

    [RelayCommand]
    private async Task HandlePickOutputFolderAsync()
    {
        var folderPath = await filePickerService.PickOutputFolderAsync();
        SetSelectedOutputFolder(folderPath);
    }

    [RelayCommand]
    private async Task HandleProcessAsync()
    {
        if (selectedFiles.Count == 0 || string.IsNullOrWhiteSpace(selectedOutputFolder))
        {
            // todo warn user message.
            return;
        }
        
        
    }

    private void SetSelectedFiles(IEnumerable<string> filePaths)
    {
        var files = filePaths.ToList();

        selectedFiles = files;

        SelectedFilesSummary = files.Count == 0
            ? "No files selected."
            : $"Selected {files.Count} file(s)";
    }

    private void SetSelectedOutputFolder(string? folderPath)
    {
        SelectedOutputFolderSummary = string.IsNullOrWhiteSpace(folderPath)
            ? "No output folder selected."
            : $"Output folder:\n{folderPath}";
    }

    private void HandleProcess()
    {
        
    }

    // todo move out of here, add comments related to preview.
    private sealed class DesignTimeFilePickerService : IFilePickerService
    {
        public Task<IReadOnlyList<string>> PickFilesAsync() => Task.FromResult<IReadOnlyList<string>>([]);

        public Task<string?> PickOutputFolderAsync() => Task.FromResult<string?>(null);
    }
}