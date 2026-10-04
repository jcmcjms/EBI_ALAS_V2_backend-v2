using StackExchange.Redis;

namespace EBI.ALAS.Api.Infrastructure.Caching;

public sealed class GarnetConnectionMultiplexer : IDisposable
{
    private readonly IConnectionMultiplexer _multiplexer;

    public GarnetConnectionMultiplexer(IConnectionMultiplexer multiplexer)
    {
        _multiplexer = multiplexer;
    }

    public IConnectionMultiplexer Connection => _multiplexer;

    public void Dispose() => _multiplexer.Dispose();
}