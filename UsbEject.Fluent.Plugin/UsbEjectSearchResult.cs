using System.Collections.ObjectModel;
using Blast.Core.Interfaces;
using Blast.Core.Results;

namespace UsbEject.Fluent.Plugin;

public class UsbEjectSearchResult : SearchResultBase
{
    public const string TagName = "eject";
    public const string PluginDesc = "Eject USB Drives";
    public const string SearchResultIcon = "\uF78B";

    public static readonly ObservableCollection<ISearchOperation> SearchOperations = new()
    {
        UsbEjectOperation.EjectOperation
    };

    public DriveInfoTip DriveInfo { get; set; }

    public static readonly ObservableCollection<SearchTag> SearchTags = new()
    {
        new SearchTag
        {
            Name = TagName,
            IconGlyph = SearchResultIcon,
            Description = PluginDesc
        }
    };

    public UsbEjectSearchResult(string resultName, string searchedText, string resultType, double score,
        string rowLabel, DriveInfoTip driveInfoTip) : base(TagName, resultName, searchedText, resultType, score,
        SearchOperations, SearchTags)
    {
        AdditionalInformation = rowLabel;
        DriveInfo = driveInfoTip;
    }

    protected override void OnSelectedSearchResultChanged()
    {
        // Leave empty
    }
}
