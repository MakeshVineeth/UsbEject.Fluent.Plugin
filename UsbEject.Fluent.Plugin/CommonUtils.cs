using Blast.API.Core.UI;
using Blast.API.OperationSystem;
using Blast.Core;

namespace UsbEject.Fluent.Plugin;

public class CommonUtils
{
    public static void ShowMessage(string? message)
    {
        UiUtilities.UiDispatcher.Post(() =>
        {
            OsUtils.OsNotificationManager.ShowNotification(new NotificationModel() { Title = "USB Eject Plugin", Content = message });
        });
    }
}
