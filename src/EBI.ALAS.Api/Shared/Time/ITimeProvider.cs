namespace EBI.ALAS.Api.Shared.Time;

public interface ITimeProvider
{
    DateTime UtcNow { get; }
    DateTimeOffset UtcNowOffset { get; }
    DateTime PhilippinesNow { get; }
    DateOnly PhilippinesToday { get; }
    TimeZoneInfo PhilippinesTimeZone { get; }
}