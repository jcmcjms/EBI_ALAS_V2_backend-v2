using Microsoft.EntityFrameworkCore;

namespace EBI.ALAS.Api.Features.SystemSettings;

public sealed class SystemSettingsStore(AppDbContext db) : ISystemSettingsStore
{
    public Task<string?> GetAsync(string key, CancellationToken ct = default) =>
        db.SystemSettings.AsNoTracking()
            .Where(s => s.Key == key)
            .Select(s => s.Value)
            .FirstOrDefaultAsync(ct);

    public async Task SetAsync(string key, string value, string? description, string category, bool isEditable, CancellationToken ct = default)
    {
        var setting = await db.SystemSettings.FirstOrDefaultAsync(s => s.Key == key, ct);
        if (setting is null)
        {
            setting = new SystemSetting
            {
                Key = key,
                Value = value,
                Description = description,
                Category = category,
                IsEditable = isEditable
            };
            db.SystemSettings.Add(setting);
        }
        else
        {
            setting.Value = value;
            setting.Description = description;
            setting.Category = category;
            setting.IsEditable = isEditable;
            setting.UpdatedAt = DateTime.UtcNow;
        }
        await db.SaveChangesAsync(ct);
    }

    public Task<IReadOnlyList<SystemSetting>> GetAllAsync(CancellationToken ct = default) =>
        db.SystemSettings.AsNoTracking().OrderBy(s => s.Category).ThenBy(s => s.Key).ToListAsync(ct)
            .ContinueWith(t => (IReadOnlyList<SystemSetting>)t.Result);

    public Task<IReadOnlyList<SystemSetting>> GetByCategoryAsync(string category, CancellationToken ct = default) =>
        db.SystemSettings.AsNoTracking().Where(s => s.Category == category).OrderBy(s => s.Key).ToListAsync(ct)
            .ContinueWith(t => (IReadOnlyList<SystemSetting>)t.Result);
}