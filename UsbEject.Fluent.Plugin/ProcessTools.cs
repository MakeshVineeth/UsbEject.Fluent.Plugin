using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace UsbEject.Fluent.Plugin
{
    public class ProcessTools
    {
        public static string IsLocked(string volumeLetter)
        {
            var list = new HashSet<string>();
            var dir = new DirectoryInfo(volumeLetter);

            // Search in all directories.
            IEnumerable<DirectoryInfo> dirs = dir.GetDirectories();
            var processesList = new List<Process>();
            foreach (DirectoryInfo d in dirs) processesList.AddRange(d.GetLockProcesses());

            // Search in all files.
            IEnumerable<FileInfo> files = dir.GetFiles();
            foreach (FileInfo fileInfoVar in files) processesList.AddRange(fileInfoVar.GetLockProcesses());

            foreach (Process process in from process in processesList
                     let name = process.ProcessName
                     where !string.IsNullOrWhiteSpace(name) && !list.Contains(name)
                     select process)
                list.Add(process.ProcessName);

            return list.Count > 0 ? string.Join(" ", list) : null;
        }
    }
}
