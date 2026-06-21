using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ContactToVCard.Models;

public partial class ContactFile(string filePath, bool isComplete = false, bool isError = false)
    : ObservableObject
{
    [ObservableProperty]
    private string _filePath = filePath;

    [ObservableProperty]
    private bool _isComplete = isComplete;

    [ObservableProperty]
    private bool _isError = isError;

    public string FileName => Path.GetFileNameWithoutExtension(FilePath);

    partial void OnFilePathChanged(string value)
    {
        OnPropertyChanged(nameof(FileName));
    }
}