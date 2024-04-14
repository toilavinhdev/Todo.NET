using StackExchange.Redis;
using Todo.NET.Extensions;

namespace Todo.NET.Redis.Caching;

public interface IRedisCacheService
{
    public ConnectionMultiplexer Connection();
    
    Task<T> GetAsync<T>(string key, int index = 1);
    
    Task<bool> SetAsync<T>(string key, T value, int index = 1);
}

public class RedisCacheService(RedisCacheOptions options) : IRedisCacheService
{
    private readonly Lazy<ConnectionMultiplexer> _lazyConnection = new (() => ConnectionMultiplexer.Connect(options.ConnectionString));

    public ConnectionMultiplexer Connection() => _lazyConnection.Value;

    private IDatabase Database(int id) => Connection().GetDatabase(id);
    
    public async Task<T> GetAsync<T>(string key, int index = 1)
    {
        var db = Database(index);
        var value = await db.StringGetAsync(key);
        return value.ToString().ToObject<T>();
    }

    public async Task<bool> SetAsync<T>(string key, T value, int index = 1)
    {
        var db = Database(index);
        return await db.StringSetAsync(key, value.ToJson());
    }
}