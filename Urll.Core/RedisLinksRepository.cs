using System.Security.Permissions;
using System.Text.Json;
using StackExchange.Redis;

namespace Urll.Core;

public class RedisLinksRepository : ILinksRepository
{
    public RedisLinksRepository(ConnectionMultiplexer connection)
    {
        _connection = connection;
    }

    public async Task<IReadOnlyCollection<Link>> GetAll()
    {
        IServer[] servers = _connection.GetServers();
        List<string> keys = new();
        foreach (IServer server in servers)
        {
            await foreach (RedisKey redisKey in server.KeysAsync(pattern: LinkKeyPrefix + "*"))
            {
                string key = redisKey.ToString();
                if (keys.Contains(key))
                {
                    continue;
                }

                keys.Add(key);
            }
        }

        List<Link> links = new();
        foreach (string key in keys)
        {
            string code = key.Split(':')[1];
            Link? link = await GetOrDefault(code);
            if (link is not null)
            {
                links.Add(link);
            }
        }

        return links;
    }

    public async Task<Link?> GetOrDefault(string code)
    {
        IDatabase db = _connection.GetDatabase();
        RedisValue redisValue = await db.StringGetAsync(LinkKeyPrefix + code);
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
        IDatabase db = _connection.GetDatabase();
        bool success = await db.StringSetAsync(LinkKeyPrefix + link.Code, json, when: When.NotExists);
        return success;
    }

    public async Task<bool> Delete(string code)
    {
        IDatabase db = _connection.GetDatabase();
        return await db.KeyDeleteAsync(LinkKeyPrefix + code);
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

    private const string LinkKeyPrefix = "link:";

    private readonly ConnectionMultiplexer _connection;
}
