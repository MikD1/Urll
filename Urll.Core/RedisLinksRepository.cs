using System.Text.Json;
using StackExchange.Redis;

namespace Urll.Core;

public class RedisLinksRepository : ILinksRepository
{
    public RedisLinksRepository(ConnectionMultiplexer connection)
    {
        _connection = connection;
        _db = _connection.GetDatabase();
    }

    public async Task<Link?> GetOrDefault(string code)
    {
        RedisValue redisValue = await _db.StringGetAsync($"{LinkKeyPrefix}:{code}");
        if (redisValue.IsNull)
        {
            return null;
        }

        TryDeserializeLink(redisValue.ToString(), out Link? link);
        return link;
    }

    public async Task<bool> Add(Link link)
    {
        string json = JsonSerializer.Serialize(link);
        bool success = await _db.StringSetAsync($"{LinkKeyPrefix}:{link.Code}", json, when: When.NotExists);
        return success;
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

    private const string LinkKeyPrefix = "link";

    private readonly ConnectionMultiplexer _connection;
    private readonly IDatabase _db;
}
