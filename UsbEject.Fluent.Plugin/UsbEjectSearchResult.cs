using System.Collections.ObjectModel;
using Blast.Core.Interfaces;
using Blast.Core.Results;

namespace UsbEject.Fluent.Plugin;

public class UsbEjectSearchResult : SearchResultBase
{
    public const string TagName = "eject";
    public const string PluginDesc = "Eject USB Drives";
    public const string SearchResultIcon = "\uEDA2";

    public static readonly ObservableCollection<ISearchOperation> SearchOperations = new()
    {
        UsbEjectOperation.EjectOperation
    };

    public static readonly ObservableCollection<SearchTag> SearchTags = new()
    {
        new SearchTag
        {
            Name = TagName,
            IconGlyph = SearchResultIcon,
            Description = PluginDesc
        }
    };

    public UsbEjectSearchResult(string searchedText, string resultType, double score,
        DriveInfoTip driveInfoTip) : base(TagName, driveInfoTip.VolumeLabel, searchedText, resultType, score,
        SearchOperations, SearchTags)
    {
        AdditionalInformation = driveInfoTip.DriveRowLabel;
        DriveInfo = driveInfoTip;
        IconGlyph = SearchResultIcon;
    }

    public DriveInfoTip DriveInfo { get; }

    protected override void OnSelectedSearchResultChanged()
    {
        // Leave empty
    }
}