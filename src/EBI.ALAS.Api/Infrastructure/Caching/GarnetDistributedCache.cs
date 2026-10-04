using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Text.Json;

namespace EBI.ALAS.Api.Infrastructure.Caching;

public sealed class GarnetDistributedCache(
    GarnetConnectionMultiplexer multiplexer,
    IOptions<GarnetOptions> options) : IDistributedCache
{
    private readonly IDatabase _database = multiplexer.Connection.GetDatabase(options.Value.Database);
    private readonly string _instanceName = options.Value.InstanceName;

    public byte[]? Get(string key) => GetAsync(key).GetAwaiter().GetResult();

    public async Task<byte[]?> GetAsync(string key, CancellationToken token = default)
    {
        var value = await _database.StringGetAsync($"{_instanceName}{key}");
        return value.HasValue ? (byte[])value! : null;
    }

    public void Set(string key, byte[] value, DistributedCacheEntryOptions options) =>
        SetAsync(key, value, options).GetAwaiter().GetResult();

    public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
    {
        var fullKey = $"{_instanceName}{key}";
        var expiry = options.AbsoluteExpirationRelativeToNow ?? options.AbsoluteExpiration - DateTimeOffset.UtcNow
            ?? TimeSpan.FromMinutes(15);

        await _database.StringSetAsync(fullKey, value, expiry);
    }

    public void Refresh(string key) => RefreshAsync(key).GetAwaiter().GetResult();

    public async Task RefreshAsync(string key, CancellationToken token = default)
    {
        var fullKey = $"{_instanceName}{key}";
        var ttl = await _database.KeyTimeToLiveAsync(fullKey);
        if (ttl.HasValue)
        {
            await _database.KeyExpireAsync(fullKey, ttl.Value);
        }
    }

    public void Remove(string key) => RemoveAsync(key).GetAwaiter().GetResult();

    public async Task RemoveAsync(string key, CancellationToken token = default)
    {
        await _database.KeyDeleteAsync($"{_instanceName}{key}");
    }
}