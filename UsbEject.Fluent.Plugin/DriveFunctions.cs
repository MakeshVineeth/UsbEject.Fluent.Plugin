namespace UsbEject.Fluent.Plugin;

public class DriveFunctions
{
    public static IEnumerable<DriveInfoTip> ListDrives()
    {
        string? mainDrive = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System));
        using VolumeDeviceClass volumes = new();
        IDictionary<string, List<string>> drivesDict = new Dictionary<string, List<string>>();
        string mainVolume = string.Empty;

        foreach (Volume eachVolume in volumes.Volumes)
        {
            string volumeLetter = eachVolume.LogicalDrive + @"\";
            List<Disk> disks = new(eachVolume.Disks);
            string volumeLabelStr = string.Empty;

            foreach (Disk eachDisk in disks)
            {
                volumeLabelStr = eachDisk.FriendlyName ?? "";
                if (string.IsNullOrWhiteSpace(volumeLabelStr)) continue;
                if (!drivesDict.ContainsKey(volumeLabelStr))
                    drivesDict.Add(volumeLabelStr, new List<string>());
            }

            if (mainDrive != null && mainDrive.Contains(volumeLetter))
            {
                mainVolume = volumeLabelStr;
                continue;
            }

            if (!string.IsNullOrWhiteSpace(mainVolume) && volumeLabelStr.Contains(mainVolume)) continue;
            if (string.IsNullOrWhiteSpace(volumeLetter) || !HasDrives(eachVolume)) continue;

            if (drivesDict.ContainsKey(volumeLabelStr)) drivesDict[volumeLabelStr].Add(volumeLetter);
        }

        foreach (string volumeKey in drivesDict.Keys)
        {
            string volumeTitle = volumeKey.Trim();
            List<string> driveLetters = drivesDict[volumeTitle];

            string rowLabel = driveLetters.Aggregate(string.Empty,
                (current, driveLetter) => current + driveLetter + GetDriveLabel(driveLetter));

            if (driveLetters.Count > 0)
                yield return new DriveInfoTip
                {
                    VolumeLabel = volumeTitle, DriveLetters = driveLetters,
                    DriveRowLabel = rowLabel
                };
        }
    }

    private static bool HasDrives(Volume volume)
    {
        int[] nums = volume.DiskNumbers;
        return nums.Length > 0;
    }

    private static string GetDriveLabel(string driveLetter)
    {
        DriveInfo[] driveInfos = DriveInfo.GetDrives();
        foreach (DriveInfo drive in driveInfos)
            if (driveLetter.Contains(drive.Name))
            {
                if (drive.DriveType != DriveType.Fixed && drive.DriveType != DriveType.Removable)
                    return string.Empty;

                string label = drive.VolumeLabel;
                if (!string.IsNullOrWhiteSpace(label)) return $" ( {label} ) ";
            }

        return string.Empty;
    }
}
