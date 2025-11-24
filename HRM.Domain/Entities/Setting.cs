namespace HRM.Domain.Entities;

public class Setting : BaseEntity
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int Version { get; set; } = 1;
    public DateTime EffectiveDate { get; set; } = DateTime.UtcNow;
    public string? Environment { get; set; } // null = all environments
    public string? Description { get; set; }
}

