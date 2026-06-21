namespace ContactToVCard.Models;

public record ContactFile(string File, bool IsProcessed = false, bool IsComplete = false, bool HasError = false);