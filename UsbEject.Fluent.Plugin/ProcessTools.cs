using System.Diagnostics;

namespace UsbEject.Fluent.Plugin;

public class ProcessTools
{
    public static string? IsLocked(string volumeLetter)
    {
        var list = new HashSet<string>();
        var dir = new DirectoryInfo(volumeLetter);

        // Search in all directories.
        IEnumerable<DirectoryInfo> dirs = dir.GetDirectories();
        var processesList = new List<Process>();
        foreach (DirectoryInfo d in dirs)
        {
            try
            {
                processesList.AddRange(d.GetLockProcesses());
            }
            catch (Exception)
            {
            }
        }

        // Search in all files.
        IEnumerable<FileInfo> files = dir.GetFiles();
        foreach (FileInfo fileInfoVar in files)
        {
            try
            {
                processesList.AddRange(fileInfoVar.GetLockProcesses());
            }
            catch (Exception)
            {
            }
        }

        foreach (Process process in from process in processesList
                                    let name = process.ProcessName
                                    where !string.IsNullOrWhiteSpace(name) && !list.Contains(name)
                                    select process)
            list.Add(process.ProcessName);

        return list.Count > 0 ? string.Join(" ", list) : null;
    }
}
