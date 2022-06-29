# UsbEject.Fluent.Plugin

List Drives to Eject           |  Show Locked Processes
:-------------------------:|:-------------------------:
![Drive Listing](/Images/Image1.png)  |  ![Error Notification](/Images/Image2.png)

This repo is a [Fluent Search](https://fluentsearch.net) plugin for ejecting drives right within FS. And also display what's locking the drive in case of failure.

# Usage

- Use the `eject` tag for listing drives.
- Use the operation `Eject` to eject the drive.

# Notes

- If a drive cannot be ejected, then the plugin will search for locked processes in each drive for some time and display a notification.
- Programs running with Admin privileges cannot be displayed.
