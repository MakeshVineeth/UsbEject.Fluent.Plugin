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
            long max_time_limit = 8000;
            var processesList = new List<Process>();
            bool locked_process_found = false;

            // Search in all root files.
            IEnumerable<FileInfo> files = dir.EnumerateFiles("*.*",
                new EnumerationOptions
                {
                    IgnoreInaccessible = true,
                    ReturnSpecialDirectories = false,
                });

            foreach (FileInfo fileInfoVar in files)
            {
                if (sw.ElapsedMilliseconds > max_time_limit)
                {
                    return string.Empty;
                }

                try
                {
                    Process[] temp_list = fileInfoVar.GetLockProcesses();
                    if (temp_list.Length > 0)
                    {
                        processesList.AddRange(temp_list);
                        locked_process_found = true;
                        break;
                    }
                }
                catch (Exception)
                {
                }
            }

            // Search in all directories.
            IEnumerable<DirectoryInfo> dirs = dir.GetDirectories();
            foreach (DirectoryInfo d in dirs)
            {
                if (locked_process_found == true)
                {
                    break;
                }

                if (sw.ElapsedMilliseconds > max_time_limit)
                {
                    return string.Empty;
                }

                try
                {
                    var nested_files = d.EnumerateFiles("*.*", new EnumerationOptions
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
                                locked_process_found = true;
                                break;
                            }
                        }
                        catch (Exception)
                        {
                        }
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
