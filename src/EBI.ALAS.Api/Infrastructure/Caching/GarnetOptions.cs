namespace EBI.ALAS.Api.Infrastructure.Caching;

public sealed class GarnetOptions
{
    public const string SectionName = "Garnet";

    public string ConnectionString { get; set; } = "localhost:6379";
    public string InstanceName { get; set; } = "ALAS_";
    public int Database { get; set; } = 0;
    public bool AbortOnConnectFail { get; set; } = false;
    public int ConnectTimeout { get; set; } = 5000;
    public int SyncTimeout { get; set; } = 5000;
}