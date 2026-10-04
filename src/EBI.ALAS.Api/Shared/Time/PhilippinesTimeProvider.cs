using System.Runtime.Versioning;

namespace EBI.ALAS.Api.Shared.Time;

[SupportedOSPlatform("windows")]
[SupportedOSPlatform("linux")]
public sealed class PhilippinesTimeProvider : ITimeProvider
{
    private static readonly TimeZoneInfo _philippinesTimeZone =
        TimeZoneInfo.FindSystemTimeZoneById("Asia/Manila");

    public DateTime UtcNow => DateTime.UtcNow;

    public DateTimeOffset UtcNowOffset => DateTimeOffset.UtcNow;

    public DateTime PhilippinesNow => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _philippinesTimeZone);

    public DateOnly PhilippinesToday => DateOnly.FromDateTime(PhilippinesNow);

    public TimeZoneInfo PhilippinesTimeZone => _philippinesTimeZone;
}