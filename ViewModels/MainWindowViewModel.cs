using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ContactToVCard.Services;

namespace ContactToVCard.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IFilePickerService filePickerService;

    public MainWindowViewModel(IFilePickerService filePickerService)
    {
        this.filePickerService = filePickerService;
    }

    [ObservableProperty]
    private string selectedFilesSummary = "No files selected.";

    [ObservableProperty]
    private string selectedOutputFolderSummary = "No output folder selected.";

    public string PickFilesText { get; } = "Select Contact Files";
    public string PickOutputFolderText { get; } = "Select Where To Save";

    [RelayCommand]
    private async Task PickFilesAsync()
    {
        var files = await filePickerService.PickFilesAsync();
        SetSelectedFiles(files);
    }

    [RelayCommand]
    private async Task PickOutputFolderAsync()
    {
        var folderPath = await filePickerService.PickOutputFolderAsync();
        SetSelectedOutputFolder(folderPath);
    }

    private void SetSelectedFiles(IEnumerable<string> filePaths)
    {
        var files = filePaths.ToList();

        SelectedFilesSummary = files.Count == 0
            ? "No files selected."
            : $"Selected {files.Count} file(s):\n{string.Join("\n", files)}";
    }

    private void SetSelectedOutputFolder(string? folderPath)
    {
        SelectedOutputFolderSummary = string.IsNullOrWhiteSpace(folderPath)
            ? "No output folder selected."
            : $"Output folder:\n{folderPath}";
    }
}