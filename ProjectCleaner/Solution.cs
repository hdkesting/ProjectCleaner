namespace ProjectCleaner;

internal class Solution(string filePath, string fileName, bool isSelected = false)
{
    public string FilePath { get; } = filePath;

    public string FileName { get; } = fileName;

    public bool IsSelected { get; set; } = isSelected;
}