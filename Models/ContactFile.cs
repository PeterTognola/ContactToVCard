using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ContactToVCard.Models;

public partial class ContactFile : ObservableObject
{
    public ContactFile(string filePath, bool isComplete = false, bool isError = false)
    {
        this.filePath = filePath;
        this.isComplete = isComplete;
        this.isError = isError;
    }

    [ObservableProperty]
    private string filePath;

    [ObservableProperty]
    private bool isComplete;

    [ObservableProperty]
    private bool isError;

    public string FileName => Path.GetFileNameWithoutExtension(FilePath);

    partial void OnFilePathChanged(string value)
    {
        OnPropertyChanged(nameof(FileName));
    }
}