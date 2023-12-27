namespace SolutionCleaner;

internal class Solution(string filePath, string solutionName, bool isSelected = false)
{
    public string FilePath { get; } = filePath;

    public string SolutionName { get; } = solutionName;

    public bool IsSelected { get; set; } = isSelected;
}