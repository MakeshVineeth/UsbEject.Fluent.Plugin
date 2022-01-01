using Blast.Core.Interfaces;
using Blast.Core.Objects;

namespace UsbEject.Fluent.Plugin;

public class UsbEjectSearchApp: ISearchApplication
{
    public ValueTask<IHandleResult> HandleSearchResult(ISearchResult searchResult)
    {
        throw new NotImplementedException();
    }

    public SearchApplicationInfo GetApplicationInfo()
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<ISearchResult> SearchAsync(SearchRequest searchRequest, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
