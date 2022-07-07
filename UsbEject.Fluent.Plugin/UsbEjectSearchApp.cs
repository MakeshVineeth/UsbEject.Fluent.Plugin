using System.Runtime.CompilerServices;
using Blast.Core.Interfaces;
using Blast.Core.Objects;
using Blast.Core.Results;
using static UsbEject.Fluent.Plugin.DriveFunctions;
using static UsbEject.Fluent.Plugin.UsbEjectSearchResult;

namespace UsbEject.Fluent.Plugin;

public class UsbEjectSearchApp : ISearchApplication
{
    private const string SearchAppName = "USBEject";
    private readonly SearchApplicationInfo _applicationInfo;

    public UsbEjectSearchApp()
    {
        _applicationInfo = new SearchApplicationInfo(SearchAppName,
            PluginDesc, SearchOperations)
        {
            MinimumSearchLength = 1,
            IsProcessSearchEnabled = false,
            IsProcessSearchOffline = false,
            SearchTagOnly = true,
            ApplicationIconGlyph = SearchResultIcon,
            SearchAllTime = ApplicationSearchTime.Fast,
            DefaultSearchTags = SearchTags,
            SearchTagName = TagName
        };
    }

    public ValueTask<IHandleResult> HandleSearchResult(ISearchResult searchResult)
    {
        if (searchResult is not UsbEjectSearchResult usbEjectSearchResult)
            throw new InvalidCastException(nameof(UsbEjectSearchResult));

        if (searchResult.SelectedOperation is not UsbEjectOperation)
            return new ValueTask<IHandleResult>(new HandleResult(true, false));

        DriveInfoTip driveInfoTip = usbEjectSearchResult.DriveInfo;
        EjectStatusEnum status = CoreFunctions.EjectDrive(driveInfoTip);
        if (status != EjectStatusEnum.Success)
        {
        }
        else
        {
            CommonUtils.ShowMessage("Status: " + status);
        }

        return new ValueTask<IHandleResult>(new HandleResult(true, false));
    }

    public SearchApplicationInfo GetApplicationInfo()
    {
        return _applicationInfo;
    }

#pragma warning disable CS1998
    public async IAsyncEnumerable<ISearchResult> SearchAsync(SearchRequest searchRequest,
        [EnumeratorCancellation] CancellationToken cancellationToken)
#pragma warning restore CS1998
    {
        string searchedTag = searchRequest.SearchedTag;
        string searchedText = searchRequest.SearchedText;

        if (string.IsNullOrWhiteSpace(searchedTag) || !searchedTag.Equals(TagName, StringComparison.Ordinal))
            yield break;

        searchedText = searchedText.Trim();

        IEnumerable<DriveInfoTip> drives = ListDrives();
        foreach (DriveInfoTip varDriveLabel in drives)
            yield return new UsbEjectSearchResult(searchedText, "USB", 2.0,
                varDriveLabel);
    }
}
