using System.ComponentModel;

using System.Runtime.CompilerServices;

namespace SolutionCleaner;

using System.ComponentModel;

public class SolutionCleanStatus(string filePath, string solutionName) : INotifyPropertyChanged
{
    private int? numberOfDeletedFiles;
    private int? numberOfDeletedFolders;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string FilePath { get; } = filePath;

    public string SolutionName { get; } = solutionName;

    public int? NumberOfDeletedFiles
    {
        get => numberOfDeletedFiles;
        set
        {
            numberOfDeletedFiles = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NumberOfDeletedFiles)));
        }
    }

    public int? NumberOfDeletedFolders
    {
        get => numberOfDeletedFolders;
        set
        {
            numberOfDeletedFolders = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NumberOfDeletedFolders)));
        }
    }

    protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
    {
        if (!Equals(field, newValue))
        {
            field = newValue;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }

        return false;
    }

    private System.Collections.IEnumerable solutionStatuses;

    public System.Collections.IEnumerable SolutionStatuses { get => solutionStatuses; set => SetProperty(ref solutionStatuses, value); }
}
