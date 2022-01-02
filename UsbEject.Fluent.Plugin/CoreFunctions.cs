namespace UsbEject.Fluent.Plugin;

public class CoreFunctions
{
    public static EjectStatusEnum EjectDrive(DriveInfoTip driveInfoTip)
    {
        try
        {
            var status = EjectStatusEnum.Null;
            string targetVolumeLabel = driveInfoTip?.VolumeLabel;
            if (string.IsNullOrWhiteSpace(targetVolumeLabel)) return EjectStatusEnum.Error;
            targetVolumeLabel = targetVolumeLabel.Trim();

            using VolumeDeviceClass volumesClass = new();
            IList<Volume> allVolumes = new List<Volume>(volumesClass.Volumes);
            foreach (Volume eachVolume in allVolumes)
            {
                string currentDriveLetter = eachVolume.LogicalDrive + @"\";
                if (!Directory.Exists(currentDriveLetter)) continue;

                List<Disk> disks = new(eachVolume.Disks);
                if (string.IsNullOrWhiteSpace(currentDriveLetter) ||
                    !driveInfoTip.DriveLetters.Contains(currentDriveLetter))
                    continue;

                if (!(from eachDisk in from eachDisk in disks
                            let eachVolumeLabel = eachDisk.FriendlyName ?? ""
                            where !string.IsNullOrWhiteSpace(eachVolumeLabel)
                            where targetVolumeLabel.Contains(eachVolumeLabel)
                            select eachDisk
                        where Directory.Exists(currentDriveLetter)
                        select eachDisk.Eject(true)).Any()) continue;

                if (Directory.Exists(currentDriveLetter))
                {
                    string lockedStr = ProcessTools.IsLocked(currentDriveLetter);
                    if (!string.IsNullOrWhiteSpace(lockedStr)) CommonUtils.ShowMessage(lockedStr);
                    break;
                }

                status = EjectStatusEnum.Success;
            }

            return status;
        }
        catch (Exception)
        {
            return EjectStatusEnum.Error;
        }
    }
}
