using System.Diagnostics;

namespace UsbEject.Fluent.Plugin;

public class ProcessTools
{
    public static string IsLocked(string volumeLetter)
    {
        try
        {
            var list = new HashSet<string>();
            var dir = new DirectoryInfo(volumeLetter);
            Stopwatch sw = Stopwatch.StartNew();
            long max_time_limit = 20000;
            var processesList = new List<Process>();

            var nested_files = dir.EnumerateFiles("*.*", new EnumerationOptions
            {
                IgnoreInaccessible = true,
                RecurseSubdirectories = true,
                ReturnSpecialDirectories = false
            });

            foreach (FileInfo fileInfoVar in nested_files)
            {
                if (sw.ElapsedMilliseconds > max_time_limit)
                {
                    return string.Empty;
                }

                try
                {
                    Process[] tempList = fileInfoVar.GetLockProcesses();
                    if (tempList.Length > 0)
                    {
                        processesList.AddRange(tempList);
                        break; // Currently breaking the loop if first locked process is found.
                    }
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

            return list.Count > 0 ? string.Join(" ", list) : string.Empty;
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }
}
