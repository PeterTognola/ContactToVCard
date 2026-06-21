namespace ContactToVCard.Models;

public record ContactFile(string FilePath, bool IsComplete = false, bool HasError = false)
{
    public bool IsComplete { get; set; } = IsComplete;
    
    public bool HasError { get; set; } = HasError;
}