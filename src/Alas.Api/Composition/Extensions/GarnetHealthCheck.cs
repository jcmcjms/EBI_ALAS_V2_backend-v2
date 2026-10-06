using System.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace Alas.Api.Composition.Extensions;

public sealed class GarnetHealthCheck : IHealthCheck
{
    private readonly IConnectionMultiplexer _multiplexer;
    private readonly ILogger<GarnetHealthCheck> _logger;

    public GarnetHealthCheck(IConnectionMultiplexer multiplexer, ILogger<GarnetHealthCheck> logger)
    {
        _multiplexer = multiplexer;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!_multiplexer.IsConnected)
            {
                return HealthCheckResult.Unhealthy(
                    "Garnet connection is not established.",
                    data: new Dictionary<string, object> { ["connected"] = false });
            }

            var sw = Stopwatch.StartNew();
            var db = _multiplexer.GetDatabase();
            var pong = await db.PingAsync();
            sw.Stop();
            var latencyMs = sw.ElapsedMilliseconds;

            var endpoints = _multiplexer.GetEndPoints();
            var connectedCount = endpoints.Count(e => _multiplexer.GetServer(e).IsConnected);

            var data = new Dictionary<string, object>
            {
                ["connected"] = true,
                ["latency_ms"] = latencyMs,
                ["endpoints_total"] = endpoints.Length,
                ["endpoints_connected"] = connectedCount
            };

            if (latencyMs > 50)
            {
                _logger.LogWarning("Garnet PING latency is {LatencyMs}ms (threshold: 50ms)", latencyMs);
                return HealthCheckResult.Degraded(
                    $"Garnet PING latency is {latencyMs}ms (expected <50ms).",
                    data: data);
            }

            return HealthCheckResult.Healthy(
                $"Garnet is healthy. PING: {latencyMs}ms, {connectedCount}/{endpoints.Length} endpoints connected.",
                data: data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Garnet health check failed");
            return HealthCheckResult.Unhealthy(
                "Garnet health check failed: " + ex.Message,
                exception: ex);
        }
    }
}