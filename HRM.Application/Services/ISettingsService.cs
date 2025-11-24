namespace HRM.Application.Services;

public interface ISettingsService
{
    Task<string?> GetSettingAsync(string key, string? environment = null);
    Task<bool> UpdateSettingAsync(string key, string value, DateTime? effectiveDate = null, string? environment = null);
    Task<List<SettingDto>> GetAllSettingsAsync(string? environment = null);
}

public class SettingDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int Version { get; set; }
    public DateTime EffectiveDate { get; set; }
    public string? Environment { get; set; }
    public string? Description { get; set; }
}

