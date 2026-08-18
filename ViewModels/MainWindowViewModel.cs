using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ContactToVCard.Models;
using ContactToVCard.Services;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace ContactToVCard.ViewModels;

public partial class MainWindowViewModel(IFilePickerService filePickerService, IConvertContactService convertContactService) : ViewModelBase
{
    public MainWindowViewModel() : this(new DesignTimeFilePickerService(), new DesignTimeContactConverterService())
    {
    }

    [ObservableProperty]
    private string _selectedFilesSummary = "No files selected.";

    [ObservableProperty]
    private string _selectedOutputFolderSummary = "No folder selected.";

    public string PickFilesText { get; } = "Select Contact Files";
    public string PickOutputFolderText { get; } = "Select Where To Save";
    public string ConvertButtonText { get; } = "Convert To VCF Files";
    public string ConvertCsvButtonText { get; } = "Convert To CSV Files";
    public string IntroductionText { get; } = "Use this app to convert your .CONTACT files to .VCF files. Start by selecting the files, the output folder, and then press \"Convert Contacts\".";
    public string TitleText { get; set; } = "Contact To VCard";

    private const string NoFilesSelectedMessage = "Please select files and an output folder for your contacts.";
    
    public ObservableCollection<ContactFile> SelectedFiles { get; } = [];
    
    private string SelectedOutputFolder { get; set; } = null!;


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

    private async Task<bool> IsExportValid()
    {
        if (SelectedFiles.Count != 0 && !string.IsNullOrWhiteSpace(SelectedOutputFolder)) return true;
        
        await
            MessageBoxManager
                .GetMessageBoxStandard("No Files Selected", NoFilesSelectedMessage)
                .ShowAsync();
        
        return false;
    }

    private void ExportFiles(string extension = ".vcf")
    {
        foreach (var file in SelectedFiles)
        {
            var process = convertContactService.ConvertAndSaveContact(file.FilePath, SelectedOutputFolder, extension);
            
            file.IsError = !process;
            file.IsComplete = true;
        }
    }

    [RelayCommand]
    private async Task HandleProcessAsync()
    {
        if (await IsExportValid()) ExportFiles();
    }

    [RelayCommand]
    private async Task HandleCsvExportAsync()
    {
        if (await IsExportValid()) ExportFiles(".csv");
    }

    private void SetSelectedFiles(IEnumerable<string> filePaths)
    {
        SelectedFiles.Clear();

        foreach (var file in filePaths.Select(x => new ContactFile(x)))
        {
            SelectedFiles.Add(file);
        }

        SelectedFilesSummary = SelectedFiles.Count == 0
            ? "No files selected."
            : $"Selected {SelectedFiles.Count} file(s)";
    }

    private void SetSelectedOutputFolder(string? folderPath)
    {
        SelectedOutputFolder = folderPath;

        SelectedOutputFolderSummary = string.IsNullOrWhiteSpace(folderPath)
            ? "No folder selected."
            : $"Output folder:\n{folderPath}";
    }
    
    #region Preview
    
    // Contains mocked preview code.
    
    private sealed class DesignTimeFilePickerService : IFilePickerService
    {
        public Task<IReadOnlyList<string>> PickFilesAsync() => Task.FromResult<IReadOnlyList<string>>([]);

        public Task<string?> PickOutputFolderAsync() => Task.FromResult<string?>(null);
    }
    
    private sealed class DesignTimeContactConverterService : IConvertContactService
    {
        public bool ConvertAndSaveContact(string file, string outputFolder, string extension = ".vcf") => true;
    }

    #endregion
}