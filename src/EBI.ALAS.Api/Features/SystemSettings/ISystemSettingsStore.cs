namespace EBI.ALAS.Api.Features.SystemSettings;

public interface ISystemSettingsStore
{
    Task<string?> GetAsync(string key, CancellationToken ct = default);
    Task SetAsync(string key, string value, string? description, string category, bool isEditable, CancellationToken ct = default);
    Task<IReadOnlyList<SystemSetting>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<SystemSetting>> GetByCategoryAsync(string category, CancellationToken ct = default);
}