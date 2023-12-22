namespace ProjectCleaner;

using System;
using System.IO;
using System.Linq;

internal class Cleaner
{
    public int DeletedFiles { get; private set; }

    public int DeletedFolders { get; private set; }

    public List<string> ErrorMessages { get; } = [];

    public void CleanSolution(string filePath)
    {
        DeletedFiles = 0;
        DeletedFolders = 0;
        string folder = Path.GetDirectoryName(filePath)!;
        var di = new DirectoryInfo(folder);
        ClearDirectory(di, false);
    }

    private void ClearDirectory(DirectoryInfo dir, bool tobedeleted)
    {
        var isProjectRoot = dir.EnumerateFiles("*.csproj").Any();

        foreach (var subdir in dir.EnumerateDirectories())
        {
            if (isProjectRoot)
            {
                if (subdir.Name == "bin" || subdir.Name == "obj")
                {
                    ClearDirectory(subdir, true);
                }
                // just skip other folders in project root
            }
            else
            {
                ClearDirectory(subdir, tobedeleted);
            }
        }

        // depth-first deletion
        if (tobedeleted)
        {
            int count = 0;
            // remove all files (subfolders are already removed)
            foreach (var file in dir.EnumerateFiles())
            {
                try
                {
                    file.Delete();
                    count++;
                    DeletedFiles++;
                }
                catch (Exception exf)
                {
                    ErrorMessages.Add("Couldn't delete file: " + file.Name + ", " + exf.Message);
                }
            }

            // then remove the folder itself
            try
            {
                dir.Delete();
                DeletedFolders++;
            }
            catch (Exception exd)
            {
                ErrorMessages.Add("Couldn't delete folder: " + dir.FullName + ", " + exd.Message);
            }
        }
    }
}
