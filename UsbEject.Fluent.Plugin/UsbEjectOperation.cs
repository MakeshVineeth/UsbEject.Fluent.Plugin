using Blast.Core.Results;

namespace UsbEject.Fluent.Plugin;

public class UsbEjectOperation : SearchOperationBase
{
    private UsbEjectOperation(string operationName, string description, string icon)
    {
        OperationName = operationName;
        Description = description;
        IconGlyph = icon;
    }

    public static UsbEjectOperation EjectOperation { get; } = new("Eject", "Ejects the selected drive", "\uE8A7");
}