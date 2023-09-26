using System.Collections.Concurrent;

namespace Urll.Core;

public class RedisLinksRepository : ILinksRepository
{
    public Task<Link?> GetOrDefault(string code)
    {
        _data.TryGetValue(code, out Link? link);
        return Task.FromResult(link);
    }

    public Task<bool> Add(Link link)
    {
        bool result = _data.TryAdd(link.Code, link);
        return Task.FromResult(result);
    }

    private readonly ConcurrentDictionary<string, Link> _data = new();
}
