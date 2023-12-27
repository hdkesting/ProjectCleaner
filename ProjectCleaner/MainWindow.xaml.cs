namespace SolutionCleaner;

using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly string configFilePath;
    private readonly string configFileFolder;

    public MainWindow()
    {
        InitializeComponent();

        configFilePath = System.IO.Path.Combine(
            System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "HansKesting",
            "ProjectCleaner",
            "SolutionPaths.txt");
        configFileFolder = System.IO.Path.GetDirectoryName(configFilePath)!;
        Directory.CreateDirectory(configFileFolder);

        foreach (var sln in ReadSolutionPaths())
        {
            SolutionPaths.Add(sln);
        }

        SolutionListView.ItemsSource = SolutionPaths;
    }

    private ObservableCollection<Solution> SolutionPaths { get; } = [];

    private List<Solution> ReadSolutionPaths()
    {
        if (File.Exists(configFilePath))
        {
            var paths = File.ReadAllLines(configFilePath);
            return paths.Select(p =>
                    {
                        var parts = p.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                        if (parts.Length == 2)
                        {
                            return new Solution(parts[0], parts[1]);
                        }
                        return null;
                    })
                .OfType<Solution>() // null check
                .ToList();
        }
        else
        {
            File.WriteAllText(configFilePath, string.Empty);
        }

        return [];
    }

    private void WriteSolutionPaths()
    {
        File.WriteAllLines(configFilePath, SolutionPaths.Select(sp => string.Join(";", sp.FilePath, sp.SolutionName)));
    }

    private void AddNewSolution(object sender, RoutedEventArgs e)
    {
        var ofd = new Microsoft.Win32.OpenFileDialog() { Filter = "Solution Files|*.sln", AddToRecent=false, CheckFileExists=true };
        var result = ofd.ShowDialog();
        if (result != true)
        {
            return;
        }

        foreach (var path in SolutionPaths)
        {
            path.IsSelected = false;
        }

        SolutionPaths.Add(new Solution(ofd.FileName, Path.GetFileNameWithoutExtension(ofd.FileName), true));
        WriteSolutionPaths();
    }

    private void CleanSolutions(object sender, RoutedEventArgs e)
    {
        using var statusWindow = new StatusWindow();

        statusWindow.SetSolutions(SolutionPaths.Where(sp => sp.IsSelected));
        statusWindow.ShowDialog();
    }

    private void RemoveSolution(object sender, RoutedEventArgs e)
    {
        var btn = (Button)sender;
        var sln = btn.Tag as string;
        if (!string.IsNullOrEmpty( sln ) )
        {
            var path = SolutionPaths.FirstOrDefault(sp => sp.FilePath == sln);
            if (path != null )
            {
                SolutionPaths.Remove(path);
            }
        }
    }
}
