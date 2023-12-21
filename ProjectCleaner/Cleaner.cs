namespace ProjectCleaner;

using System;
using System.IO;
using System.Linq;

internal static class Cleaner
{
    public static int DeletedFiles { get; private set; }
    public static int DeletedFolders { get; private set; }

    public static void CleanSolution(string filePath)
    {
        DeletedFiles = 0;
        DeletedFolders = 0;
        string folder = Path.GetDirectoryName(filePath)!;
        var di = new DirectoryInfo(folder);
        ClearDirectory(di, false);
    }

    private static void ClearDirectory(DirectoryInfo dir, bool tobedeleted)
    {
        //Console.WriteLine("Checking " + dir);
        var isProjectRoot = dir.EnumerateFiles("*.csproj").Any();
        if (isProjectRoot)
        {
            Console.WriteLine("Project root: " + dir);
        }

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
                    Console.WriteLine("Couldn't delete file: " + file.Name + ", " + exf.Message);
                }
            }
            // Console.WriteLine($"Deleted {count} files from {dir.FullName}");

            // then remove the folder itself
            try
            {
                // Console.WriteLine("Deleting folder " + dir.FullName);
                dir.Delete();
                DeletedFolders++;
            }
            catch (Exception exd)
            {
                Console.WriteLine("Couldn't delete folder: " + dir.FullName + ", " + exd.Message);
            }
        }
    }
}
