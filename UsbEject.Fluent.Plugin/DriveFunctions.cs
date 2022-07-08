using System.Management;

namespace UsbEject.Fluent.Plugin
{
    public class DriveFunctions
    {
        public static IEnumerable<DriveInfoTip> ListDrives()
        {
            using VolumeDeviceClass volumes = new();
            IDictionary<string, List<string>> drivesDict = new Dictionary<string, List<string>>();
            List<DriveInfo> drives = GetExternalDrives();
            Dictionary<string, string> all_drive_letters = new();

            foreach (DriveInfo item in drives)
            {
                all_drive_letters[item.Name] = item.VolumeLabel;
            }

            foreach (Volume eachVolume in volumes.Volumes)
            {
                string logicalDrive = eachVolume.LogicalDrive;
                if (string.IsNullOrWhiteSpace(logicalDrive)) continue;

                string volumeLetter = logicalDrive + @"\";
                if (!all_drive_letters.ContainsKey(volumeLetter))
                {
                    continue;
                }

                List<Disk> disks = new(eachVolume.Disks);
                string volumeLabelStr = string.Empty;

                foreach (Disk eachDisk in disks)
                {
                    volumeLabelStr = eachDisk.FriendlyName ?? "";
                    if (string.IsNullOrWhiteSpace(volumeLabelStr)) continue;

                    // Adding volumes to dict and avoiding duplicates at the same time.
                    if (!drivesDict.ContainsKey(volumeLabelStr))
                        drivesDict.Add(volumeLabelStr, new List<string>());
                }

                if (drivesDict.ContainsKey(volumeLabelStr)) drivesDict[volumeLabelStr].Add(volumeLetter);
            }

            foreach (string volumeKey in drivesDict.Keys)
            {
                string volumeTitle = volumeKey.Trim();
                List<string> driveLetters = drivesDict[volumeTitle];

                string rowLabel = driveLetters.Aggregate(string.Empty,
                    (current, driveLetter) => current + driveLetter + " ( " + all_drive_letters[driveLetter] + " ) ");

                if (driveLetters.Count > 0)
                {
                    yield return new DriveInfoTip
                    {
                        VolumeLabel = volumeTitle,
                        DriveLetters = driveLetters,
                        DriveRowLabel = rowLabel
                    };
                }
            }
        }

        public static List<DriveInfo> GetExternalDrives()
        {
            /* 
            
            Code taken from https://stackoverflow.com/a/56381695
            Author: TJ Rockefeller
            Author Profile: https://stackoverflow.com/users/4708150/tj-rockefeller
            
            */

            var drives = DriveInfo.GetDrives();
            var externalDrives = new List<DriveInfo>();

            var allPhysicalDisks = new ManagementObjectSearcher("select MediaType, DeviceID from Win32_DiskDrive").Get();

            foreach (var physicalDisk in allPhysicalDisks)
            {
                var allPartitionsOnPhysicalDisk = new ManagementObjectSearcher($"associators of {{Win32_DiskDrive.DeviceID='{physicalDisk["DeviceID"]}'}} where AssocClass = Win32_DiskDriveToDiskPartition").Get();
                foreach (var partition in allPartitionsOnPhysicalDisk)
                {
                    if (partition == null)
                        continue;

                    var allLogicalDisksOnPartition = new ManagementObjectSearcher($"associators of {{Win32_DiskPartition.DeviceID='{partition["DeviceID"]}'}} where AssocClass = Win32_LogicalDiskToPartition").Get();
                    foreach (var logicalDisk in allLogicalDisksOnPartition)
                    {
                        if (logicalDisk == null)
                            continue;

                        var drive = drives.Where(x => x.Name.StartsWith(logicalDisk["Name"] as string, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                        var mediaType = (physicalDisk["MediaType"] as string).ToLowerInvariant();
                        if (mediaType.Contains("external") || mediaType.Contains("removable"))
                            externalDrives.Add(drive);
                    }
                }
            }
            return externalDrives;
        }
    }
}