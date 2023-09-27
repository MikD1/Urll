using System.Collections.Concurrent;
using System.Text.Json;

namespace Urll.Core;

public class RedisLinksRepository : ILinksRepository
{
    public Task<Link?> GetOrDefault(string code)
    {
        _data.TryGetValue(code, out string? json);
        TryDeserializeLink(json, out Link? link);
        return Task.FromResult(link);
    }

    public Task<bool> Add(Link link)
    {
        string json = JsonSerializer.Serialize(link);
        bool result = _data.TryAdd(link.Code, json);
        return Task.FromResult(result);
    }

    private bool TryDeserializeLink(string? json, out Link? link)
    {
        if (json is null)
        {
            link = null;
            return false;
        }

        try
        {
            link = JsonSerializer.Deserialize<Link>(json);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            link = null;
            return false;
        }
    }

    private readonly ConcurrentDictionary<string, string> _data = new();
}
