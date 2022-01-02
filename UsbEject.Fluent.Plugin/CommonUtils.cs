using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace UsbEject.Fluent.Plugin
{
    public class CommonUtils
    {
        public static void ShowMessage(string message)
        {
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);

            XmlNodeList stringElements = toastXml.GetElementsByTagName("text");
            stringElements[0].AppendChild(toastXml.CreateTextNode("USB Eject"));
            stringElements[1].AppendChild(toastXml.CreateTextNode(message));

            var toast = new ToastNotification(toastXml);
            ToastNotifier toastNotifier =
                ToastNotificationManager.CreateToastNotifier("USB Eject");
            toastNotifier.Show(toast);
        }
    }
}
